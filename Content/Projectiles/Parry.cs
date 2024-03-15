using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class Parry : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 45;
        Projectile.Opacity = 0;
        Projectile.scale = 0.5f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.velocity = new Vector2(0, -4);
        Projectile.ai[1] = 0.2f;
    }

    public override void AI()
    {
        if (Projectile.timeLeft >= 35)
            FadingSystem.FadeIn(Projectile, 10);
        
        Projectile.ai[0]++;

        if (Projectile.ai[0] <= 25)
            Projectile.velocity.Y += 0.16f;

        if (Projectile.timeLeft <= 15) 
            FadingSystem.FadeOut(Projectile, 15);

        if (Projectile.scale < 1.1f)
            Projectile.scale *= 1.05f;

        if (Projectile.ai[1] < 1)
            Projectile.ai[1] *= 1.1f; // for text scale
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D textureBackground = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/ParryBG").Value;
        Texture2D textureText = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = Color.White * Projectile.Opacity;
        
        Main.EntitySpriteDraw(textureBackground, drawPos, null, color, 0, textureBackground.Size()/2, Projectile.scale, 
            SpriteEffects.None);
        Main.EntitySpriteDraw(textureText, drawPos, null, color, 0, textureText.Size()/2, Projectile.ai[1],
            SpriteEffects.None);
        
        return false;
    }
}