﻿namespace EdmxSourceGenerator.Internal;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class EntityType
{
    internal EntityType(string @namespace, XElement element)
    {
        this.Namespace = @namespace;
        var elem = (XElement)element.FirstNode;
        do
        {
            if (elem.Name.LocalName is "Property")
            {
                var name = elem.Attribute("Name").Value;
                var type = elem.Attribute("Type").Value;
                var nullable = elem.Attribute("Nullable")?.Value;
                if (string.IsNullOrEmpty(nullable))
                {
                    nullable = "true";
                }

                this.Properties.Add(
                    new Property(
                        name,
                        type,
                        Convert.ToBoolean(nullable)));
            }
            else if (elem.Name.LocalName is "NavigationProperty")
            {
                var name = elem.Attribute("Name").Value;
                var fromRole = elem.Attribute("FromRole").Value;
                var toRole = elem.Attribute("ToRole").Value;
                this.NavigationProperties.Add(new NavigationProperty(name, fromRole, toRole));
            }

            elem = (XElement)elem.NextNode;
        }
        while (elem != null);
    }

    internal string Namespace { get; private set; }
    internal string Name { get; set; }
    internal List<Property> Properties { get; private set; } = new();
    internal List<NavigationProperty> NavigationProperties { get; set; } = new();

    public override string ToString()
    {
        var result = $@"// <auto-generated/>
namespace {this.Namespace};

using System;
using System.Collections.Generic;

public partial class {this.Name}
{{
    ";
        if (this.ConstructorNeeded())
        {
            result += $@"public {this.Name}()
    {{
";
            foreach (var navigationProperty in this.NavigationProperties)
            {
                var code = navigationProperty.ToConstructorCode();
                if (this.NavigationProperties.IndexOf(navigationProperty) + 1 < this.NavigationProperties.Count)
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        result += $@"{code}
";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(code))
                    {
                        result += $@"{code}
    ";
                    }
                    else
                    {
                        result += "    ";
                    }
                }

            }

            result += @"}

    ";
        }

        foreach (var property in this.Properties)
        {
            if (this.Properties.IndexOf(property) + 1 < this.Properties.Count)
            {
                result += $@"{property}
    ";
            }
            else
            {
                result += $@"{property}
";
            }
        }

        foreach (var navigationProperty in this.NavigationProperties)
        {
            if (this.NavigationProperties.IndexOf(navigationProperty) == 0)
            {
                result += @"
";
            }

            result += $@"    {navigationProperty}";
        }

        result += @"}
";
        return result;
    }

    private bool ConstructorNeeded()
    {
        var result = false;
        foreach (var navigationProperty in this.NavigationProperties)
        {
            if (navigationProperty.IsCollection)
            {
                result = true;
            }
        }

        return result;
    }
}
