using FluentAssertions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Xunit;
using L = System.Action; // Lambda

namespace Kuic.Csv.Tests
{
    [ExcludeFromCodeCoverage]
    public class CsvConfigurationTests
    {
        [Fact]
        public void Configuration_should_use_current_culture()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                foreach (var cultureName in new[] { "fr-FR", "en-US", "ja-JP" })
                {
                    CultureInfo.CurrentCulture = new CultureInfo(cultureName);
                    var configuration = new CsvConfiguration();
                    configuration.Culture.Should().Be(CultureInfo.CurrentCulture);
                }
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        [Fact]
        public void Configuration_should_use_current_culture_separator()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                foreach (var cultureName in new[] { "fr-FR", "en-US", "ja-JP" })
                {
                    CultureInfo.CurrentCulture = new CultureInfo(cultureName);
                    var configuration = new CsvConfiguration();
                    configuration.Separator.Should().Be(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                }
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        [Fact]
        public void Separator_should_not_contain_double_quotes()
        {
            var configuration = new CsvConfiguration();
            L action = () => configuration.Separator = "\"";
            action.Should().Throw<ArgumentException>().Where(ex => ex.ParamName == "value");
        }

        [Fact]
        public void Separator_should_not_contain_new_lines()
        {
            var configuration = new CsvConfiguration();
            L action = () => configuration.Separator = "\n";
            action.Should().Throw<ArgumentException>().Where(ex => ex.ParamName == "value");
        }

        [Fact]
        public void Separator_should_not_be_empty()
        {
            var configuration = new CsvConfiguration();
            L action = () => configuration.Separator = string.Empty;
            action.Should().Throw<ArgumentException>().Where(ex => ex.ParamName == "value");
        }

        [Fact]
        public void Separator_should_not_be_null()
        {
            var configuration = new CsvConfiguration();
            L action = () => configuration.Separator = null!;
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "value");
        }

        [Fact]
        public void Encoding_should_not_be_null()
        {
            var configuration = new CsvConfiguration();
            L action = () => configuration.Encoding = null!;
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "value");
        }

        [Fact]
        public void Culture_should_not_be_null()
        {
            var configuration = new CsvConfiguration();
            L action = () => configuration.Culture = null!;
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "value");
        }

        [Fact]
        public void SetCulture_should_throw_if_parameter_is_null()
        {
            var configuration = new CsvConfiguration();
            L action = () => configuration.SetCulture(null!);
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "cultureName");
        }

        [Fact]
        public void Culture_should_not_change_separator_if_set_manually()
        {
            var configuration = new CsvConfiguration();
            configuration.Separator = "#";
            configuration.SetCulture("fr-FR");
            configuration.SetCulture("en-US");
            configuration.Separator.Should().Be("#");
        }

        [Fact]
        public void Culture_should_change_separator()
        {
            var configuration = new CsvConfiguration();
            configuration.SetCulture("fr-FR");
            var frSeparator = configuration.Separator;
            configuration.SetCulture("en-US");
            configuration.Separator.Should().NotBe(frSeparator);
        }
    }
}
