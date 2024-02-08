using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx.Guns;

public class SerenityRevolving : RevolvingGun
{
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/Serenity";

    protected override int ProjectileToShoot => Main.rand.NextBool(3)
        ? ModContent.ProjectileType<ChlorophyteDartProj>()
        : ModContent.ProjectileType<SereneBolt>();

    protected override float ShotVelocity => 11;
    protected override Color LightColor => Color.YellowGreen;
}