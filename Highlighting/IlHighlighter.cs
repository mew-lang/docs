namespace MewDocs.Highlighting;

/// <summary>
/// Highlights <c>il</c>, <c>msil</c> and <c>cil</c> fences: the disassembly of what the compiler
/// emitted, and Mew's LIR, which is written in the same vocabulary.
/// </summary>
public sealed class IlHighlighter() : EmbeddedGrammarHighlighter("source.il", "MewDocs.Highlighting.il.tmLanguage.json", "il", "msil", "cil");
