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

        var resxFiles = context.AdditionalTextsProvider.Where(static atp => atp.Path.Contains("resx"));

        var resxFilesCollected = resxFiles.Collect();
        var compilationAdResx = context.CompilationProvider.Combine(resxFilesCollected);
        var compilationResAndAnalyzer = context.AnalyzerConfigOptionsProvider.Combine(compilationAdResx);

        var incrementalContainer = compilationResAndAnalyzer
            .Select((tuple, _) => new IncrementalValueProviderContainer(
                AnalyzerConfigOptionsProvider: tuple.Left,
                Compilation: tuple.Right.Left,
                AdditionalTexts: tuple.Right.Right));

        context.RegisterSourceOutput(compilationResAndAnalyzer, (sourceProductionContext, source) =>
        {
            if (source.Right.Right.LastOrDefault() is null)
            {
                return;
            }

            var filePath = source.Right.Right.LastOrDefault()!.Path;

            var culture = GetCultureNameFromResxFilePath(Path.GetFileNameWithoutExtension(filePath));

            var analyzerConfigOptionsProvider = source.Left;

            var options = analyzerConfigOptionsProvider.GetOptions(source.Right.Right.LastOrDefault()!);


            var generateSharedClassResult = options.TryGetValue("build_metadata.EmbeddedResource.GenerateSharedClass",
                out string? generateSharedClass);
            if (generateSharedClassResult && bool.TryParse(generateSharedClass, out var generateSharedClassBool) &&
                generateSharedClassBool)
            {
                if (culture == null)
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                    var generatedFileName = fileNameWithoutExtension + ".g.cs";

                    var classNameResult =
                        options.TryGetValue("build_metadata.EmbeddedResource.ClassName", out var className);
                    var namespaceValueResult = options.TryGetValue("build_metadata.EmbeddedResource.NamespaceValue",
                        out var namespaceValue);

                    if (classNameResult == false)
                    {
                        className = fileNameWithoutExtension;
                    }

                    if (namespaceValueResult == false)
                    {
                        var mainSyntaxTree = source.Right.Left.SyntaxTrees.First(x => x.HasCompilationUnitRoot);

                        var rootDirectory = Path.GetDirectoryName(mainSyntaxTree.FilePath);
                        var resxDirectory = Path.GetDirectoryName(filePath);

                        if (string.IsNullOrWhiteSpace(rootDirectory) == false && string.IsNullOrWhiteSpace(resxDirectory))
                        {
                            var relativePath = rootDirectory.Replace(resxDirectory!, string.Empty);

                            if (relativePath is not { })
                            {
                                namespaceValue = PathToNamespace(relativePath!);
                            }
                        }
                        else
                        {

                        }

                    }
                    else
                    {
                        // ReportNamespaceError(sourceProductionContext, filePath);
                    }

                    var generatedSharedSource = ResourceManagerClassGenerator.GenerateSource(namespaceValue!, className!, filePath);

                    try
                    {
                        sourceProductionContext.AddSource(generatedFileName, generatedSharedSource);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            var generateForAndroidResult =
                options.TryGetValue("build_metadata.EmbeddedResource.GenerateAndroidStrings",
                    out var generateForAndroid);

            if (generateForAndroidResult && bool.TryParse(generateForAndroid, out bool generateForAndroidBool) &&
                generateForAndroidBool)
            {
                var androidResourcesPathResult = options.TryGetValue("build_metadata.EmbeddedResource.AndroidResourcesFolderPath",
                    out var androidResourcesPath);

                if (androidResourcesPathResult && string.IsNullOrWhiteSpace(androidResourcesPath) == false)
                {
                    var valuesFolder = "values";

                    if (culture != null)
                    {
                        valuesFolder += $"-{culture}";
                    }

                    var stringsPath = Path.Combine(androidResourcesPath, valuesFolder, "strings.xml");

                    AndroidStringsGenerator.GenerateAndroidStrings(filePath, stringsPath);
                }
            }
        });
    }

    private string? GetCultureNameFromResxFilePath(string resxFilePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(resxFilePath);
        var lastDotIndex = fileName.LastIndexOf('.');
        if (lastDotIndex == -1)
        {
            return null;
        }

        var cultureName = fileName.Substring(lastDotIndex + 1);
        if (cultureName.Length == 2)
        {
            return cultureName;
        }

        return cultureName.Contains('-') ? cultureName : null;
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
