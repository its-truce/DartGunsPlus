using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class BouncingShroom : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.penetrate = 2;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 450;
        Projectile.light = 0.2f;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        FadingSystem.FadeIn(Projectile, 5);

        if (Projectile.ai[0] == 30)
        {
            Projectile.velocity *= -1;
            Projectile.ai[0] = 0;
        }

        Projectile.rotation += MathHelper.ToRadians(3);
        if (Main.rand.NextBool(60))
            Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.GlowingMushroom);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.ai[0] = 29;
        VisualSystem.SpawnDustCircle(target.Center, DustID.GlowingMushroom, scale: 0.8f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 offset = new(Projectile.width / 2f, Projectile.height / 2f);
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + offset;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.8f);
            Color color = new Color(200, 200, 255, 0) * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

            if (k % 2 == 0)
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[k], frame.Size() / 2, sizec, SpriteEffects.None);
        }

        return true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
}