using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.UI;

public class LaserPlayer : ModPlayer
{
    private const int LaserMax = 10; // Default maximum value of example resource
    private int _laserMax; // Buffer variable that is used to reset maximum resource to default value in ResetDefaults().

    // Here we create a custom resource, similar to mana or health.
    // Creating some variables to define the current value of our example resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
    public int LaserCurrent; // Current value of our example resource
    public int LaserMax2; // Maximum amount of our example resource. We will change that variable to increase maximum amount of our resource

    public override void Initialize()
    {
        _laserMax = LaserMax;
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
        LaserMax2 = _laserMax;
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
        LaserCurrent = Utils.Clamp(LaserCurrent, 0, LaserMax2);
    }

    private void CapResourceGodMode()
    {
        if (Main.myPlayer == Player.whoAmI && Player.creativeGodMode) LaserCurrent = LaserMax2;
    }
}