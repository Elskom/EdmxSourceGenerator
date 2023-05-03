namespace EdmxSourceGenerator.Internal;

internal class EntitySet
{
    internal EntitySet(string name, string entityType)
    {
        this.Name = name;
        this.EntityType = entityType;
    }

    internal string Name { get; private set; }
    internal string EntityType { get; private set; }

    public override string ToString()
        => $@"public virtual DbSet<{this.EntityType}> {this.Name} {{ get; set; }} = null!;
";
}
