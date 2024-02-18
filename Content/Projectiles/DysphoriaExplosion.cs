using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class DysphoriaExplosion : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Projectiles/Smoke2";

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 102;
        Projectile.height = 102;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.timeLeft = 30;
        Projectile.extraUpdates = 1;
        Projectile.Opacity = 0;
        Projectile.scale = 0.7f;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 10);

        if (Projectile.scale < 1f)
            Projectile.scale += 0.016f;

        if (Projectile.timeLeft < 11)
            FadingSystem.FadeOut(Projectile, 10);

        if (Projectile.timeLeft % 30 == 0)
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

        Projectile.ai[0] += 0.0333f;

        bool purple = Projectile.ai[1] == 1;
        Color color = purple ? new Color(185, 133, 240) : new Color(55, 224, 112);
        Lighting.AddLight(Projectile.Center, color.ToVector3());
    }

    public override void OnKill(int timeLeft)
    {
        if (Projectile.ai[1] != 1)
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, Projectile.type, Projectile.damage / 2,
                Projectile.knockBack, Projectile.owner, ai1: 1);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        float alpha = Projectile.ai[1] == 1 ? 1 - Projectile.ai[0] : Projectile.ai[0];
        Color color = Color.Lerp(new Color(185, 133, 240, 0), new Color(55, 224, 112, 0), alpha);

        for (int i = 0; i < 2; i++)
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldVelocity.ToRotation(), texture.Size() / 2,
                Projectile.scale * 1.5f, SpriteEffects.None);

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke6").Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Sparkle").Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        float alpha = Projectile.ai[1] == 1 ? Projectile.ai[0] : 1 - Projectile.ai[0];
        Color color = Color.Lerp(new Color(185, 133, 240, 0), new Color(55, 224, 112, 0), alpha);

        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.ai[0],
            texture.Size() / 2, 0.15f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, drawPos, null, color, 1 - Projectile.ai[0],
            texture2.Size() / 2, 0.15f, SpriteEffects.None);
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