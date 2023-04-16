namespace Test.Generators;

internal static class CompilationErrorProvider
{
    public static void ReportNamespaceError(SourceProductionContext context, string fileName)
    {
        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                "LG001",
                "Expected NamespaceValue for resx file",
                $"You have enabled generation for C# class for resources. Please, set NamespaceValue property in csproj for your resx file: {fileName}",
                "CodeGeneration",
                DiagnosticSeverity.Error,
                true),
            null,
            DiagnosticSeverity.Error);

        context.ReportDiagnostic(diagnostic);
    }

    public static void ReportAndroidResourcesDirectoryError(SourceProductionContext context, string fileName)
    {
        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                "LG002",
                "Expected AndroidResourcesFolderPath for resx file",
                $"You have enabled generation strings.xml for Android. Please, set AndroidResourcesFolderPath property in csproj for your resx file: {fileName}",
                "CodeGeneration",
                DiagnosticSeverity.Error,
                true),
            null,
            DiagnosticSeverity.Error);

        context.ReportDiagnostic(diagnostic);
    }

    public static void ReportClassNameError(SourceProductionContext context, string fileName)
    {
        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                "LG003",
                "Expected ClassName for resx file",
                $"You have enabled generation for C# class for resources. Please, set ClassName property in csproj for your resx file: {fileName}",
                "CodeGeneration",
                DiagnosticSeverity.Error,
                true),
            null,
            DiagnosticSeverity.Error);

        context.ReportDiagnostic(diagnostic);
    }
}

