namespace MewDocs.Stdlib.Model;

using System.Collections.Immutable;
using System.Xml.Linq;

/// <summary>Reads <c>resources/stdlib.xml</c> into a <see cref="StdlibModel"/>. Element order is kept as written.</summary>
public static class StdlibXmlLoader
{
    public static StdlibModel Load(string path)
    {
        var document = XDocument.Load(path);
        var root = document.Root ?? throw new InvalidDataException($"{path} has no root element.");

        var namespaces = root.Elements("Namespace")
            .Select(ReadNamespace)
            .OrderBy(n => n.Name, StringComparer.Ordinal)
            .ToImmutableArray();

        return new StdlibModel(namespaces);
    }

    private static StdlibNamespace ReadNamespace(XElement element)
    {
        var name = Attr(element, "Name") ?? throw new InvalidDataException("A <Namespace> has no Name.");
        var rest = name == "std" ? "std" : name["std.".Length..];

        var types = element.Elements()
            .Where(e => e.Name.LocalName is "Type" or "Union" or "Interface")
            .Select(ReadType)
            .ToImmutableArray();

        var extensions = element.Elements("Extension")
            .Select(e => new StdlibExtension(
                Attr(e, "Type") ?? throw new InvalidDataException("An <Extension> has no Type."),
                e.Elements("Method").Select(ReadFunction).ToImmutableArray()))
            .ToImmutableArray();

        var functions = element.Elements("Function").Select(ReadFunction).ToImmutableArray();

        return new StdlibNamespace(
            Name: name,
            Slug: rest.Replace('.', '-'),
            Uid: $"stdlib.{rest}",
            Types: types,
            Extensions: extensions,
            Functions: functions);
    }

    private static StdlibType ReadType(XElement element)
    {
        var kind = element.Name.LocalName switch
        {
            "Union" => StdlibTypeKind.Union,
            "Interface" => StdlibTypeKind.Interface,
            _ => StdlibTypeKind.Type,
        };

        return new StdlibType(
            Kind: kind,
            Name: Attr(element, "Name") ?? throw new InvalidDataException($"A <{element.Name.LocalName}> has no Name."),
            Summary: Summary(element),
            TypeParameters: Names(element, "TypeParameter"),
            Implements: element.Elements("Implements").Select(e => Required(e, "Type")).ToImmutableArray(),
            Cases: element.Elements("Case").Select(c => new StdlibCase(
                Required(c, "Name"),
                Summary(c),
                c.Elements("Payload").Select(p => Required(p, "Type")).ToImmutableArray())).ToImmutableArray(),
            Fields: element.Elements("Field").Select(f => new StdlibField(
                Required(f, "Name"),
                Required(f, "Type"),
                Flag(f, "Mutable"),
                Summary(f))).ToImmutableArray(),
            Methods: element.Elements("Method").Select(ReadFunction).ToImmutableArray());
    }

    private static StdlibFunction ReadFunction(XElement element) => new(
        Name: Required(element, "Name"),
        Returns: Attr(element, "Returns") ?? "void",
        Summary: Summary(element),
        IsStatic: Flag(element, "Static"),
        IsDefault: Flag(element, "Default"),
        Implements: Attr(element, "Implements"),
        Attributes: Names(element, "Attribute"),
        TypeParameters: Names(element, "TypeParameter"),
        Parameters: element.Elements("Parameter").Select(p => new StdlibParameter(
            Required(p, "Name"),
            Required(p, "Type"),
            Flag(p, "Variadic"),
            Attr(p, "Default"))).ToImmutableArray());

    private static string? Attr(XElement element, string name) => element.Attribute(name)?.Value;

    private static string Required(XElement element, string name) =>
        Attr(element, name) ?? throw new InvalidDataException($"A <{element.Name.LocalName}> has no {name}.");

    private static bool Flag(XElement element, string name) =>
        string.Equals(Attr(element, name), "true", StringComparison.OrdinalIgnoreCase);

    private static ImmutableArray<string> Names(XElement element, string child) =>
        element.Elements(child).Select(e => Required(e, "Name")).ToImmutableArray();

    /// <summary>The summary with its line breaks folded, since the XML wraps long sentences.</summary>
    private static string? Summary(XElement element)
    {
        var text = element.Element("Summary")?.Value;
        return string.IsNullOrWhiteSpace(text)
            ? null
            : string.Join(' ', text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
    }
}
