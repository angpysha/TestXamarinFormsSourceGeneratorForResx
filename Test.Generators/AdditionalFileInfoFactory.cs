using Microsoft.CodeAnalysis.Diagnostics;
using Test.Generators.Models;

namespace Test.Generators;

internal delegate bool TryGetValueDelegate(out string?  value);

internal static class AdditionalFileInfoFactory
{
    public static AdditionalFileConfigInfo Create(AnalyzerConfigOptions options, AdditionalText additionalText)
    {
        var generateSharedClass = GetBool(options.TryGetGenerateShared);
        var generateAndroidResources = GetBool(options.TryGetGenerateAndroidString);
        var className = GetString(options.TryGetClassName);
        var @namespace = GetString(options.TryGetNamespaceValue);
        var androidResourcesPath = GetString(options.TryGetAndroidResourcesFolderPath);


        return new AdditionalFileConfigInfo(
            GenerateSharedClass: generateSharedClass,
            GenerateAndroidStrings: generateAndroidResources,
            ClassName: className,
            Namespace: @namespace,
            AndroidResourcesPath: androidResourcesPath,
            AdditionalText: additionalText,
            FilePath: additionalText.Path,
            Culture: GetCultureNameFromResxFilePath(additionalText.Path));
    }

    private static string? GetCultureNameFromResxFilePath(string resxFilePath)
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

    private static bool GetBool(TryGetValueDelegate tryGetFunction)
    {
        var operationResult = tryGetFunction(out var returnedValue);

        if (operationResult && string.IsNullOrWhiteSpace(returnedValue) == false)
        {
            var conversionResult = bool.TryParse(returnedValue, out bool boolValue);

            return conversionResult && boolValue;
        }

        return operationResult;
    }

    private static string? GetString(TryGetValueDelegate tryGetFunction)
    {
        var operationResult = tryGetFunction(out var returnedValue);

        return returnedValue;
    }
}
