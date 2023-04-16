using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Test.Generators.Models;

internal record IncrementalValueProviderContainer(
    AnalyzerConfigOptionsProvider AnalyzerConfigOptionsProvider,
    Compilation Compilation,
    ImmutableArray<AdditionalText> AdditionalTexts);
