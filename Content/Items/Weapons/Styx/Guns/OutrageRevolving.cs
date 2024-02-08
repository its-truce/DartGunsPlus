using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx.Guns;

public class OutrageRevolving : RevolvingGun
{
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/StellarOutrage";

    protected override int ProjectileToShoot => Main.rand.NextBool(3) ? ModContent.ProjectileType<SpectreDartProj>() : ModContent.ProjectileType<LargeStar>();

    protected override float ShotVelocity => 14;
    protected override Color LightColor => new(255, 135, 120);
}