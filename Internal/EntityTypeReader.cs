namespace EdmxSourceGenerator.Internal;

using System.Collections.Generic;
using System.Xml.Linq;

internal class EntityTypeReader
{
    internal EntityTypeReader(string @namespace, XElement element)
    {
        var elem = (XElement)element.FirstNode;
        do
        {
            if (elem.Name.LocalName == "EntityType")
            {
                var name = elem.Attribute("Name").Value;
                if (!name.StartsWith("FK_"))
                {
                    this.EntityTypes.Add(new EntityType(@namespace, elem)
                    {
                        Name = name
                    });
                }
            }
            else if (elem.Name.LocalName == "EnumType")
            {
                var name = elem.Attribute("Name").Value;
                this.EnumTypes.Add(new EnumType(@namespace, name, elem));
            }
            else if (elem.Name.LocalName == "EntityContainer")
            {
                var name = elem.Attribute("Name").Value;
                this.DataContext = new DataContext(@namespace, name, elem);
            }

            elem = (XElement)elem.NextNode;
        }
        while (elem != null);
    }

    internal List<(string FileName, string Code)> GetGeneratedFiles()
    {
        var results = new List<(string FileName, string Code)>();
        foreach (var en in this.EntityTypes)
        {
            results.Add(($"{en.Name}.g.cs", en.ToString()));
        }

        foreach (var e in this.EnumTypes)
        {
            results.Add(($"{e.Name}.g.cs", e.ToString()));
        }

        results.Add(($"{this.DataContext.Name}.g.cs", this.DataContext.ToString()));
        results.Add(($"{this.DataContext.Name}.Methods.g.cs", this.DataContext.ToMethodCodeString()));
        return results;
    }

    internal DataContext DataContext { get; set; }
    internal List<EntityType> EntityTypes { get; private set; } = new();
    internal List<EnumType> EnumTypes { get; private set; } = new();
}
