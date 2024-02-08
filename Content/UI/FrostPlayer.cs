using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.UI;

public class FrostPlayer : ModPlayer
{
    private const int DefaultFrostMax = 10; // Default maximum value of example resource
    private int _frostMax; // Buffer variable that is used to reset maximum resource to default value in ResetDefaults().

    // Here we create a custom resource, similar to mana or health.
    // Creating some variables to define the current value of our example resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
    public int FrostCurrent; // Current value of our example resource
    public int FrostMax2; // Maximum amount of our example resource. We will change that variable to increase maximum amount of our resource

    public override void Initialize()
    {
        _frostMax = DefaultFrostMax;
    }

    public override void ResetEffects()
    {
        ResetVariables();
    }

    public override void UpdateDead()
    {
        ResetVariables();
    }

    private void ResetVariables()
    {
        FrostMax2 = _frostMax;
    }

    public override void PostUpdateMiscEffects()
    {
        UpdateResource();
    }

    public override void PostUpdate()
    {
        CapResourceGodMode();
    }

    private void UpdateResource()
    {
        FrostCurrent = Utils.Clamp(FrostCurrent, 0, FrostMax2);
    }

    private void CapResourceGodMode()
    {
        if (Main.myPlayer == Player.whoAmI && Player.creativeGodMode) FrostCurrent = FrostMax2;
    }
}