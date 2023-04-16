using Microsoft.CodeAnalysis;
using Test.Generators.Models;

namespace Test.Generators;

internal static class AndroidStringsGeneratorRunner
{
    public static void Run(
        AdditionalFileConfigInfo additionalFileConfigInfo,
        IncrementalValueProviderContainer valueProviderContainer,
        SourceProductionContext sourceProductionContext)
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
            CompilationErrorProvider.ReportAndroidResourcesDirectoryError(sourceProductionContext, additionalFileConfigInfo.FilePath);
        }
    }
}

