namespace MewDocs.Stdlib;

using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Model;

/// <summary>
/// The text-level HTML behind the standard library pages: signatures, type references with every
/// declared type name linked to the page that declares it, and summaries with their backtick spans.
/// The page structure itself (headings, tables, sections) is Razor, under <c>Stdlib/Components</c>.
/// </summary>
public sealed partial class StdlibHtml
{
    public const string RoutePrefix = "/stdlib/";

    private static readonly HashSet<string> Primitives = new(StringComparer.Ordinal)
    {
        "void", "bool", "char", "string",
        "i8", "i16", "i32", "i64", "u8", "u16", "u32", "u64", "f32", "f64",
    };

    private readonly StdlibModel _model;

    public StdlibHtml(StdlibModel model)
    {
        _model = model;
    }

    /// <summary>A one-line description of the namespace for the sidebar, search and llms.txt.</summary>
    public static string Describe(StdlibNamespace ns)
    {
        var parts = new List<string>();
        Count(parts, ns.Unions.Count(), "union", "unions");
        Count(parts, ns.Interfaces.Count(), "interface", "interfaces");
        Count(parts, ns.PlainTypes.Count(), "type", "types");
        Count(parts, ns.Functions.Length, "function", "functions");
        Count(parts, ns.Extensions.Length, "extension", "extensions");

        return parts.Count switch
        {
            0 => $"The {ns.Name} namespace.",
            1 => $"The {ns.Name} namespace: {parts[0]}.",
            _ => $"The {ns.Name} namespace: {string.Join(", ", parts.Take(parts.Count - 1))} and {parts[^1]}.",
        };

        static void Count(List<string> parts, int count, string one, string many)
        {
            if (count > 0)
            {
                parts.Add($"{count} {(count == 1 ? one : many)}");
            }
        }
    }

    /// <summary>A Mew-style signature, with every type name linked where the library declares it.</summary>
    public MarkupString Signature(StdlibFunction f, string? owner, StdlibNamespace ns, ISet<string> outerScope)
    {
        var scope = new HashSet<string>(outerScope, StringComparer.Ordinal);
        scope.UnionWith(f.TypeParameters);

        var sb = new StringBuilder("<code>");
        if (f.IsStatic && owner is not null)
        {
            sb.Append(Escape(owner)).Append("::");
        }

        sb.Append(Escape(f.Name));
        if (f.TypeParameters.Length > 0)
        {
            sb.Append("&lt;").Append(Escape(string.Join(", ", f.TypeParameters))).Append("&gt;");
        }

        sb.Append('(');
        sb.Append(string.Join(", ", f.Parameters.Select(p =>
        {
            var text = $"{Escape(p.Name)}: {(p.Variadic ? "..." : "")}{TypeReferenceHtml(p.Type, scope, ns)}";
            return p.Default is null ? text : $"{text} = {Escape(p.Default)}";
        })));
        sb.Append(')');

        if (f.Returns != "void")
        {
            sb.Append(" -&gt; ").Append(TypeReferenceHtml(f.Returns, scope, ns));
        }

        return new MarkupString(sb.Append("</code>").ToString());
    }

    /// <summary>
    /// The function's summary followed by its attributes as badges, each linked to the attributes page
    /// when one is known. <paramref name="required"/> prefixes "Required." for interface members without a default.
    /// A method that implements an interface member and has no summary of its own borrows the interface's,
    /// and gets a badge linking to that member, so <c>iter()</c> on an adapter type is not a blank row.
    /// </summary>
    public MarkupString Description(StdlibFunction f, StdlibNamespace ns, string? attributesPageUrl, bool required = false)
    {
        var text = SummaryHtml(f.Summary);
        var badges = new List<string>();

        if (f.Implements is not null && Resolve(StdlibModel.BareName(f.Implements), ns) is var (interfaceNs, @interface))
        {
            var member = @interface.Methods.FirstOrDefault(m => m.Name == f.Name);
            if (text.Length == 0 && member is not null)
            {
                text = SummaryHtml(member.Summary);
            }

            var label = $"{@interface.DisplayName}.{f.Name}";
            var anchor = MemberAnchor(@interface.DisplayName, f.Name);
            badges.Add($"<a href=\"{RoutePrefix}{interfaceNs.Slug}/#{anchor}\"><code>{Escape(label)}</code></a>");
        }

        if (required)
        {
            text = "Required. " + text;
        }

        foreach (var attribute in f.Attributes)
        {
            var badge = $"<code>{Escape(attribute)}</code>";
            if (attributesPageUrl is not null)
            {
                badge = $"<a href=\"{Escape(attributesPageUrl)}#{Escape(attribute)}\">{badge}</a>";
            }

            badges.Add(badge);
        }

        foreach (var badge in badges)
        {
            text = text.Length == 0 ? badge : $"{text} {badge}";
        }

        return new MarkupString(text);
    }

    /// <summary>
    /// The id of a member's row: the owner's anchor and the member name, so <c>Enumerable&lt;T&gt;.filter</c>
    /// is <c>#enumerablet-filter</c>. Free functions use the name alone.
    /// </summary>
    public static string MemberAnchor(string? owner, string name) =>
        Anchors.For(owner is null ? name : $"{owner}-{name}");

    /// <summary>A union case with its payload types, as code: <c>Some(T)</c>.</summary>
    public MarkupString Case(StdlibCase c, ISet<string> scope, StdlibNamespace current)
    {
        var payload = c.Payloads.IsEmpty
            ? ""
            : $"({string.Join(", ", c.Payloads.Select(p => TypeReferenceHtml(p, scope, current)))})";
        return new MarkupString($"<code>{Escape(c.Name)}{payload}</code>");
    }

    /// <summary>A field with its type, as code: <c>mut name: Type</c>.</summary>
    public MarkupString Field(StdlibField f, ISet<string> scope, StdlibNamespace current) =>
        new($"<code>{(f.Mutable ? "mut " : "")}{Escape(f.Name)}: {TypeReferenceHtml(f.Type, scope, current)}</code>");

    /// <summary>A type reference wrapped in <c>&lt;code&gt;</c>, for prose that names a type.</summary>
    public MarkupString Code(string type, ISet<string> scope, StdlibNamespace current) =>
        new($"<code>{TypeReferenceHtml(type, scope, current)}</code>");

    /// <summary>A link to a type's section on its own page, shown as code. <paramref name="text"/> defaults to the display name.</summary>
    public static MarkupString Link(StdlibNamespace ns, StdlibType type, string? text = null) =>
        new($"<code>{LinkHtml(ns, type, text ?? type.DisplayName)}</code>");

    /// <summary>A link to a section on the current page, shown as code.</summary>
    public static MarkupString AnchorLink(string anchor, string text) =>
        new($"<a href=\"#{anchor}\"><code>{Escape(text)}</code></a>");

    /// <summary>Escapes a summary and turns its backtick spans into <c>&lt;code&gt;</c>.</summary>
    public static MarkupString Summary(string? summary) => new(SummaryHtml(summary));

    /// <summary>Joins already-rendered items as "a, b and c".</summary>
    public static MarkupString JoinWithAnd(IEnumerable<MarkupString> items)
    {
        var list = items.Select(i => i.Value).ToList();
        return new MarkupString(list.Count switch
        {
            0 => string.Empty,
            1 => list[0],
            _ => $"{string.Join(", ", list.Take(list.Count - 1))} and {list[^1]}",
        });
    }

    public static string Escape(string text) => WebUtility.HtmlEncode(text);

    private string TypeReferenceHtml(string type, ISet<string> scope, StdlibNamespace current)
    {
        var sb = new StringBuilder(type.Length + 32);
        foreach (Match token in TypeTokens().Matches(type))
        {
            var text = token.Value;
            if (!char.IsLetter(text[0]) && text[0] != '_')
            {
                sb.Append(Escape(text));
                continue;
            }

            if (text == "fn" || scope.Contains(text) || Primitives.Contains(text))
            {
                sb.Append(Escape(text));
                continue;
            }

            sb.Append(Resolve(text, current) is var (ns, declared) ? LinkHtml(ns, declared, text) : Escape(text));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Finds the type a bare name refers to: qualified names look in their namespace, unqualified ones in
    /// the current namespace and then <c>std</c>, which is in scope everywhere. Null when nothing declares it.
    /// </summary>
    private (StdlibNamespace Namespace, StdlibType Type)? Resolve(string name, StdlibNamespace current)
    {
        var dot = name.LastIndexOf('.');
        if (dot >= 0)
        {
            var ns = _model.FindNamespace(name[..dot]);
            var type = ns?.FindType(name[(dot + 1)..]);
            return ns is not null && type is not null ? (ns, type) : null;
        }

        if (current.FindType(name) is { } local)
        {
            return (current, local);
        }

        var std = _model.FindNamespace("std");
        return std?.FindType(name) is { } shared ? (std, shared) : null;
    }

    private static string LinkHtml(StdlibNamespace ns, StdlibType type, string text) =>
        $"<a href=\"{RoutePrefix}{ns.Slug}/#{type.Anchor}\">{Escape(text)}</a>";

    private static string SummaryHtml(string? summary)
    {
        if (string.IsNullOrEmpty(summary))
        {
            return string.Empty;
        }

        return Backticks().Replace(Escape(summary), m => $"<code>{m.Groups[1].Value}</code>");
    }

    [GeneratedRegex(@"[A-Za-z_][A-Za-z0-9_.]*|[^A-Za-z_]+")]
    private static partial Regex TypeTokens();

    [GeneratedRegex("`([^`]+)`")]
    private static partial Regex Backticks();
}
