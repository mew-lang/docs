namespace MewDocs.Styling;

using System.Collections.Immutable;
using MonorailCss.Theme;
using Pennington.MonorailCss;

/// <summary>
/// The site's look on top of the DocSite template: syntax colors, <c>@apply</c> overrides for
/// the template's code blocks and alerts, CSS for the article chrome, and prose rules for
/// links and inline code. The palettes these name come from <see cref="MewColorScheme"/>.
/// </summary>
/// <remarks>
/// Entries in <see cref="Applies"/> replace Pennington's own blocks for the same selector, so a
/// code-block or alert rule here is the whole rule, not an addition to it.
/// </remarks>
internal static class MewStyling
{
    /// <summary>
    /// Comments, plain identifiers and function names stay in the text grays, so color is
    /// spent on what a reader scans for: keywords, strings, types and constants. Types and
    /// constants have no slot here; <see cref="Applies"/> colors them.
    /// </summary>
    public static SyntaxTheme SyntaxTheme { get; } = new()
    {
        Keyword = "primary",
        String = "mew-string",
        Variable = "base",
        Function = "base",
        Comment = "base",
    };

    /// <summary><c>@apply</c> blocks, keyed by selector, that replace or add to Pennington's.</summary>
    public static readonly ImmutableDictionary<string, string> Applies =
        ImmutableDictionary.CreateRange(new Dictionary<string, string>
        {
            // Keywords use the brighter 400 step in the dark theme; 300 washes the orange out.
            { ".hljs-keyword", "text-primary-700 dark:text-primary-400" },
            { ".hljs-type", "text-mew-type-700 dark:text-mew-type-300" },
            { ".hljs-number", "text-mew-constant-700 dark:text-mew-constant-300" },
            { ".hljs-literal", "text-mew-constant-700 dark:text-mew-constant-300" },
            { ".hljs-comment", "text-base-500 italic dark:text-base-400" },

            // Code blocks: one surface for the label bar and the code, a hairline between them.
            {
                ".code-highlight-wrapper",
                "my-6 rounded-lg border border-base-200 bg-base-50 dark:bg-base-900 dark:border-base-800 overflow-hidden"
            },
            {
                ".code-highlight-wrapper .codeblock-head",
                "flex items-center justify-between px-4 py-1.5 border-b border-base-200 dark:border-base-800 font-mono text-[12px]"
            },
            { ".code-highlight-wrapper .codeblock-lang", "text-base-500" },
            {
                ".tab-container",
                "flex flex-col rounded-lg overflow-hidden border border-base-200 bg-base-50 dark:bg-base-900 dark:border-base-800"
            },
            {
                ".tab-list",
                "flex flex-row flex-nowrap overflow-x-auto px-2 border-b border-base-200 dark:border-base-800 [scrollbar-width:none] [&::-webkit-scrollbar]:hidden"
            },

            // A note is a quiet aside, not a blue box: a raised neutral panel with an orange
            // icon. Important keeps Pennington's orange tint and warning its amber.
            {
                ".markdown-alert-note",
                "bg-base-200/40 border-base-200 dark:bg-base-900 dark:border-base-800 fill-primary-600 dark:fill-primary-400"
            },
            { ".markdown-alert-note .markdown-alert-title", "text-base-900 dark:text-base-50" },

            // Nested sidebar groups hang off a hairline, like a tree.
            {
                "#nav-sidebar nav > ul > li > ul",
                "ml-2.5 mb-2 pl-3 border-l border-base-200 dark:border-base-800"
            },
        });

    /// <summary>
    /// Restyles parts of the article that the DocSite template renders with fixed utility
    /// classes. An <c>@apply</c> block can't do this: utilities sit in a later cascade layer
    /// and win whatever the selector. This CSS goes into <c>ExtraStyles</c>, which is not
    /// layered, so it wins instead. The selectors reach in by the template's structure.
    /// </summary>
    public const string ArticleChrome = """
        /* The area label above the title reads as a quiet path segment, not a badge. */
        #main-content > header > span {
            margin-bottom: 0.75rem;
            padding: 0;
            border: 0;
            border-radius: 0;
            background: none;
            font-family: var(--font-mono);
            font-size: 12.5px;
            font-weight: 400;
            letter-spacing: normal;
            text-transform: lowercase;
            color: var(--color-base-500);
        }

        .dark #main-content > header > span {
            color: var(--color-base-400);
        }

        #main-content > header h1 {
            font-weight: 600;
            letter-spacing: -0.02em;
        }

        #main-content > header p {
            font-size: 1.125rem;
        }

        /* Body copy at 16px; the template's prose-sm is 14px. Prose sizes are in em, so
           headings and spacing scale along with it. */
        #main-content > .prose {
            font-size: 1rem;
            line-height: 1.7;
        }

        nav[aria-label="Page navigation"] > a {
            border-radius: 0.5rem;
        }

        nav[aria-label="Page navigation"] > a > span:first-child {
            font-family: var(--font-mono);
            font-size: 12px;
            text-transform: lowercase;
        }
        """;

    /// <summary>
    /// Links are underlined faintly and the line darkens on hover. Inline code is a small
    /// bordered chip, one step off the page in either theme, so a name like <c>count</c> does
    /// not read like the words around it. Code blocks are <c>pre &gt; code</c> and keep their
    /// own styling.
    /// </summary>
    public static ProseCustomization ExtendProse(ProseCustomization baseline) => new()
    {
        Customization = theme =>
        {
            string Color(string color, string shade) =>
                theme.ResolveValue(shade, [$"--color-{color}"]) ?? "#000000";

            string Mix(string color, string percent) =>
                $"color-mix(in oklch, {color} {percent}, transparent)";

            var mew = new Dictionary<string, ProseElementRules>
            {
                ["DEFAULT"] = Rules(
                    Rule("a",
                        ("color", Color("primary", "700")),
                        ("font-weight", "500"),
                        ("text-decoration-line", "underline"),
                        ("text-decoration-thickness", "1px"),
                        ("text-underline-offset", "3px"),
                        ("text-decoration-color", Mix(Color("primary", "700"), "30%")),
                        ("transition", "text-decoration-color .15s")),
                    Rule("a:hover",
                        ("text-decoration-color", Color("primary", "700"))),
                    Rule(":not(pre) > code",
                        ("padding", "1px 0.3em"),
                        ("border", $"1px solid {Color("base", "300")}"),
                        ("border-radius", "0.25rem"),
                        ("background-color", Color("base", "200")),
                        ("overflow-wrap", "break-word"),
                        ("box-decoration-break", "clone"),
                        ("-webkit-box-decoration-break", "clone"))),
                ["invert"] = Rules(
                    Rule("a",
                        ("color", Color("primary", "400")),
                        ("text-decoration-color", Mix(Color("primary", "400"), "35%"))),
                    Rule("a:hover",
                        ("text-decoration-color", Color("primary", "400"))),
                    Rule(":not(pre) > code",
                        ("border-color", Color("base", "800")),
                        ("background-color", Color("base", "900")))),
            };

            return Merge(baseline.Customization(theme), mew);
        },
    };

    // Our rules replace the baseline's for the same selector; everything else is kept.
    private static ImmutableDictionary<string, ProseElementRules> Merge(
        ImmutableDictionary<string, ProseElementRules> baseline,
        Dictionary<string, ProseElementRules> overlay)
    {
        var result = baseline;
        foreach (var (section, rules) in overlay)
        {
            if (baseline.TryGetValue(section, out var existing))
            {
                var replaced = rules.Rules.Select(r => r.Selector).ToHashSet();
                result = result.SetItem(section, new ProseElementRules
                {
                    Rules = [.. existing.Rules.Where(r => !replaced.Contains(r.Selector)), .. rules.Rules],
                });
            }
            else
            {
                result = result.SetItem(section, rules);
            }
        }

        return result;
    }

    private static ProseElementRule Rule(string selector, params (string Property, string Value)[] declarations) =>
        new()
        {
            Selector = selector,
            Declarations = [.. declarations.Select(d => new ProseDeclaration { Property = d.Property, Value = d.Value })],
        };

    private static ProseElementRules Rules(params ProseElementRule[] rules) => new() { Rules = [.. rules] };
}
