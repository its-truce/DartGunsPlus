using System;
using DartGunsPlus.Content.Items.Weapons;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class HalloweenRitual : ModProjectile
{
    private float _rotSpeed = 0.3f;
    private Player Owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 256;
        Projectile.height = 256;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.timeLeft = 2;
        Projectile.Opacity = 0;
        Projectile.scale = 0.86f;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 10);

        if (Projectile.scale < 1)
            Projectile.scale += 0.016f;

        if (_rotSpeed < 4)
            _rotSpeed *= 1.05f;

        Projectile.rotation += MathHelper.ToRadians(_rotSpeed);
        Projectile.Center = Owner.Center;

        //if (Projectile.timeLeft < 11)
        //  FadingSystem.FadeOut(Projectile, 10);

        Projectile.timeLeft += 2;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = new Color(255, 82, 30, 0) * Projectile.Opacity;

        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale / 2,
            SpriteEffects.None);

        if (Owner.HeldItem.ModItem is not HalloweenHex)
            Projectile.Kill();

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

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        ParticleOrchestraSettings settings = new()
        {
            PositionInWorld = target.Center,
            MovementVector = target.velocity.RotatedByRandom(Math.Tau),
            UniqueInfoPiece = (int)VisualSystem.HueForParticle(Color.Red)
        };

        ParticleOrchestrator.BroadcastOrRequestParticleSpawn(ParticleOrchestraType.ChlorophyteLeafCrystalShot, settings);
    }
}