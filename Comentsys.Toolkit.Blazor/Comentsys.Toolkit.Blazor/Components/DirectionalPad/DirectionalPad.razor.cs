namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Directional Pad Component for Blazor WebAssembly
/// </summary>
public partial class DirectionalPad
{
    private const string black = "#000000";
    private const int default_size = 200;
    private const int repeat_delay = 200;
    private const int directions = 4;
    
    private bool _capture;
    private string _up = black;
    private string _down = black;
    private string _left = black;
    private string _right = black;
    private CancellationTokenSource? _cancel;

    /// <summary>
    /// Ends Capture
    /// </summary>
    private void EndCapture()
    {
        _capture = false;
        _cancel?.Cancel();
        _cancel = null;
    }

    /// <summary>
    /// Start Capture and Invoke Direction Changed Event
    /// </summary>
    /// <param name="direction">Direction Pad Direction</param>
    private async Task StartCapture(DirectionalPadDirection direction)
    {
        _capture = true;
        if (DirectionChanged.HasDelegate)
        {
            await DirectionChanged.InvokeAsync(direction);
            if(RepeatOnHold)
            {
                _cancel = new CancellationTokenSource();
                try
                {
                    while (_capture && !_cancel.IsCancellationRequested)
                    {
                        await Task.Delay(repeat_delay, _cancel.Token);
                        if (_capture)
                            await DirectionChanged.InvokeAsync(direction);
                    }
                }
                catch (TaskCanceledException)
                {
                    // No action needed as Task Cancelled
                }
            }
        }
    }

    /// <summary>
    /// On Parameters Set
    /// </summary>
    protected override void OnParametersSet()
    {
        if (Fill.HasValue)
        {
            var fill = Fill.AsHtmlColor();
            _up = fill ?? black;
            _down = fill ?? black;
            _left = fill ?? black;
            _right = fill ?? black;
        }
        else if (DirectionsFill is not null)
        {
            var fills = Helper.Pad(DirectionsFill, Color.Black, directions);
            _up = fills[0].AsHtmlColor() ?? black;
            _right = fills[1].AsHtmlColor() ?? black;
            _down = fills[2].AsHtmlColor() ?? black;
            _left = fills[3].AsHtmlColor() ?? black;
        }
        if (!Size.HasValue)
            Size = default_size;
        else if (Size < 0)
            Size = default_size;
    }

    /// <summary>
    /// Title Text of Directional Pad
    /// </summary>
    [Parameter]
    public string? Title { get; set; }

    /// <summary>
    /// Fill Colour of Directional Pad
    /// </summary>
    [Parameter]
    public Color? Fill { get; set; }

    /// <summary>
    /// Fill Colours of Directional Pad in clockwise order: Up, Right, Down, Left
    /// </summary>
    [Parameter]
    public Color[]? DirectionsFill { get; set; }

    /// <summary>
    /// Size of Directional Pad
    /// </summary>
    [Parameter]
    public int? Size { get; set; } = default_size;

    /// <summary>
    /// Indicates DirectionChanged should fire repeatedly when Directional Pad direction is held
    /// </summary>
    [Parameter]
    public bool RepeatOnHold { get; set; } = false;

    /// <summary>
    /// Event Triggered when Directional Pad Direction Changed
    /// </summary>
    [Parameter]
    public EventCallback<DirectionalPadDirection> DirectionChanged { get; set; }
}