using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class PlanteraSeed : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.SeedPlantera);
        Projectile.friendly = true;
        Projectile.hostile = false;
        AIType = ProjectileID.SeedPlantera;
        Projectile.tileCollide = false;
    }
}