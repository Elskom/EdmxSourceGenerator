namespace EdmxSourceGenerator.Internal;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class EntityTypeReader
{
    internal EntityTypeReader(string @namespace, XElement element)
    {
        var elem = (XElement)element.FirstNode;
        do
        {
            switch (elem.Name.LocalName)
            {
                case "EntityType":
                {
                    var name = elem.Attribute("Name")!.Value;
                    if (!name.StartsWith("FK_"))
                    {
                        this.EntityTypes.Add(new EntityType(@namespace, elem)
                        {
                            Name = name
                        });
                    }

                    break;
                }
                case "EnumType":
                {
                    var name = elem.Attribute("Name")!.Value;
                    this.EnumTypes.Add(new EnumType(@namespace, name, elem));
                    break;
                }
                case "EntityContainer":
                {
                    var name = elem.Attribute("Name")!.Value;
                    this.DataContext = new DataContext(@namespace, name, elem);
                    break;
                }
            }

            elem = (XElement)elem.NextNode;
        }
        while (elem != null);
    }

    internal IEnumerable<(string fileName, string code)> GetGeneratedFiles()
    {
        var results = this.EntityTypes.Select(en => ($"{en.Name}.g.cs", en.ToString())).ToList();
        results.AddRange(this.EnumTypes.Select(e => ($"{e.Name}.g.cs", e.ToString())));
        results.Add(($"{this.DataContext.Name}.g.cs", this.DataContext.ToString()));
        results.Add(($"{this.DataContext.Name}.Methods.g.cs", this.DataContext.ToMethodCodeString()));
        return results;
    }

    private DataContext DataContext { get; }
    private List<EntityType> EntityTypes { get; } = new();
    private List<EnumType> EnumTypes { get; } = new();
}
