namespace Comentsys.Toolkit.Blazor;

/// <summary>
/// Display Mode
/// </summary>
public enum DisplayMode
{
    /// <summary>
    /// Display any Value containing 0 - 9, Dash, Colon and Space also used for unsupported characters
    /// </summary>
    Value,
    /// <summary>
    /// Display Time as Time Format of HH:mm:ss by Default
    /// </summary>
    Time,
    /// <summary>
    /// Display Date as Date Format of dd-MM-yyyy by Default
    /// </summary>
    Date,
    /// <summary>
    /// Display Time and Date as Time Date Format of HH:mm:ss dd-MM-yyyy by Default
    /// </summary>
    TimeDate
}
