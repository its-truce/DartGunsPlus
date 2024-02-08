using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class DukeWhirlpool : ModProjectile
{
    private float _rotSpeed = 0.3f;

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 640;
        Projectile.height = 640;
        Projectile.penetrate = -1;
        Projectile.friendly = false;
        Projectile.timeLeft = 600;
        Projectile.Opacity = 0;
        Projectile.scale = 0.125f;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void OnSpawn(IEntitySource source)
    {
        foreach (Projectile proj in Main.projectile)
            if (proj.type == Projectile.type && proj.whoAmI != Projectile.whoAmI && proj.active && proj.owner == Projectile.owner)
                proj.Kill();
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 10);

        if (_rotSpeed < 4)
            _rotSpeed *= 1.05f;

        if (Projectile.scale < 0.8f)
            Projectile.scale += 0.04f;

        Projectile.rotation += MathHelper.ToRadians(_rotSpeed);

        if (Projectile.timeLeft < 11)
            FadingSystem.FadeOut(Projectile, 10);

        Lighting.AddLight(Projectile.Center, Color.Turquoise.ToVector3());

        if (Main.rand.NextBool(30))
            Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.FishronWings);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = new(24, 200, 119, 0);

        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale * 0.15f,
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