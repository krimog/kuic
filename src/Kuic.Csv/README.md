# Kuic.Csv

## Create a CSV file from a collection with default configuration.
```csharp
// We get the collection from somewhere.
IEnumerable<SomeType> collection = GetCollection();

// We export it into a file.
await collection.ToCsv().ToFileAsync("path/to/my/file.csv");
```

## Manually define CSV columns
```csharp
// We get the collection from somewhere.
IEnumerable<SomeType> collection = GetCollection();

// We get the builder object.
var builder = collection.ToCsv();

// We manually define the columns.
builder.AddColumn("First column header", someTypeObject => someTypeObject.firstProperty);

// We can chain the calls
builder
    .AddColumn("Second column header", someTypeObject => someTypeObject.SecondProperty)
    .AddColumn("Third column header", someTypeObject => "A constant value")
    .AddColumn("Fourth column header", someTypeObject => SomeMethod(someTypeObject));

// And finally we write it into a file.
await builder.ToFileAsync("path/to/my/file.csv");
```

## Configure the builder
```csharp
// We get the collection from somewhere.
IEnumerable<SomeType> collection = GetCollection();

// We get the builder configure it.
var builder = collection.ToCsv(c => 
{
    c.AddHeaders = false; // True by default
    c.AddBomInFile = false; // True by default
    c.Encoding = Encoding.ASCII; // UTF8 by default

    c.Culture = new CultureInfo("en-US"); // CultureInfo.CurrentCulture by default
    // or
    c.SetCulture("en-US");

    c.Separator = ";"; // Culture.TextInfo.ListSeparator by default
});

// And finally we write it into a file.
await builder.ToFileAsync("path/to/my/file.csv");
```

## Write into an existing Stream
```csharp
// We get the collection from somewhere.
IEnumerable<SomeType> collection = GetCollection();
await builder.ToStreamAsync(streamToWriteInto);
```

## Write into a new MemoryStream
```csharp
// We get the collection from somewhere.
IEnumerable<SomeType> collection = GetCollection();
var memoryStream = await builder.ToStreamAsync(null);
```