using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class EuphoriaCircle : ModProjectile
{
    private Color _color = new(255, 90, 200);
    private NPC Target => Main.npc[(int)Projectile.ai[0]];

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 512;
        Projectile.height = 229;
        Projectile.scale = 0.1f;
        Projectile.penetrate = -1;
        Projectile.friendly = false;
        Projectile.timeLeft = 25;
        Projectile.extraUpdates = 1;
        Projectile.Opacity = 0;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (Projectile.ai[0] == 1) // spawned by spear
            Projectile.scale = 0.6f;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 10);
        if (Projectile.timeLeft < 11)
            FadingSystem.FadeOut(Projectile, 10);

        float maxScale = Projectile.ai[1] == 1 ? 0.7f : 0.5f;
        if (Projectile.scale < maxScale)
            Projectile.scale += 0.016f;

        Projectile.Center = Target.Center + new Vector2(0, 30);

        Lighting.AddLight(Projectile.Center, _color.ToVector3());
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = _color * Projectile.Opacity;
        color.A = 0;

        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldVelocity.ToRotation(), texture.Size() / 2,
            Projectile.scale * 0.4f, SpriteEffects.None);
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

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
    }
}