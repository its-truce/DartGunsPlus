using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class EuphoriaSword : ModProjectile
{
    private bool firstFrame = true;

    public override void SetDefaults()
    {
        Projectile.tileCollide = false;
        Projectile.width = 44;
        Projectile.height = 44;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.Opacity = 0;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        if (firstFrame)
        {
            Projectile.rotation = MathHelper.ToRadians(45);
            firstFrame = false;
        }

        Lighting.AddLight(Projectile.position, new Color(233, 210, 65).ToVector3());

        if (Main.rand.NextBool(30))
        {
            var dust2 = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, DustID.CrystalPulse2, Projectile.velocity.X, Projectile.velocity.Y,
                100);
            Main.dust[dust2].noGravity = true;
            Main.dust[dust2].velocity *= 0.8f;
            Main.dust[dust2].fadeIn = 1f;
        }

        if (Projectile.timeLeft >= 285)
            FadingSystem.FadeIn(Projectile, 15);

        if (Projectile.timeLeft <= 15)
            FadingSystem.FadeOut(Projectile, 15);

        if (Projectile.rotation < MathHelper.ToRadians(135))
            Projectile.rotation += MathHelper.ToRadians(2.8f);

        if (Projectile.timeLeft <= 275 && Projectile.velocity.Y < 10)
            Projectile.velocity.Y += 2f;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 64 - Projectile.alpha / 4) * Projectile.Opacity;
    }

    public override void OnKill(int timeLeft)
    {
        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
        for (var i = 0; i < 20; i++)
        {
            var dust2 = Dust.NewDust(Projectile.position, Projectile.width / 3, Projectile.height / 3, DustID.CrystalPulse2, Projectile.velocity.X, Projectile.velocity.Y,
                100, default, 1.2f);
            Main.dust[dust2].noGravity = true;
            Main.dust[dust2].velocity *= 2f;
            Main.dust[dust2].fadeIn = 1.3f;
        }
    }
}