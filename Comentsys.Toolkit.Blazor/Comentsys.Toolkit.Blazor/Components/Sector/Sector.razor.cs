namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Sector Component for Blazor WebAssembly and Blazor Server
/// </summary>
public partial class Sector
{
    private const string black = "#000000";
    private const int default_size = 200;

    private string? _fill = black;
    private string? _stroke = black;
    private string? _path;
    private double _width;
    private double _height;
    private double _minX;
    private double _minY;

    private double Radius => (Size == null ? default_size : Size.Value) / 2;
    private double Cy => Radius;
    private double Cx => Radius;
    
    /// <summary>
    /// On Parameters Set
    /// </summary>
    protected override void OnParametersSet()
    {
        _fill = Fill.AsHtmlColor() ?? black;
        _stroke = Stroke.AsHtmlColor() ?? black;
        _path = Helper.CalculateSectorPath(true, Cx, Cy, Radius, Start, Finish, Hole, StrokeWidth, 
            out _minX, out _minY, out _width, out _height);
    }

    /// <summary>
    /// Title Text of Sector
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Fill Colour of Sector
    /// </summary>
    [Parameter]
    public Color? Fill { get; set; }

    /// <summary>
    /// Stroke Colour of Sector
    /// </summary>
    [Parameter]
    public Color? Stroke { get; set; }

    /// <summary>
    /// Stroke Width of Sector
    /// </summary>
    [Parameter]
    public double StrokeWidth { get; set; }

    /// <summary>
    /// Start Angle of Sector
    /// </summary>
    [Parameter]
    public double Start { get; set; }

    /// <summary>
    /// Finish Angle of Sector
    /// </summary>
    [Parameter]
    public double Finish { get; set; }

    /// <summary>
    /// Size of Sector Hole
    /// </summary>
    [Parameter]
    public double Hole { get; set; }

    /// <summary>
    /// Size of Sector
    /// </summary>
    [Parameter]
    public int? Size { get; set; } = default_size;
}