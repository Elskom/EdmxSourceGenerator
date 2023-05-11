namespace EdmxSourceGenerator.Internal;

using System.Collections.Generic;
using System.Xml.Linq;

internal class EdmxFile
{
    internal EdmxFile(string content)
    {
        var document = XDocument.Parse(content);
        if (document.Root!.Name.LocalName != "Edmx")
        {
            return;
        }

        var edmxRuntime = (XElement)document.Root.FirstNode.NextNode;
        var edmxCsdl = (XElement)edmxRuntime?.FirstNode.NextNode.NextNode.NextNode;
        var csdlSchema = (XElement)edmxCsdl?.FirstNode;
        if (csdlSchema == null)
        {
            return;
        }

        this.Namespace = csdlSchema.Attribute("Namespace")!.Value;
        this.TypeReader = new EntityTypeReader(this.Namespace, csdlSchema);
        this.GeneratedFiles.AddRange(this.TypeReader.GetGeneratedFiles());
    }

    internal string Namespace { get; }
    internal List<(string fileName, string code)> GeneratedFiles { get; } = new();
    private EntityTypeReader TypeReader { get; }
}
