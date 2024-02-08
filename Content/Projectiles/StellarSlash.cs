using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class StellarSlash : ModProjectile
{
    private bool _hit;
    private float _scale = 0.3f;
    private Color LerpedColor => Color.Lerp(new Color(255, 135, 120, 0), new Color(225, 225, 149, 0), Projectile.ai[1]) * Projectile.Opacity;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Bolt";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.ignoreWater = true;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 30;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        FadingSystem.FadeIn(Projectile, 10);

        if (!_hit)
        {
            if (Projectile.velocity.Length() < 8)
                Projectile.velocity *= 1.1f;

            if (_scale < 1f)
                _scale *= 1.1f;
        }
        else
        {
            if (Projectile.velocity.Length() > 2)
                Projectile.velocity *= 0.98f;

            if (_scale > 0.4f)
                _scale *= 0.97f;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();
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
            Color color = LerpedColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length)
                                      * Projectile.Opacity;
            color.A = 0;
            Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[k], frame.Size() / 2, sizec * 0.2f * new Vector2(_scale, 1),
                SpriteEffects.None);
        }

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        _hit = true;

        if (Main.rand.NextBool(2))
        {
            Vector2 spawnPos = target.Center + new Vector2(60, 0).RotatedByRandom(Math.Tau);
            Vector2 velocity = spawnPos.DirectionTo(target.Center) * 2;

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), spawnPos, velocity, ModContent.ProjectileType<StellarSlash>(), Projectile.damage,
                2, Projectile.owner, ai1: Projectile.ai[1]);
        }
    }
}