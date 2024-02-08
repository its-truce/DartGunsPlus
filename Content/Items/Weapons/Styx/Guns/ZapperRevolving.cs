using DartGunsPlus.Content.Items.Ammo;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx.Guns;

public class ZapperRevolving : RevolvingGun
{
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/DartZapper";
    protected override int ProjectileToShoot => ModContent.ProjectileType<DartProjectile>();
    protected override float ShotVelocity => 8;
    protected override Color LightColor => Color.Brown;
    protected override float[] InputsAI => new float[] { default, default, 5 };
}