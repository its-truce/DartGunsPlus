using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class KatanaStar : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Projectiles/Spotlight";

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 115;
        Projectile.height = 115;
        Projectile.penetrate = -1;
        Projectile.friendly = false;
        Projectile.Opacity = 0;
        Projectile.scale = 1f;
        Projectile.timeLeft = 30;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = MathHelper.ToRadians(-45);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 15);
        
        if (Projectile.scale > 0.3f)
            Projectile.scale *= 0.96f;

        if (Projectile.rotation < 0)
            Projectile.rotation += MathHelper.ToRadians(3);

        if (Projectile.timeLeft < 11)
            FadingSystem.FadeOut(Projectile, 10);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = Color.Lerp(new Color(255, 205, 0), new Color(236, 216, 126), Main.masterColor);
            
        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2,
            Projectile.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(texture, drawPos, null, new Color(255, 255, 255, 0), Projectile.rotation, texture.Size() / 2,
            Projectile.scale * 0.5f, SpriteEffects.None);
        
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