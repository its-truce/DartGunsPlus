using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class DysphoriaExplosion : ModProjectile
{
    private readonly Color _color = Color.Lerp(new Color(185, 133, 240), new Color(55, 224, 112), Main.masterColor) * 0.4f;
    public override string Texture => "DartGunsPlus/Content/Systems/Spotlight";
    private Player Owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 74;
        Projectile.height = 74;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.timeLeft = 40;
        Projectile.Opacity = 0;
        Projectile.scale = 1.3f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 25;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = MathHelper.ToRadians(-45);
        if (Owner.Distance(Projectile.Center) < 80)
        {
            Owner.velocity = new Vector2(9, 0).RotatedBy(Projectile.DirectionTo(Owner.Center).ToRotation());
            Owner.velocity.Y -= 3f;
        }
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 20);

        if (Projectile.scale > 0.8f)
            Projectile.scale -= 0.04f;

        if (Projectile.rotation < 0)
            Projectile.rotation += MathHelper.ToRadians(3);

        if (Projectile.timeLeft < 11)
            FadingSystem.FadeOut(Projectile, 10);

        if (Projectile.timeLeft == 36)
            SoundEngine.PlaySound(AudioSystem.ReturnSound("rocket"), Projectile.Center);

        Projectile.ai[0] += 1 / 40; // for lerp
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Circle").Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        Color color = _color;
        color.A = 0;

        Main.EntitySpriteDraw(texture2, drawPos, null, Color.Lerp(new Color(32, 206, 117, 0), color, 0.4f),
            Projectile.rotation, texture2.Size() / 2, 2f - Projectile.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(texture, drawPos, null, Color.Lerp(new Color(255, 255, 255, 0), color, 0.3f),
            Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.velocity.Y -= 10;
    }

    public override void PostDraw(Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke1").Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke5").Value;
        Texture2D texture3 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke6").Value;

        Color color = _color;
        color.A = 0;

        Color color1 = Color.Lerp(new Color(32, 206, 117, 0), color, Projectile.ai[0]);
        Color color2 = Color.Lerp(Color.BlueViolet, color, 0.6f);
        Color color3 = Color.Lerp(Color.SeaGreen, color, 0.1f);
        color1.A = 0;
        color2.A = 0;
        color3.A = 0;

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, color2 * Projectile.Opacity, 0,
            texture2.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None);
        Main.EntitySpriteDraw(texture3, Projectile.Center - Main.screenPosition, null, color2 * Projectile.Opacity, Projectile.rotation,
            texture3.Size() / 2, Projectile.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color1 * Projectile.Opacity, 0,
            texture.Size() / 2, Projectile.scale * 1.15f, SpriteEffects.None);
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