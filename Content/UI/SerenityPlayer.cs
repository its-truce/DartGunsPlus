using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.UI;

public class SerenityPlayer : ModPlayer
{
    private const int SerenityMax = 3; // Default maximum value of example resource
    private int _serenityMax; // Buffer variable that is used to reset maximum resource to default value in ResetDefaults().

    // Here we create a custom resource, similar to mana or health.
    // Creating some variables to define the current value of our example resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
    public int SerenityCurrent; // Current value of our example resource
    public int SerenityMax2; // Maximum amount of our example resource. We will change that variable to increase maximum amount of our resource

    public override void Initialize()
    {
        _serenityMax = SerenityMax;
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
        SerenityMax2 = _serenityMax;
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
        SerenityCurrent = Utils.Clamp(SerenityCurrent, 0, SerenityMax2);
    }

    private void CapResourceGodMode()
    {
        if (Main.myPlayer == Player.whoAmI && Player.creativeGodMode)
            SerenityCurrent = SerenityMax2;
    }
}