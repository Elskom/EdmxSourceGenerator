namespace EdmxSourceGenerator.Internal;

internal class NavigationProperty
{
    internal NavigationProperty(string name, string fromRole, string toRole)
    {
        this.Name = name;
        this.FromRole = fromRole;
        this.ToRole = toRole;
    }

    internal string Name { get; private set; }
    internal string FromRole { get; private set; }
    internal string ToRole { get; private set; }
    internal bool IsCollection => this.ToRole != this.Name;

    public override string ToString()
        => $@"[System.Diagnostics.CodeAnalysis.SuppressMessage(""Microsoft.Usage"", ""CA2227:CollectionPropertiesShouldBeReadOnly"")]
    public virtual {(this.IsCollection ? $"ICollection<{this.ToRole}>" : this.ToRole)} {this.Name} {{ get; set; }}
";
    internal string ToConstructorCode()
        => this.IsCollection ? $"        this.{this.Name} = new HashSet<{this.ToRole}>();" : string.Empty;
}
