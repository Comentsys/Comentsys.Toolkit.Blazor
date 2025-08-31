namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Extensions
/// </summary>
public static class Extensions
{
    /// <summary>
    /// As HTML Colour
    /// </summary>
    /// <param name="color">Drawing Color</param>
    /// <returns>HTML Colour String</returns>
    public static string AsHtmlColor(this Color color) =>
        $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    /// <summary>
    /// As HTML Colour
    /// </summary>
    /// <param name="color">Drawing Color</param>
    /// <returns>HTML Colour String</returns>
    public static string? AsHtmlColor(this Color? color) =>
        color?.AsHtmlColor();

    /// <summary>
    /// As Data Uri
    /// </summary>
    /// <param name="assetResource">Asset Resource</param>
    /// <returns>Data Uri</returns>
    public static Uri? AsDataUri(this AssetResource? assetResource)
    {
        try
        {
            if (assetResource != null)
            {
                var svg = assetResource?.ToBase64EncodedSvgString();
                return string.IsNullOrWhiteSpace(svg) ? null : new Uri($"data:image/svg+xml;base64,{svg}");
            }
            return null;
        }
        catch
        {
            return null; // Return null if any error occurs
        }
    }

    /// <summary>
    /// As Data Uri
    /// </summary>
    /// <param name="imageResource">Image Resource</param>
    /// <returns>Data Uri</returns>
    public static Uri? AsDataUri(this ImageResource? imageResource)
    {
        try
        {
            if (imageResource != null)
            {
                var png = imageResource?.ToBase64EncodedPngString();
                return string.IsNullOrWhiteSpace(png) ? null : new Uri($"data:image/png;base64,{png}");
            }
            return null;
        }
        catch
        {
            return null; // Return null if any error occurs
        }
    }
}
