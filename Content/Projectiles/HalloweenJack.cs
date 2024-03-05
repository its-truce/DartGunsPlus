using System;
using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Items.Weapons;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class HalloweenJack : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 32;
        Projectile.height = 34;
        Projectile.penetrate = 1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 1200;
        Projectile.light = 0.3f;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        FadingSystem.FadeIn(Projectile, 15);

        if (Projectile.ai[1] != 0)
        {
            Projectile.friendly = true;
            Projectile.ai[1]++;

            if (Projectile.ai[1] == 240)
                Projectile.Kill();
        }
        else
        {
            Projectile.ai[2] += MathHelper.ToRadians(1);
            const int dist = 120;
            Projectile.Center = Owner.Center + new Vector2(dist, 0).RotatedBy(Projectile.ai[2]);
            Projectile.velocity = Vector2.Zero;

            if (Owner.HeldItem.ModItem is not HalloweenHex)
                Projectile.Kill();
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.Kill();
        VisualSystem.SpawnDustCircle(target.Center, ModContent.DustType<GlowFastDecelerate>(), 12, color: Color.OrangeRed, scale: 0.9f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        string path = Projectile.friendly ? "Trail" : "Glowball";
        float scale = Projectile.friendly ? 0.5f : 0.07f;

        Texture2D texture = ModContent.Request<Texture2D>($"DartGunsPlus/Content/Projectiles/{path}").Value;

        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 offset = new(Projectile.width / 2f, Projectile.height / 2f);
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPos = Projectile.oldPos[k] + offset - Main.screenPosition;
            float sizec = Projectile.scale * Math.Clamp((Projectile.oldPos.Length - k * 2) / (Projectile.oldPos.Length * 0.8f), 0, 1);
            Color color = new Color(255, 82, 30, 0) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

            if (Projectile.Center.Distance(Projectile.oldPos[k] + offset) > 3)
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.velocity.ToRotation(), frame.Size() / 2, sizec * scale, SpriteEffects.None);
        }

        return true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void OnKill(int timeLeft)
    {
        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(),
            Projectile.damage * 2, 7, Projectile.owner);
    }
}