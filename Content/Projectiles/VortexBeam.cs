using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class VortexBeam : ModProjectile
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
        Projectile.penetrate = 1;
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
            UniqueInfoPiece = (int)VisualSystem.HueForParticle(new Color(0, 240, 190))
        };

        ParticleOrchestrator.BroadcastOrRequestParticleSpawn(ParticleOrchestraType.ChlorophyteLeafCrystalShot, settings);

        Lighting.AddLight(Projectile.Center, new Color(0, 240, 136).ToVector3());
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.damage = (int)(Projectile.damage * 0.85f);
    }
}