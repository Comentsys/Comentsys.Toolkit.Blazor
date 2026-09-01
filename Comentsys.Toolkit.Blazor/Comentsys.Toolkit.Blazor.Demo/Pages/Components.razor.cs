namespace Comentsys.Toolkit.Blazor.Demo.Pages;

public partial class Components
{
    private static readonly ComponentLink[] ComponentLinks =
    [
        new("Asset", "asset", "Assets", "Render packaged SVG resources inline or as image data URIs."),
    new("Clock", "clock", "Display", "Show analogue time with configurable face and hand colours."),
    new("SegmentDisplay", "segment-display", "Display", "Render time, date, or numeric values in seven-segment style."),
    new("MatrixDisplay", "matrix-display", "Display", "Render time, date, or numeric values in dot-matrix style."),
    new("Dial", "dial", "Input", "Capture a rotary numeric value with pointer input."),
    new("DirectionalPad", "directional-pad", "Input", "Capture up, right, down, and left actions."),
    new("DirectionalStick", "directional-stick", "Input", "Capture joystick angle and ratio values."),
    new("Donut", "donut", "Charts", "Visualise proportions as a donut or pie chart."),
    new("Gauge", "gauge", "Charts", "Show a value between minimum and maximum bounds."),
    new("Sector", "sector", "Shapes", "Render an arc or ring segment from start and finish angles.")
    ];

    private sealed record ComponentLink(string Name, string Slug, string Group, string Description);
}