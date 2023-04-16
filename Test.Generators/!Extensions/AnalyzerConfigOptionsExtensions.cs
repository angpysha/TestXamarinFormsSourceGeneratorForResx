using Microsoft.CodeAnalysis.Diagnostics;

namespace Test.Generators;

internal static class AnalyzerConfigOptionsExtensions
{
    public static bool TryGetClassName(this AnalyzerConfigOptions options, out string? className)
    {
        return options.TryGetValue(PropertiesNamesConstants.ClassNamePropertyName, out className);
    }

    public static bool TryGetGenerateShared(this AnalyzerConfigOptions options, out string? generateShared)
    {
        return options.TryGetValue(PropertiesNamesConstants.GenerateSharedClassPropertyName, out generateShared);
    }

    public static bool TryGetNamespaceValue(this AnalyzerConfigOptions options, out string? namespaceValue)
    {
        return options.TryGetValue(PropertiesNamesConstants.NamespaceValuePropertyName, out namespaceValue);
    }

    public static bool TryGetGenerateAndroidString(this AnalyzerConfigOptions options,
        out string? generateAndroidString)
    {
        return options.TryGetValue(PropertiesNamesConstants.GenerateAndroidStringPropertyName,
            out generateAndroidString);
    }

    public static bool TryGetAndroidResourcesFolderPath(this AnalyzerConfigOptions options,
        out string? androidResourcesFolderPath)
    {
        return options.TryGetValue(PropertiesNamesConstants.AndroidResourcesFolderPathPropertyName,
            out androidResourcesFolderPath);
    }
}
