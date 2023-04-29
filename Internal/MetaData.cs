﻿namespace EdmxSourceGenerator.Internal;

using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

internal class MetaData
{
    internal MetaData(string content, List<EdmxFile> edmxFiles)
    {
        this.Content = content;
        this.EdmxFiles = edmxFiles;
    }

    internal List<EdmxFile> EdmxFiles { get; private set; }
    private string Content { get; set; }

    internal string GenerateCode()
    {
        var result = string.Empty;
        if (this.EdmxFiles.Any())
        {
            result = $@"// <auto-generated/>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

";
            if (this.EdmxFiles.Count == 1)
            {
                var edmxFile = this.EdmxFiles[0];
                result += $@"namespace {edmxFile.Namespace};

";
                foreach (var entityType in edmxFile.TypeReader.EntityTypes)
                {
                    result += this.WriteEntityMetadata(entityType);
                }
            }
            else
            {
                foreach (var edmxFile in this.EdmxFiles)
                {
                    result += $@"namespace {edmxFile.Namespace}
{{
";
                    foreach (var entityType in edmxFile.TypeReader.EntityTypes)
                    {
                        result += this.WriteEntityMetadata(entityType, true);
                    }

                    result += @"}
";
                }
            }
        }

        return result;
    }

    private string WriteEntityMetadata(EntityType entityType, bool indent = false)
    {
        // Setup the input
        using var content = new StringReader(this.Content);

        // Load the stream
        var yaml = new YamlStream();
        yaml.Load(content);

        var result = string.Empty;
        var mapping = (YamlMappingNode)yaml.Documents.First().RootNode;
        foreach (var node in mapping.Children)
        {
            result += $@"
[(typeof({node.Key}MetaData))]
{(indent ? "    " : "")}public partial class {node.Key}
{(indent ? "    " : "")}{{
{(indent ? "    " : "")}    private sealed class {node.Key}MetaData
{(indent ? "    " : "")}    {{
";
            foreach (var valueNode in ((YamlSequenceNode)node.Value).Children)
            {
                foreach (var child in ((YamlMappingNode)valueNode).Children)
                {
                    foreach (var attributes in ((YamlSequenceNode)child.Value).Children)
                    {
                        result += $@"{(indent ? "    " : "")}        [{attributes}]
";
                    }

                    var property = entityType.Properties.FirstOrDefault(prop => prop.Name == child.Key.ToString());
                    var navicationProperty = entityType.NavigationProperties.FirstOrDefault(np => np.Name == child.Key.ToString());
                    if (property != null)
                    {
                        result += $@"{(indent ? "    " : "")}        {property}
";
                    }
                    else if (navicationProperty != null)
                    {
                        result += $@"{(indent ? "    " : "")}        public {(navicationProperty.IsCollection ? $"ICollection<{navicationProperty.ToRole}>" : navicationProperty.ToRole)}  {navicationProperty.Name} {{ get; set; }}
";
                    }
                }
            }

            result += $@"{(indent ? "    " : "")}}}
";
        }

        return result;
    }
}
