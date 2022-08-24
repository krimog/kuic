using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Kuic.Core.Collections;
using Xunit;
using L = System.Action; // Lambda
using A = System.Func<System.Threading.Tasks.Task>; // Async lambda

namespace Kuic.Core.Tests
{
    [ExcludeFromCodeCoverage]
    public class EnumerableExtensionsTests
    {
#nullable disable
        [Fact]
        public void DistinctAs_should_throw_if_source_is_null()
        {
            IEnumerable<int> collection = null;
            L action = () => collection.DistinctAs(i => i % 10).ToArray();
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "source");
        }

        [Fact]
        public void DistinctAs_should_throw_if_selector_is_null()
        {
            IEnumerable<int> collection = Enumerable.Range(1, 10);
            L action = () => collection.DistinctAs<int, int>(null).ToArray();
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "selector");
        }

        [Fact]
        public void DistinctAs_should_throw_before_enumeration()
        {
            IEnumerable<int> collection = null;
            L action = () => collection.DistinctAs(i => i % 10);
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "source");
        }
#nullable restore

        [Fact]
        public void DistinctAs_should_only_return_first_distinct_element()
        {
            var source = new[] { 10, 13, 5, 20, 15, 5, 24 };
            var expected = new[] { 10, 13, 5, 24 };
            var result = source.DistinctAs(i => i % 10);
            result.Should().Equal(expected);
        }

#nullable disable
        [Fact]
        public void Union_should_throw_if_source_is_null()
        {
            IEnumerable<int> collection = null;
            L action = () => collection.Union(3);
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "source");
        }

        [Fact]
        public void Union_should_throw_if_otherElements_is_null()
        {
            IEnumerable<int> collection = Enumerable.Range(1, 10);
            L action = () => collection.Union(null);
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "otherElements");
        }
#nullable restore

        [Fact]
        public void Union_should_add_elements_at_the_end_of_collection()
        {
            IEnumerable<int> collection = Enumerable.Range(1, 5);
            var result = collection.Union(6);
            result.Should().Equal(1, 2, 3, 4, 5, 6);
        }

        [Fact]
        public void IsNullOrEmpty_should_be_false_if_collection_has_elements()
        {
            IEnumerable<int>? collection = Enumerable.Range(1, 5);
            var result = collection.IsNullOrEmpty();
            result.Should().BeFalse();
        }

        [Fact]
        public void IsNullOrEmpty_should_be_true_if_collection_is_null()
        {
            IEnumerable<int>? collection = null;
            var result = collection.IsNullOrEmpty();
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNullOrEmpty_should_be_true_if_collection_is_empty()
        {
            IEnumerable<int>? collection = Enumerable.Empty<int>();
            var result = collection.IsNullOrEmpty();
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNotNullNorEmpty_should_be_true_if_collection_has_elements()
        {
            IEnumerable<int>? collection = Enumerable.Range(1, 5);
            var result = collection.IsNotNullNorEmpty();
            result.Should().BeTrue();
        }

        [Fact]
        public void IsNotNullNorEmpty_should_be_false_if_collection_is_null()
        {
            IEnumerable<int>? collection = null;
            var result = collection.IsNotNullNorEmpty();
            result.Should().BeFalse();
        }

        [Fact]
        public void IsNotNullNorEmpty_should_be_false_if_collection_is_empty()
        {
            IEnumerable<int>? collection = Enumerable.Empty<int>();
            var result = collection.IsNotNullNorEmpty();
            result.Should().BeFalse();
        }

        [Fact]
        public void EmptyIfNull_should_return_an_empty_collection_if_source_is_null()
        {
            IEnumerable<int>? collection = null;
            var result = collection.EmptyIfNull();
            result.Should().NotBeNull();
        }

        [Fact]
        public void EmptyIfNull_should_return_source_if_not_null()
        {
            IEnumerable<int>? collection = Enumerable.Range(1, 5);
            var result = collection.EmptyIfNull();
            result.Should().Equal(collection);
        }
    }
}
