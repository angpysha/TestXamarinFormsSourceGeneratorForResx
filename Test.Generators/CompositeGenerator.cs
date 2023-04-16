using Test.Generators.Models;

namespace Test.Generators;

internal static class CompositeGenerator
{
    public static void RunResourceManagerGeneratorIfNeeded(
        bool shouldRun, 
        AdditionalFileConfigInfo additionalFileConfigInfo,
        IncrementalValueProviderContainer valueProviderContainer,
        SourceProductionContext sourceProductionContext)
    {
        if (shouldRun == false)
        {
            return;
        }

        ResourceManagerGeneratorRunner.Run(additionalFileConfigInfo, valueProviderContainer, sourceProductionContext);
       
    }

    public static void RunAndroidGeneratorIfNeeded(
        bool shouldRun, 
        AdditionalFileConfigInfo additionalFileConfigInfo,
        IncrementalValueProviderContainer valueProviderContainer,
        SourceProductionContext sourceProductionContext)
    {
        if (shouldRun == false)
        {
            return;
        }

        AndroidStringsGeneratorRunner.Run(additionalFileConfigInfo, valueProviderContainer, sourceProductionContext);
    }
}
