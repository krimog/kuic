using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Xunit;
using L = System.Action; // Lambda

namespace Kuic.Csv.Tests
{
    [ExcludeFromCodeCoverage]
    public class CsvColumnTests
    {
        [Fact]
        public void Ctor_with_header_but_empty_accessor_sould_throw()
        {
            L action = () => new CsvColumn<object, object>("Header", null!);
            action.Should().Throw<ArgumentException>().Where(ex => ex.ParamName == "accessor");
        }

        [Fact]
        public void Ctor_with_empty_accessor_sould_throw()
        {
            L action = () => new CsvColumn<object, object>(null!);
            action.Should().Throw<ArgumentException>().Where(ex => ex.ParamName == "accessor");
        }

        [Fact]
        public void GetStringValue_should_return_string_empty_when_accessor_returns_null()
        {
            var column = new CsvColumn<string, object?>(_ => null);
            column.GetStringValue("Value", CultureInfo.InvariantCulture).Should().NotBeNull();
            column.GetStringValue("Value", CultureInfo.InvariantCulture).Should().BeEmpty();
        }
    }
}
