using BaseLib.Abstracts;
using Interloper.InterloperCode.Extensions;
using Godot;

namespace Interloper.InterloperCode.Character;

public class InterloperPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Interloper.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}