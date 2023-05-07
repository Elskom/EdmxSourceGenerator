namespace EdmxSourceGenerator.Internal;

internal class FunctionImport
{
    internal FunctionImport(string name)
        => this.Name = name;

    private string Name { get; }

    public override string ToString()
        => $@"
    public virtual int {this.Name}()
        => ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction(""{this.Name}"");
";
}
