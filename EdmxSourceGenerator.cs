namespace EdmxSourceGenerator;

using Internal;
using Microsoft.CodeAnalysis;
using System.Linq;

[Generator(LanguageNames.CSharp)]
public class EdmxSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // find all additional files that end with .edmx, or named "MetaData.yml.
        var additionalFiles = context.AdditionalTextsProvider.Where(
            static file => file.Path.EndsWith(".edmx") || file.Path.EndsWith("MetaData.yml"));
        var combined = additionalFiles.Collect();
        context.RegisterSourceOutput(combined, (sourceProductionContext, additionalTexts) =>
        {
            var metaDataGenerator = new MetaDataGenerator(additionalTexts.ToArray());
            foreach (var (fileName, code) in metaDataGenerator.GetGeneratedFiles())
            {
                sourceProductionContext.AddSource(fileName, code);
            }
        });
    }
}
