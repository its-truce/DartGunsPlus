using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class DysphoriaShock : ModProjectile
{
    private Color _purpleCol = new(55, 224, 112);
    private float _rotSpeed = 0.6f;
    private int TargetWhoAmI => (int)Projectile.ai[0];

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.timeLeft = 25;
        Projectile.extraUpdates = 1;
        Projectile.Opacity = 0;
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

        if (_rotSpeed < 4)
            _rotSpeed *= 1.1f;

        Projectile.scale += 0.016f;
        Projectile.Center = Main.npc[TargetWhoAmI].Center;
        Projectile.rotation += MathHelper.ToRadians(_rotSpeed);

        Lighting.AddLight(Projectile.Center, _purpleCol.ToVector3());
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = _purpleCol * Projectile.Opacity;
        color.A = 0;

        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldVelocity.ToRotation(), texture.Size() / 2,
            Projectile.scale, SpriteEffects.None);
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
            PositionInWorld = Projectile.Center,
            MovementVector = Projectile.velocity,
            UniqueInfoPiece = (int)VisualSystem.HueForParticle(_purpleCol)
        };

        ParticleOrchestrator.BroadcastOrRequestParticleSpawn(ParticleOrchestraType.TrueNightsEdge, settings);
    }
}