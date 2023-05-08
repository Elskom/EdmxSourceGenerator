namespace EdmxSourceGenerator.Internal;

internal class Property
{
    internal Property(string name, string type, bool nullable)
    {
        this.Name = name;
        this.Type = type;
        this.Nullable = nullable;
    }

    private string Name { get; }
    private string Type { get; }
    private bool Nullable { get; }

    public override string ToString()
        => $@"public {this.TypeToCodeString()}{(this.Nullable ? "?" : "")} {this.Name} {{ get; set; }}";

    private string TypeToCodeString()
    {
        var splittedType = this.Type.Split('.');
        return this.Type switch
        {
            "String" => "string",
            "Boolean" => "bool",
            "Int32" => "int",
            "Decimal" => "decimal",
            "Double" => "double",
            "Int16" => "short",
            "Byte" => "byte",
            "Byte[]" => "byte[]",
            "Object" => "object",
            _ => splittedType[splittedType.Length - 1],
        };
    }

    internal bool SystemUsingNeeded()
        => this.Type switch
        {
            "DateTime" => true,
            "DateTimeOffset" => true,
            "TimeSpan" => true,
            _ => false,
        };
}
