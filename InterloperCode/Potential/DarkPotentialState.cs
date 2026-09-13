namespace Interloper.InterloperCode.Potential;

public class DarkPotentialState
{
    public int Current { get; set; }
    public int Max { get; set; } = 100;
    public bool AutoGrantEnergy { get; set; }
    public bool AutoUseThresholds { get; set; }
    public int LastGrantedEnergyIndex { get; set; } = -1;
    public int LastActivatedLevelIndex { get; set; } = -1;
}