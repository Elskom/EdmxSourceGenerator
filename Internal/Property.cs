namespace EdmxSourceGenerator.Internal;

using System;

internal class Property
{
    internal Property(string name, string maxLength, string type, bool nullable)
    {
        this.Name = name;
        this.MaxLength = maxLength;
        this.Type = type;
        this.Nullable = nullable;
    }

    internal string Name { get; }
    private string MaxLength { get; }
    private string Type { get; }
    private bool Nullable { get; }

    public override string ToString()
        => $@"public {this.TypeToCodeString()}{(this.Nullable ? "?" : "")} {this.Name} {{ get; set; }}";

    internal bool SystemUsingNeeded()
        => this.Type switch
        {
            "DateTime" => true,
            "DateTimeOffset" => true,
            "TimeSpan" => true,
            _ => false,
        };

    internal bool IsPrimaryKey(string entityType)
        => this.Name.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase)
           && this.Name.Equals($"{entityType}Id", StringComparison.InvariantCultureIgnoreCase);

    internal string MaxLengthToCodeString()
        => !string.IsNullOrEmpty(this.MaxLength)
            ? $"[{(this.Type == "String" ? "StringLength": "MaxLength")}({this.MaxLength})]"
            : string.Empty;

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
}
