using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class SereneShock : ModProjectile
{
    private float _rotSpeed = 0.6f;
    private NPC Target => Main.npc[(int)Projectile.ai[0]];
    public override string Texture => "DartGunsPlus/Content/Projectiles/DysphoriaShock";

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.timeLeft = 30;
        Projectile.Opacity = 0;
        Projectile.scale = 0.2f;
    }


    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 10);

        if (_rotSpeed < 4)
            _rotSpeed *= 1.1f;

        if (Projectile.scale < 1.3f)
            Projectile.scale += 0.04f;

        Projectile.ai[1] += 0.033f; // for color lerp
        Projectile.Center = Target.Center;
        Projectile.rotation += MathHelper.ToRadians(_rotSpeed);

        if (Projectile.timeLeft < 11)
            FadingSystem.FadeOut(Projectile, 10);

        Lighting.AddLight(Projectile.Center, new Color(62, 240, 93).ToVector3());
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = Color.Lerp(new Color(233, 233, 82, 0), new Color(62, 240, 93, 0), 1 - Projectile.ai[1]);

        for (int i = 0; i < 2; i++)
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldVelocity.ToRotation(), texture.Size() / 2, Projectile.scale * 0.15f,
                SpriteEffects.None);
        return false;
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