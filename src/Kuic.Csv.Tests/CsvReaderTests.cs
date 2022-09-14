using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Kuic.Core.Helpers;
using Xunit;
using A = System.Func<System.Threading.Tasks.Task>; // Async lambda
using L = System.Action; // Lambda

namespace Kuic.Csv.Tests
{
    [ExcludeFromCodeCoverage]
    public class CsvReaderTests
    {
        [Fact]
        public void Constructor_should_throw_if_stream_is_null()
        {
            L action = () => _ = new CsvReader<CsvTestObject>((Stream)null!);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_correctly_with_index_columns()
        {
            var csv = """
                Hello World;Ignore;10,3
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(false))
                .AddColumn<string>((o, v) => o.StringProperty = v, 0)
                .AddColumn<double>((o, v) => o.DoubleProperty = v, 2);

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("Hello World");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be("Second line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_correctly_with_column_names()
        {
            var csv = """
                First Column;Second Column;Third Column
                Hello World;Ignore;10,3
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true))
                .AddColumn<string>((o, v) => o.StringProperty = v, "First Column")
                .AddColumn<double>((o, v) => o.DoubleProperty = v, "Third Column");

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("Hello World");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be("Second line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_correctly_with_property_names_and_headers()
        {
            var csv = """
                First Column;Second Column;Third Column
                Hello World;Ignore;10,3
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true))
                .AddColumn(nameof(CsvTestObject.StringProperty), "First Column")
                .AddColumn(nameof(CsvTestObject.DoubleProperty), "Third Column");

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("Hello World");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be("Second line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_correctly_with_property_names_and_indexes()
        {
            var csv = """
                First Column;Second Column;Third Column
                Hello World;Ignore;10,3
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true))
                .AddColumn(nameof(CsvTestObject.StringProperty), 0)
                .AddColumn(nameof(CsvTestObject.DoubleProperty), 2);

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("Hello World");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be("Second line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_correctly_with_property_accessors_and_headers()
        {
            var csv = """
                First Column;Second Column;Third Column
                Hello World;Ignore;10,3
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true))
                .AddColumn(o => o.StringProperty, "First Column")
                .AddColumn(o => o.DoubleProperty, "Third Column");

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("Hello World");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be("Second line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_correctly_with_property_accessors_and_indexes()
        {
            var csv = """
                First Column;Second Column;Third Column
                Hello World;Ignore;10,3
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true))
                .AddColumn(o => o.StringProperty, 0)
                .AddColumn(o => o.DoubleProperty, 2);

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("Hello World");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be("Second line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_with_double_quotes()
        {
            var csv = """
                "Hello World";"Ignore";"10,3"
                "Second line";"Still ignore";"5,97"
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(false))
                .AddColumn<string>((o, v) => o.StringProperty = v, 0)
                .AddColumn<double>((o, v) => o.DoubleProperty = v, 2);

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("Hello World");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be("Second line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_with_separator_inside_double_quotes()
        {
            var csv = """
                "Hello;World";"Ignore";"10,3"
                "Second;line";"Still ignore";"5,97"
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(false))
                .AddColumn<string>((o, v) => o.StringProperty = v, 0)
                .AddColumn<double>((o, v) => o.DoubleProperty = v, 2);

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("Hello;World");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be("Second;line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_with_new_line_inside_double_quotes()
        {
            var csv = """
                "Hello
                World";"Ignore";"10,3"
                "Second
                line";"Still ignore";"5,97"
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(false))
                .AddColumn<string>((o, v) => o.StringProperty = v, 0)
                .AddColumn<double>((o, v) => o.DoubleProperty = v, 2);

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be($"Hello{Environment.NewLine}World");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be($"Second{Environment.NewLine}line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_with_double_quotes_inside_double_quotes()
        {
            var csv = """"
                """Hello World""";"Ignore";"10,3"
                """Second""line""";"Still ignore";"5,97"
                """";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(false))
                .AddColumn<string>((o, v) => o.StringProperty = v, 0)
                .AddColumn<double>((o, v) => o.DoubleProperty = v, 2);

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("\"Hello World\"");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be("\"Second\"line\"");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_empty_columns()
        {
            var csv = """
                "Hello World";"Ignore";
                "Second line";;"5,97"
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(false))
                .AddColumn<string>((o, v) => o.StringProperty = v, 0)
                .AddColumn<double>((o, v) => o.DoubleProperty = v, 2);

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("Hello World");
            result[0].DoubleProperty.Should().Be(0);
            result[1].StringProperty.Should().Be("Second line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_read_correctly_with_auto_generated_columns()
        {
            var csv = """
                StringProperty;Second Column;DoubleProperty
                Hello World;Ignore;10,3
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true));

            var result = await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            result.Should().HaveCount(2);
            result[0].StringProperty.Should().Be("Hello World");
            result[0].DoubleProperty.Should().Be(10.3);
            result[1].StringProperty.Should().Be("Second line");
            result[1].DoubleProperty.Should().Be(5.97);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_throw_format_exception_for_unclosed_double_quote()
        {
            var csv = """
                StringProperty;Second Column;DoubleProperty
                Hello World;Ignore;10,3
                "Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true));

            A action = async () => await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            await action.Should().ThrowAsync<CsvFormatException>().Where(ex => ex.Line == 3 && !ex.Character.HasValue);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_throw_format_exception_for_opening_double_quote_late()
        {
            var csv = """
                StringProperty;Second Column;DoubleProperty
                Hello" World;Ignore;10,3
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true));

            A action = async () => await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            await action.Should().ThrowAsync<CsvFormatException>().Where(ex => ex.Line == 2 && ex.Character == 6);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_throw_format_exception_for_closing_double_quote_early()
        {
            var csv = """
                StringProperty;Second Column;DoubleProperty
                "Hello" World;Ignore;10,3
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true));

            A action = async () => await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            await action.Should().ThrowAsync<CsvFormatException>().Where(ex => ex.Line == 2 && ex.Character == 8);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_throw_format_exception_for_too_many_columns()
        {
            var csv = """
                StringProperty;Second Column;DoubleProperty
                Hello World;Ignore;10,3;
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true));

            A action = async () => await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            await action.Should().ThrowAsync<CsvFormatException>().Where(ex => ex.Line == 2 && !ex.Character.HasValue);
        }

        [Fact]
        public async Task ReadAsAsyncEnumerable_should_throw_format_exception_for_too_few_columns()
        {
            var csv = """
                StringProperty;Second Column;DoubleProperty
                Hello World;Ignore
                Second line;Still ignore;5,97
                """;
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            using var reader = new CsvReader<CsvTestObject>(stream, GetConfiguration(true));

            A action = async () => await reader.ReadAsAsyncEnumerable().ToArrayAsync();
            await action.Should().ThrowAsync<CsvFormatException>().Where(ex => ex.Line == 2 && !ex.Character.HasValue);
        }

        private static ICsvReaderConfiguration GetConfiguration(bool hasHeaders)
        {
            ICsvReaderConfiguration configuration = new CsvConfiguration();
            configuration.SetCulture("fr-FR");
            configuration.Separator = ";";
            configuration.HasHeaders = hasHeaders;
            return configuration;
        }
    }
}
