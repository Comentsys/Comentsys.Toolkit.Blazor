namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Dial Component for Blazor WebAssembly
/// </summary>
public partial class Dial
{
    private const string black = "#000000";
    private const string grey = "#808080";
    private const int default_size = 200;
    private const int default_knob = 15;

    private string? _foreground = black;
    private string? _fill = grey;
    private bool _capture;

    private double Radius => (Size ?? default_size) / 2.0;
    private double KnobWidth => Knob ?? default_knob;
    private double Curve => KnobWidth / 2.0;
    private double Rotate => Value;
    private double Cx => Radius;
    private double Cy => Radius;

    /// <summary>
    /// Update Dial Value based on Position
    /// </summary>
    /// <param name="x">X Position</param>
    /// <param name="y">X Position</param>
    private void UpdateValue(double x, double y)
    {
        var angle = Math.Atan2(y - Cy, x - Cx) * (180 / Math.PI) + 90;
        if (angle < 0)
            angle += 360;
        var value = Math.Clamp(Math.Round(angle), Minimum, Maximum);
        if (Value != value)
        {
            Value = value;
            ValueChanged.InvokeAsync(Value);
            StateHasChanged();
        }
    }

    /// <summary>
    /// Start Capture
    /// </summary>
    /// <param name="e">Pointer Event Args</param>
    private void StartCapture(PointerEventArgs e)
    {
        _capture = true;
        UpdateValue(e.OffsetX, e.OffsetY);
    }

    /// <summary>
    /// End Capture
    /// </summary>
    private void EndCapture() =>
        _capture = false;

    /// <summary>
    /// Move Knob
    /// </summary>
    /// <param name="e"></param>
    private void MoveKnob(PointerEventArgs e)
    {
        if (_capture)
            UpdateValue(e.OffsetX, e.OffsetY);
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
    /// Title Text of Dial
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Foreground Colour of Dial
    /// </summary>
    [Parameter]
    public Color? Foreground { get; set; }

    /// <summary>
    /// Fill Colour of Dial
    /// </summary>
    [Parameter]
    public Color? Fill { get; set; }

    /// <summary>
    /// Size of Dial
    /// </summary>
    [Parameter]
    public int? Size { get; set; } = default_size;

    /// <summary>
    /// Width of Knob of Dial
    /// </summary>
    [Parameter]
    public int? Knob { get; set; } = default_knob;

    /// <summary>
    /// Minimum Value of Dial
    /// </summary>
    [Parameter]
    public double Minimum { get; set; } = 0;

    /// <summary>
    /// Maximum Value of Dial
    /// </summary>
    [Parameter]
    public double Maximum { get; set; } = 360;

    /// <summary>
    /// Value of Dial
    /// </summary>
    [Parameter]
    public double Value { get; set; }

    /// <summary>
    /// Event Triggered when Dial Value changed
    /// </summary>
    [Parameter]
    public EventCallback<double> ValueChanged { get; set; }
}