namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Clock Component for Blazor WebAssembly or Blazor Server with IsRealTimeForWebAssembly as False
/// </summary>
public partial class Clock
{
    private const string black = "#000000";
    private const string white = "#ffffff";
    private const int default_size = 200;
    private const double clock_diameter = 200;
    private const double stroke_width = 10;
    private const int clock_interval = 1000;
    private const int seconds_width = 2;
    private const int minutes_width = 4;
    private const int hours_width = 8;
    private const double radius = (clock_diameter / 2) - (stroke_width / 2);
    private const double diameter = clock_diameter + stroke_width;
    private const double adjusted = diameter / 2;
    private const double seconds_height = radius - 10;
    private const double minutes_height = radius - 20;
    private const double hours_height = radius - 30;
    private const int hands = 3;

    private readonly List<(double x, double y, double width, double height, double angle)> _markers = [];
    private readonly Timer _timer = new();
    private string? _background = white;
    private string? _foreground = white;
    private string? _stroke = black;
    private string? _hour = black;
    private string? _minute = black;
    private string? _second = black;
    
    private double SecondAngle => TimeSource().Second * 6;
    private double MinuteAngle => TimeSource().Minute * 6 + TimeSource().Second / 10.0;
    private double HourAngle => TimeSource().Hour * 30 + TimeSource().Minute / 2.0;

    /// <summary>
    /// Generate Markers
    /// </summary>
    private void GenerateMarkers()
    {
        _markers.Clear();
        for (int i = 0; i < 60; i++)
        {
            var isMajor = i % 5 == 0;
            _markers.Add((
                x: adjusted - (isMajor ? 1.5 : 0.5),
                y: adjusted - radius + stroke_width / 2 - (isMajor ? 9 : 5),
                width: isMajor ? 3 : 1,
                height: isMajor ? 8 : 4,
                angle: i * 6
            ));
        }
    }

    /// <summary>
    /// On Initialised
    /// </summary>
    protected override void OnInitialized()
    {
        GenerateMarkers();
        if (IsRealTimeForWebAssembly)
        {
            _timer.Interval = clock_interval;
            _timer.Elapsed += (s, e) =>
            {
                if(!IsRealTimeForWebAssembly)                
                    _timer.Stop();                
                InvokeAsync(StateHasChanged);
            };
            _timer?.Start();
        }
        else
            _timer?.Dispose();
    }

    /// <summary>
    /// On Parameters Set
    /// </summary>
    protected override void OnParametersSet()
    {
        if (Fill.HasValue)
        {
            var fill = Fill.AsHtmlColor();
            _hour = fill ?? black;
            _minute = fill ?? black;
            _second = fill ?? black;
        }
        else if (HandsFill is not null)
        {
            var fills = Helper.Pad(HandsFill, Color.Black, hands);
            _hour = fills[0].AsHtmlColor() ?? black;
            _minute = fills[1].AsHtmlColor() ?? black;
            _second = fills[2].AsHtmlColor() ?? black;
        }
        _stroke = Stroke.AsHtmlColor() ?? black;
        _foreground = Foreground.AsHtmlColor() ?? white;
        _background = Background.AsHtmlColor() ?? white;
        if (IsRealTimeForWebAssembly)
            _timer?.Start();
        else
            _timer?.Stop();
    }

    /// <summary>
    /// Title Text of Clock
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Background Colour of Clock
    /// </summary>
    [Parameter]
    public Color? Background { get; set; }

    /// <summary>
    /// Foreground Colour of Clock Markers
    /// </summary>
    [Parameter]
    public Color? Foreground { get; set; }

    /// <summary>
    /// Fill Colour of Clock
    /// </summary>
    [Parameter]
    public Color? Fill { get; set; }

    /// <summary>
    /// Fill Colours of Clock Hands in order: Hour, Minute, Second
    /// </summary>
    [Parameter]
    public Color[]? HandsFill { get; set; }

    /// <summary>
    /// Stroke Colour of Clock
    /// </summary>
    public Color? Stroke { get; set; }

    /// <summary>
    /// Size of Clock
    /// </summary>
    [Parameter]
    public int? Size { get; set; } = default_size;

    /// <summary>
    /// Is Real Time for Blazor WebAssembly?
    /// </summary>
    [Parameter]
    public bool IsRealTimeForWebAssembly { get; set; } = true;

    /// <summary>
    /// Show Second Hand on Clock?
    /// </summary>
    [Parameter] 
    public bool ShowSecondHand { get; set; } = true;

    /// <summary>
    /// Show Minute Hand on Clock?
    /// </summary>
    [Parameter] 
    public bool ShowMinuteHand { get; set; } = true;

    /// <summary>
    /// Show Hour Hand on Clock?
    /// </summary>
    [Parameter] 
    public bool ShowHourHand { get; set; } = true;

    /// <summary>
    /// Time Source of Clock
    /// </summary>
    [Parameter]
    public Func<DateTime> TimeSource { get; set; } = () => DateTime.Now;
}

