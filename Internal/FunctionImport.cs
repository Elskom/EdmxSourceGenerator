namespace EdmxSourceGenerator.Internal;

internal class FunctionImport
{
    internal FunctionImport(bool referencesEfCore, string name)
    {
        this.Name = name;
        this.ReferencesEfCore = referencesEfCore;
    }

    private string Name { get; }
    private bool ReferencesEfCore { get; }

    public override string ToString()
        => $@"
    public virtual int {this.Name}()
        => {(this.ReferencesEfCore ? $"this.Database.ExecuteSqlRaw(\"EXEC [dbo].[{this.Name}]\");": $"((IObjectContextAdapter)this).ObjectContext.ExecuteFunction(\"{this.Name}\");")}
";
}
