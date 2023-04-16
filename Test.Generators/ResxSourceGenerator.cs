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
            if (valueProviderContainer.AdditionalTexts.LastOrDefault() is not { } additionalText)
            {
                return;
            }

            var options = valueProviderContainer.AnalyzerConfigOptionsProvider.GetOptions(additionalText);

            var additionalFileConfigInfo =
                AdditionalFileInfoFactory.Create(options, additionalText);

            CompositeGenerator.RunResourceManagerGeneratorIfNeeded(
                additionalFileConfigInfo is { GenerateSharedClass: true, Culture: null },
                additionalFileConfigInfo, valueProviderContainer, sourceProductionContext);

            CompositeGenerator.RunAndroidGeneratorIfNeeded(additionalFileConfigInfo.GenerateAndroidStrings,
                additionalFileConfigInfo, valueProviderContainer, sourceProductionContext);
        });
    }
}
