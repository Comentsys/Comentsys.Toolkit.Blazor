namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Asset Component for Blazor WebAssembly and Blazor Server
/// </summary>
public partial class Asset
{
    private const string image = "<img src=\"{0}\"{1}/>";    
    private const string height = " height=\"{0}\"";
    private const string width = " width=\"{0}\"";
    private const string title_tag = "<title>{0}</title>";
    private const string title_attr = " title=\"{0}\"";
    private const char insert = '>';

    private string? _svg;

    /// <summary>
    /// Get Height and Width Elements
    /// </summary>
    /// <returns>Height and / or Width Elements</returns>
    private string GetHeightAndWidthElements()
    {
        var elements = string.Empty;
        var heightValue = Height != null ? Height : UseAssetResourceHeight == true ? AssetResource?.Height : null;
        var widthValue = Width != null ? Width : UseAssetResourceWidth == true ? AssetResource?.Width : null;            
        if (heightValue != null)
            elements = $"{elements}{string.Format(height, heightValue)}";
        if (widthValue != null)
            elements = $"{elements}{string.Format(width, widthValue)}";
        return elements;
    }

    /// <summary>
    /// Add Inline SVG Title Text
    /// </summary>
    /// <param name="svg">SVG</param>
    /// <returns>SVG</returns>
    private string AddInlineSvgTitle(string svg)
    {
        if (string.IsNullOrWhiteSpace(Title))
            return svg;
        var title = string.Format(title_tag, Title);
        var index = svg.IndexOf(insert) + 1;
        if(index > 0)
            svg = svg.Insert(index, title);
        return svg;
    }

    /// <summary>
    /// Add Inline SVG Height and Width
    /// </summary>
    /// <param name="svg">SVG</param>
    /// <returns>SVG</returns>
    private string AddInlineSvgHeightAndWidth(string svg)
    {
        var elements = GetHeightAndWidthElements();
        if(string.IsNullOrWhiteSpace(elements))
            return svg;
        var index = svg.IndexOf(insert);
        if (index > 0)
            svg = svg.Insert(index, elements);
        return svg;
    }

    /// <summary>
    /// Get Inline SVG
    /// </summary>
    /// <param name="asset">Asset Resource</param>
    /// <returns>Inline SVG</returns>
    private string? GetInlineSvg(AssetResource? asset)
    {
        var svg = asset?.ToSvgString();
        if(string.IsNullOrWhiteSpace(svg))
            return null;
        svg = AddInlineSvgTitle(svg);
        svg = AddInlineSvgHeightAndWidth(svg);
        return svg;
    }

    /// <summary>
    /// Get Image SVG
    /// </summary>
    /// <param name="asset">Asset Resource</param>
    /// <returns>Image SVG</returns>
    private string? GetImageSvg(AssetResource? asset)
    {
        var uri = asset?.AsDataUri();
        if (uri is null)
            return null;
        var elements = string.Empty;
        if (!string.IsNullOrWhiteSpace(Title))
            elements = string.Format(title_attr, Title);
        elements = $"{elements}{GetHeightAndWidthElements()}";
        return string.Format(image, uri, elements);
    }

    /// <summary>
    /// On Parameters Set
    /// </summary>
    protected override void OnParametersSet() => 
        _svg = Mode switch
        {
            AssetMode.Inline => GetInlineSvg(AssetResource),
            _ => GetImageSvg(AssetResource)            
        };

    /// <summary>
    /// Title Text of Asset
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Asset Mode of Asset Resource as Image Tag with Base 64 Encoded SVG or Inline SVG
    /// </summary>
    [Parameter]
    public AssetMode? Mode { get; set; } = AssetMode.Image;

    /// <summary>
    /// Asset Resource of Asset
    /// </summary>
    [Parameter]
    public AssetResource? AssetResource { get; set; }

    /// <summary>
    /// Use Asset Resource Height if Height not specified
    /// </summary>
    [Parameter]
    public bool? UseAssetResourceHeight { get; set; }

    /// <summary>
    /// Use Asset Resource Width if Width not specified
    /// </summary>
    [Parameter]
    public bool? UseAssetResourceWidth { get; set; }

    /// <summary>
    /// Asset Resource Height
    /// </summary>
    [Parameter]
    public int? Height { get; set; }

    /// <summary>
    /// Asset Resource Width
    /// </summary>
    [Parameter]
    public int? Width { get; set; }
}