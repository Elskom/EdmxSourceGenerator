namespace EdmxSourceGenerator.Internal;

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

    public override string ToString()
        => $@"{(this.IsCollection ? @"[SuppressMessage(""Microsoft.Usage"", ""CA2227:CollectionPropertiesShouldBeReadOnly"")]
    ": "")}public virtual {(this.IsCollection ? $"ICollection<{this.ToRole}>" : this.ToRole)} {this.Name} {{ get; set; }}
";

    internal string ToConstructorCode()
        => this.IsCollection ? $"        this.{this.Name} = new HashSet<{this.ToRole}>();" : string.Empty;
}
