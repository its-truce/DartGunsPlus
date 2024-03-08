using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class Hook : ModProjectile
{
    private Vector2 _startLocation;
    private Player Owner => Main.player[Projectile.owner];
    private ref float TargetNPC => ref Projectile.ai[0];
    private bool HitTarget => Projectile.ai[2] != 0;
    
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.SingleGrappleHook[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.width = 22;
        Projectile.height = 26;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 600;
    }

    public override void OnSpawn(IEntitySource source)
    {
        TargetNPC = -1;
    }

    public override void AI()
    {
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        _startLocation = Owner.MountedCenter + new Vector2(Owner.inventory[Owner.selectedItem].width/4 * Owner.direction, 0).RotatedBy
            (Owner.DirectionTo(Projectile.Center).ToRotation());
        Projectile.rotation = Owner.DirectionTo(Projectile.Center).ToRotation();

        float offset = Owner.direction == 1 ? 0 : MathF.PI;
        Owner.itemRotation = Projectile.rotation + offset;
        Projectile.direction = Projectile.Center.X > Owner.Center.X ? 1 : -1;
        Owner.ChangeDir(Projectile.direction);
        
        if (Main.mouseRight && Main.mouseRightRelease)
            Projectile.Kill();
        
        if (Owner.Center.Distance(Projectile.Center) > 600)
            Projectile.Kill();
        
        if (HitTarget)
        {
            if (Projectile.ai[2] == 2)
            {
                NPC target = Main.npc[(int)TargetNPC];
                Projectile.Center = target.Center;
            }
            
            Projectile.friendly = false;
            Projectile.velocity = Vector2.Zero;
            
            Owner.velocity = Vector2.Lerp(Owner.velocity.SafeNormalize(Owner.velocity) * 7, Owner.DirectionTo(Projectile.Center) * 14, 0.7f);
        
            if (Owner.Hitbox.Intersects(Projectile.Hitbox))
                Projectile.Kill();

            const int moveAmount = 2;

            if (Owner.controlLeft)
                Owner.position.X -= moveAmount;
            else if (Owner.controlRight)
                Owner.position.X += moveAmount;
            else if (Owner.controlUp || Owner.controlJump)
                Owner.position.Y -= moveAmount;
            else if (Owner.controlDown)
                Owner.position.Y += moveAmount;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D chainTexture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/HookChain").Value;
        Color color = Color.Turquoise;
        color.A = 0;
        VisualSystem.DrawLine(Main.spriteBatch, chainTexture, Projectile.Center, _startLocation, color * Projectile.Opacity);
        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.ai[2] = 2;
        TargetNPC = target.whoAmI;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.ai[2] = 1;
        return false;
    }
}