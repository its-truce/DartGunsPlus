using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class LightningIcon : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];
    private float Distance => Projectile.ai[1];

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 2;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (Distance == 0)
            Projectile.ai[1] = 50;
    }

    public override void AI()
    {
        Entity target = Projectile.ai[0] == 0 ? Owner : Main.npc[(int)Projectile.ai[0]];
        Projectile.Center = target.Center - new Vector2(0, Distance);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}