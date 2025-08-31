namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Value of Directional Stick Component
/// </summary>
/// <param name="angle">Stick Angle around Centre</param>
/// <param name="ratio"> Stick Ratio from Centre</param>
public class DirectionalStickValue(double angle, double ratio)
{
    /// <summary>
    /// Stick Angle around Centre
    /// </summary>
    public double Angle { get; set; } = angle;

    /// <summary>
    /// Stick Ratio from Centre
    /// </summary>
    public double Ratio { get; set; } = ratio;
}
