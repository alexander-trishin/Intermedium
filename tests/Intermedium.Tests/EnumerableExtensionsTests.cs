using System.Collections.Generic;
using FluentAssertions;
using Intermedium.Core.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class EnumerableExtensionsTests
    {
        [TestMethod]
        public void EmptyIfNull_Null_ReturnsEmptyCollection()
        {
            var collection = ((IEnumerable<object>)null).EmptyIfNull();

            collection.Should().NotBeNull().And.BeEmpty();
        }

        [TestMethod]
        public void EmptyIfNull_FilledCollection_ReturnsSameCollection()
        {
            var collection = new[] { 0 };

            var actual = collection.EmptyIfNull();

            actual.Should().BeSameAs(collection);
        }

        [TestMethod]
        public void Sort_WithoutComparer_ReturnsSameCollection()
        {
            IEnumerable<int> collection = new[] { 0 };
            IComparer<int> comparer = null;

            var actual = collection.Sort(comparer);

            actual.Should().BeSameAs(collection);
        }

        [TestMethod]
        public void Sort_DefaultComparer_ReturnsSortedCollection()
        {
            IEnumerable<int> collection = new[] { 9, 8, 5, 3, 4, 1, 0 };
            IComparer<int> comparer = Comparer<int>.Default;

            var actual = collection.Sort(comparer);

            actual.Should().BeInAscendingOrder();
        }
    }
}
