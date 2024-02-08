using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace DartGunsPlus.Content.Items.Weapons.Styx.Guns;

public class BumbleRevolving : RevolvingGun
{
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/BumbleBarrage";
    protected override int ProjectileToShoot => Main.rand.NextBool(3) ? ProjectileID.GiantBee : ProjectileID.Bee;
    protected override float ShotVelocity => 8;
    protected override Color LightColor => Color.Yellow;
}