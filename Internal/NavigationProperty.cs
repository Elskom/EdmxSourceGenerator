namespace EdmxSourceGenerator.Internal;

using System;

internal class NavigationProperty
{
    internal NavigationProperty(string name, string toRole)
    {
        this.Name = name;
        this.ToRole = toRole;
    }

    private string Name { get; }
    private string ToRole { get; }
    internal bool IsCollection
        => this.ToRole != this.Name;

    internal bool IsForeignKey(Property property, string entityType)
    {
        var  propertyName = property.Name;
        if (propertyName.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase))
        {
            propertyName = propertyName.Remove(propertyName.Length - 2);
        }

        return this.ToRole.StartsWith(propertyName, StringComparison.InvariantCultureIgnoreCase)
               && !property.Name.Equals($"{entityType}Id", StringComparison.InvariantCultureIgnoreCase);
    }

    public override string ToString()
        => $@"{(this.IsCollection ? @"[SuppressMessage(""Microsoft.Usage"", ""CA2227:CollectionPropertiesShouldBeReadOnly"")]
    ": "")}public virtual {(this.IsCollection ? $"ICollection<{this.ToRole}>" : this.ToRole)} {this.Name} {{ get; set; }}
";

    internal string ToConstructorCode()
        => this.IsCollection ? $"        this.{this.Name} = new HashSet<{this.ToRole}>();" : string.Empty;
}
