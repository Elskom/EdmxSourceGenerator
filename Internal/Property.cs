namespace EdmxSourceGenerator.Internal;

internal class Property
{
    internal Property(string name, string type, bool nullable)
    {
        this.Name = name;
        this.Type = type;
        this.Nullable = nullable;
    }

    internal string Name { get; private set; }
    internal string Type { get; private set; }
    internal bool Nullable { get; private set; }

    public override string ToString()
        => $@"public {this.TypeToCodeString()}{(this.Nullable == true ? "?" : "")} {this.Name} {{ get; set; }}";

    private string TypeToCodeString()
    {
        var splittedType = this.Type.Split('.');
        return this.Type switch
        {
            "String" => "string",
            "Boolean" => "bool",
            "Int32" => "int",
            "Decimal" => "decimal",
            _ => splittedType[splittedType.Length - 1],
        };
    }
}
