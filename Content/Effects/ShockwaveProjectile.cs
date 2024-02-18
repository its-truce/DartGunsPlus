using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Effects;

public class ShockwaveProjectile : ModProjectile
{
    private const float DistortStrengh = 10;
    private const int RippleCount = 2;
    private const int RippleSize = 3;
    private const int RippleSpeed = 15;
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.timeLeft = 120;
    }

    public override void AI()
    {
        if (Projectile.timeLeft is <= 180 and < 118)
        {
            Projectile.ai[0] = 1; // Set state to exploded
            Projectile.alpha = 255; // Make the Projectile invisible.
            Projectile.friendly = false; // Stop the bomb from hurting enemies.

            if (Main.netMode != NetmodeID.Server && !Filters.Scene["Shockwave"].IsActive())
                Filters.Scene.Activate("Shockwave", Projectile.Center).GetShader().UseColor(RippleCount, RippleSize, RippleSpeed).UseTargetPosition(Projectile.Center);

            if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive())
            {
                float progress = (180f - Projectile.timeLeft) / 60f;
                Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(DistortStrengh * (1 - progress / 3f));
            }
        }
    }

    public override void OnKill(int timeLeft)
    {
        if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive()) Filters.Scene["Shockwave"].Deactivate();
    }
}