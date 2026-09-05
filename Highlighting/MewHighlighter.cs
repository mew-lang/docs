namespace MewDocs.Highlighting;

using System.Net;
using System.Text;
using Pennington.Highlighting;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

public sealed class MewHighlighter : ICodeHighlighter
{
    private const string ScopeName = "source.mew";
    private const string GrammarResource = "MewDocs.Highlighting.mew.tmLanguage.json";

    private static readonly TimeSpan TokenizeTimeLimit = TimeSpan.FromSeconds(5);
    private static readonly Lock RegistryAccessLock = new();
    private static readonly Lazy<IGrammar?> Grammar = new(LoadGrammar, isThreadSafe: true);

    private static readonly (string Scope, string CssClass)[] ScopeMappings =
    [
        ("comment", "hljs-comment"),
        ("punctuation.definition.comment", "hljs-comment"),

        ("entity.name.function", "hljs-title"),
        ("entity.name.type", "hljs-type"),

        ("keyword.control", "hljs-keyword"),
        ("keyword.operator", "hljs-operator"),
        ("keyword", "hljs-keyword"),

        ("storage.type", "hljs-keyword"),
        ("storage.modifier", "hljs-keyword"),

        ("constant.numeric", "hljs-number"),
        ("constant.language", "hljs-literal"),
        ("constant.character.escape", "hljs-regexp"),
        ("constant.other", "hljs-literal"),

        ("string", "hljs-string"),
        ("punctuation.definition.string", "hljs-string"),

        ("support.function", "hljs-built_in"),
        ("support.type", "hljs-keyword"),
        ("support.constant", "hljs-literal"),

        ("meta.attribute", "hljs-meta"),

        ("punctuation", "hljs-punctuation"),
        ("variable", "hljs-variable"),
    ];

    public IReadOnlySet<string> SupportedLanguages { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "mew" };

    public int Priority => 100;

    public string Highlight(string code, string language)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return string.Empty;
        }

        var grammar = Grammar.Value;
        if (grammar is null)
        {
            return $"<pre><code class=\"language-{language} code\">{WebUtility.HtmlEncode(code)}</code></pre>";
        }

        lock (RegistryAccessLock)
        {
            return TokenizeAndRender(code, grammar);
        }
    }

    private static IGrammar? LoadGrammar()
    {
        // The stock RegistryOptions supplies the theme and the bundled grammars; the locator below
        // adds Mew on top of it, keyed by scope name.
        var options = new MewRegistryOptions(new RegistryOptions(ThemeName.DarkPlus));
        return new Registry(options).LoadGrammar(ScopeName);
    }

    private static string TokenizeAndRender(string code, IGrammar grammar)
    {
        var sb = new StringBuilder();
        sb.Append("<pre><code>");

        var lines = code.Replace("\r\n", "\n").Split('\n');
        IStateStack? ruleStack = null;

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var result = grammar.TokenizeLine(line, ruleStack, TokenizeTimeLimit);
            ruleStack = result.RuleStack;

            var currentIndex = 0;
            foreach (var token in result.Tokens)
            {
                if (token.StartIndex > currentIndex)
                {
                    sb.Append(WebUtility.HtmlEncode(line[currentIndex..token.StartIndex]));
                }

                var length = Math.Min(token.Length, line.Length - token.StartIndex);
                if (length <= 0)
                {
                    continue;
                }

                var text = WebUtility.HtmlEncode(line.Substring(token.StartIndex, length));
                var cssClass = GetHljsClassForScopes(token.Scopes);

                sb.Append(cssClass is null ? text : $"<span class=\"{cssClass}\">{text}</span>");
                currentIndex = token.StartIndex + length;
            }

            if (currentIndex < line.Length)
            {
                sb.Append(WebUtility.HtmlEncode(line[currentIndex..]));
            }

            if (i < lines.Length - 1)
            {
                sb.Append('\n');
            }
        }

        sb.Append("</code></pre>");
        return sb.ToString();
    }

    private static string? GetHljsClassForScopes(List<string> scopes)
    {
        for (var i = scopes.Count - 1; i >= 0; i--)
        {
            foreach (var (scope, cssClass) in ScopeMappings)
            {
                if (scopes[i].StartsWith(scope, StringComparison.Ordinal))
                {
                    return cssClass;
                }
            }
        }

        return null;
    }

    private sealed class MewRegistryOptions(RegistryOptions inner) : IRegistryOptions
    {
        public IRawGrammar? GetGrammar(string scopeName)
        {
            if (!string.Equals(scopeName, ScopeName, StringComparison.Ordinal))
            {
                return inner.GetGrammar(scopeName);
            }

            using var stream = typeof(MewHighlighter).Assembly.GetManifestResourceStream(GrammarResource)
                ?? throw new InvalidOperationException($"Embedded grammar '{GrammarResource}' is missing.");
            using var reader = new StreamReader(stream);
            return GrammarReader.ReadGrammarSync(reader);
        }

        public ICollection<string> GetInjections(string scopeName) => inner.GetInjections(scopeName);

        public IRawTheme GetTheme(string scopeName) => inner.GetTheme(scopeName);

        public IRawTheme GetDefaultTheme() => inner.GetDefaultTheme();
    }
}
