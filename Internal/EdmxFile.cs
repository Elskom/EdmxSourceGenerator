namespace EdmxSourceGenerator.Internal;

using System.Collections.Generic;
using System.Xml.Linq;

internal class EdmxFile
{
    internal EdmxFile(string content)
    {
        var document = XDocument.Parse(content);
        if (document.Root.Name.LocalName == "Edmx")
        {
            var edmxRuntime = (XElement)document.Root.FirstNode.NextNode;
            if (edmxRuntime != null)
            {
                var edmxCsdl = (XElement)edmxRuntime.FirstNode.NextNode.NextNode.NextNode;
                if (edmxCsdl != null)
                {
                    var csdlSchema = (XElement)edmxCsdl.FirstNode;
                    if (csdlSchema != null)
                    {
                        this.Namespace = csdlSchema.Attribute("Namespace").Value;
                        this.TypeReader = new EntityTypeReader(this.Namespace, csdlSchema);
                        this.GeneratedFiles.AddRange(this.TypeReader.GetGeneratedFiles());
                    }
                }
            }
        }
    }

    internal string Namespace { get; private set; }
    internal List<(string FileName, string Code)> GeneratedFiles { get; private set; } = new();
    internal EntityTypeReader TypeReader { get; private set; }
}
