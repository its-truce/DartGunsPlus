using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx.Guns;

public class EnchantedRevolving : RevolvingGun
{
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/EnchantedDartcaster";

    protected override int ProjectileToShoot => Main.rand.NextBool(3) ? ModContent.ProjectileType<FrostburnDartProj>() : ModContent.ProjectileType<EnchantedDart>();

    protected override float ShotVelocity => 6;
    protected override Color LightColor => Color.DeepSkyBlue;
}