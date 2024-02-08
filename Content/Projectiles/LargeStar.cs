using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class LargeStar : ModProjectile
{
    private bool _increase = true;
    private Color LerpedColor => Color.Lerp(new Color(255, 135, 120, 0), new Color(225, 225, 149, 0), Projectile.ai[1]);

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 232;
        Projectile.height = 219;
        Projectile.scale = 0.15f;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        const float lerpingIncrement = 1f / 10;

        if (Projectile.ai[1] >= 1)
            _increase = false;
        else if (Projectile.ai[1] <= 0)
            _increase = true;

        if (_increase)
            Projectile.ai[1] += lerpingIncrement;
        else
            Projectile.ai[1] -= lerpingIncrement;

        Projectile.rotation += MathHelper.ToRadians(2);

        Lighting.AddLight(Projectile.Center, LerpedColor.ToVector3());

        if (Projectile.timeLeft % 20 == 0)
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StarTrail>(),
                Projectile.damage / 2, 1, Projectile.owner, Projectile.rotation, Projectile.ai[1]);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Sparkle").Value;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, LerpedColor, Projectile.rotation, texture.Size() / 2,
            Projectile.scale, SpriteEffects.None);

        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 offset = Projectile.Size / 2;
            Vector2 drawPos2 = Projectile.oldPos[k] - Main.screenPosition + offset;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.8f);
            Color color2 = LerpedColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) *
                           Projectile.Opacity;

            Main.EntitySpriteDraw(texture2, drawPos2, null, color2, 0, texture2.Size() / 2, sizec * 0.4f,
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
        if (Main.rand.NextBool(2))
        {
            Vector2 spawnPos = target.Center + new Vector2(60, 0).RotatedByRandom(Math.Tau);
            Vector2 velocity = spawnPos.DirectionTo(target.Center) * 2;

            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), spawnPos, velocity, ModContent.ProjectileType<StellarSlash>(), Projectile.damage,
                2, Projectile.owner, ai1: Projectile.ai[1]);
        }
    }
}