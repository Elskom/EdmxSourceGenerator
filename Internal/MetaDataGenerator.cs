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
            .Select(additionalText => new EdmxFile(additionalText.GetText().ToString()))
            .ToList();
        this.MetaData = additionalTexts
            .Where(additionalText => additionalText.Path.EndsWith("MetaData.yml"))
            .Select(additionalText => new MetaData(additionalText.GetText().ToString(), edmxFiles))
            .FirstOrDefault();
        if (this.MetaData == null)
        {
            this.EdmxFiles = edmxFiles;
        }
    }

    internal List<EdmxFile> EdmxFiles { get; private set; }
    internal MetaData MetaData { get; private set; }

    internal List<(string FileName, string Code)> GetGeneratedFiles()
    {
        var results = new List<(string FileName, string Code)>();
        if (this.MetaData != null)
        {
            foreach (var edmxFile in this.MetaData.EdmxFiles)
            {
                results.AddRange(edmxFile.GeneratedFiles);
            }

            results.Add(("MetaData.g.cs", this.MetaData.GenerateCode()));
        }
        else
        {
            foreach (var edmxFile in this.EdmxFiles)
            {
                results.AddRange(edmxFile.GeneratedFiles);
            }
        }

        return results;
    }
}
