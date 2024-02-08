using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx.Guns;

public class HalloweenRevolving : RevolvingGun
{
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/HalloweenHex";

    protected override int ProjectileToShoot => Main.rand.NextBool(3) ? ModContent.ProjectileType<HeavenlyDartProj>() : ModContent.ProjectileType<HalloweenJack>();

    protected override float ShotVelocity => 14;
    protected override Color LightColor => Color.Orange;
    protected override float[] InputsAI => new float[] { default, 1, default };
}