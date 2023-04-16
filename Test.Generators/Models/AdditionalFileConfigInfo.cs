namespace Test.Generators.Models;

internal record AdditionalFileConfigInfo(
    bool GenerateSharedClass,
    bool GenerateAndroidStrings,
    string? ClassName,
    string? Namespace,
    string? AndroidResourcesPath,
    AdditionalText AdditionalText,
    string FilePath,
    string? Culture);
