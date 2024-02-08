using DartGunsPlus.Content.Items.Ammo;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx.Guns;

public class FrenzyRevolving : RevolvingGun
{
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/StellarFrenzy";

    protected override int ProjectileToShoot => Main.rand.NextBool(3) ? ModContent.ProjectileType<FlamingDartProj>() : ProjectileID.Starfury;

    protected override float ShotVelocity => 9;
    protected override Color LightColor => Color.DeepPink;
}