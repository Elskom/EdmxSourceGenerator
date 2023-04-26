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
                        var @namespace = csdlSchema.Attribute("Namespace").Value;
                        var typeReader = new EntityTypeReader(@namespace, csdlSchema);
                        this.GeneratedFiles.AddRange(typeReader.GetGeneratedFiles());
                    }
                }
            }
        }
    }

    internal List<(string FileName, string Code)> GeneratedFiles { get; set; } = new();
}
