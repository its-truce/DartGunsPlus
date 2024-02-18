using System;
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

public class Explosion : ModProjectile
{
    private float _rot1;
    private float _rot2;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Smoke6";

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 75;
        Projectile.height = 75;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 21;
        Projectile.Opacity = 0;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Main.rand.NextFloat(MathF.Tau);
        _rot1 = Main.rand.NextFloat(MathF.Tau);
        _rot2 = Main.rand.NextFloat(MathF.Tau);
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 10);

        if (Projectile.timeLeft < 10)
            FadingSystem.FadeOut(Projectile, 9);

        if (Projectile.timeLeft == 10)
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

        Color color = Projectile.ai[0] == 0 ? Color.OrangeRed : Color.CornflowerBlue;
        Lighting.AddLight(Projectile.Center, color.ToVector3());
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke1").Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke2").Value;

        Color color1 = Projectile.ai[0] == 0 ? new Color(255, 140, 0, 0) : new Color(0, 140, 255, 0);
        Color color2 = Projectile.ai[0] == 0 ? new Color(255, 255, 0, 0) : new Color(0, 255, 255, 0);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color1 * Projectile.Opacity, _rot1,
            texture.Size() / 2, 0.9f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, color2 * Projectile.Opacity, _rot2,
            texture2.Size() / 2, 0.5f, SpriteEffects.None);

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Sparkle").Value;
        Color color1 = Projectile.ai[0] == 0 ? new Color(255, 82, 30, 0) : new Color(30, 82, 255, 0);
        Color color2 = Projectile.ai[0] == 0 ? new Color(255, 10, 0, 0) : new Color(60, 70, 255, 0);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color1 * Projectile.Opacity, Projectile.rotation,
            texture.Size() / 2, 0.75f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, color2 * Projectile.Opacity, Projectile.rotation,
            texture2.Size() / 2, 0.75f, SpriteEffects.None);
    }
}