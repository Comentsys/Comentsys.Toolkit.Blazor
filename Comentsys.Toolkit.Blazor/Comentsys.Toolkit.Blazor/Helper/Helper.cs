namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Helper Class
/// </summary>
internal static class Helper
{
    private static readonly int[] cardinal_angles = [ 0, 90, 180, 270 ];
    private static readonly char[] display_values = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', ':', ' '];

    /// <summary>
    /// Pad 
    /// </summary>
    /// <param name="source">Source Colours</param>
    /// <param name="pad">Pad Colour</param>
    /// <param name="total">Total Colours</param>
    /// <returns>Padded Colours</returns>
    internal static Color[] Pad(Color[]? source, Color pad, int total)
    {
        source ??= [];
        var result = new Color[total];
        if (source.Length < total)
        {
            for (int i = 0; i < source.Length; i++)
                result[i] = source[i];
            for (int i = source.Length; i < total; i++)
                result[i] = pad;
        }
        else
            Array.Copy(source, result, total);
        return result;
    }

    /// <summary>
    /// Is angle in sweep?
    /// </summary>
    /// <param name="angle">Angle</param>
    /// <param name="start">Start</param>
    /// <param name="sweep"></param>
    /// <returns></returns>
    private static bool InSweep(double angle, double start, double sweep) =>
        (angle - start + 360) % 360 <= sweep;

    /// <summary>
    /// Calculate Sector Path
    /// </summary>
    /// <param name="bounds">Calculate Bounds?</param>
    /// <param name="cx">Centre X</param>
    /// <param name="cy"> Centre Y</param>
    /// <param name="radius">Radius</param>
    /// <param name="start">Start</param>
    /// <param name="finish">Finish</param>
    /// <param name="hole">Hole</param>
    /// <param name="stroke">Stroke</param>
    /// <param name="minX">Minimum X</param>
    /// <param name="minY">Minimum Y</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    internal static string CalculateSectorPath(bool bounds, double cx, double cy,
        double radius, double start, double finish, double hole, double stroke,
        out double minX, out double minY, out double width, out double height)
    {
        try
        {
            // Clamp sweep
            if (finish <= 0)
                finish = 0;
            if (finish > 360)
                finish = 360;
            // Adjust radii for inner stroke
            double padding = stroke / 2.0;
            double adjRadius = Math.Max(0, radius - padding);
            double adjHole = Math.Max(0, hole - padding);
            // Convert angles to radians
            double startRad = Math.PI / 180 * (start - 90);
            double endRad = Math.PI / 180 * (start + finish - 90);
            // Points on outer arc
            double x1 = cx + adjRadius * Math.Cos(startRad);
            double y1 = cy + adjRadius * Math.Sin(startRad);
            double x2 = cx + adjRadius * Math.Cos(endRad);
            double y2 = cy + adjRadius * Math.Sin(endRad);
            // Points on inner arc
            double x3 = cx + adjHole * Math.Cos(endRad);
            double y3 = cy + adjHole * Math.Sin(endRad);
            double x4 = cx + adjHole * Math.Cos(startRad);
            double y4 = cy + adjHole * Math.Sin(startRad);
            // Determine Bounds
            if (bounds)
            {
                double[] xPoints = { x1, x2, x3, x4 };
                double[] yPoints = { y1, y2, y3, y4 };
                double maxX = xPoints.Max();
                double maxY = yPoints.Max();
                minX = xPoints.Min();
                minY = yPoints.Min();
                // Expand Bounds for cardinal angles if within sweep
                if (finish > 0 && finish < 360)
                {
                    foreach (var cardinal in cardinal_angles)
                    {
                        if (InSweep(cardinal, start, finish))
                        {
                            double rad = Math.PI / 180 * (cardinal - 90);
                            double cxOuter = cx + adjRadius * Math.Cos(rad);
                            double cyOuter = cy + adjRadius * Math.Sin(rad);
                            if (cxOuter < minX)
                                minX = cxOuter;
                            if (cxOuter > maxX)
                                maxX = cxOuter;
                            if (cyOuter < minY)
                                minY = cyOuter;
                            if (cyOuter > maxY)
                                maxY = cyOuter;
                        }
                    }
                }
                if (finish == 360)
                {
                    minX = cx - radius - padding;
                    minY = cy - radius - padding;
                    width = (radius + padding) * 2;
                    height = width;
                }
                else
                {
                    minX -= padding;
                    minY -= padding;
                    width = (maxX + padding) - minX;
                    height = (maxY + padding) - minY;
                }
            }
            else
                minX = minY = width = height = 0;
            // Determine if the arc is larger than 180 degrees
            int largeArc = finish > 180 ? 1 : 0;
            int sweep = 1; // Always Clockwise
            if (adjHole > 0)
            {
                // Donut sector
                return $"M{x4},{y4} L{x1},{y1} A{adjRadius},{adjRadius} 0 {largeArc},{sweep} {x2},{y2} L{x3},{y3} A{adjHole},{adjHole} 0 {largeArc},{0} {x4},{y4} Z";
            }
            else
            {
                // Pie sector
                return $"M{cx},{cy} L{x1},{y1} A{adjRadius},{adjRadius} 0 {largeArc},{sweep} {x2},{y2} Z";
            }
        }
        catch
        {
            minX = minY = width = height = 0;
            return string.Empty; // Fallback to empty path on error
        }
    }

    /// <summary>
    /// Get Display Value List from Value
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>List of Value</returns>
    internal static List<Value> GetDisplayValues(string value)
    {
        var mapping = display_values.Select((c, index) => new { Char = c, Value = (Value)index }).ToDictionary(x => x.Char, x => x.Value);
        return [.. value.Select(ch => mapping.TryGetValue(ch, out var enumValue) ? enumValue : Value.Blank)];
    }
}
