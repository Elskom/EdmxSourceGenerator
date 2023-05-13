﻿namespace EdmxSourceGenerator.Internal;

using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

internal class DataContext
{
    internal DataContext(bool referencesEfCore, string @namespace, string name, XElement element)
    {
        this.ReferencesEfCore = referencesEfCore;
        this.Namespace = @namespace;
        this.Name = name;
        var elem = (XElement)element.FirstNode;
        do
        {
            switch (elem.Name.LocalName)
            {
                case "EntitySet":
                {
                    var entityName = elem.Attribute("Name")!.Value;
                    var splittedEntityType = elem.Attribute("EntityType")!.Value.Split('.');
                    var entityType = splittedEntityType[splittedEntityType.Length - 1];
                    this.EntitySets.Add(new(entityName, entityType));
                    break;
                }
                case "FunctionImport":
                {
                    var functionName = elem.Attribute("Name")!.Value;
                    this.FunctionImports.Add(new(referencesEfCore, functionName));
                    break;
                }
            }

            elem = (XElement)elem.NextNode;
        }
        while (elem != null);
    }

    internal string Name { get; }
    private string Namespace { get; }
    private bool ReferencesEfCore { get; }
    private List<EntitySet> EntitySets { get; } = new();
    private List<FunctionImport> FunctionImports { get; } = new();

    public string ToMethodCodeString()
    {
        var result = new StringBuilder($@"// <auto-generated/>
namespace {this.Namespace};

{(this.ReferencesEfCore ? "using Microsoft.EntityFrameworkCore;" : "using System.Data.Entity;")}
using System.Threading.Tasks;

public partial class {this.Name}
{{
");
        foreach (var entitySet in this.EntitySets)
        {
            _ = result.Append($@"    public async Task Create{entitySet.EntityType}Async({entitySet.EntityType} {entitySet.EntityType.ToLowerInvariant()})
    {{
        _ = this.{entitySet.Name}.Add({entitySet.EntityType.ToLowerInvariant()});
        _ = await this.SaveChangesAsync().ConfigureAwait(false);
    }}

    public async Task Edit{entitySet.EntityType}Async({entitySet.EntityType} {entitySet.EntityType.ToLowerInvariant()})
    {{
        this.Entry({entitySet.EntityType.ToLowerInvariant()}).State = EntityState.Modified;
        _ = await this.SaveChangesAsync().ConfigureAwait(false);
    }}

    public async Task Delete{entitySet.EntityType}Async({entitySet.EntityType} {entitySet.EntityType.ToLowerInvariant()})
    {{
        if ({entitySet.EntityType.ToLowerInvariant()} != null)
        {{
            this.Entry({entitySet.EntityType.ToLowerInvariant()}).State = EntityState.Deleted;
            _ = await this.SaveChangesAsync().ConfigureAwait(false);
        }}
    }}
{(this.EntitySets.IndexOf(entitySet) + 1 < this.EntitySets.Count ? @"
" : "")}");
        }
        _ = result.Append(@"}
");
        return result.ToString();
    }

    public override string ToString()
    {
        var result = new StringBuilder($@"// <auto-generated/>
#nullable enable
namespace {this.Namespace};

{(this.ReferencesEfCore ? @"using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Threading;
using Microsoft.EntityFrameworkCore;" : @"using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;")}
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class {this.Name} : DbContext
{{
    public {this.Name}()
        : base({(this.ReferencesEfCore ? $"GetOptions(\"name={this.Name}\")" : $"\"name={this.Name}\"")})
    {{
    }}

    public static {this.Name} Create()
        => new();

    {(this.ReferencesEfCore ? $@"private static DbContextOptions<{this.Name}> GetOptions(string connectionString)
    {{
        var optionsBuilder = new DbContextOptionsBuilder<{this.Name}>();
        optionsBuilder.UseSqlServer(connectionString);
        return optionsBuilder.Options;
    }}

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {{
        var entities = this.ChangeTracker.Entries().Where(
            e => e.State == EntityState.Deleted || e.State == EntityState.Modified || e.State == EntityState.Added);
        foreach (var entity in entities)
        {{
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(entity.Entity);
            if (!Validator.TryValidateObject(entity.Entity, validationContext, validationResults, true))
            {{
                foreach (var validationResult in validationResults)
                {{
                    Debug.Write($""Error: {{validationResult.ErrorMessage}}"");
                    foreach (var memberName in validationResult.MemberNames)
                    {{
                        Debug.Write($"", Property: {{memberName}}"");
                    }}

                    Debug.WriteLine(string.Empty);
                }}

                throw new ValidationException(""One or more validation errors occurred."");
            }}
        }}

        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }}" : @"public override async Task<int> SaveChangesAsync()
    {
        try
        {
            return await base.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (DbEntityValidationException ex)
        {
            foreach (var validationError in ex.EntityValidationErrors.SelectMany(eve => eve.ValidationErrors))
            {
                Debug.WriteLine($""Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}"");
            }

            throw;
        }
    }")}

    protected override void OnModelCreating({(this.ReferencesEfCore ? "ModelBuilder" : "DbModelBuilder")} modelBuilder)
        => throw new {(this.ReferencesEfCore ? "NotImplementedException" : "UnintentionalCodeFirstException")}();

");
        foreach (var entitySet in this.EntitySets)
        {
            _ = result.Append($"    {entitySet}");
        }

        foreach (var functionImport in this.FunctionImports)
        {
            _ = result.Append(functionImport);
        }

        _ = result.Append(@"}
");
        return result.ToString();
    }
}
