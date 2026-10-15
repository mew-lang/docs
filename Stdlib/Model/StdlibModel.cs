namespace MewDocs.Stdlib.Model;

using System.Collections.Immutable;
using System.Text;

/// <summary>The standard library as described by <c>resources/stdlib.xml</c>.</summary>
public sealed class StdlibModel
{
    private readonly Dictionary<string, StdlibNamespace> _byName;
    private readonly Dictionary<string, StdlibNamespace> _bySlug;

    public StdlibModel(ImmutableArray<StdlibNamespace> namespaces)
    {
        Namespaces = namespaces;
        _byName = namespaces.ToDictionary(n => n.Name, StringComparer.Ordinal);
        _bySlug = namespaces.ToDictionary(n => n.Slug, StringComparer.Ordinal);

        // Reverse index: which types implement each interface, across every namespace.
        var implementors = new Dictionary<string, List<(StdlibNamespace, StdlibType)>>(StringComparer.Ordinal);
        foreach (var ns in namespaces)
        {
            foreach (var type in ns.Types)
            {
                foreach (var implemented in type.Implements)
                {
                    var key = BareName(implemented);
                    if (!implementors.TryGetValue(key, out var list))
                    {
                        implementors[key] = list = [];
                    }

                    list.Add((ns, type));
                }
            }
        }

        Implementors = implementors.ToImmutableDictionary(
            kv => kv.Key,
            kv => kv.Value.ToImmutableArray(),
            StringComparer.Ordinal);
    }

    public ImmutableArray<StdlibNamespace> Namespaces { get; }

    /// <summary>Types implementing an interface, keyed by the interface's bare name (<c>Enumerable</c>, not <c>Enumerable&lt;T&gt;</c>).</summary>
    public ImmutableDictionary<string, ImmutableArray<(StdlibNamespace Namespace, StdlibType Type)>> Implementors { get; }

    public StdlibNamespace? FindNamespace(string name) => _byName.GetValueOrDefault(name);

    public StdlibNamespace? FindNamespaceBySlug(string slug) => _bySlug.GetValueOrDefault(slug);

    /// <summary>Strips the type arguments off a type reference: <c>Enumerable&lt;T&gt;</c> becomes <c>Enumerable</c>.</summary>
    public static string BareName(string typeReference)
    {
        var lt = typeReference.IndexOf('<');
        return lt < 0 ? typeReference : typeReference[..lt];
    }
}

public sealed record StdlibNamespace(
    string Name,
    string Slug,
    string Uid,
    ImmutableArray<StdlibType> Types,
    ImmutableArray<StdlibExtension> Extensions,
    ImmutableArray<StdlibFunction> Functions)
{
    public IEnumerable<StdlibType> Unions => Types.Where(t => t.Kind == StdlibTypeKind.Union);

    public IEnumerable<StdlibType> Interfaces => Types.Where(t => t.Kind == StdlibTypeKind.Interface);

    public IEnumerable<StdlibType> PlainTypes => Types.Where(t => t.Kind == StdlibTypeKind.Type);

    public StdlibType? FindType(string name) => Types.FirstOrDefault(t => t.Name == name);
}

public enum StdlibTypeKind
{
    Type,
    Union,
    Interface,
}

public sealed record StdlibType(
    StdlibTypeKind Kind,
    string Name,
    string? Summary,
    ImmutableArray<string> TypeParameters,
    ImmutableArray<string> Implements,
    ImmutableArray<StdlibCase> Cases,
    ImmutableArray<StdlibField> Fields,
    ImmutableArray<StdlibFunction> Methods)
{
    /// <summary>The name with its type parameters, as written in Mew: <c>Result&lt;T, E&gt;</c>.</summary>
    public string DisplayName => TypeParameters.IsEmpty ? Name : $"{Name}<{string.Join(", ", TypeParameters)}>";

    /// <summary>The heading id a Markdown page gave this type, so existing <c>#optiont</c> links keep working.</summary>
    public string Anchor => Anchors.For(DisplayName);
}

public sealed record StdlibCase(string Name, string? Summary, ImmutableArray<string> Payloads);

public sealed record StdlibField(string Name, string Type, bool Mutable, string? Summary);

public sealed record StdlibFunction(
    string Name,
    string Returns,
    string? Summary,
    bool IsStatic,
    bool IsDefault,
    string? Implements,
    ImmutableArray<string> Attributes,
    ImmutableArray<string> TypeParameters,
    ImmutableArray<StdlibParameter> Parameters);

public sealed record StdlibParameter(string Name, string Type, bool Variadic, string? Default);

/// <summary>An <c>impl</c> block that adds members to a type the library does not declare, such as <c>string</c>.</summary>
public sealed record StdlibExtension(string Type, ImmutableArray<StdlibFunction> Methods)
{
    public string Anchor => Anchors.For(Type);
}

public static class Anchors
{
    /// <summary>
    /// Mirrors Markdig's auto-identifier: lowercase, drop punctuation, spaces become hyphens.
    /// <c>Result&lt;T, E&gt;</c> becomes <c>resultt-e</c>, which is what the hand-written pages
    /// produced and what the language docs link to.
    /// </summary>
    public static string For(string heading)
    {
        var sb = new StringBuilder(heading.Length);
        foreach (var ch in heading)
        {
            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(char.ToLowerInvariant(ch));
            }
            else if (ch is ' ' or '-' or '_')
            {
                sb.Append('-');
            }
        }

        return sb.ToString().Trim('-');
    }
}
