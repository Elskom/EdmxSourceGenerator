﻿namespace EdmxSourceGenerator;

using Internal;
using Microsoft.CodeAnalysis;
using System.Linq;

[Generator(LanguageNames.CSharp)]
public class EdmxSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // find all additional files that end with .edmx, or named "MetaData.yml.
        var referencesEfCore = context.CompilationProvider.Select(
            (c, _) => ReferencesEfCore(c));
        var additionalFiles = context.AdditionalTextsProvider.Where(
            static file => file.Path.EndsWith(".edmx") || file.Path.EndsWith("MetaData.yml"));
        var collected = additionalFiles.Collect();
        var combined = collected.Combine(referencesEfCore);
        var metaDataGenerators = combined.Select(
            (inputs, _) => new MetaDataGenerator(inputs.Right, inputs.Left.ToArray()));
        context.RegisterSourceOutput(metaDataGenerators, (sourceProductionContext, metaDataGenerator) =>
        {
            foreach (var (fileName, code) in metaDataGenerator.GetGeneratedFiles())
            {
                sourceProductionContext.AddSource(fileName, code);
            }
        });
    }

    private static bool ReferencesEfCore(Compilation compilation)
    {
        var symbol = compilation.GetTypeByMetadataName("Microsoft.EntityFrameworkCore.DbContext");
        return symbol != null && symbol.ContainingNamespace.ToString().StartsWith("Microsoft.EntityFrameworkCore");
    }
}
