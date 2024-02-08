using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DartGunsPlus.Content.Projectiles;

public class EuphoriaSword : ModProjectile
{
    private Color _color = new Color(255, 255, 100);
    
    public override void SetDefaults()
    {
        Projectile.tileCollide = false;
        Projectile.width = 85;
        Projectile.height = 266;
        Projectile.scale = 0.6f;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 300;
        Projectile.Opacity = 0;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.position, _color.ToVector3());

        if (Main.rand.NextBool(30))
        {
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, DustID.CrystalPulse2, Projectile.velocity.X,
                Projectile.velocity.Y, 100);
            Main.dust[dust2].noGravity = true;
            Main.dust[dust2].velocity *= 0.8f;
            Main.dust[dust2].fadeIn = 1f;
        }

        if (Projectile.timeLeft >= 285)
            FadingSystem.FadeIn(Projectile, 15);

        if (Projectile.timeLeft == 285)
            SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, Projectile.Center);

        if (Projectile.timeLeft <= 18)
            FadingSystem.FadeOut(Projectile, 15);

        if (Projectile.velocity.Y < 15)
            Projectile.velocity.Y *= 1.2f;

        if (Projectile.scale < 1f)
            Projectile.scale *= 1.06f;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Vector2 drawOrigin = texture.Size() / 2;
        Color color = _color;
        color.A = 0;
        
        Main.EntitySpriteDraw(texture, drawPos, null, color, 0, drawOrigin, Projectile.scale * 0.4f, SpriteEffects.None);
        
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
        
        for (int i = 0; i < 20; i++)
        {
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width / 3, Projectile.height / 3, DustID.RainbowRod, Projectile.velocity.X,
                Projectile.velocity.Y, 100, _color);
            Main.dust[dust2].noGravity = true;
            Main.dust[dust2].velocity *= 2f;
            Main.dust[dust2].fadeIn = 1.3f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(AudioSystem.ReturnSound("boom"), target.Center);
        
        int randDust = Main.rand.Next(15, 31);
        for (int i = 0; i < randDust; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.RainbowRod, 0f, 0f, 100, _color, 0.8f);
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