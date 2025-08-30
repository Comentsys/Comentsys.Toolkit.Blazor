namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Directional Stick Component for Blazor WebAssembly
/// </summary>
public partial class DirectionalStick
{
    private const string black = "#000000";
    private const string grey = "#808080";
    private const int default_size = 200;
    
    private string? _foreground = black;
    private string? _fill = grey;
    private double _knobX;
    private double _knobY;
    private bool _capture;

    private double Radius => (Size == null ? default_size : Size.Value) / 2;
    private double Knob => Radius / 2;  
    private double Centre => Radius + Knob;
    private double Total => (Radius * 2) + (Knob * 2);

    /// <summary>
    /// Reset Knob Position to Centre
    /// </summary>
    private void Reset()
    {
        _knobX = Centre;
        _knobY = Centre;
        if(_capture)
            ValueChanged.InvokeAsync(new(0, 0));
        _capture = false;
    }

    /// <summary>
    /// Start Capture
    /// </summary>
    /// <param name="e">Pointer Event Args</param>
    private void StartCapture(PointerEventArgs e) => 
        _capture = true;

    /// <summary>
    /// End Capture
    /// </summary>
    /// <param name="e">Pointer Event Args</param>
    private void EndCapture(PointerEventArgs e) =>
        Reset();

    /// <summary>
    /// Move Knob based on pointer movement by calculating pointer position relative to SVG element
    /// </summary>
    /// <param name="e">Pointer Event Args</param>
    private void MoveKnob(PointerEventArgs e)
    {
        try
        {
            if (!_capture) 
                return;
            var x = e.OffsetX;
            var y = e.OffsetY;
            var dx = x - Centre;
            var dy = y - Centre;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            if (distance > Radius)
            {
                var angle = Math.Atan2(dy, dx);
                x = Centre + Math.Cos(angle) * Radius;
                y = Centre + Math.Sin(angle) * Radius;
            }
            _knobX = x;
            _knobY = y;
            var degrees = Math.Atan2(dy, dx) * (180 / Math.PI);
            var ratio = Math.Min(distance / Radius, 1.0);
            ValueChanged.InvokeAsync(new(degrees, ratio));
        }
        catch
        {
            Size = default_size;
            Reset();
        }
    }

    /// <summary>
    /// On Initialized
    /// </summary>
    protected override void OnInitialized() => 
        Reset();

    /// <summary>
    /// On Parameters Set
    /// </summary>
    protected override void OnParametersSet()
    {
        _fill = Fill.AsHtmlColor() ?? grey;
        _foreground = Foreground.AsHtmlColor() ?? black;
    }

    /// <summary>
    /// Title Text of Directional Stick
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Foreground Colour of Directional Stick Knob
    /// </summary>
    [Parameter]
    public Color? Foreground { get; set; }

    /// <summary>
    /// Fill Colour of Directional Stick
    /// </summary>
    [Parameter]
    public Color? Fill { get; set; }

    /// <summary>
    /// Size of Directional Stick
    /// </summary>
    [Parameter]
    public int? Size { get; set; } = default_size;

    /// <summary>
    /// Sensitivity of Directional Stick
    /// </summary>
    [Parameter]
    public double Sensitivity { get; set; } = 1.0;

    /// <summary>
    /// Event Triggered when Directional Stick Value Changed
    /// </summary>
    [Parameter]
    public EventCallback<DirectionalStickValue> ValueChanged { get; set; }
}