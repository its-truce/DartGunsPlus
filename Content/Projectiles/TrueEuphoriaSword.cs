using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class TrueEuphoriaSword : ModProjectile
{
    private readonly string[] TexturePaths = { "EuphoriaSword", "EuphoriaSwordPink", "EuphoriaSwordBlue" }; // Projectile.ai[1] is used as index
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => "DartGunsPlus/Content/Projectiles/EuphoriaSword";

    public override void SetDefaults()
    {
        Projectile.width = 44;
        Projectile.height = 44;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 30;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeOut(Projectile, 10);

        if (Projectile.timeLeft <= 7)
            FadingSystem.FadeOut(Projectile, 7);

        Projectile.velocity *= 0.97f;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(47);

        switch (Projectile.ai[1])
        {
            case 0:
                Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3());
                break;

            case 1:
                Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3());
                break;

            case 2:
                Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3());
                break;
        }

        if (Main.rand.NextBool(30))
        {
            var dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.CrystalPulse2, Projectile.velocity.X * .2f,
                Projectile.velocity.Y * .2f, 200, Scale: 1.2f);
            dust.noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var texture = (Texture2D)ModContent.Request<Texture2D>($"DartGunsPlus/Content/Projectiles/{TexturePaths[(int)Projectile.ai[1]]}");

        var drawOrigin = texture.Size() / 2;
        var drawPos = Projectile.Center - Main.screenPosition;
        var color = Color.White * Projectile.Opacity;
        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Projectile.ai[1] != 2)
        {
            var spawnPos = target.Center + new Vector2(120, 0).RotatedByRandom(Math.Tau);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), spawnPos, spawnPos.DirectionTo(target.Center) * 9, Projectile.type,
                Projectile.damage, Projectile.knockBack, Owner.whoAmI, ai1: Projectile.ai[1] + 1);
        }

        for (var i = 0; i < 4; i++)
        {
            var dust2 = Dust.NewDust(Projectile.position, Projectile.width / 3, Projectile.height / 3, DustID.CrystalPulse2, Projectile.velocity.X, Projectile.velocity.Y,
                100, default, 1.2f);
            Main.dust[dust2].noGravity = true;
            Main.dust[dust2].velocity *= 2f;
            Main.dust[dust2].fadeIn = 1.3f;
        }
    }
}