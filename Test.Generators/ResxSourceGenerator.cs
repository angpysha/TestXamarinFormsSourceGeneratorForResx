using Test.Generators.Models;

namespace Test.Generators;

[Generator]
public class ResxSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // #if DEBUG
        //             if (!Debugger.IsAttached)
        //             {
        //                 Debugger.Launch();
        //             }
        // #endif

        var resxFiles = context.AdditionalTextsProvider
            .Where(static atp => atp.Path.Contains("resx"))
            .Collect();

        var resxFilesPlusCompilation = context.CompilationProvider.Combine(resxFiles);
        var resxFilesPlusCompilationPlusAnalyzerConfigOptions = context.AnalyzerConfigOptionsProvider.Combine(resxFilesPlusCompilation);

        var incrementalContainer = resxFilesPlusCompilationPlusAnalyzerConfigOptions
            .Select((tuple, _) => new IncrementalValueProviderContainer(
                AnalyzerConfigOptionsProvider: tuple.Left,
                Compilation: tuple.Right.Left,
                AdditionalTexts: tuple.Right.Right));

        context.RegisterSourceOutput(incrementalContainer, (sourceProductionContext, valueProviderContainer) =>
        {
            if (valueProviderContainer.AdditionalTexts.LastOrDefault() is null)
            {
                return;
            }

            var options = valueProviderContainer.AnalyzerConfigOptionsProvider.GetOptions(valueProviderContainer.AdditionalTexts.LastOrDefault()!);

            var additionalFileConfigInfo =
                AdditionalFileInfoFactory.Create(options, valueProviderContainer.AdditionalTexts.LastOrDefault()!);

            if (additionalFileConfigInfo is { GenerateSharedClass: true, Culture: null })
            {
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(additionalFileConfigInfo.FilePath);
                var generatedFileName = fileNameWithoutExtension + ".g.cs";

                var className = additionalFileConfigInfo.ClassName;
                var namespaceValue = additionalFileConfigInfo.Namespace;

                if (string.IsNullOrWhiteSpace(className))
                {
                    className = fileNameWithoutExtension;
                }

                if (string.IsNullOrWhiteSpace(namespaceValue))
                {
                    var mainSyntaxTree = valueProviderContainer.Compilation.SyntaxTrees.First(x => x.HasCompilationUnitRoot);

                    var rootDirectory = Path.GetDirectoryName(mainSyntaxTree.FilePath);
                    var resxDirectory = Path.GetDirectoryName(additionalFileConfigInfo.FilePath);

                    if (string.IsNullOrWhiteSpace(rootDirectory) == false && string.IsNullOrWhiteSpace(resxDirectory) == false)
                    {
                        var relativePath = rootDirectory.Replace(resxDirectory!, string.Empty);

                        if (relativePath is not { })
                        {
                            namespaceValue = PathToNamespace(relativePath!);
                        }
                    }
                }

                var generatedSharedSource = ResourceManagerClassGenerator.GenerateSource(namespaceValue!, className!, additionalFileConfigInfo.FilePath);

                sourceProductionContext.AddSource(generatedFileName, generatedSharedSource);
            }

            if (additionalFileConfigInfo.GenerateAndroidStrings)
            {
                if (string.IsNullOrWhiteSpace(additionalFileConfigInfo.AndroidResourcesPath) == false)
                {
                    var valuesFolder = "values";

                    if (additionalFileConfigInfo.Culture != null)
                    {
                        valuesFolder += $"-{additionalFileConfigInfo.Culture}";
                    }

                    var stringsPath = Path.Combine(additionalFileConfigInfo.AndroidResourcesPath!, valuesFolder,
                        "strings.xml");

                    AndroidStringsGenerator.GenerateAndroidStrings(additionalFileConfigInfo.FilePath, stringsPath);
                }
                else
                {
                    ReportAndroidResourcesDirectoryError(sourceProductionContext, additionalFileConfigInfo.FilePath);
                }
            }
        });
    }

    private void ReportNamespaceError(SourceProductionContext context, string fileName)
    {
        var diagnostic = Diagnostic.Create(
            new DiagnosticDescriptor(
                "LG001",
                "Expected NamespaceValue for resx file",
                $"Please, set NamespaceValue property in csproj for your resx file: {fileName}",
                "CodeGeneration",
                DiagnosticSeverity.Error,
                true),
            null,
            DiagnosticSeverity.Error);

        context.ReportDiagnostic(diagnostic);
    }

    private void ReportAndroidResourcesDirectoryError(SourceProductionContext context, string fileName)
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

    private string PathToNamespace(string path)
    {
        path = path.Replace('\\', '/');

        if (path.StartsWith("/"))
        {
            path = path.Substring(1);
        }

        return path.Replace('/', '.');
    }
}
