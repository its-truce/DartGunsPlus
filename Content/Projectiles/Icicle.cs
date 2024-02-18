using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class Icicle : ModProjectile
{
    private const int NumFrames = 6;
    private Color _drawColor;

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.SharpTears);
        Projectile.aiStyle = -1;
        Projectile.light = 0;
        Projectile.friendly = true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Color[] colors = { new(93, 200, 255, 0), new(0, 200, 255, 0), new(0, 126, 255, 0) };
        _drawColor = Main.rand.NextFromList(colors);
    }

    public override void AI()
    {
        Projectile.ai[0] += 1f;

        if (Projectile.localAI[0] == 0f)
        {
            Projectile.localAI[0] = 1f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.frame = Main.rand.Next(NumFrames);
            SoundEngine.PlaySound(in SoundID.Item60, Projectile.Center);
        }

        if (Projectile.ai[0] >= 30)
            Projectile.Opacity -= 0.2f;

        if (Projectile.ai[0] >= 35)
            Projectile.Kill();
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float collisionPoint = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
            Projectile.Center + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 200f * Projectile.scale, 22f * Projectile.scale,
            ref collisionPoint);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle frame = texture.Frame(1, 6, 0, Projectile.frame);
        Vector2 origin = new(16f, frame.Height * 0.5f);
        Vector2 scaleLerped = new(Projectile.scale);
        float lerpValue4 = Utils.GetLerpValue(35f, 30f, Projectile.ai[0], true);
        scaleLerped.Y *= lerpValue4;

        Main.EntitySpriteDraw(TextureAssets.Extra[98].Value,
            Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) - Projectile.velocity * Projectile.scale * 0.5f, null,
            _drawColor, Projectile.rotation + (float)Math.PI / 2f, TextureAssets.Extra[98].Value.Size() / 2f,
            Projectile.scale * 0.9f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), frame, _drawColor, Projectile.rotation,
            origin, scaleLerped, SpriteEffects.None);

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        for (float i = 0; i < 10; i++)
        {
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f) * Projectile.scale +
                                            Projectile.velocity.SafeNormalize(Vector2.UnitY) * i * 200f * Projectile.scale, DustID.SnowflakeIce,
                Main.rand.NextVector2Circular(3f, 3f), newColor: new Color(201, 195, 255), Scale: 0.4f);

            dust.velocity.Y += -0.3f;
            dust.velocity += Projectile.velocity * 0.2f;
            dust.alpha = 100;
        }
    }
}