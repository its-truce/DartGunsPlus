using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx;

public class StyxRune : ModProjectile
{
    private bool _increase = true;
    private float _rotSpeed = 0.1f;

    private Color LerpedColor => Color.Lerp(new Color(85, 105, 181, 0), new Color(42, 34, 80, 0), Projectile.ai[1])
                                 * Projectile.Opacity;

    private Player Owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 1500;
        Projectile.height = 1500;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.timeLeft = 2;
        Projectile.Opacity = 0;
        Projectile.scale = 0.15f;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 10);

        if (Projectile.scale < 0.4f)
            Projectile.scale += 0.016f;

        if (_rotSpeed < 3)
            _rotSpeed *= 1.03f;

        Projectile.rotation += MathHelper.ToRadians(_rotSpeed);
        Projectile.Center = Owner.Center;

        // for color lerping
        const float lerpingIncrement = 1f / 30;

        _increase = Projectile.ai[1] switch
        {
            >= 1 => false,
            <= 0 => true,
            _ => _increase
        };

        if (_increase)
            Projectile.ai[1] += lerpingIncrement;
        else
            Projectile.ai[1] -= lerpingIncrement;

        if (Owner.HeldItem.ModItem is not Styx)
            Projectile.Kill();
        else
            Projectile.timeLeft += 2;

        Projectile.ai[2]++; // for multicolor blend rot
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Items/Weapons/Styx/Blend").Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        Main.EntitySpriteDraw(texture, drawPos, null, LerpedColor, Projectile.rotation, texture.Size() / 2, Projectile.scale / 2,
            SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, drawPos, null, new Color(255, 255, 255, 0) * Projectile.Opacity,
            MathHelper.ToRadians(Projectile.ai[2]) / 3, texture2.Size() / 2, Projectile.scale * 1.3f, SpriteEffects.None);

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
            MovementVector = Vector2.One.RotatedByRandom(Math.Tau),
            UniqueInfoPiece = (int)VisualSystem.HueForParticle(LerpedColor)
        };

        ParticleOrchestrator.BroadcastOrRequestParticleSpawn(ParticleOrchestraType.ChlorophyteLeafCrystalShot, settings);
    }
}