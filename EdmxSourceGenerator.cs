namespace EdmxSourceGenerator;

using global::EdmxSourceGenerator.Internal;
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
        context.RegisterSourceOutput(combined, (context, additionalTexts) =>
        {
            var metaDataGenerator = new MetaDataGenerator(additionalTexts.ToArray());
            foreach (var (FileName, Code) in metaDataGenerator.GetGeneratedFiles())
            {
                context.AddSource(FileName, Code);
            }
        });
    }
}
