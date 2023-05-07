namespace EdmxSourceGenerator.Internal;

internal class EntitySet
{
    internal EntitySet(string name, string entityType)
    {
        this.Name = name;
        this.EntityType = entityType;
    }

    internal string Name { get; }
    internal string EntityType { get; }

    public override string ToString()
        => $@"public virtual DbSet<{this.EntityType}> {this.Name} {{ get; set; }} = null!;
";
}
