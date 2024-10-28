# Avro Code Generator for C#

This library provides a source generator for Avro types, generating C# code compatible with the Apache.Avro package. The
generated code implements interfaces such as `ISpecificRecord` and `FixedRecord`, making it fully interoperable with
functionality that works with the Apache.Avro generated code.

## Features Overview

- **C# Classes and Records**: Generates C# classes or records for Avro records. Using records provides benefits like
  structural equality and `ToString()` implementation.


- **Required Fields**: Marks properties for non-optional fields as `required`, making it easier to catch unset required
  fields at compile time.


- **Namespace Mapping**: Correctly maps namespaces.


- **Union Types**: Supports union types (instead of just generating `object` fields for unions as Apache.Avro does).


- **Type Mappings**: Support for
    - Custom logical types
    - Fail on unknown logical types (can be configured to fail if required, as a fallback to Apache Avro behaviour).


- **Type Hints**: Provides hints for types to wrap/convert them.


- **Configuration**: Can be configured globally or per schema file.

## Type hints and Wrapping Types

The generator supports wrapping types in custom classes.
This feature is useful to provide "custom" or even domain-specific types for generated messages.

Consider this example:

```json
{
  "type": "record",
  "name": "Message",
  "fields": [
    {
      "name": "Id",
      "type": {
        "type": "int",
        "typeHint": "user-id"
      }
    }
  ]
}
```

Here the schema provides a hint that the `Id` field represents some "user id".
The generator now can "interpret" this hint and wrap the `Id` field in a custom class, like `UserId`.

We can enable it by providing a mapping for the `user-id` hint into a type
(read [Configuration](#configuration) section for more details):

```json
{
  "TypeHintName": "typeHint",
  "TypeMappings": {
    "user-id": "MyDomainNamespace.UserId"
  }
}
```

The generator will now generate property `Id` of type `UserId` instead of `int`.:

```csharp
public required MyDomainNamespace.UserId Id { get; set; }
```

### How does it work?

The Type Wrappers feature makes use of dotnet's 
[Type Conversion](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter?view=net-8.0) 
mechanism.
The generated code will expect a
[TypeConverter](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.typeconverter?view=net-8.0) 
for the custom type, which will be used to convert 
the custom type to/from the underlying "base" type.

If you use something like [Strongly](https://github.com/lucasteles/Strongly) 
then there converters can be generated "for free" by these very useful libraries.
Otherwise, you will need to implement the `TypeConverter` yourself.

### Type wrappers vs. Logical Types

Type wrappers are similar to Logical Types in Avro, but they are less invasive.
With type hints there is no requirement or expectation for the clients to know about it, or to handle it.
Other clients are free to ignore the hint, or to interpret it in any way they want.

And logical types can also be wrapped:

```json
{
  "type": "record",
  "name": "Message",
  "fields": [
    {
      "name": "Id",
      "type": {
        "type": "string",
        "logicalType": "uuid",
        "typeHint": "user-id"
      }
    }
  ]
}
```
In this case, if `user-id` is "unknown" of gets ignored, then the `Id` field will be interpreted as `Guid`,
since `uuid` is a known logical type.

But we did use `user-id` as a logical type, then the clients that don't know about this logical type
would either have to fail by saying "unknown logical type" 
or to ignore the logical type and interpret the field as `string`.
Which would be much less convenient than having a `Guid`.

This way, type hints can be viewed as a "lighter" version of logical types
or as an interpretable piece of documentation.

Note that the attribute name itself, `typeHint`, is arbitrary and can be changed in the configuration,
globally of for each schema file individually.

## Configuration

This tool can be configured using the following properties:

- `NamespaceMappings`: A dictionary that maps original namespaces to new namespaces.
- `GenerateRecords`: (default: `true`) A boolean that specifies whether to generate record types (`true` or `false`).
- `GenerateRequiredFields`: (default: `true`) A boolean that specifies whether to generate required fields (`true` or `false`).
- `DebuggerDisplayFields`: (default: `none`) Defines the fields displayed in the debugger. Possible values are:
    - `required`: Only required fields will be displayed.
    - `all`: All fields will be displayed.
    - `none`: No fields will be displayed.
- `FailUnknownLogicalTypes` (default: `false`): A boolean that determines whether the process should fail if unknown logical types are encountered (`true` or `false`).
- `TypeMappings`: A dictionary that maps logical types to specific class names.
- `TypeHintName`: (default: `typeHint`) The name used to add type hints in the generated code.

There are a couple of ways to configure set these properties for the generator:

- Globally, using an `avrogen.config.json` file.
- Globally, using properties in the `.csproj` file.
- Per-file, using properties in the `.csproj` file.

You can use any combination of these methods to configure the generator.
The per-file configuration will override the global configuration.

#### Global configuration using `avrogen.config.json`

Create an `avrogen.config.json` file in the root of your project:

```json
{
  "NamespaceMappings": {
    "Original.Namespace.Name": "Testing.Messages"
  },
  "GenerateRecords": false,
  "GenerateRequiredFields": false,
  "DebuggerDisplayFields": "required",
  "FailUnknownLogicalTypes": true,
  "TypeMappings": {
    "user-id": "Contrib.Avro.CodeGen.Tests.UserId",
    "department-id": "Contrib.Avro.CodeGen.Tests.DepartmentId"
  },
  "TypeHintName": "typeHint"
}
```

and add it to your project file:

```xml
<ItemGroup>
    <AdditionalFiles Include="avrogen.config.json" />
</ItemGroup>
```

#### Global Configuration using `.csproj` Properties

These properties can also be set in the `.csproj` file, with `Avro_` prefix 
(to avoid potential conflicts with other properties):

```xml

<PropertyGroup>
    <Avro_FailUnknownLogicalTypes>true</Avro_FailUnknownLogicalTypes>
    <Avro_GenerateRecords>true</Avro_GenerateRecords>
    <Avro_GenerateRequiredFields>true</Avro_GenerateRequiredFields>
    <Avro_DebuggerDisplayFields>required</Avro_DebuggerDisplayFields>
    <Avro_TypeMappings>user-id:Contrib.Avro.CodeGen.Tests.UserId,department-id:Contrib.Avro.CodeGen.Tests.DepartmentId</Avro_TypeMappings>
    <Avro_NamespaceMappings>Original.Namespace.Name:Testing.Messages</Avro_NamespaceMappings>
    <Avro_TypeHintName>typeHint</Avro_TypeHintName>
</PropertyGroup>
```

Values for "dictionary" properties (`TypeMappings` and `NamespaceMappings`) should be 
comma-separated lists of `key:value` pairs.

**Note**: Sadly, C# Source generators do not support passing multi-line values to MSBuild properties, 
therefore comma-separated lists of key-value pairs are used.

#### Per-File Configuration

You can also configure settings for specific AVSC files.
These settings will override the global settings for the specific file.

```xml

<ItemGroup>
    <AdditionalFiles Include="$(PkgTest_avro_contract)\schemas\**\*.avsc"
                     NamespaceMappings="Original.Namespace.Name:New.Namespace.Name"
                     TypeMappings="duration:System.TimeSpan"
                     DebuggerDisplayFields="required"
                     GenerateRecords="true"
                     GenerateRequiredFields="true"
                     FailUnknownLogicalTypes="true"
    />
</ItemGroup>
```

Values for "dictionary" properties (`TypeMappings` and `NamespaceMappings`) should be
comma-separated lists of `key:value` pairs.

**Note**: Sadly, C# Source generators do not support passing multi-line values to MSBuild properties,
therefore comma-separated lists of key-value pairs are used.


## Installation

Add the source generator package to your project:

```xml

<ItemGroup>
    <PackageReference Include="Contrib.Avro.Codegen" Version="1.0.0"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false"/>
</ItemGroup>
```

Additionally, reference `Contrib.Avro` for common types, such as the `Choice` type used for unions:

```xml

<ItemGroup>
    <PackageReference Include="Contrib.Avro" Version="1.0.0"/>
</ItemGroup>
```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request on GitHub.
