using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class GoreMusket : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 512;
        Projectile.height = 512;
        Projectile.scale = 0.15f; // 30
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 10;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 5);

        if (Projectile.timeLeft < 5)
            FadingSystem.FadeOut(Projectile, 4);

        if (Projectile.timeLeft == 5)
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

        Lighting.AddLight(Projectile.Center, new Color(255, 231, 143).ToVector3());
        Projectile.rotation = Projectile.ai[0];
        Projectile.velocity = Owner.velocity * 0.2f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 231, 143, 0) * Projectile.Opacity,
            Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Musket2").Value;

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, new Color(255, 204, 20, 0), Projectile.rotation,
            texture2.Size() / 2, Projectile.scale * 0.7f, SpriteEffects.None);
    }
}