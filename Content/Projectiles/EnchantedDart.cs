using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class EnchantedDart : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ModContent.ProjectileType<DartProjectile>());
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        Color[] colors = { Color.SteelBlue, Color.PaleVioletRed, Color.Gold, Color.HotPink };
        Lighting.AddLight(Projectile.Center, Color.AliceBlue.ToVector3());

        if (Main.rand.NextBool(20))
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, newColor: Main.rand.NextFromList(colors));

        if (Projectile.velocity.Length() < 12)
            Projectile.velocity *= 1.02f;
        else if (Projectile.velocity.Length() > 4)
            Projectile.velocity *= 0.98f;

        FadingSystem.FadeIn(Projectile, 45);
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}