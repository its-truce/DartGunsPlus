using DartGunsPlus.Content.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.UI;

public class EuphoriaPlayer : ModPlayer
{
    private const int EuphoriaMax = 900; // Default maximum value of example resource
    private int _euphoriaMax; // Buffer variable that is used to reset maximum resource to default value in ResetDefaults().

    // Here we create a custom resource, similar to mana or health.
    // Creating some variables to define the current value of our example resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
    public int EuphoriaCurrent; // Current value of our example resource
    public int EuphoriaMax2; // Maximum amount of our example resource. We will change that variable to increase maximum amount of our resource

    public override void Initialize()
    {
        _euphoriaMax = EuphoriaMax;
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
        EuphoriaMax2 = _euphoriaMax;
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
        EuphoriaCurrent = Utils.Clamp(EuphoriaCurrent, 0, EuphoriaMax2);
    }

    private void CapResourceGodMode()
    {
        if (Main.myPlayer == Player.whoAmI && Player.creativeGodMode) EuphoriaCurrent = EuphoriaMax2;

        if (EuphoriaCurrent < EuphoriaMax2 && Player.ownedProjectileCounts[ModContent.ProjectileType<RevolvingSword>()] <= 0)
            EuphoriaCurrent++;
    }
}