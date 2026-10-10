namespace MewDocs.Highlighting;

/// <summary>Highlights <c>mew</c> fences with the grammar the editor extensions use.</summary>
public sealed class MewHighlighter() : EmbeddedGrammarHighlighter("source.mew", "MewDocs.Highlighting.mew.tmLanguage.json", "mew");
