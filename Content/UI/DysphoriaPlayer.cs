using DartGunsPlus.Content.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.UI;

public class DysphoriaPlayer : ModPlayer
{
    private const int DysphoriaMax = 900; // Default maximum value of example resource
    private int _dysphoriaMax; // Buffer variable that is used to reset maximum resource to default value in ResetDefaults().

    // Here we create a custom resource, similar to mana or health.
    // Creating some variables to define the current value of our example resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
    public int DysphoriaCurrent; // Current value of our example resource
    public int DysphoriaMax2; // Maximum amount of our example resource. We will change that variable to increase maximum amount of our resource

    public override void Initialize()
    {
        _dysphoriaMax = DysphoriaMax;
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
        DysphoriaMax2 = _dysphoriaMax;
        
        if (DysphoriaCurrent < DysphoriaMax2 && Player.ownedProjectileCounts[ModContent.ProjectileType<DysphoriaMagnet>()] <= 0)
            DysphoriaCurrent++;
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
        DysphoriaCurrent = Utils.Clamp(DysphoriaCurrent, 0, DysphoriaMax2);
    }

    private void CapResourceGodMode()
    {
        if (Main.myPlayer == Player.whoAmI && Player.creativeGodMode) 
            DysphoriaCurrent = DysphoriaMax2;
        
        if (DysphoriaCurrent < DysphoriaMax2 && Player.ownedProjectileCounts[ModContent.ProjectileType<DysphoriaMagnet>()] <= 0)
            DysphoriaCurrent++;
    }
}