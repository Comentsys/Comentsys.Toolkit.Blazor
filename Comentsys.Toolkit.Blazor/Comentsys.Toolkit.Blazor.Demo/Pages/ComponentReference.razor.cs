using Comentsys.Assets.Display;
using Comentsys.Assets.FluentEmoji;
using Comentsys.Toolkit.Blazor.Demo.Shared;
using Microsoft.AspNetCore.Components;
using System.Drawing;

namespace Comentsys.Toolkit.Blazor.Demo.Pages;

public partial class ComponentReference
{
    [Parameter]
    public string? Component { get; set; }

    private readonly Color Comentsys = Color.FromArgb(132, 0, 132);
    private readonly Color ComentsysLight = Color.FromArgb(163, 64, 163);
    private readonly Color ComentsysDark = Color.FromArgb(90, 0, 92);
    private readonly Color ComentsysMuted = Color.FromArgb(181, 181, 181);
    private readonly double[] DonutItems = [12, 24, 36, 28, 16, 20, 30, 18];

    private string _themePrimary = "#840084";
    private string _themeLight = "#a340a3";
    private string _themeDark = "#5a005c";
    private string _themeMuted = "#b5b5b5";
    private int _assetHeight = 128;
    private FluentEmojiType _assetEmoji = FluentEmojiType.GrinningFace;
    private int _clockSize = 220;
    private bool _clockRealtime = true;
    private bool _clockHours = true;
    private bool _clockMinutes = true;
    private bool _clockSeconds = true;
    private string _segmentValue = "12:34:56";
    private string _segmentMode = "Value";
    private int _segmentHeight = 96;
    private int _segmentWidth = 320;
    private string _matrixValue = "43:21";
    private string _matrixMode = "Value";
    private string _matrixStyle = "Square";
    private int _matrixHeight = 96;
    private int _matrixWidth = 320;
    private double _dialValue = 135;
    private int _dialSize = 220;
    private int _dialKnob = 16;
    private int _padSize = 220;
    private bool _padRepeat;
    private int _stickSize = 190;
    private double _stickSensitivity = 1.0;
    private int _donutSize = 220;
    private double _donutHole = 58;
    private double _donutStroke = 4;
    private int _donutSectors = 4;
    private int _gaugeSize = 220;
    private double _gaugeValue = 72;
    private int _gaugeNeedle = 6;
    private int _sectorSize = 220;
    private double _sectorStart = 20;
    private double _sectorFinish = 300;
    private double _sectorHole = 74;
    private double _sectorStroke = 4;
    private string _dialEvent = "ValueChanged: waiting for input";
    private string _directionEvent = "DirectionChanged: waiting for input";
    private string _stickEvent = "ValueChanged: waiting for input";
    private DirectionalPadDirection? _lastDirection;
    private int _directionCount;

    private string Slug => Component?.ToLowerInvariant() ?? string.Empty;
    private bool HasComponentReference => Slug is "asset" or "clock" or "segment-display" or "matrix-display" or "dial" or "directional-pad" or "directional-stick" or "donut" or "gauge" or "sector";
    private string FullPageTitle => $"{Title}.razor";
    private string FullPageSnippet => Slug switch
    {
        "asset" => AssetPageSnippet,
        "clock" => ClockPageSnippet,
        "segment-display" => SegmentPageSnippet,
        "matrix-display" => MatrixPageSnippet,
        "dial" => DialPageSnippet,
        "directional-pad" => PadPageSnippet,
        "directional-stick" => StickPageSnippet,
        "donut" => DonutPageSnippet,
        "gauge" => GaugePageSnippet,
        "sector" => SectorPageSnippet,
        _ => string.Empty
    };
    private FluentEmojiType SelectedEmoji => _assetEmoji;
    private static readonly FluentEmojiType[] EmojiTypes = Enum.GetValues<FluentEmojiType>();
    private DisplayMode SelectedSegmentMode => GetDisplayMode(_segmentMode);
    private DisplayMode SelectedMatrixMode => GetDisplayMode(_matrixMode);
    private bool IsSegmentRealtime => SelectedSegmentMode != DisplayMode.Value;
    private bool IsMatrixRealtime => SelectedMatrixMode != DisplayMode.Value;
    private string SegmentDisplayKey => _segmentMode;
    private string MatrixDisplayKey => $"{_matrixMode}:{_matrixStyle}";
    private bool HasThemeColours => UsesThemePrimary || UsesThemeLight || UsesThemeDark || UsesThemeMuted;
    private bool UsesThemePrimary => Slug is "clock" or "segment-display" or "dial" or "directional-pad" or "donut" or "sector";
    private bool UsesThemeLight => Slug is "clock" or "matrix-display" or "directional-pad" or "directional-stick" or "donut" or "gauge";
    private bool UsesThemeDark => Slug is "clock" or "directional-stick" or "donut" or "gauge" or "sector";
    private bool UsesThemeMuted => Slug is "clock" or "dial" or "donut";
    private Style SelectedMatrixStyle => _matrixStyle switch
    {
        "Circle" => Style.Circle,
        "Hexagon" => Style.Hexagon,
        "Octagon" => Style.Octagon,
        _ => Style.Square
    };
    private double[] SelectedDonutItems => [.. DonutItems.Take(_donutSectors)];
    private Color ThemePrimary => ColorTranslator.FromHtml(_themePrimary);
    private Color ThemeLight => ColorTranslator.FromHtml(_themeLight);
    private Color ThemeDark => ColorTranslator.FromHtml(_themeDark);
    private Color ThemeMuted => ColorTranslator.FromHtml(_themeMuted);
    private Color[] ClockHands => [ThemeDark, ThemePrimary, ThemeLight];
    private Color[] DirectionalFills => [ThemePrimary, ThemeLight, ThemePrimary, ThemeLight];
    private Color[] ChartFills => [.. Enumerable.Range(0, _donutSectors).Select(index => ChartPalette[index % ChartPalette.Length])];
    private Color[] ChartPalette => [ThemePrimary, ThemeLight, ThemeDark, ThemeMuted];

    private string Title => Slug switch
    {
        "asset" => "Asset",
        "clock" => "Clock",
        "segment-display" => "SegmentDisplay",
        "matrix-display" => "MatrixDisplay",
        "dial" => "Dial",
        "directional-pad" => "DirectionalPad",
        "directional-stick" => "DirectionalStick",
        "donut" => "Donut",
        "gauge" => "Gauge",
        "sector" => "Sector",
        _ => "Component"
    };

    private string Group => Slug switch
    {
        "asset" => "Assets",
        "clock" or "segment-display" or "matrix-display" => "Display",
        "dial" or "directional-pad" or "directional-stick" => "Input",
        "donut" or "gauge" => "Charts",
        "sector" => "Shapes",
        _ => "Reference"
    };

    private string Description => Slug switch
    {
        "asset" => "Render AssetResource values as an image data URI or inline SVG.",
        "clock" => "Display an analogue clock with configurable colours and time source.",
        "segment-display" => "Show time, date, or values with seven-segment glyphs.",
        "matrix-display" => "Show time, date, or values with dot-matrix glyphs.",
        "dial" => "Use pointer input to select a value between minimum and maximum.",
        "directional-pad" => "Raise directional events for up, right, down, and left.",
        "directional-stick" => "Raise angle and ratio values from joystick-style pointer input.",
        "donut" => "Render value proportions as a donut or pie chart.",
        "gauge" => "Indicate a value within a bounded range.",
        "sector" => "Render a circular segment with optional inner hole.",
        _ => "Developer reference for Comentsys Toolkit Blazor components."
    };

    private void DialChanged(double value)
    {
        _dialValue = value;
        _dialEvent = $"ValueChanged: {value:0}";
    }

    private void DirectionChanged(DirectionalPadDirection direction)
    {
        _directionCount = _lastDirection == direction ? _directionCount + 1 : 1;
        _lastDirection = direction;
        _directionEvent = _padRepeat
            ? $"DirectionChanged: {direction} fired {_directionCount} time{(_directionCount == 1 ? string.Empty : "s")} for this direction"
            : $"DirectionChanged: {direction}";
    }

    private void StickChanged(DirectionalStickValue value) =>
        _stickEvent = $"ValueChanged: angle {value.Angle:0}, ratio {value.Ratio:0.00}";

    private static DisplayMode GetDisplayMode(string mode) => mode switch
    {
        "Time" => DisplayMode.Time,
        "Date" => DisplayMode.Date,
        "TimeDate" => DisplayMode.TimeDate,
        _ => DisplayMode.Value
    };

    private static readonly ParameterRow[] AssetRows =
    [
        new("Title", "string?", "Accessible SVG title text."),
    new("AssetResource", "AssetResource?", "Resource to render."),
    new("Mode", "AssetMode?", "Image data URI or inline SVG."),
    new("UseAssetResourceHeight", "bool?", "Uses the resource height when available."),
    new("UseAssetResourceWidth", "bool?", "Uses the resource width when available."),
    new("Height", "int?", "Optional rendered height."),
    new("Width", "int?", "Optional rendered width.")
    ];

    private static readonly ParameterRow[] ClockRows =
    [
        new("Title", "string?", "Accessible SVG title text."),
    new("Background", "Color?", "Clock face background colour."),
    new("Foreground", "Color?", "Tick and marker colour."),
    new("Fill", "Color?", "Clock face fill colour."),
    new("HandsFill", "Color[]?", "Hour, minute, and second hand colours."),
    new("Stroke", "Color?", "Clock outline stroke colour."),
    new("Size", "int?", "Rendered SVG size."),
    new("IsRealTimeForWebAssembly", "bool", "Refreshes the clock in WebAssembly."),
    new("ShowSecondHand", "bool", "Shows or hides the second hand."),
    new("ShowMinuteHand", "bool", "Shows or hides the minute hand."),
    new("ShowHourHand", "bool", "Shows or hides the hour hand."),
    new("TimeSource", "Func<DateTime>", "Provides a fixed or custom time source.")
    ];

    private static readonly ParameterRow[] DisplayRows =
    [
        new("Title", "string?", "Accessible SVG title text."),
    new("Fill", "Color?", "Default display segment colour."),
    new("DisplayFill", "Color[]?", "Optional colour per character."),
    new("Mode", "DisplayMode?", "Time, Date, TimeDate, or Value."),
    new("Value", "string?", "Value rendered when Mode is Value."),
    new("TimeFormat", "string?", "Format used when Mode is Time."),
    new("DateFormat", "string?", "Format used when Mode is Date."),
    new("TimeDateFormat", "string?", "Format used when Mode is TimeDate."),
    new("DateTimeSource", "Func<DateTime>", "Provides a fixed or custom date/time source."),
    new("IsRealTimeForWebAssembly", "bool", "Refreshes live time values in WebAssembly."),
    new("Height", "int?", "Optional display height."),
    new("Width", "int?", "Optional display width.")
    ];

    private static readonly ParameterRow[] MatrixRows =
    [
        new("Title", "string?", "Accessible SVG title text."),
    new("Fill", "Color?", "Default display dot colour."),
    new("DisplayFill", "Color[]?", "Optional colour per character."),
    new("Style", "Style?", "Dot matrix glyph style."),
    new("Mode", "DisplayMode?", "Time, Date, TimeDate, or Value."),
    new("Value", "string?", "Value rendered when Mode is Value."),
    new("TimeFormat", "string?", "Format used when Mode is Time."),
    new("DateFormat", "string?", "Format used when Mode is Date."),
    new("TimeDateFormat", "string?", "Format used when Mode is TimeDate."),
    new("DateTimeSource", "Func<DateTime>", "Provides a fixed or custom date/time source."),
    new("IsRealTimeForWebAssembly", "bool", "Refreshes live time values in WebAssembly."),
    new("Height", "int?", "Optional display height."),
    new("Width", "int?", "Optional display width.")
    ];

    private static readonly ParameterRow[] DialRows =
    [
        new("Title", "string?", "Accessible SVG title text."),
    new("Foreground", "Color?", "Knob colour."),
    new("Fill", "Color?", "Dial face colour."),
    new("Size", "int?", "Rendered SVG size."),
    new("Knob", "int?", "Knob width."),
    new("Minimum", "double", "Minimum selected value."),
    new("Maximum", "double", "Maximum selected value."),
    new("Value", "double", "Current value."),
    new("ValueChanged", "EventCallback<double>", "Raised when the value changes.")
    ];

    private static readonly ParameterRow[] PadRows =
    [
        new("Title", "string?", "Accessible SVG title text."),
    new("Fill", "Color?", "Default direction fill colour."),
    new("DirectionsFill", "Color[]?", "Up, right, down, and left colours."),
    new("Size", "int?", "Rendered SVG size."),
    new("RepeatOnHold", "bool", "Raises repeated direction events while held."),
    new("DirectionChanged", "EventCallback<DirectionalPadDirection>", "Reports the selected direction.")
    ];

    private static readonly ParameterRow[] StickRows =
    [
        new("Title", "string?", "Accessible SVG title text."),
    new("Foreground", "Color?", "Knob colour."),
    new("Fill", "Color?", "Base colour."),
    new("Size", "int?", "Rendered SVG size."),
    new("Sensitivity", "double", "Pointer sensitivity setting."),
    new("ValueChanged", "EventCallback<DirectionalStickValue>", "Reports angle and ratio.")
    ];

    private static readonly ParameterRow[] DonutRows =
    [
        new("Title", "string?", "Accessible SVG title text."),
    new("SectorsFill", "Color[]?", "Sector colours in clockwise order."),
    new("Stroke", "Color?", "Sector stroke colour."),
    new("Size", "int?", "Rendered SVG size."),
    new("StrokeWidth", "double", "Sector stroke width."),
    new("Hole", "double", "Inner radius for donut charts."),
    new("Items", "double[]?", "Values converted into chart proportions.")
    ];

    private static readonly ParameterRow[] GaugeRows =
    [
        new("Title", "string?", "Accessible SVG title text."),
    new("Foreground", "Color?", "Needle and tick colour."),
    new("Fill", "Color?", "Gauge face colour."),
    new("Size", "int?", "Rendered SVG size."),
    new("Needle", "int?", "Needle width."),
    new("Minimum", "double", "Minimum gauge value."),
    new("Maximum", "double", "Maximum gauge value."),
    new("Value", "double", "Current gauge value.")
    ];

    private static readonly ParameterRow[] SectorRows =
    [
        new("Title", "string?", "Accessible SVG title text."),
    new("Fill", "Color?", "Sector fill colour."),
    new("Stroke", "Color?", "Sector stroke colour."),
    new("StrokeWidth", "double", "Outer stroke width."),
    new("Start", "double", "Start angle of the rendered arc."),
    new("Finish", "double", "Finish angle of the rendered arc."),
    new("Hole", "double", "Inner radius for ring sectors."),
    new("Size", "int?", "Rendered SVG size.")
    ];

    private const string AssetSnippet = @"<Asset Height=""128""
       Mode=""AssetMode.Inline""
       AssetResource=""ShadedFluentEmoji.Get(FluentEmojiType.GrinningFace)"" />";

    private const string ClockSnippet = @"<Clock Size=""220""
       HandsFill=""@ClockHands""
       IsRealTimeForWebAssembly=""true"" />";

    private const string SegmentSnippet = @"<SegmentDisplay Mode=""DisplayMode.Value""
                Value=""12:34:56""
                Height=""96""
                Fill=""@Comentsys"" />";

    private const string MatrixSnippet = @"<MatrixDisplay Mode=""DisplayMode.Value""
               Value=""43:21""
               Style=""Style.Square""
               Height=""96""
               Fill=""@ComentsysLight"" />";

    private const string DialSnippet = @"<Dial Value=""@_dialValue""
      ValueChanged=""DialChanged""
      Fill=""@Comentsys""
      Foreground=""@ComentsysMuted"" />";

    private const string PadSnippet = @"<DirectionalPad DirectionsFill=""@DirectionalFills""
                DirectionChanged=""DirectionChanged"" />";

    private const string StickSnippet = @"<DirectionalStick Fill=""@ComentsysLight""
                  Foreground=""@ComentsysDark""
                  ValueChanged=""StickChanged"" />";

    private const string DonutSnippet = @"<Donut Hole=""58""
       StrokeWidth=""4""
       Items=""@DonutItems""
       SectorsFill=""@ChartFills"" />";

    private const string GaugeSnippet = @"<Gauge Minimum=""0""
       Maximum=""100""
       Value=""72""
       Fill=""@ComentsysLight""
       Foreground=""@ComentsysDark"" />";

    private const string SectorSnippet = @"<Sector Start=""20""
        Finish=""300""
        Hole=""74""
        Fill=""@Comentsys""
        Stroke=""@ComentsysDark"" />";

    private const string AssetPageSnippet = @"@page ""/asset-demo""
@using Comentsys.Assets.FluentEmoji
@using Comentsys.Assets.FluentEmoji.Shaded
@using Comentsys.Toolkit.Blazor

<h1>Asset demo</h1>

<Asset Height=""128""
       Mode=""AssetMode.Inline""
       AssetResource=""ShadedFluentEmoji.Get(FluentEmojiType.GrinningFace)"" />";

    private const string ClockPageSnippet = @"@page ""/clock-demo""
@using System.Drawing
@using Comentsys.Toolkit.Blazor

<h1>Clock demo</h1>

<Clock Size=""220""
       Background=""Color.White""
       Foreground=""Color.Gray""
       HandsFill=""@Hands""
       IsRealTimeForWebAssembly=""true"" />

@code {
    private Color[] Hands => [Color.DarkMagenta, Color.Purple, Color.MediumPurple];
}";

    private const string SegmentPageSnippet = @"@page ""/segment-display-demo""
@using System.Drawing
@using Comentsys.Assets.Display
@using Comentsys.Toolkit.Blazor

<h1>SegmentDisplay demo</h1>

<SegmentDisplay Mode=""DisplayMode.Value""
                Value=""12:34:56""
                Height=""96""
                Width=""320""
                Fill=""Color.Purple"" />";

    private const string MatrixPageSnippet = @"@page ""/matrix-display-demo""
@using System.Drawing
@using Comentsys.Assets.Display
@using Comentsys.Toolkit.Blazor

<h1>MatrixDisplay demo</h1>

<MatrixDisplay Mode=""DisplayMode.Value""
               Value=""43:21""
               Style=""Style.Square""
               Height=""96""
               Width=""320""
               Fill=""Color.MediumPurple"" />";

    private const string DialPageSnippet = @"@page ""/dial-demo""
@using System.Drawing
@using Comentsys.Toolkit.Blazor

<h1>Dial demo</h1>

<Dial Value=""@_value""
      Fill=""Color.Purple""
      Foreground=""Color.MediumPurple""
      ValueChanged=""OnValueChanged"" />

<p>Value: @_value:0</p>

@code {
    private double _value = 135;

    private void OnValueChanged(double value) => _value = value;
}";

    private const string PadPageSnippet = @"@page ""/directional-pad-demo""
@using System.Drawing
@using Comentsys.Toolkit.Blazor

<h1>DirectionalPad demo</h1>

<DirectionalPad DirectionsFill=""@DirectionColours""
                RepeatOnHold=""true""
                DirectionChanged=""OnDirectionChanged"" />

<p>Direction: @_direction</p>

@code {
    private string _direction = ""Waiting"";
    private Color[] DirectionColours => [Color.Purple, Color.MediumPurple, Color.Purple, Color.MediumPurple];

    private void OnDirectionChanged(DirectionalPadDirection direction) =>
        _direction = direction.ToString();
}";

    private const string StickPageSnippet = @"@page ""/directional-stick-demo""
@using System.Drawing
@using Comentsys.Toolkit.Blazor

<h1>DirectionalStick demo</h1>

<DirectionalStick Fill=""Color.MediumPurple""
                  Foreground=""Color.DarkMagenta""
                  Sensitivity=""1.0""
                  ValueChanged=""OnValueChanged"" />

<p>Angle: @_angle:0, ratio: @_ratio:0.00</p>

@code {
    private double _angle;
    private double _ratio;

    private void OnValueChanged(DirectionalStickValue value)
    {
        _angle = value.Angle;
        _ratio = value.Ratio;
    }
}";

    private const string DonutPageSnippet = @"@page ""/donut-demo""
@using System.Drawing
@using Comentsys.Toolkit.Blazor

<h1>Donut demo</h1>

<Donut Size=""220""
       Hole=""58""
       StrokeWidth=""4""
       Stroke=""Color.White""
       Items=""@Items""
       SectorsFill=""@Colours"" />

@code {
    private double[] Items => [12, 24, 36, 28];
    private Color[] Colours => [Color.Purple, Color.MediumPurple, Color.DarkMagenta, Color.Gray];
}";

    private const string GaugePageSnippet = @"@page ""/gauge-demo""
@using System.Drawing
@using Comentsys.Toolkit.Blazor

<h1>Gauge demo</h1>

<Gauge Size=""220""
       Minimum=""0""
       Maximum=""100""
       Value=""72""
       Fill=""Color.MediumPurple""
       Foreground=""Color.DarkMagenta""
       Needle=""6"" />";

    private const string SectorPageSnippet = @"@page ""/sector-demo""
@using System.Drawing
@using Comentsys.Toolkit.Blazor

<h1>Sector demo</h1>

<Sector Size=""220""
        Start=""20""
        Finish=""300""
        Hole=""74""
        StrokeWidth=""4""
        Fill=""Color.Purple""
        Stroke=""Color.DarkMagenta"" />";
}