using System;

#if HAVE_CONCURRENT_DICTIONARY
using System.Collections.Concurrent;
#else
using System.Collections.Generic;
#endif

namespace Intermedium.Compatibility
{
    internal sealed class ConcurrentStore<TKey, TValue>
    {
#if HAVE_CONCURRENT_DICTIONARY
        private readonly ConcurrentDictionary<TKey, TValue> _store
             = new ConcurrentDictionary<TKey, TValue>();
#else
        private readonly object _lock = new object();
        private Dictionary<TKey, TValue> _store = new Dictionary<TKey, TValue>();
#endif

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
#if HAVE_CONCURRENT_DICTIONARY
            return _store.GetOrAdd(key, valueFactory);
#else
            if (_store.TryGetValue(key, out var valueFromStore))
            {
                return valueFromStore;
            }

            var value = valueFactory(key);

            lock (_lock)
            {
                if (_store is null)
                {
                    _store = new Dictionary<TKey, TValue> { [key] = value };
                }
                else
                {
                    if (_store.TryGetValue(key, out var checkValue))
                    {
                        return checkValue;
                    }

                    var newStore = new Dictionary<TKey, TValue>(_store) { [key] = value };
                    _store = newStore;
                }

                return value;
            }
#endif
        }

        internal int Count => _store.Count;
    }
}
