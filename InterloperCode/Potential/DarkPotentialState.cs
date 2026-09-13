namespace Interloper.InterloperCode.Potential;

public class DarkPotentialState
{
    public int Current { get; set; }
    public int Max { get; set; } = 100;
    public bool AutoGrantEnergy { get; set; }
    public int LastGrantedEnergyIndex { get; set; } = -1;
}