using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx;

public class BlackHole : ModProjectile
{
    private Color _purpleCol = new(45, 65, 141, 0);
    private float _rotSpeed = 0.6f;
    private int TargetWhoAmI => (int)Projectile.ai[0];

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 200;
        Projectile.height = 200;
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
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Items/Weapons/Styx/Spike").Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = _purpleCol * Projectile.Opacity;
        color.A = 0;
        Color color2 = new Color(142, 134, 180, 0) * Projectile.Opacity;

        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2,
            Projectile.scale * 0.4f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, drawPos, null, color2, MathHelper.PiOver4, texture2.Size() / 2,
            1f, SpriteEffects.None);

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