namespace MewDocs.Styling;

using System.Collections.Immutable;
using MonorailCss.Theme;
using Pennington.MonorailCss;

/// <summary>
/// The site palette: warm stone grays with one orange, the orange of the ear in the logo.
/// Orange fills both the <c>primary</c> and <c>accent</c> slots so the chrome never shows a
/// second brand color. Three quieter palettes exist only to color code: green for strings,
/// blue for types and purple for numbers and other constants.
/// </summary>
internal sealed class MewColorScheme : IColorScheme
{
    // The logo's orange, #f68104, is about oklch(0.72 0.18 55).
    private const double OrangeHue = 55;
    private const double OrangeChroma = 0.17;

    // Code token hues. They share a chroma so no token shouts over the others.
    private const double TokenChroma = 0.11;
    private const double StringHue = 160;
    private const double TypeHue = 255;
    private const double ConstantHue = 310;

    // Stone's darkest step is close to black. Lift it a little so the dark theme reads as
    // warm charcoal rather than a void, and so raised surfaces (base-900) stay distinct.
    private static readonly ImmutableDictionary<string, string> LiftedDarkest =
        ImmutableDictionary<string, string>.Empty.Add("950", "oklch(0.18 0.004 60)");

    /// <inheritdoc />
    public Theme ApplyToTheme(Theme theme) =>
        theme
            .AddColorPalette("mew-orange", ColorPaletteGenerator.GenerateForeground(OrangeHue, OrangeChroma))
            .AddColorPalette("mew-string", ColorPaletteGenerator.GenerateForeground(StringHue, TokenChroma))
            .AddColorPalette("mew-type", ColorPaletteGenerator.GenerateForeground(TypeHue, TokenChroma))
            .AddColorPalette("mew-constant", ColorPaletteGenerator.GenerateForeground(ConstantHue, TokenChroma))
            .MapColorPalette("mew-orange", "primary")
            .MapColorPalette("mew-orange", "accent")
            .MapColorPalette("stone", "base")
            .AddColorPalette("base", LiftedDarkest);
}
