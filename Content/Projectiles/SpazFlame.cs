using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class SpazFlame : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";
    
    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.damage = 0;
        Projectile.friendly = false;
        Projectile.timeLeft = 30;
    }
    
    public override void AI()
    {
        Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y), 
            Projectile.width, Projectile.height, DustID.RainbowRod, Projectile.velocity.X, Projectile.velocity.Y, 100, Color.LightGreen, 0.8f);
        dust.noGravity = true;
    }

    public override void OnKill(int timeLeft)
    { 
        for (int i = 0; i < 20; i++)
        {
            Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.RainbowRod,
                (0f - Projectile.velocity.X) * 0.2f, (0f - Projectile.velocity.Y) * 0.2f, 100, Color.LightGreen, 0.5f);
            dust.noGravity = true;
            Dust dust2 = dust;
            Dust dust3 = dust2;
            dust3.velocity *= 2f;
            dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.RainbowRod,
                (0f - Projectile.velocity.X) * 0.2f, (0f - Projectile.velocity.Y) * 0.2f, 100, Color.LightGreen, 0.2f);
            dust2 = dust;
            dust3 = dust2;
            dust3.velocity *= 2f;
        }
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.LightGreen;
    }
}