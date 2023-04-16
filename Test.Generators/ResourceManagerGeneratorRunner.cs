using Test.Generators.Models;

namespace Test.Generators;

internal static class ResourceManagerGeneratorRunner
{
    public static void Run(
        AdditionalFileConfigInfo additionalFileConfigInfo,
        IncrementalValueProviderContainer valueProviderContainer,
        SourceProductionContext sourceProductionContext)
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

        if (string.IsNullOrWhiteSpace(className) == false && string.IsNullOrWhiteSpace(namespaceValue) == false)
        {
            var generatedSharedSource =
                ResourceManagerClassGenerator.GenerateSource(namespaceValue!, className!,
                    additionalFileConfigInfo.FilePath);
            sourceProductionContext.AddSource(generatedFileName, generatedSharedSource);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                CompilationErrorProvider.ReportClassNameError(sourceProductionContext,
                    additionalFileConfigInfo.FilePath);
            }

            if (string.IsNullOrWhiteSpace(namespaceValue))
            {
                CompilationErrorProvider.ReportNamespaceError(sourceProductionContext, additionalFileConfigInfo.FilePath);
            }
        }
    }

    private static string PathToNamespace(string path)
    {
        path = path.Replace('\\', '/');

        if (path.StartsWith("/"))
        {
            path = path.Substring(1);
        }

        return path.Replace('/', '.');
    }
}

