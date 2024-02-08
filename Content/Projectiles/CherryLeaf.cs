using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class CherryLeaf : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Projectiles/Glowball";

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 15;
        Projectile.height = 15;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
        Projectile.light = 0.5f;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 15);

        if (Projectile.timeLeft < 20)
            FadingSystem.FadeOut(Projectile, 20);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Color color = new Color(255, 160, 210, 0) * (1f - Projectile.alpha) * (Projectile.oldPos.Length / (float)Projectile.oldPos.Length);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation,
            texture.Size() / 2, 0.06f, SpriteEffects.None);

        if (Projectile.timeLeft > 20)
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * Projectile.Opacity,
                Projectile.rotation, texture.Size() / 2, 0.03f, SpriteEffects.None); // white center

        return false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity.Y *= -1;
        return false;
    }
}