using Mdazor;
using MewDocs.Components;
using MewDocs.Highlighting;
using MewDocs.Client;
using MewDocs.Stdlib;
using MewDocs.Stdlib.Model;
using MewDocs.Styling;
using Pennington.Content;
using Beck;
using Beck.Skia;
using Beck.Text;
using Pennington.Beck;
using Pennington.DocSite;
using Pennington.Favicon;
using Pennington.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDocSite(() => new DocSiteOptions
{
    SiteTitle = "Mew",
    SiteDescription = "A programming language under construction.",
    CanonicalBaseUrl = "https://mew-lang.org",
    GitHubUrl = "https://github.com/mew-lang",

    // The logo is inlined so the cat follows the text color in both themes; only the ear
    // keeps the brand orange. The wordmark is set in the code face, like the language.
    HeaderContent = """
        <a href="/" class="flex items-center gap-2.5 text-base-900 dark:text-base-50">
          <svg viewBox="150 30 1480 1720" class="h-7 w-auto" aria-hidden="true">
            <g transform="translate(-363.35308,-362.98301)" fill="currentColor">
              <path fill="#f68104" d="m 693.261,1880.06 c 0,0 54.006,0.396 117.645,-25.223 126.358,-50.868 291.581,-175.62 245.509,-417.003 L 710.086,995.991 642.345,1607.669 Z" />
              <path d="M 1834.43,2083.83 1557.095,993.82 1352.197,1509.098 c -89.264,-57.675 -187.831,-48.844 -291.364,-4.512 l -281.961,-297.905 -85.609,673.369 -149.764,-0.082 0.694,-1263.1 593.384,626.936 658.219,-826.64 162.613,1666.67 H 1834.43 Z" />
              <circle cx="1127.77" cy="1815.73" r="63.457" />
              <circle cx="1474.37" cy="1815.73" r="63.457" />
              <path d="m 1379.7,1922.25 -51.327,51.327 -51.327,-51.327 c 28.347,-28.347 74.307,-28.347 102.654,0 z" />
            </g>
          </svg>
          <span class="font-mono font-bold text-lg tracking-tight">mew</span>
        </a>
        <span class="hidden sm:inline-block font-mono text-[12px] leading-none text-base-500 dark:text-base-400 border border-base-200 dark:border-base-800 rounded px-1.5 py-1">draft spec</span>
        """,

    // The /playground page lives in this assembly.
    AdditionalRoutingAssemblies = [typeof(Program).Assembly],

    FooterContent = """
        <footer class="mt-16 py-8 border-t border-base-200 dark:border-base-800 text-center text-sm text-base-500">
          <span class="font-mono">mew</span> is a programming language under construction.
        </footer>
        """,

    // Warm stone grays, one orange, and a code-first look: IBM Plex Sans for prose and
    // JetBrains Mono for code and the small labels around it. See Styling/.
    ColorScheme = new MewColorScheme(),
    SyntaxTheme = MewStyling.SyntaxTheme,
    CustomCssFrameworkSettings = settings => settings with
    {
        Applies = settings.Applies.SetItems(MewStyling.Applies),
    },
    ExtendProseCustomization = MewStyling.ExtendProse,

    DisplayFontFamily = "'IBM Plex Sans', ui-sans-serif, system-ui, sans-serif",
    BodyFontFamily = "'IBM Plex Sans', ui-sans-serif, system-ui, sans-serif",
    MonoFontFamily = "'JetBrains Mono', ui-monospace, 'Cascadia Code', Consolas, monospace",
    FontPreloads =
    [
        new FontPreload("/fonts/ibm-plex-sans.woff2"),
        new FontPreload("/fonts/jetbrains-mono.woff2"),
    ],

    ExtraStyles = $$"""
        /* Both faces are variable and cut to Latin; anything outside the range falls back
           to the system stack. */
        @font-face {
            font-family: 'IBM Plex Sans';
            font-style: normal;
            font-weight: 100 700;
            font-display: swap;
            src: url(/fonts/ibm-plex-sans.woff2) format('woff2');
            unicode-range: U+0000-00FF, U+0131, U+0152-0153, U+02BB-02BC, U+02C6, U+02DA, U+02DC, U+0304, U+0308, U+0329, U+2000-206F, U+20AC, U+2122, U+2191, U+2193, U+2212, U+2215, U+FEFF, U+FFFD;
        }

        @font-face {
            font-family: 'JetBrains Mono';
            font-style: normal;
            font-weight: 100 800;
            font-display: swap;
            src: url(/fonts/jetbrains-mono.woff2) format('woff2');
            unicode-range: U+0000-00FF, U+0131, U+0152-0153, U+02BB-02BC, U+02C6, U+02DA, U+02DC, U+0304, U+0308, U+0329, U+2000-206F, U+20AC, U+2122, U+2191, U+2193, U+2212, U+2215, U+FEFF, U+FFFD;
        }

        body {
            -webkit-font-smoothing: antialiased;
        }

        /* JetBrains Mono draws == and -> as single glyphs. A language reference should show
           the characters you type. */
        code, kbd, pre {
            font-variant-ligatures: none;
        }

        {{MewStyling.ArticleChrome}}
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
        // The /playground route is a Razor page in this assembly. The area lists it in the
        // sidebar; its index.md exists only so the entry is a direct link rather than a
        // TOC toggle, and the Razor route wins over it when the URL is served.
        new ContentArea("Playground", "playground"),
    ],

    ConfigurePennington = penn =>
    {
        penn.Highlighting.AddHighlighter(new MewHighlighter());
        penn.Highlighting.AddHighlighter(new IlHighlighter());
    },
});

// The playground island renders as WebAssembly in the browser. It is prerendered here first,
// which constructs it, so its service has to exist on the server as well. <MewPipeline> in
// Markdown uses the same service to compile its program while the page renders.
builder.Services.AddSingleton<MewCompiler>();
builder.Services.AddMdazorComponent<MewPipeline>();

// Diagrams are Beck YAML in ```beck fences, rendered to SVG on the server. The DocSite
// template switches themes with a .dark class on <html>, not a data-theme attribute, so the
// SVG is told to key its dark palette off that class.
//
// Labels are measured with the same faces the page renders (IBM Plex Sans and JetBrains Mono)
// rather than Beck's embedded Inter metrics, so a card is sized to the text it will actually
// hold. The browser loads the woff2 files under wwwroot/fonts; Skia reads the TTF cuts of the
// same faces from resources/fonts, which are never served.
var fontDir = Path.Combine(builder.Environment.ContentRootPath, "resources", "fonts");
var beckFont = new BeckFontSpec
{
    Family = "IBM Plex Sans",
    MonoFamily = "JetBrains Mono",
    Files = new Dictionary<int, string>
    {
        [400] = Path.Combine(fontDir, "IBMPlexSans-Regular.ttf"),
        [500] = Path.Combine(fontDir, "IBMPlexSans-Medium.ttf"),
        [600] = Path.Combine(fontDir, "IBMPlexSans-SemiBold.ttf"),
        [700] = Path.Combine(fontDir, "IBMPlexSans-Bold.ttf"),
    },
    MonoFiles = new Dictionary<int, string>
    {
        [400] = Path.Combine(fontDir, "JetBrainsMono-Regular.ttf"),
        [500] = Path.Combine(fontDir, "JetBrainsMono-Medium.ttf"),
        [700] = Path.Combine(fontDir, "JetBrainsMono-Bold.ttf"),
    },
};
builder.Services.AddPenningtonBeck(beck =>
{
    beck.RenderOptions = new SvgRenderOptions
    {
        ThemeHooks = ThemeHooks.Class,
        Font = beckFont,
        Measurer = new SkiaTextMeasurer(beckFont),
    };
});

// The standard library reference is generated from resources/stdlib.xml rather than written by
// hand: one page per namespace, published into the "Standard Library" area by a content service.
builder.Services.AddSingleton(_ =>
    StdlibXmlLoader.Load(Path.Combine(builder.Environment.ContentRootPath, "resources", "stdlib.xml")));
builder.Services.AddSingleton<StdlibHtml>();
builder.Services.AddSingleton<StdlibContentService>();
builder.Services.AddSingleton<IContentService>(sp => sp.GetRequiredService<StdlibContentService>());

var app = builder.Build();

app.UseDocSite();

await app.RunDocSiteAsync(args);
