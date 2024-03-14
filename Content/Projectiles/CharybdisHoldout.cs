using DartGunsPlus.Content.Items.Weapons;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class CharybdisHoldout : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = Main.projFrames[ProjectileID.LaserMachinegun];
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.LaserMachinegun);
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.hide = true;
    }

    public override void AI()
    {
        Projectile.ai[0] += 1f;
        int numShots = 0;

        if (Projectile.ai[0] >= 40f) numShots++;
        if (Projectile.ai[0] >= 80f) numShots++;
        if (Projectile.ai[0] >= 120f) numShots++;

        const int delayTime = 24;
        const int additionalDelay = 6;
        Projectile.ai[1] += 1f;
        bool shouldFire = false;

        if (Projectile.ai[1] >= delayTime - additionalDelay * numShots)
        {
            Projectile.ai[1] = 0f;
            shouldFire = true;
        }

        Projectile.frameCounter += 1 + numShots;

        if (Projectile.frameCounter >= 4)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= 6)
                Projectile.frame = 0;
        }

        if (Projectile.soundDelay <= 0)
        {
            Projectile.soundDelay = delayTime - additionalDelay * numShots;

            if (Projectile.ai[0] != 1f)
            {
                SoundStyle returnSound = AudioSystem.ReturnSound("dart", 0.3f);
                SoundEngine.PlaySound(in returnSound, Projectile.position);
            }
        }

        if (Projectile.ai[1] == 1f && Projectile.ai[0] != 1f)
        {
            Vector2 spinningPoint = Vector2.UnitX * 24f;
            spinningPoint = spinningPoint.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
            Vector2 dustPosition = Projectile.Center + spinningPoint;

            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(dustPosition - Vector2.One * 8f, 16, 16, DustID.IceTorch, Projectile.velocity.X / 2f,
                    Projectile.velocity.Y / 2f, 100);
                dust.velocity *= 0.66f;
                dust.noGravity = true;
                dust.scale = 1.4f;
            }
        }

        Vector2 mountOffset = Owner.RotatedRelativePoint(Owner.MountedCenter);

        float shootSpeed = Owner.HeldItem.shootSpeed * Projectile.scale;
        Vector2 targetPosition = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY) - mountOffset;

        if (Owner.gravDir == -1f)
            targetPosition.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - mountOffset.Y;

        Vector2 shootDirection = Vector2.Normalize(targetPosition);

        if (float.IsNaN(shootDirection.X) || float.IsNaN(shootDirection.Y))
            shootDirection = -Vector2.UnitY;

        shootDirection *= shootSpeed;

        if (shootDirection.X != Projectile.velocity.X || shootDirection.Y != Projectile.velocity.Y)
            Projectile.netUpdate = true;

        Projectile.velocity = shootDirection;

        if (shouldFire && Main.myPlayer == Projectile.owner && Owner.HeldItem.ModItem.Type == ModContent.ItemType<Charybdis>())
        {
            Projectile.hide = false;

            if (!Owner.noItems && !Owner.CCed && Owner.channel && Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage,
                    out float knockback, out int ammoItemId, true))
            {
                // Resharper disable All
                int projType = projToShoot;
                if (Main.rand.NextBool(3))
                    projType = ModContent.ProjectileType<VortexBeam>(); // Resharper restore All

                const int numProjectiles = 7;

                for (int j = 0; j < 2; j++)
                {
                    Vector2 projectilePosition = Projectile.Center + new Vector2(Main.rand.Next(-numProjectiles, numProjectiles + 1),
                        Main.rand.Next(-numProjectiles, numProjectiles + 1));
                    Vector2 projectileVelocity = Vector2.Normalize(Projectile.velocity) * speed;

                    projectileVelocity = projectileVelocity.RotatedBy(Main.rand.NextDouble() * 0.19634954631328583 - 0.09817477315664291);

                    Projectile.NewProjectile(Owner.GetSource_ItemUse_WithPotentialAmmo(Owner.HeldItem, ammoItemId), projectilePosition, projectileVelocity, projType, damage,
                        knockback, Projectile.owner);

                    if (Main.rand.NextBool(1, 5))
                        Owner.PickAmmo(Owner.HeldItem, out int _, out float _, out int _, out float _,
                            out int _);
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
    }
}