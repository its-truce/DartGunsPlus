using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class TerraBolt : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";
    private static Color Color => Color.Lerp(new Color(34, 177, 77), Color.YellowGreen, Main.masterColor) * 0.4f;

    public override void SetDefaults()
    {
        Projectile.netImportant = true;
        Projectile.tileCollide = true;
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.aiStyle = 43;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 360;
        Projectile.hide = true;
        Projectile.extraUpdates = 10;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 6;
    }

    public override void AI()
    {
        ParticleOrchestraSettings settings = new()
        {
            PositionInWorld = Projectile.Center,
            MovementVector = Projectile.velocity,
            UniqueInfoPiece = (int)VisualSystem.HueForParticle(Color)
        };

        ParticleOrchestrator.BroadcastOrRequestParticleSpawn(ParticleOrchestraType.ChlorophyteLeafCrystalShot, settings);
    }
}