using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx.Guns;

public class LaserRevolving : RevolvingGun
{
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/LaserTherapy";

    protected override int ProjectileToShoot => Main.rand.NextBool(3) ? ModContent.ProjectileType<LuminiteDartProj>() : ModContent.ProjectileType<LaserLightning>();

    protected override float ShotVelocity => 8;
    protected override Color LightColor => Color.HotPink;
    private Vector2 SpawnPos => Projectile.Center + new Vector2(ProjTexture.Width * Projectile.direction, 0).RotatedBy(Projectile.rotation) / 2;

    protected override float[] InputsAI => new[]
    {
        0, SpawnPos.X, SpawnPos.Y
    };
}