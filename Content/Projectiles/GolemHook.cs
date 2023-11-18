using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class GolemHook : ModProjectile
{
    private static Asset<Texture2D> _chainTexture;

    public override void Load()
    {
        _chainTexture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/GolemHookChain");
    }

    public override void Unload()
    {
        _chainTexture = null;
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.SingleGrappleHook[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.IlluminantHook);
    }

    public override float GrappleRange()
    {
        return 400f;
    }

    public override void GrappleRetreatSpeed(Player player, ref float speed)
    {
        speed = 18;
    }

    public override void GrapplePullSpeed(Player player, ref float speed)
    {
        speed = 16;
    }

    public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY)
    {
        var dirToPlayer = Projectile.DirectionTo(player.Center);
        const float hangDist = 50f;
        grappleX += dirToPlayer.X * hangDist;
        grappleY += dirToPlayer.Y * hangDist;
    }

    // Draws the grappling hook's chain.
    public override bool PreDrawExtras()
    {
        var playerCenter = Main.player[Projectile.owner].MountedCenter;
        var center = Projectile.Center;
        var directionToPlayer = playerCenter - Projectile.Center;
        var chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
        var distanceToPlayer = directionToPlayer.Length();

        while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer))
        {
            directionToPlayer /= distanceToPlayer; // get unit vector
            directionToPlayer *= _chainTexture.Height(); // multiply by chain link length

            center += directionToPlayer; // update draw position
            directionToPlayer = playerCenter - center; // update distance
            distanceToPlayer = directionToPlayer.Length();

            var drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));
            Main.EntitySpriteDraw(_chainTexture.Value, center - Main.screenPosition, _chainTexture.Value.Bounds, drawColor, chainRotation, _chainTexture.Size() * 0.5f,
                1f, SpriteEffects.None);
        }

        // Stop vanilla from drawing the default chain.
        return false;
    }
}