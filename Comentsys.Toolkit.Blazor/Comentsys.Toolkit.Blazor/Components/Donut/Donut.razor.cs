namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Donut Component for Blazor WebAssembly and Blazor Server
/// </summary>
public partial class Donut
{
    private const string black = "#000000";
    private const double total = 100;
    private const double circle = 360;
    private const int default_size = 200;

    private readonly List<(string path, string fill)> _sectors = [];
    private string? _stroke = black;

    private double Radius => (Size == null ? default_size : Size.Value) / 2;
    private double Cy => Radius;
    private double Cx => Radius;

    /// <summary>
    /// Get Percentages
    /// </summary>
    /// <returns>List of Percentage</returns>
    private List<double> GetPercentages() =>
        Items is null ? [] : [.. Items
        .Select(item => item / Items.Sum() * 100)
        .OrderBy(o => o)];

    /// <summary>
    /// On Parameters Set
    /// </summary>
    protected override void OnParametersSet()
    {
        _sectors.Clear();
        try
        {
            double finish = 0;
            var value = circle / total;
            var percentages = GetPercentages();
            var fills = Helper.Pad(SectorsFill, Color.Gray, percentages.Count);
            _stroke = Stroke.AsHtmlColor() ?? black;
            for (int i = 0; i < percentages.Count; i++)
            {
                var start = finish;
                var percent = percentages[i];
                var sweep = value * percent;
                finish = sweep + start;
                if (finish >= circle)
                    finish = sweep;
                var path = Helper.CalculateSectorPath(false, Cx, Cy, Radius, start, finish, Hole, StrokeWidth, 
                    out _, out _, out _, out _);
                var fill = fills[i].AsHtmlColor();
                _sectors.Add((path, fill));
            }
        }
        catch
        {
            // Empty Donut on Error
        }
    }

    /// <summary>
    /// Title Text of Donut
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Fill Colours of Donut Sectors in clockwise order
    /// </summary>
    [Parameter]
    public Color[]? SectorsFill { get; set; }

    /// <summary>
    /// Stroke Colour of Donut Sectors
    /// </summary>
    [Parameter]
    public Color? Stroke { get; set; }

    /// <summary>
    /// Size of Donut
    /// </summary>
    [Parameter]
    public int? Size { get; set; } = default_size;

    /// <summary>
    /// Stroke Width of Donut Sectors
    /// </summary>
    [Parameter]
    public double StrokeWidth { get; set; }

    /// <summary>
    /// Size of Donut Hole
    /// </summary>
    [Parameter]
    public double Hole { get; set; }

    /// <summary>
    /// Donut Items
    /// </summary>
    [Parameter]
    public double[]? Items { get; set; }
}
