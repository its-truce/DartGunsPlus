using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx.Guns;

public class RosemaryRevolving : RevolvingGun
{
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/RosemaryThyme";

    protected override int ProjectileToShoot => Main.rand.NextBool(3)
        ? ModContent.ProjectileType<HighVelocityDartProj>()
        : ModContent.ProjectileType<PlanteraSeed>();

    protected override float ShotVelocity => 11;
    protected override Color LightColor => Color.Green;
}