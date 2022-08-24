using FluentAssertions;
using Kuic.Core.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using Xunit;
using A = System.Func<System.Threading.Tasks.Task>; // Async lambda
using L = System.Action; // Lambda

namespace Kuic.Csv.Tests
{
    [ExcludeFromCodeCoverage]
    public class CsvBuilderTests
    {
        [Fact]
        public void ToCsv_on_null_enumerable_should_throw()
        {
            IEnumerable<CsvTestObject> enumerable = null!;
            L action = () => enumerable.ToCsv();
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "source");
        }

        [Fact]
        public void ToCsv_with_null_configuration_should_throw()
        {
            var enumerable = GetEnumerable();
            L action = () => enumerable.ToCsv((CsvConfiguration)null!);
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "configuration");
        }

        [Fact]
        public void ToCsv_with_null_configure_should_throw()
        {
            var enumerable = GetEnumerable();
            L action = () => enumerable.ToCsv((Action<CsvConfiguration>)null!);
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "configure");
        }

        [Fact]
        public void Ctor_with_null_enumerable_should_throw()
        {
            IEnumerable<CsvTestObject> enumerable = null!;
            L action = () => new CsvBuilder<CsvTestObject>(enumerable);
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "sourceData");
        }

        [Fact]
        public void Ctor_with_null_configuration_should_throw()
        {
            IEnumerable<CsvTestObject> enumerable = GetEnumerable();
            L action = () => new CsvBuilder<CsvTestObject>(enumerable, null!);
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "configuration");
        }

        [Fact]
        public void Ctor_with_no_configuration_should_create_default()
        {
            var enumerable = GetEnumerable();
            var builder = new CsvBuilder<CsvTestObject>(enumerable);
            builder.Configuration.Should().BeEquivalentTo(new CsvConfiguration());
        }

        [Fact]
        public void Configuration_should_be_passable_in_parameters()
        {
            var builder = GetEnumerable().ToCsv(new CsvConfiguration { Separator = "#" });
            builder.Configuration.Separator.Should().Be("#");
        }

        [Fact]
        public void Configuration_should_be_settable_with_lambda()
        {
            var builder = GetEnumerable().ToCsv(c => c.Separator = "#");
            builder.Configuration.Separator.Should().Be("#");
        }

        [Fact]
        public void Configuration_should_be_settable_afterwards()
        {
            var builder = GetEnumerable().ToCsv();
            builder.Configuration = new CsvConfiguration { Separator = "#" };
            builder.Configuration.Separator.Should().Be("#");
        }

        [Fact]
        public void Configuration_should_not_be_settable_to_null_afterwards()
        {
            var builder = GetEnumerable().ToCsv();
            L action = () => builder.Configuration = null!;
            action.Should().Throw<ArgumentNullException>().Where(ex => ex.ParamName == "value");
        }

        [Fact]
        public async Task ToStreamAsync_should_auto_generate_columns()
        {
            var builder = GetEnumerable().ToCsv(c => c.Separator = ";");
            using var stream = await builder.ToStreamAsync(null);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var firstLine = await reader.ReadLineAsync();
            firstLine.Should().Be($"{nameof(CsvTestObject.StringProperty)};{nameof(CsvTestObject.DoubleProperty)}");
        }

        [Fact]
        public async Task ToStreamAsync_should_escape_double_quotes_in_values()
        {
            var builder = new CsvTestObject { StringProperty = "Hello\"World" }.AsEnumerable().ToCsv(c => c.AddHeaders = false).AddColumn("String", o => o.StringProperty);
            using var stream = await builder.ToStreamAsync(null);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var firstLine = await reader.ReadLineAsync();
            firstLine.Should().Be("\"Hello\"\"World\"");
        }

        [Fact]
        public async Task ToStreamAsync_should_escape_separator_in_values()
        {
            var builder = new CsvTestObject { StringProperty = "Hello;World" }.AsEnumerable().ToCsv(c =>
            {
                c.AddHeaders = false;
                c.Separator = ";";
            }).AddColumn("String", o => o.StringProperty);
            using var stream = await builder.ToStreamAsync(null);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var firstLine = await reader.ReadLineAsync();
            firstLine.Should().Be("\"Hello;World\"");
        }

        [Fact]
        public async Task ToStreamAsync_should_not_escape_non_separator_in_values()
        {
            var builder = new CsvTestObject { StringProperty = "Hello;World" }.AsEnumerable().ToCsv(c =>
            {
                c.AddHeaders = false;
                c.Separator = ",";
            }).AddColumn("String", o => o.StringProperty);
            using var stream = await builder.ToStreamAsync(null);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var firstLine = await reader.ReadLineAsync();
            firstLine.Should().Be("Hello;World");
        }

        [Fact]
        public async Task ToStreamAsync_should_escape_new_line_in_values()
        {
            var builder = new CsvTestObject { StringProperty = "Hello\nWorld" }.AsEnumerable().ToCsv(c =>
            {
                c.AddHeaders = false;
                c.Separator = ";";
            }).AddColumn("String", o => o.StringProperty);
            using var stream = await builder.ToStreamAsync(null);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var firstLine = await reader.ReadToEndAsync();
            firstLine.Trim().Should().Be("\"Hello\nWorld\"");
        }

        [Fact]
        public async Task ToStreamAsync_should_escape_double_quotes_in_header()
        {
            var builder = new CsvTestObject { StringProperty = "Hello\"World" }.AsEnumerable().ToCsv().AddColumn("\"String\"", o => o.StringProperty);
            using var stream = await builder.ToStreamAsync(null);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var firstLine = await reader.ReadLineAsync();
            firstLine.Should().Be("\"\"\"String\"\"\"");
        }

        [Fact]
        public async Task ToStreamAsync_should_throw_if_a_column_has_no_header()
        {
            var builder = new CsvTestObject { StringProperty = "Hello\"World" }.AsEnumerable().ToCsv(c => c.AddHeaders = true).AddColumn(o => o.StringProperty);
            A action = () => builder.ToStreamAsync(null);
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task ToStreamAsync_should_use_the_given_stream()
        {
            var builder = new CsvTestObject { StringProperty = "Hello\"World" }.AsEnumerable().ToCsv(c => c.AddHeaders = true).AddColumn("String", o => o.StringProperty);
            using var memStream = new MemoryStream();
            var resultStream = await builder.ToStreamAsync(memStream);
            resultStream.Should().BeSameAs(memStream);
        }

        [Fact]
        public async Task ToStreamAsync_should_format_with_culture()
        {
            foreach (var cultureName in new[] { "fr-FR", "en-US", "ja-JP" })
            {
                var culture = new CultureInfo(cultureName);
                var builder = GetEnumerable().ToCsv(c =>
                {
                    c.Separator = ";";
                    c.Culture = culture;
                    c.AddHeaders = false;
                });

                using var stream = await builder.ToStreamAsync(null);
                stream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(stream);
                var currentLine = await reader.ReadLineAsync();

                CsvTestObject elt = GetEnumerable().First();
                currentLine.Should().Be($"{elt.StringProperty?.ToString(culture)};{elt.DoubleProperty.ToString(culture)}");
            }
        }

        [Fact]
        public async Task ToStreamAsync_should_use_given_columns()
        {
            var builder = GetEnumerable().ToCsv(c => c.Separator = ";")
                .AddColumn("Column1", _ => string.Empty)
                .AddColumn("Column2", _ => string.Empty)
                .AddColumn("Column3", _ => string.Empty);

            using var stream = await builder.ToStreamAsync(null);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            var firstLine = await reader.ReadLineAsync();
            firstLine.Should().Be("Column1;Column2;Column3");
        }

        [Fact]
        public async Task ToFileAsync_should_create_the_file()
        {
            var filePath = $"./test-{Guid.NewGuid()}.csv";
            try
            {
                await GetEnumerable().ToCsv().ToFileAsync(filePath);
                File.Exists(filePath).Should().BeTrue();
            }
            finally
            {
                if (File.Exists(filePath)) File.Delete(filePath);
            }
        }

        [Fact]
        public async Task ToFileAsync_should_contain_bom()
        {
            var filePath = $"./test-{Guid.NewGuid()}.csv";
            try
            {
                Encoding.UTF32.GetPreamble().Should().NotBeEmpty();
                await GetEnumerable().ToCsv(new CsvConfiguration { AddBomInFile = true, Encoding = Encoding.UTF32 }).ToFileAsync(filePath);
                var fileContent = await File.ReadAllBytesAsync(filePath);
                fileContent.Should().StartWith(Encoding.UTF32.GetPreamble());
            }
            finally
            {
                if (File.Exists(filePath)) File.Delete(filePath);
            }
        }

        [Fact]
        public async Task ToFileAsync_should_match_exactly()
        {
            var filePath = $"./test-{Guid.NewGuid()}.csv";
            try
            {
                await GetEnumerable().Concat(new CsvTestObject { StringProperty = "Escaped;", DoubleProperty = 1000 }.AsEnumerable()).ToCsv(c =>
                {
                    c.AddBomInFile = true;
                    c.Encoding = Encoding.UTF8;
                    c.SetCulture("en-US");
                    c.AddHeaders = true;
                    c.Separator = ";";
                })
                    .AddColumn("String column", o => o.StringProperty)
                    .AddColumn("Double column", o => o.DoubleProperty * 2D)
                    .ToFileAsync(filePath);
                var fileContent = await File.ReadAllBytesAsync(filePath);
                var expectedBytes = await Assembly.GetExecutingAssembly().GetManifestResourceStream("Kuic.Csv.Tests.ExpectedResults.Expected.csv")!.ReadAllBytesAsync();

                fileContent.Should().Equal(expectedBytes);
            }
            finally
            {
                if (File.Exists(filePath)) File.Delete(filePath);
            }
        }

        [Fact]
        public async Task ToFileAsync_should_throw_if_filePath_is_null()
        {
            string filePath = null!;
            A action = () => GetEnumerable().ToCsv().ToFileAsync(filePath);
            await action.Should().ThrowAsync<ArgumentNullException>().Where(ex => ex.ParamName == "filePath");
        }

        private IEnumerable<CsvTestObject> GetEnumerable()
        {
            return Enumerable.Range(0, 100).Select(i => new CsvTestObject { SetOnlyProperty = $"Str{i}", DoubleProperty = i / 10D });
        }
    }

    [ExcludeFromCodeCoverage]
    internal class CsvTestObject
    {
        public string? StringProperty { get; set; }
        public double DoubleProperty { get; set; }
        public string SetOnlyProperty { set => StringProperty = value; }
        public int this[int nb] => nb;
    }
}