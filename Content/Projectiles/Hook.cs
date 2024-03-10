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
    private enum TargetIndicator
    {
        Retract = -1,
        Default = 0,
        Tile = 1,
        Target = 2
    }
    
    private Vector2 _startLocation;
    private Player Owner => Main.player[Projectile.owner];
    private ref float TargetNPC => ref Projectile.ai[0];
    private bool HitTarget => Projectile.ai[2] > (int)TargetIndicator.Default;
    
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
        Projectile.height = 22;
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
        _startLocation = Owner.MountedCenter + new Vector2((Owner.HeldItem.width * 0.8f + Owner.MountXOffset) * Owner.direction, 0).RotatedBy(Owner.itemRotation);
        Projectile.rotation = Owner.DirectionTo(Projectile.Center).ToRotation();

        float offset = Owner.direction == 1 ? 0 : MathF.PI;
        Owner.itemRotation = Projectile.rotation + offset;
        Projectile.direction = Projectile.Center.X > Owner.Center.X ? 1 : -1;
        Owner.ChangeDir(Projectile.direction);
        Lighting.AddLight(Projectile.Center, Color.Turquoise.ToVector3());

        if (Main.mouseRight && Main.mouseRightRelease && Projectile.timeLeft < 595 || Owner.Center.Distance(Projectile.Center) > 800)
            Projectile.ai[2] = (int)TargetIndicator.Retract;

        if (Projectile.ai[2] != (int)TargetIndicator.Default)
        {
            Projectile.friendly = false;

            if (Owner.Center.Distance(Projectile.Center) < 100)
            {
                Projectile.timeLeft = 1;

                if (Projectile.ai[2] == (int)TargetIndicator.Target)
                {
                    Owner.velocity = Vector2.Zero;
                    NPC target = Main.npc[(int)TargetNPC];
                    
                    Projectile.NewProjectile(Owner.GetSource_ItemUse(Owner.HeldItem), Owner.Center, Vector2.Zero, ModContent.ProjectileType<ScyllaShoot>(),
                        0, 0, Owner.whoAmI, target.whoAmI);
                }
            }
        }

        if (Projectile.ai[2] == (int)TargetIndicator.Retract)
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(Projectile.velocity) * 8, Projectile.DirectionTo(Owner.Center) * 16,
                0.6f);
            Projectile.velocity *= 1.1f;
        }

        
        if (HitTarget)
        {
            if (Projectile.ai[2] == (int)TargetIndicator.Target)
            {
                NPC target = Main.npc[(int)TargetNPC];
                Projectile.Center = target.Center;

                if (!target.active)
                    Projectile.ai[2] = (int)TargetIndicator.Retract;

                if (target.width + target.height < 130 && target.CanBeChasedBy()) // threshold for large enemy
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(Projectile.velocity) * 8, Projectile.DirectionTo(Owner.Center) * 16,
                        0.6f);
                    
                    target.velocity = Vector2.Lerp(target.velocity.SafeNormalize(target.velocity) * 8, target.DirectionTo(Owner.Center) * 16,
                        0.6f);
                }
                else
                    Owner.velocity = Vector2.Lerp(Owner.velocity.SafeNormalize(Owner.velocity) * 7, Owner.DirectionTo(Projectile.Center) * 14, 0.7f);
            }
            

            if (Projectile.ai[2] == (int)TargetIndicator.Tile)
            {
                Owner.velocity = Vector2.Lerp(Owner.velocity.SafeNormalize(Owner.velocity) * 7, Owner.DirectionTo(Projectile.Center) * 14, 0.7f);
                Projectile.velocity = Vector2.Zero;
            }
            
            const int moveAmount = 2;

            if (Owner.controlLeft)
                Owner.velocity.X -= moveAmount;
            else if (Owner.controlRight)
                Owner.velocity.X += moveAmount;
            else if (Owner.controlUp || Owner.controlJump)
                Owner.velocity.Y -= moveAmount;
            else if (Owner.controlDown)
                Owner.velocity.Y += moveAmount;
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
        Projectile.ai[2] = (int)TargetIndicator.Target;
        TargetNPC = target.whoAmI;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.ai[2] = (int)TargetIndicator.Tile;
        return false;
    }
}