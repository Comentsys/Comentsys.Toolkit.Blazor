using System.Drawing;

namespace Comentsys.Toolkit.Blazor.Demo.Pages;

public partial class Scenarios
{
    private readonly Color Comentsys = Color.FromArgb(132, 0, 132);
    private readonly Color ComentsysLight = Color.FromArgb(163, 64, 163);
    private readonly Color ComentsysDark = Color.FromArgb(90, 0, 92);
    private double _temperatureDialValue = 154;
    private double _targetTemperature = 22;

    private double ComfortLevel => Math.Max(0, 100 - Math.Abs(_targetTemperature - 21) * 12);
    private double PrimaryEnergyLoad => Math.Max(10, Math.Abs(_targetTemperature - 21) * 10);
    private double[] EnergyBalance => [PrimaryEnergyLoad, 100 - PrimaryEnergyLoad];
    private string ClimateMode => _targetTemperature switch
    {
        < 19 => "Heating is active to reach a warmer comfort target.",
        > 24 => "Cooling is active to reduce the target temperature.",
        _ => "Eco comfort mode is active."
    };

    private Color[] ClockHands => [ComentsysDark, Comentsys, ComentsysLight];
    private Color[] EnergyFills => [Comentsys, ComentsysLight];

    private void TemperatureChanged(double value)
    {
        _temperatureDialValue = value;
        _targetTemperature = Math.Round(16 + value / 360 * 14);
    }
}