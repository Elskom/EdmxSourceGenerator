namespace EdmxSourceGenerator.Internal;

using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

internal class MetaDataGenerator
{
    internal MetaDataGenerator(AdditionalText[] additionalTexts)
    {
        var edmxFiles = additionalTexts
            .Where(additionalText => additionalText.Path.EndsWith(".edmx"))
            .Select(additionalText => new EdmxFile(additionalText.GetText()!.ToString()))
            .ToList();
        this.MetaData = additionalTexts
            .Where(additionalText => additionalText.Path.EndsWith("MetaData.yml"))
            .Select(additionalText => new MetaData(additionalText.GetText()!.ToString(), edmxFiles))
            .FirstOrDefault();
        if (this.MetaData == null)
        {
            this.EdmxFiles = edmxFiles;
        }
    }

    private List<EdmxFile> EdmxFiles { get; }
    private MetaData MetaData { get; }

    internal List<(string fileName, string code)> GetGeneratedFiles()
    {
        var edmxFiles = this.MetaData != null ? this.MetaData.EdmxFiles : this.EdmxFiles;
        var results = new List<(string fileName, string code)>();
        foreach (var edmxFile in edmxFiles)
        {
            results.AddRange(edmxFile.GeneratedFiles);
        }

        if (this.MetaData != null)
        {
            results.Add(("MetaData.g.cs", this.MetaData.GenerateCode()));
        }

        return results;
    }
}
