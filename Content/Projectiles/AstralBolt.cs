using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class AstralBolt : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";

    public override void SetDefaults()
    {
        Projectile.netImportant = true;
        Projectile.tileCollide = true;
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.aiStyle = 43;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.hide = true;
        Projectile.extraUpdates = 180;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 6;
    }

    public override void AI()
    {
        ParticleOrchestraSettings settings = new()
        {
            PositionInWorld = Projectile.Center,
            MovementVector = Projectile.velocity,
            UniqueInfoPiece = 75
        };

        ParticleOrchestrator.BroadcastOrRequestParticleSpawn(ParticleOrchestraType.ChlorophyteLeafCrystalShot, settings);

        //Lighting.AddLight(Projectile.Center, 0, 1, 0);
    }
}