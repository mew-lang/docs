namespace MewDocs.Stdlib;

using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MewDocs.Stdlib.Model;

/// <summary>
/// Renders one namespace of the standard library as the HTML body of a documentation page.
/// The output is the same shape the hand-written Markdown pages produced (overview tables, then a
/// section per type), so it sits inside the DocSite's <c>prose</c> styling and keeps the heading
/// ids the rest of the site links to. Type names in signatures link to the page that declares them.
/// </summary>
public sealed partial class StdlibHtmlRenderer
{
    public const string RoutePrefix = "/stdlib/";

    private static readonly HashSet<string> Primitives = new(StringComparer.Ordinal)
    {
        "void", "bool", "char", "string",
        "i8", "i16", "i32", "i64", "u8", "u16", "u32", "u64", "f32", "f64",
    };

    private readonly StdlibModel _model;

    public StdlibHtmlRenderer(StdlibModel model)
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

    /// <param name="ns">The namespace to render.</param>
    /// <param name="attributesPageUrl">Where the language docs describe attributes, so <c>noreturn</c> can link to it. Null leaves it unlinked.</param>
    public string RenderNamespace(StdlibNamespace ns, string? attributesPageUrl)
    {
        var sb = new StringBuilder();
        var unions = ns.Unions.ToList();
        var interfaces = ns.Interfaces.ToList();
        var types = ns.PlainTypes.ToList();

        // Overview tables, one per kind, in the order the hand-written pages used.
        OverviewTable(sb, "Unions", "Union", unions);
        OverviewTable(sb, "Interfaces", "Interface", interfaces);
        OverviewTable(sb, "Types", "Type", types);

        if (ns.Functions.Length > 0)
        {
            Heading(sb, 2, "Functions", "functions");
            Table(sb, "Function", "Description", ns.Functions.Select(f =>
                (Signature(f, owner: null, ns, outerScope: new HashSet<string>()), Description(f, attributesPageUrl))));
        }

        if (ns.Extensions.Length > 0)
        {
            Heading(sb, 2, "Extensions", "extensions");
            Table(sb, "Type", "Description", ns.Extensions.Select(e =>
                ($"<a href=\"#{e.Anchor}\"><code>{Escape(e.Type)}</code></a>", $"What <code>{Escape(ns.Name)}</code> adds to <code>{Escape(e.Type)}</code>")));
        }

        // One section per declaration.
        foreach (var type in unions.Concat(interfaces).Concat(types))
        {
            TypeSection(sb, ns, type, attributesPageUrl);
        }

        foreach (var extension in ns.Extensions)
        {
            Heading(sb, 2, $"<code>{Escape(extension.Type)}</code>", extension.Anchor);
            sb.Append("<p>The members <code>").Append(Escape(ns.Name)).Append("</code> adds to <code>")
              .Append(Escape(extension.Type)).Append("</code>.</p>\n");
            MemberTable(sb, ns, extension.Methods, owner: extension.Type, outerScope: new HashSet<string>(), requiredMarker: false, attributesPageUrl);
        }

        return sb.ToString();
    }

    private void OverviewTable(StringBuilder sb, string heading, string column, List<StdlibType> types)
    {
        if (types.Count == 0)
        {
            return;
        }

        Heading(sb, 2, heading, Anchors.For(heading));
        Table(sb, column, "Description", types.Select(t =>
            ($"<a href=\"#{t.Anchor}\"><code>{Escape(t.DisplayName)}</code></a>", Summary(t.Summary))));
    }

    private void TypeSection(StringBuilder sb, StdlibNamespace ns, StdlibType type, string? attributesPageUrl)
    {
        var scope = new HashSet<string>(type.TypeParameters, StringComparer.Ordinal);

        Heading(sb, 2, $"<code>{Escape(type.DisplayName)}</code>", type.Anchor);

        if (type.Summary is not null)
        {
            sb.Append("<p>").Append(Summary(type.Summary)).Append("</p>\n");
        }

        if (type.Implements.Length > 0)
        {
            sb.Append("<p>Implements ")
              .Append(JoinWithAnd(type.Implements.Select(i => $"<code>{TypeReference(i, scope, ns)}</code>")))
              .Append(".</p>\n");
        }

        if (type.Kind == StdlibTypeKind.Interface
            && _model.Implementors.TryGetValue(type.Name, out var implementors)
            && implementors.Length > 0)
        {
            sb.Append("<p>Implemented by ")
              .Append(JoinWithAnd(implementors
                  .OrderBy(i => i.Namespace.Name, StringComparer.Ordinal)
                  .ThenBy(i => i.Type.Name, StringComparer.Ordinal)
                  .Select(i => $"<code>{Link(i.Namespace, i.Type, i.Namespace == ns ? i.Type.DisplayName : $"{i.Namespace.Name}.{i.Type.DisplayName}")}</code>")))
              .Append(".</p>\n");
        }

        if (type.Cases.Length > 0)
        {
            Table(sb, "Case", "Description", type.Cases.Select(c =>
            {
                var payload = c.Payloads.IsEmpty
                    ? ""
                    : $"({string.Join(", ", c.Payloads.Select(p => TypeReference(p, scope, ns)))})";
                return ($"<code>{Escape(c.Name)}{payload}</code>", Summary(c.Summary));
            }));
        }

        if (type.Fields.Length > 0)
        {
            Table(sb, "Field", "Description", type.Fields.Select(f =>
                ($"<code>{(f.Mutable ? "mut " : "")}{Escape(f.Name)}: {TypeReference(f.Type, scope, ns)}</code>", Summary(f.Summary))));
        }

        if (type.Methods.Length > 0)
        {
            MemberTable(sb, ns, type.Methods, owner: type.DisplayName, scope,
                requiredMarker: type.Kind == StdlibTypeKind.Interface, attributesPageUrl);
        }
    }

    private void MemberTable(
        StringBuilder sb,
        StdlibNamespace ns,
        IEnumerable<StdlibFunction> methods,
        string owner,
        ISet<string> outerScope,
        bool requiredMarker,
        string? attributesPageUrl)
    {
        Table(sb, "Member", "Description", methods.Select(m =>
        {
            var description = Description(m, attributesPageUrl);
            if (requiredMarker && !m.IsDefault)
            {
                description = "Required. " + description;
            }

            return (Signature(m, owner, ns, outerScope), description);
        }));
    }

    /// <summary>A Mew-style signature, with every type name linked where the library declares it.</summary>
    private string Signature(StdlibFunction f, string? owner, StdlibNamespace ns, ISet<string> outerScope)
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
            var text = $"{Escape(p.Name)}: {(p.Variadic ? "..." : "")}{TypeReference(p.Type, scope, ns)}";
            return p.Default is null ? text : $"{text} = {Escape(p.Default)}";
        })));
        sb.Append(')');

        if (f.Returns != "void")
        {
            sb.Append(" -&gt; ").Append(TypeReference(f.Returns, scope, ns));
        }

        return sb.Append("</code>").ToString();
    }

    private static string Description(StdlibFunction f, string? attributesPageUrl)
    {
        var text = Summary(f.Summary);
        foreach (var attribute in f.Attributes)
        {
            var badge = $"<code>{Escape(attribute)}</code>";
            if (attributesPageUrl is not null)
            {
                badge = $"<a href=\"{Escape(attributesPageUrl)}#{Escape(attribute)}\">{badge}</a>";
            }

            text = text.Length == 0 ? badge : $"{text} {badge}";
        }

        return text;
    }

    /// <summary>
    /// Renders a type reference such as <c>fn(A, T) -&gt; Result&lt;T, std.io.File[]&gt;</c>, escaping it and
    /// wrapping each declared type name in a link to its section. Type parameters and primitives stay plain.
    /// </summary>
    private string TypeReference(string type, ISet<string> scope, StdlibNamespace current)
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

            var dot = text.LastIndexOf('.');
            StdlibNamespace? ns;
            StdlibType? declared;
            if (dot >= 0)
            {
                ns = _model.FindNamespace(text[..dot]);
                declared = ns?.FindType(text[(dot + 1)..]);
            }
            else
            {
                ns = current;
                declared = current.FindType(text);
                if (declared is null)
                {
                    ns = _model.FindNamespace("std");
                    declared = ns?.FindType(text);
                }
            }

            sb.Append(ns is not null && declared is not null ? Link(ns, declared, text) : Escape(text));
        }

        return sb.ToString();
    }

    private static string Link(StdlibNamespace ns, StdlibType type, string text) =>
        $"<a href=\"{RoutePrefix}{ns.Slug}/#{type.Anchor}\">{Escape(text)}</a>";

    /// <summary>Escapes a summary and turns its backtick spans into <c>&lt;code&gt;</c>.</summary>
    private static string Summary(string? summary)
    {
        if (string.IsNullOrEmpty(summary))
        {
            return "";
        }

        return Backticks().Replace(Escape(summary), m => $"<code>{m.Groups[1].Value}</code>");
    }

    private static void Heading(StringBuilder sb, int level, string innerHtml, string id) =>
        sb.Append($"<h{level} id=\"{id}\">{innerHtml}</h{level}>\n");

    /// <summary>
    /// A two-column table for browsers, plus a list twin for the llms.txt sidecar. Pennington turns each page
    /// into Markdown by walking its HTML, and that walker knows headings, paragraphs, lists and code but
    /// not tables, so the table is marked humans-only and the list robots-only.
    /// </summary>
    private static void Table(StringBuilder sb, string firstColumn, string secondColumn, IEnumerable<(string Html, string DescriptionHtml)> rows)
    {
        var list = rows.ToList();

        sb.Append("<div class=\"humans-only\">\n<table>\n<thead>\n<tr><th>").Append(Escape(firstColumn))
          .Append("</th><th>").Append(Escape(secondColumn)).Append("</th></tr>\n</thead>\n<tbody>\n");
        foreach (var (html, description) in list)
        {
            sb.Append("<tr><td>").Append(html).Append("</td><td>").Append(description).Append("</td></tr>\n");
        }

        sb.Append("</tbody>\n</table>\n</div>\n");

        sb.Append("<ul class=\"robots-only\">\n");
        foreach (var (html, description) in list)
        {
            sb.Append("<li>").Append(html);
            if (description.Length > 0)
            {
                sb.Append(" — ").Append(description);
            }

            sb.Append("</li>\n");
        }

        sb.Append("</ul>\n");
    }

    private static string JoinWithAnd(IEnumerable<string> items)
    {
        var list = items.ToList();
        return list.Count switch
        {
            0 => "",
            1 => list[0],
            _ => $"{string.Join(", ", list.Take(list.Count - 1))} and {list[^1]}",
        };
    }

    private static string Escape(string text) => WebUtility.HtmlEncode(text);

    [GeneratedRegex(@"[A-Za-z_][A-Za-z0-9_.]*|[^A-Za-z_]+")]
    private static partial Regex TypeTokens();

    [GeneratedRegex("`([^`]+)`")]
    private static partial Regex Backticks();
}
