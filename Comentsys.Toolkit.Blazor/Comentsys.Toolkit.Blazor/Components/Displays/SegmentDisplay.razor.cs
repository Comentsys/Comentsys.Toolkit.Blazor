namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Segment Display Component for Blazor WebAssembly or Blazor Server with IsRealTimeForWebAssembly as False
/// </summary>
public partial class SegmentDisplay
{
    private const string height = "height";
    private const string width = "width";
    private const string black = "#000000";
    private const string svg_end = "</svg>";
    private const int clock_interval = 1000;    
    private const decimal segment_gap = 8.0m;
    private const decimal segment_width = 86.5m;
    private const decimal segment_height = 155.25m;    
    private const string default_time_format = "HH:mm:ss";
    private const string default_date_format = "dd-MM-yyyy";
    private const string default_time_date_format = "HH:mm:ss dd-MM-yyyy";
    private const string group_tag = "<g transform=\"translate({0},{1})\">{2}</g>";
    private const string svg_start = "<svg viewBox=\"0 0 86.5 155\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\">";

    private readonly List<(Value value, string group, string fill)> _glyphs = [];
    private readonly Dictionary<string, object> _attributes = [];
    private readonly List<string> _groups = [];
    private readonly Timer _timer = new();

    private decimal _width;
    private string? _fill;    
    private string[]? _fills;
    private Color? _prevFill;
    private Color[]? _prevDisplayFill;
    private List<Value> _values = [];
    
    /// <summary>
    /// Extract Display Glyph from SVG
    /// </summary>
    /// <param name="svg">SVG</param>
    /// <returns>Glyph Content</returns>
    private static string? ExtractGlyph(string svg) => string.IsNullOrWhiteSpace(svg) ? null :
        svg.Replace(svg_start, string.Empty).Replace(svg_end, string.Empty).Trim();

    /// <summary>
    /// Cache Glyphs for Display
    /// </summary>
    /// <param name="fill">Glyph Colour</param>
    private void CacheGlyphs(Color fill)
    {
        var htmlFill = fill.AsHtmlColor() ?? black;
        foreach (Value value in Enum.GetValues<Value>())
        {
            if (_glyphs.Any(g => g.value == value && g.fill == htmlFill))
                continue;
            var svg = Segment.Get(value, fill)?.ToSvgString();
            if (string.IsNullOrWhiteSpace(svg))
                continue;
            var glyph = ExtractGlyph(svg);
            _glyphs.RemoveAll(g => g.value == value && g.fill == htmlFill);
            _glyphs.Add((value, glyph ?? string.Empty, htmlFill));
        }
    }

    /// <summary>
    /// Get Glyph SVG by Value and Fill
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="fill">Fill</param>
    /// <returns>Glyph</returns>
    private string GetGlyph(Value value, string fill)
    {
        var glyph = _glyphs.FirstOrDefault(g => g.value == value && g.fill == fill);
        return glyph.group ?? string.Empty;
    }

    /// <summary>
    /// Get Value
    /// </summary>
    /// <returns>Value</returns>
    private string GetValue() => Mode switch
    {
        DisplayMode.Time => DateTimeSource().ToString(TimeFormat ?? default_time_format),
        DisplayMode.Date => DateTimeSource().ToString(DateFormat ?? default_date_format),
        DisplayMode.TimeDate => DateTimeSource().ToString(TimeDateFormat ?? default_time_date_format),
        _ => Value
    } ?? string.Empty;

    /// <summary>
    /// Update Display
    /// </summary>
    private void Update()
    {
        var value = GetValue();
        var values = Helper.GetDisplayValues(value);
        if (Fill is null && DisplayFill is not null)
        {
            if(_prevDisplayFill is null || _prevDisplayFill != DisplayFill)
            {
                var colours = Helper.Pad(DisplayFill, Color.Black, values.Count);
                var fills = colours.Select(c => c.AsHtmlColor());
                for (int i = 0; i < colours.Length; i++)
                    CacheGlyphs(colours[i]);
                _fills = [.. fills];
                _prevDisplayFill = DisplayFill;
            }            
        }
        else
        {
            if(_prevFill is null ||_prevFill != Fill)
            {
                var colour = Fill ?? Color.Black;
                var fill = Fill.AsHtmlColor();
                CacheGlyphs(colour);
                _fill = fill;
                _prevFill = Fill;
            }
        }
        if (!values.SequenceEqual(_values))
        {
            _groups.Clear();
            decimal x = 0;
            foreach (var (v, i) in values.Select((v, i) => (v, i)))
            {
                var fill = _fill is not null ? _fill : 
                    _fills is not null && i < _fills.Length ? _fills[i] : black;
                var glyph = GetGlyph(v, fill);
                x = i * (segment_width + segment_gap);
                var group = string.Format(group_tag, x, 0, glyph);
                _groups.Add(group);
            }
            _width = x + segment_width;
            _values = values;
        }
    }

    /// <summary>
    /// On Initialised 
    /// </summary>
    protected override void OnInitialized()
    {
        if (IsRealTimeForWebAssembly && Mode != DisplayMode.Value)
        {
            _timer.Interval = clock_interval;
            _timer.Elapsed += (s, e) =>
            {
                if (!(IsRealTimeForWebAssembly && Mode != DisplayMode.Value))
                    _timer.Stop();
                Update();
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
        Update();
        if (Height.HasValue)
            _attributes[height] = Height.Value;
        if (Width.HasValue)
            _attributes[width] = Width.Value;
        if (IsRealTimeForWebAssembly && Mode != DisplayMode.Value)
            _timer?.Start();
        else
            _timer?.Stop();
    }

    /// <summary>
    /// Title Text of Display
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Fill colour of the Display
    /// </summary>
    [Parameter]
    public Color? Fill { get; set; }

    /// <summary>
    /// Fill colours of the Display in order from left to right
    /// </summary>
    [Parameter]
    public Color[]? DisplayFill { get; set; }

    /// <summary>
    /// Display Mode of Time, Date, TimeDate or Value
    /// </summary>
    [Parameter]
    public DisplayMode? Mode { get; set; } = DisplayMode.Time;

    /// <summary>
    /// Value of the Display if using Display Mode of Value
    /// </summary>
    [Parameter]
    public string? Value { get; set; }

    /// <summary>
    /// Time Format of Display if using Display Mode of Time
    /// </summary>
    [Parameter]
    public string? TimeFormat { get; set; } = default_time_format;

    /// <summary>
    /// Date Format of Display if using Display Mode of Date
    /// </summary>
    [Parameter]
    public string? DateFormat { get; set; } = default_date_format;

    /// <summary>
    /// Time and Date Format of Display if using Display Mode of TimeDate
    /// </summary>
    [Parameter]
    public string? TimeDateFormat { get; set; } = default_time_date_format;

    /// <summary>
    /// Date Time Source of Display if using Display Mode of Time, Date or TimeDate
    /// </summary>
    [Parameter]
    public Func<DateTime> DateTimeSource { get; set; } = () => DateTime.Now;

    /// <summary>
    /// Is Real Time for Blazor WebAssembly if using Display Mode of Time, Date or TimeDate
    /// </summary>
    [Parameter]
    public bool IsRealTimeForWebAssembly { get; set; } = true;

    /// <summary>
    /// Display Height
    /// </summary>
    [Parameter]
    public int? Height { get; set; }

    /// <summary>
    /// Display Width
    /// </summary>
    [Parameter]
    public int? Width { get; set; }
}
