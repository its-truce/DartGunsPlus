using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class SereneSlash : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 51;
        Projectile.height = 51;
        Projectile.aiStyle = 0;
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

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.direction = (int)Projectile.velocity.SafeNormalize(Projectile.velocity).X;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(-25 * Projectile.direction);
    }

    public override void AI()
    {
        FadingSystem.FadeOut(Projectile, 10);

        if (Projectile.timeLeft <= 7)
            FadingSystem.FadeOut(Projectile, 7);

        Projectile.velocity *= 0.97f;
        if (Projectile.rotation < Projectile.velocity.ToRotation() + MathHelper.ToRadians(30 * Projectile.direction))
            Projectile.rotation += MathHelper.ToRadians(3f * Projectile.direction);
        Projectile.ai[1] += 0.033f; // for color lerp

        if (Projectile.scale < 1.5f)
            Projectile.scale *= 1.03f;

        Lighting.AddLight(Projectile.Center, new Color(62, 240, 93).ToVector3());
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = Color.Lerp(new Color(233, 233, 82, 0), new Color(62, 240, 93, 0), Projectile.ai[1]);

        for (int i = 0; i < 2; i++)
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2,
                Projectile.scale * 0.1f, SpriteEffects.None);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        int randDust = Main.rand.Next(15, 31);
        for (int i = 0; i < randDust; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.TerraBlade, 0f, 0f, 100, Color.DarkTurquoise, 0.8f);
            dust.velocity *= 1.6f;
            dust.velocity.Y -= 1f;
            dust.velocity += Projectile.velocity;
            dust.noGravity = true;
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        // Calculate the scaled hitbox size based on the Projectile.scale
        int scaledWidth = (int)(projHitbox.Width * Projectile.scale);
        int scaledHeight = (int)(projHitbox.Height * Projectile.scale);

        // Calculate the position of the scaled hitbox
        int scaledX = projHitbox.X + (projHitbox.Width - scaledWidth) / 2;
        int scaledY = projHitbox.Y + (projHitbox.Height - scaledHeight) / 2;

        // Create the scaled hitbox
        Rectangle scaledHitbox = new(scaledX, scaledY, scaledWidth, scaledHeight);

        // Check for collision between the scaled hitbox and the target
        return scaledHitbox.Intersects(targetHitbox);
    }
}