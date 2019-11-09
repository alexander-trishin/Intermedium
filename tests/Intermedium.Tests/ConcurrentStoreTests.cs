using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Intermedium.Compatibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class ConcurrentStoreTests
    {
        [TestMethod]
        public void GetOrAdd_TryAddOneHundredRecords_AddsOnlyOneRecord()
        {
            var store = new ConcurrentStore<int, object>();

            Parallel.ForEach(Enumerable.Repeat(1, 100), i =>
            {
                store.GetOrAdd(i, x => new object());
            });

            store.Count.Should().Be(1);
        }
    }
}
