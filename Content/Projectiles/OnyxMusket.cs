using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class OnyxMusket : ModProjectile
{
    private float _rot1;
    private float _rot2;
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => "DartGunsPlus/Content/Projectiles/Smoke1";

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 102;
        Projectile.height = 102;
        Projectile.scale = 0.75f; // 30
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 10;
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
        FadingSystem.FadeIn(Projectile, 5);

        if (Projectile.timeLeft < 5)
            FadingSystem.FadeOut(Projectile, 4);

        Lighting.AddLight(Projectile.Center, Color.DarkViolet.ToVector3());
        Projectile.rotation = Projectile.ai[0];
        Projectile.velocity = Owner.velocity * 0.2f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke3").Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke4").Value;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(35, 35, 49, 0) * Projectile.Opacity,
            _rot1, texture.Size() / 2, 0.3f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, new Color(162, 0, 255, 0) * Projectile.Opacity,
            _rot2, texture2.Size() / 2, 0.4f, SpriteEffects.None);

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke5").Value;
        Color color = new Color(195, 65, 255, 0) * (1f - Projectile.alpha) * (Projectile.oldPos.Length / (float)Projectile.oldPos.Length);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation,
            texture.Size() / 2, 0.5f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, new Color(228, 121, 255, 0), Projectile.rotation,
            texture2.Size() / 2, 0.5f, SpriteEffects.None);
    }
}