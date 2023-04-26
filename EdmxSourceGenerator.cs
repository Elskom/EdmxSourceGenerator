namespace EdmxSourceGenerator;

using global::EdmxSourceGenerator.Internal;
using Microsoft.CodeAnalysis;

[Generator(LanguageNames.CSharp)]
public class EdmxSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // find all additional files that end with .edmx
        var edmxFiles = context.AdditionalTextsProvider.Where(
            static file => file.Path.EndsWith(".edmx"));
        context.RegisterSourceOutput(edmxFiles, (context, edmxFile) =>
        {
            var _edmxFile = new EdmxFile(edmxFile.GetText().ToString());
            foreach (var (FileName, Code) in _edmxFile.GeneratedFiles)
            {
                context.AddSource(FileName, Code);
            }
        });
    }
}
