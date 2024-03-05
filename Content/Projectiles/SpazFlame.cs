using DartGunsPlus.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class SpazFlame : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";
    
    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 15;
    }
    
    public override void AI()
    {
        Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y), 
            Projectile.width, Projectile.height, ModContent.DustType<GlowFastDecelerate>(), Projectile.velocity.X, Projectile.velocity.Y, 100, Color.LimeGreen, 
            0.8f);
        dust.noGravity = true;
    }

    public override void OnKill(int timeLeft)
    { 
        for (int i = 0; i < 20; i++)
        {
            Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 
                ModContent.DustType<GlowFastDecelerate>(), (0f - Projectile.velocity.X) * 0.2f, (0f - Projectile.velocity.Y) * 0.2f, 100,
                Color.LimeGreen, 0.5f);
            dust.noGravity = true;
            Dust dust2 = dust;
            Dust dust3 = dust2;
            dust3.velocity *= 2f;
            dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 
                ModContent.DustType<GlowFastDecelerate>(), (0f - Projectile.velocity.X) * 0.2f, (0f - Projectile.velocity.Y) * 0.2f, 100,
                Color.LimeGreen, 0.2f);
            dust2 = dust;
            dust3 = dust2;
            dust3.velocity *= 2f;
        }
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.LimeGreen;
    }
}