using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class MartianSpark : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 125;
        Projectile.height = 125;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 30;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        if (Projectile.timeLeft % 15 == 0)
            Projectile.rotation = Main.rand.NextFloat(0, MathF.Tau);

        FadingSystem.FadeIn(Projectile, 3);

        if (Projectile.timeLeft < 4)
            FadingSystem.FadeOut(Projectile, 3);

        Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3());
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Color drawColor = Color.SkyBlue * Projectile.Opacity;
        drawColor.A = 0;
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, texture.Size() / 2, 1f,
            SpriteEffects.None);
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(AudioSystem.ReturnSound("boom"), Projectile.Center);
    }
}