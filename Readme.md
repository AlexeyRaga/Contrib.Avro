# Avro Code Generator for C#

This library provides a source generator for Avro types, generating C# code compatible with the Apache.Avro package. The
generated code implements interfaces such as `ISpecificRecord` and `FixedRecord`, making it fully interoperable with
functionality that works with the Apache.Avro generated code.

## Features

- **C# Classes and Records**: Generates C# classes or records for Avro records. Using records provides benefits like
  structural equality and `ToString()` implementation.
- **Required Fields**: Marks properties for non-optional fields as `required`, making it easier to catch unset required
  fields at compile time.
- **Namespace Mapping**: Correctly maps namespaces.
- **Union Types**: Supports union types (instead of just generating `object` fields for unions as Apache.Avro does).
- **Type Mappings**: Supports custom logical types, wrapper types, and an option to not fail on unknown logical types
  (can be configured to fail if required).
- **Types Hints**: Provides hints for types to wrap/convert them.
- **Configuration**: Can be configured globally or per AVSC file.

## Configuration

### Global Configuration

You can configure the generator using project-wide settings in your `.csproj` file:

```xml

<PropertyGroup>
    <Avro_FailUnknownLogicalTypes>true</Avro_FailUnknownLogicalTypes>
    <Avro_GenerateRecords>true</Avro_GenerateRecords>
    <Avro_GenerateRequiredFields>true</Avro_GenerateRequiredFields>
    <Avro_DebuggerDisplayFields>required</Avro_DebuggerDisplayFields>
    <Avro_TypeMappings>user-id:Contrib.Avro.CodeGen.Tests.UserId</Avro_TypeMappings>
    <Avro_NamespaceMapping>Original.Namespace.Name:Testing.Messages</Avro_NamespaceMapping>
</PropertyGroup>
```

### Per-File Configuration

You can also configure settings for specific AVSC files:

```xml

<ItemGroup>
    <AdditionalFiles Include="$(PkgTest_avro_contract)\schemas\**\*.avsc"
                     NamespaceMapping="Original.Namespace.Name:New.Namespace.Name"
                     TypeMappings="duration:System.TimeSpan"
                     DebuggerDisplayFields="required"
                     GenerateRecords="true"
                     GenerateRequiredFields="true"
                     FailUnknownLogicalTypes="true"
    />
</ItemGroup>
```

## Configuration Options

- **`NamespaceMapping`**: Specifies namespace mappings. This is a dictionary where the key is the original namespace and
  the value is the new namespace.
    - **Format**: Comma-separated list of `Original.Namespace.Name:New.Namespace.Name` pairs.
    - **Default**: Empty dictionary
    - **Global Configuration Example**:
      ```xml
      <Avro_NamespaceMapping>Original.Namespace.Name:Testing.Messages</Avro_NamespaceMapping>
      ```
    - **Per-File Configuration Example**:
      ```xml
      <AdditionalFiles Include="$(PkgTest_avro_contract)\schemas\**\*.avsc" 
                       NamespaceMapping="Original.Namespace.Name:New.Namespace.Name" />
      ```

- **`GenerateRecords`**: Determines whether to generate C# records instead of classes for Avro records.
    - **Default**: `true`
    - **Global Configuration Example**:
      ```xml
      <Avro_GenerateRecords>false</Avro_GenerateRecords>
      ```
    - **Per-File Configuration Example**:
      ```xml
      <AdditionalFiles Include="$(PkgTest_avro_contract)\schemas\**\*.avsc" 
                       GenerateRecords="false" />
      ```

- **`GenerateRequiredFields`**: Marks properties for non-optional fields as `required`, making it easier to catch unset
  required fields at compile time.
    - **Default**: `true`
    - **Global Configuration Example**:
      ```xml
      <Avro_GenerateRequiredFields>false</Avro_GenerateRequiredFields>
      ```
    - **Per-File Configuration Example**:
      ```xml
      <AdditionalFiles Include="$(PkgTest_avro_contract)\schemas\**\*.avsc" 
                       GenerateRequiredFields="false" />
      ```

- **`DebuggerDisplayFields`**: Specifies which fields should be displayed in the debugger.
    - **Possible values**: `none`,`required`, or `all`.
    - **Default**: `none`
    - **Global Configuration Example**:
      ```xml
      <Avro_DebuggerDisplayFields>required</Avro_DebuggerDisplayFields>
      ```
    - **Per-File Configuration Example**:
      ```xml
      <AdditionalFiles Include="$(PkgTest_avro_contract)\schemas\**\*.avsc" 
                       DebuggerDisplayFields="required" />
      ```

- **`FailUnknownLogicalTypes`**: Specifies whether the generator should fail on unknown logical types.
    - **Default**: `false`
    - **Global Configuration Example**:
      ```xml
      <Avro_FailUnknownLogicalTypes>true</Avro_FailUnknownLogicalTypes>
      ```
    - **Per-File Configuration Example**:
      ```xml
      <AdditionalFiles Include="$(PkgTest_avro_contract)\schemas\**\*.avsc" 
                       FailUnknownLogicalTypes="true" />
      ```

- **`LogicalTypes`**: Defines custom logical type mappings. This is a dictionary where the key is the logical type name
  and the value is the corresponding C# type.
    - **Format**: Comma-separated list of `logical-type-name:CSharp.Type` pairs.
    - **Default**: Empty dictionary
    - **Global Configuration Example**:
      ```xml
      <Avro_LogicalTypes>user-id:Contrib.Avro.CodeGen.Tests.UserId</Avro_LogicalTypes>
      ```
    - **Per-File Configuration Example**:
      ```xml
      <AdditionalFiles Include="$(PkgTest_avro_contract)\schemas\**\*.avsc" LogicalTypes="duration:System.TimeSpan" />
      ```

## Installation

Add the source generator package to your project:

```xml

<ItemGroup>
    <PackageReference Include="Contrib.Avro.Codegen" Version="1.0.0"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="true"/>
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
