using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class Hook : ModProjectile
{
    private SlotId _soundSlot;
    private Vector2 _startLocation;
    private Player Owner => Main.player[Projectile.owner];
    private ref float TargetNPC => ref Projectile.ai[0];
    private bool HitTarget => Projectile.ai[2] > (int)TargetIndicator.Default;


    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.SingleGrappleHook[Projectile.type] = true;
        ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 900;
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
        _soundSlot = SoundEngine.PlaySound(AudioSystem.ReturnSound("grapplethrow", 0.3f), Owner.Center);
    }

    public override void AI()
    {
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        _startLocation = Owner.MountedCenter + new Vector2((Owner.HeldItem.width * 0.8f + Owner.MountXOffset) * Owner.direction, -5).RotatedBy(Owner.itemRotation);
        Projectile.rotation = Owner.DirectionTo(Projectile.Center).ToRotation();

        float offset = Owner.direction == 1 ? 0 : MathF.PI;
        Owner.itemRotation = Projectile.rotation + offset;
        Projectile.direction = Projectile.Center.X > Owner.Center.X ? 1 : -1;
        Owner.ChangeDir(Projectile.direction);
        Lighting.AddLight(Projectile.Center, Color.SeaGreen.ToVector3());

        if ((Main.mouseRight && Main.mouseRightRelease && Projectile.timeLeft < 595) || Owner.Center.Distance(Projectile.Center) > 800)
            Projectile.ai[2] = (int)TargetIndicator.Retract;

        if (Projectile.ai[2] != (int)TargetIndicator.Default)
        {
            Projectile.friendly = false;

            if (Owner.Center.Distance(Projectile.Center) < 100)
            {
                SoundEngine.PlaySound(AudioSystem.ReturnSound("grapplefinish", 0.4f), Owner.Center);
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
            Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(Projectile.velocity) * 12, Projectile.DirectionTo(Owner.Center) * 20,
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

                if (target.width + target.height > 130 || !target.CanBeChasedBy()) // threshold for large enemy
                {
                    MoveOwner();
                }
                else
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(Projectile.velocity) * 8, Projectile.DirectionTo(Owner.Center) * 16,
                        0.6f);

                    target.velocity = Vector2.Lerp(target.velocity.SafeNormalize(target.velocity) * 8, target.DirectionTo(Owner.Center) * 16,
                        0.6f);
                }
            }

            if (Projectile.ai[2] == (int)TargetIndicator.Tile)
                MoveOwner();
        }

        return;

        void MoveOwner()
        {
            Owner.velocity = Vector2.Lerp(Owner.velocity.SafeNormalize(Owner.velocity) * 7, Owner.DirectionTo(Projectile.Center) * 14, 0.7f);
            Projectile.velocity = Vector2.Zero;

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
        Texture2D glowTexture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Glowball").Value;
        Color color = new(0, 240, 136, 0);

        VisualSystem.DrawLine(Main.spriteBatch, glowTexture, Projectile.Center, _startLocation,
            color * Projectile.Opacity, 0.02f, heightOverloadMult: 0.4f);
        VisualSystem.DrawLine(Main.spriteBatch, chainTexture, Projectile.Center, _startLocation, color * Projectile.Opacity, 0.9f);

        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.ai[2] = (int)TargetIndicator.Target;
        TargetNPC = target.whoAmI;
        ManageTargetSounds();
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.ai[2] = (int)TargetIndicator.Tile;
        ManageTargetSounds();
        return false;
    }

    private void ManageTargetSounds()
    {
        SoundEngine.TryGetActiveSound(_soundSlot, out ActiveSound activeSound);
        activeSound?.Stop();
        _soundSlot = SoundEngine.PlaySound(AudioSystem.ReturnSound("grapplereel"), Projectile.Center);
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        fallThrough = false;
        return true;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.TryGetActiveSound(_soundSlot, out ActiveSound activeSound);
        activeSound?.Stop();
    }

    private enum TargetIndicator
    {
        Retract = -1,
        Default = 0,
        Tile = 1,
        Target = 2
    }
}