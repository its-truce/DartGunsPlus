using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class StarTrail : ModProjectile
{
    private Color LerpedColor => Color.Lerp(new Color(255, 135, 120, 0), new Color(225, 225, 149, 0), Projectile.ai[1]) * Projectile.Opacity;
    public override string Texture => "DartGunsPlus/Content/Projectiles/LargeStar";

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 232;
        Projectile.height = 219;
        Projectile.scale = 0.1f;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 60;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        if (Projectile.timeLeft > 29)
            FadingSystem.FadeIn(Projectile, 30);
        if (Projectile.timeLeft < 11)
            FadingSystem.FadeOut(Projectile, 10);

        Projectile.rotation = Projectile.ai[0];
        Lighting.AddLight(Projectile.Center, LerpedColor.ToVector3());
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        for (int i = 0; i < 2; i++)
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, LerpedColor, Projectile.rotation, texture.Size() / 2,
                Projectile.scale, SpriteEffects.None);

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
}