namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Gauge Component for Blazor WebAssembly and Blazor Server
/// </summary>
public partial class Gauge
{
    private const string black = "#000000";
    private const string grey = "#808080";
    private const int default_size = 200;
    private const int default_needle = 5;
    private const double sweep_angle = 300;
    private const int min_value = 0;
    private const int max_value = 100;

    private readonly List<(double x, double y, double width, double height, double angle)> _markers = [];
    private string? _foreground = black;
    private string? _fill = grey;

    private double Radius => (Size ?? default_size) / 2.0;
    private double Middle => (Size ?? default_size) / 20.0;
    private double NeedleHeight => Radius - 30;
    private double NeedleX => Radius - ((Needle ?? default_needle) / 2.0);
    private double NeedleY => Radius - NeedleHeight;
    private double NeedleAngle => (-sweep_angle / 2) + (Value - Minimum) / (Maximum - Minimum) * sweep_angle;

    /// <summary>
    /// Generate Markers
    /// </summary>
    private void GenerateMarkers()
    {
        _markers.Clear();
        var startAngle = -sweep_angle / 2;
        var endAngle = sweep_angle / 2;
        var angleRange = endAngle - startAngle;
        var markerCount = Maximum - Minimum;
        for (int i = 0; i <= markerCount; i++)
        {
            var isMajor = i % 5 == 0;
            var width = isMajor ? 4 : 2;
            var height = isMajor ? 16 : 8;
            var x = Radius - (width / 2.0);
            var y = Radius - Radius + (isMajor ? 6 : 10);
            var angle = startAngle + (i * angleRange / markerCount);
            _markers.Add((x, y, width, height, angle));
        }
    }

    /// <summary>
    /// On Initialized
    /// </summary>
    protected override void OnInitialized()
    {
        Minimum = Math.Max(min_value, Minimum);
        Maximum = Math.Min(max_value, Maximum);
        Value = Math.Clamp(Value, (int)Minimum, (int)Maximum);
        GenerateMarkers();
    }

    /// <summary>
    /// On Parameters Set
    /// </summary>
    protected override void OnParametersSet()
    {
        _fill = Fill.AsHtmlColor() ?? grey;
        _foreground = Foreground.AsHtmlColor() ?? black;
    }

    /// <summary>
    /// Title Text of Gauge
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Foreground Colour of Gauge
    /// </summary>
    [Parameter]
    public Color? Foreground { get; set; }

    /// <summary>
    /// Fill Colour of Gauge
    /// </summary>
    [Parameter]
    public Color? Fill { get; set; }

    /// <summary>
    /// Size of Gauge
    /// </summary>
    [Parameter]
    public int? Size { get; set; } = default_size;

    /// <summary>
    /// Width of Needle of Gauge
    /// </summary>
    [Parameter]
    public int? Needle { get; set; } = default_needle;

    /// <summary>
    /// Minimum Value of Gauge (Greater Than or Equal to 0)
    /// </summary>
    [Parameter]
    public double Minimum { get; set; } = min_value;

    /// <summary>
    /// Maximum Value of Gauge (Less Than or Equal to 100)
    /// </summary>
    [Parameter]
    public double Maximum { get; set; } = max_value;

    /// <summary>
    /// Value of Gauge
    /// </summary>
    [Parameter]
    public double Value { get; set; }
}
