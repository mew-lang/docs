using Mdazor;
using MewDocs.Components;
using MewDocs.Highlighting;
using MewDocs.Pipeline;
using Pennington.DocSite;
using Pennington.Favicon;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDocSite(() => new DocSiteOptions
{
    SiteTitle = "Mew",
    SiteDescription = "A programming language under construction.",
    CanonicalBaseUrl = "https://mew-lang.org",
    GitHubUrl = "https://github.com/mew-lang",

    HeaderContent = """
        <a href="/" class="flex items-center gap-2 font-semibold">
          <img src="/img/logo.svg" alt="" width="28" height="28" class="rounded bg-white p-px" />
          <span>Mew</span>
        </a>
        """,

    FooterContent = """
        <footer class="mt-16 py-8 text-center text-sm text-base-500">
          Mew is a programming language under construction.
        </footer>
        """,

    ExtraStyles = """
        #nav-sidebar nav > ul > li > ul {
            margin-left: 0.625rem;
            margin-bottom: 0.5rem;
            padding-left: 0.75rem;
            border-left: 1px solid var(--color-base-200);
        }

        .dark #nav-sidebar nav > ul > li > ul {
            border-left-color: var(--color-base-800);
        }

        /* The prose defaults only switch inline code to a monospace font, so a
           name like `count` reads like the words around it. Give it a tinted
           pill instead. The fill is one step from the page background toward
           the text, and the border one step further, in both themes. Code
           blocks are `pre > code` and keep their own styling. */
        .prose :not(pre) > code:not(.not-prose *) {
            padding: 1px 0.25rem;
            border: 1px solid var(--color-base-300);
            border-radius: 0.25rem;
            background-color: var(--color-base-200);
            -webkit-box-decoration-break: clone;
            box-decoration-break: clone;
        }

        .dark .prose :not(pre) > code:not(.not-prose *) {
            border-color: var(--color-base-700);
            background-color: var(--color-base-800);
        }

        .prose :not(pre) > code::before,
        .prose :not(pre) > code::after {
            content: none;
        }

        /* Mermaid renders client-side by swapping the <code> for an <svg>, which
           leaves the code-block chrome and its "mermaid" language label wrapped
           around a diagram. Drop the chrome and centre the figure. */
        [data-language="mermaid"] .codeblock-head {
            display: none;
        }

        [data-language="mermaid"],
        [data-language="mermaid"] .standalone-code-container,
        [data-language="mermaid"] .standalone-code-highlight,
        [data-language="mermaid"] pre {
            background: none;
            border: 0;
            padding: 0;
            margin: 0;
        }

        [data-language="mermaid"] .mermaid-diagram {
            display: flex;
            justify-content: center;
        }

        /* Pennington maps mermaid's subgraph fill to primary-700 and its label to
           the accent, which on this palette is a solid purple panel captioned in
           lime. Redraw subgraphs as quiet panels instead. Mermaid inlines its own
           <style> into the SVG under a generated id, so these need !important. */
        .mermaid-diagram .cluster rect {
            fill: var(--color-base-200) !important;
            stroke: var(--color-base-300) !important;
        }

        .mermaid-diagram .cluster text,
        .mermaid-diagram .cluster span,
        .mermaid-diagram .cluster-label text,
        .mermaid-diagram .cluster-label span {
            fill: var(--color-base-700) !important;
            color: var(--color-base-700) !important;
        }

        .dark .mermaid-diagram .cluster rect {
            fill: var(--color-base-900) !important;
            stroke: var(--color-base-700) !important;
        }

        .dark .mermaid-diagram .cluster text,
        .dark .mermaid-diagram .cluster span,
        .dark .mermaid-diagram .cluster-label text,
        .dark .mermaid-diagram .cluster-label span {
            fill: var(--color-base-200) !important;
            color: var(--color-base-200) !important;
        }
        """,

    Favicons = new FaviconOptions
    {
        Icons =
        [
            new FaviconLink("/img/favicon.svg") { Type = "image/svg+xml" },
            new FaviconLink("/img/favicon.png") { Type = "image/png", Sizes = "32x32" },
            new FaviconLink("/img/favicon.ico") { Sizes = "any" },
        ],
    },

    Areas =
    [
        new ContentArea("Getting Started", "getting-started"),
        new ContentArea("Compiler", "compiler"),
        new ContentArea("Language", "language"),
        new ContentArea("Standard Library", "stdlib"),
        new ContentArea("Future", "future"),
    ],

    ConfigurePennington = penn =>
    {
        penn.Highlighting.AddHighlighter(new MewHighlighter());
        penn.Highlighting.AddHighlighter(new IlHighlighter());
    },
});

// <MewPipeline> in Markdown compiles its program while the page renders and shows every stage.
builder.Services.AddSingleton<MewCompiler>();
builder.Services.AddMdazorComponent<MewPipeline>();

var app = builder.Build();
app.UseDocSite();
await app.RunDocSiteAsync(args);
