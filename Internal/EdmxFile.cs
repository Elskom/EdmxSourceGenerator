namespace EdmxSourceGenerator.Internal;

using System.Collections.Generic;
using System.Xml.Linq;

internal class EdmxFile
{
    internal EdmxFile(bool referencesEfCore, string content)
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
        this.TypeReader = new EntityTypeReader(referencesEfCore, this.Namespace, csdlSchema);
    }

    internal string Namespace { get; }

    internal IEnumerable<(string fileName, string code)> GetGeneratedFiles()
        => this.TypeReader.GetGeneratedFiles();

    private EntityTypeReader TypeReader { get; }
}
