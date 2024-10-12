using Microsoft.CodeAnalysis;

namespace Contrib.Avro.Codegen;

public static class Diagnostics
{
    private static readonly DiagnosticDescriptor InfoDescriptor = new(
        id: "AVRO101",
        title: "Info",
        messageFormat: "{0}",
        category: "SourceGeneration",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true
    );

    private static readonly DiagnosticDescriptor ErrorDescriptor = new(
        id: "AVRO301",
        title: "Error",
        messageFormat: "{0}",
        category: "SourceGeneration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static void Info(this SourceProductionContext context, string message, Location? location = null) =>
        context.ReportDiagnostic(Diagnostic.Create(InfoDescriptor, location ?? Location.None, message));

    public static void Info(this GeneratorExecutionContext context, string message, Location? location = null) =>
        context.ReportDiagnostic(Diagnostic.Create(InfoDescriptor, location ?? Location.None, message));

    public static void Error(this SourceProductionContext context, string message, Location? location = null) =>
        context.ReportDiagnostic(Diagnostic.Create(ErrorDescriptor, location ?? Location.None, message));

    public static void Error(this GeneratorExecutionContext context, string message, Location? location = null) =>
        context.ReportDiagnostic(Diagnostic.Create(ErrorDescriptor, location ?? Location.None, message));
}
