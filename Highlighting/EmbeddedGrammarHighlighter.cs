namespace MewDocs.Highlighting;

using System.Net;
using System.Text;
using Pennington.Highlighting;
using TextMateSharp.Grammars;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

/// <summary>
/// Highlights a language with a TextMate grammar embedded in this assembly, rendering the
/// tokens as the <c>hljs-</c> classes the site's code styles know. Subclasses name the
/// grammar's scope, its resource and the fence languages it serves.
/// </summary>
public abstract class EmbeddedGrammarHighlighter : ICodeHighlighter
{
    private static readonly TimeSpan TokenizeTimeLimit = TimeSpan.FromSeconds(5);

    // TextMateSharp's tokenizer is not thread-safe, and the grammars share one registry.
    private static readonly Lock RegistryAccessLock = new();

    private static readonly (string Scope, string CssClass)[] ScopeMappings =
    [
        ("comment", "hljs-comment"),
        ("punctuation.definition.comment", "hljs-comment"),

        ("entity.name.function", "hljs-title"),
        ("entity.name.type", "hljs-type"),
        ("entity.name.label", "hljs-symbol"),
        ("entity.name.namespace", "hljs-meta"),

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

    private readonly Lazy<IGrammar?> _grammar;

    protected EmbeddedGrammarHighlighter(string scopeName, string grammarResource, params string[] languages)
    {
        SupportedLanguages = new HashSet<string>(languages, StringComparer.OrdinalIgnoreCase);
        _grammar = new Lazy<IGrammar?>(() => LoadGrammar(scopeName, grammarResource), isThreadSafe: true);
    }

    public IReadOnlySet<string> SupportedLanguages { get; }

    public int Priority => 100;

    public string Highlight(string code, string language)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return string.Empty;
        }

        var grammar = _grammar.Value;
        if (grammar is null)
        {
            return $"<pre><code class=\"language-{language} code\">{WebUtility.HtmlEncode(code)}</code></pre>";
        }

        lock (RegistryAccessLock)
        {
            return TokenizeAndRender(code, grammar);
        }
    }

    private static IGrammar? LoadGrammar(string scopeName, string grammarResource)
    {
        // The stock RegistryOptions supplies the theme and the bundled grammars; the locator below
        // adds the embedded grammar on top of it, keyed by scope name.
        var options = new EmbeddedRegistryOptions(new RegistryOptions(ThemeName.DarkPlus), scopeName, grammarResource);
        return new Registry(options).LoadGrammar(scopeName);
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

    private sealed class EmbeddedRegistryOptions(IRegistryOptions inner, string scopeName, string grammarResource) : IRegistryOptions
    {
        public IRawGrammar? GetGrammar(string requested)
        {
            if (!string.Equals(requested, scopeName, StringComparison.Ordinal))
            {
                return inner.GetGrammar(requested);
            }

            using var stream = typeof(EmbeddedGrammarHighlighter).Assembly.GetManifestResourceStream(grammarResource)
                ?? throw new InvalidOperationException($"Embedded grammar '{grammarResource}' is missing.");
            using var reader = new StreamReader(stream);
            return GrammarReader.ReadGrammarSync(reader);
        }

        public ICollection<string> GetInjections(string requested) => inner.GetInjections(requested);

        public IRawTheme GetTheme(string requested) => inner.GetTheme(requested);

        public IRawTheme GetDefaultTheme() => inner.GetDefaultTheme();
    }
}
