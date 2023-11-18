using System;
using DartGunsPlus.Content.Items.Weapons;
using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Globals;

public class OnHitProjectile : GlobalProjectile
{
    private int ItemType;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse_WithAmmo use && ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart) ItemType = use.Item.type;
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (ItemType == ModContent.ItemType<Blazebringer>())
        {
            target.AddBuff(BuffID.OnFire, 180);

            if (Main.rand.NextBool(3))
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ProjectileID.Volcano, damageDone / 2,
                    hit.Knockback, projectile.owner);
        }

        else if (ItemType == ModContent.ItemType<VerdantStalker>() && projectile.type != ProjectileID.BladeOfGrass)
        {
            target.AddBuff(BuffID.Poisoned, 60);
            Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center - new Vector2(20, 0), Vector2.Zero,
                ProjectileID.BladeOfGrass, (int)(damageDone * 0.75f), hit.Knockback / 2, projectile.owner, 50);
        }

        else if (ItemType == ModContent.ItemType<Katanakaze>())
        {
            var velocity = new Vector2(Main.rand.Next(-3, 4), Main.rand.Next(-3, 4));
            Projectile.NewProjectile(projectile.GetSource_Death(), target.Center, velocity, ProjectileID.Muramasa, damageDone / 2,
                hit.Knockback / 2, projectile.owner, 1);
        }

        else if (ItemType == ModContent.ItemType<VioletVigilante>())
        {
            var velocity = new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3));

            if (Main.rand.NextBool(4))
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, velocity, ProjectileID.LightsBane, damageDone / 2,
                    hit.Knockback, projectile.owner, 1);
        }

        else if (ItemType == ModContent.ItemType<CrimsonCobra>())
        {
            if (Main.rand.NextBool(2))
            {
                target.AddBuff(BuffID.BloodButcherer, 540);
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ProjectileID.BloodButcherer, 0,
                    0, projectile.owner, 1, target.whoAmI);
            }

            CameraSystem.Screenshake(4, 4);
            var numParticles = 75;

            for (var i = 0; i < numParticles; i++)
                Dust.NewDust(target.position, target.width, target.height, DustID.Blood, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
        }

        else if (ItemType == ModContent.ItemType<MidnightMarauder>() && projectile.type != ModContent.ProjectileType<MidnightBolt>())
        {
            CameraSystem.Screenshake(3, 3);
            VisualSystem.SpawnDustCircle(target.Center, DustID.Shadowflame, 30);
            int[] angles = { 45, 135, 225, 315 };

            if (Main.rand.NextBool(3))
                foreach (var angle in angles)
                {
                    var spawnPos = target.Center + new Vector2(150f, 0f).RotatedBy(MathHelper.ToRadians(angle));
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), spawnPos, spawnPos.DirectionTo(target.Center) * 10,
                        ModContent.ProjectileType<MidnightBolt>(), damageDone / 2, hit.Knockback, projectile.owner);
                }
        }

        else if (ItemType == ModContent.ItemType<RosemaryThyme>())
        {
            if (Main.rand.NextBool(7))
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<PlanteraShooter>(),
                    damageDone, 0, projectile.owner, Main.rand.NextFloat(0, MathF.Tau), ai2: damageDone);
        }

        else if (ItemType == ModContent.ItemType<Euphoria>())
        {
            var settings = new ParticleOrchestraSettings
            {
                PositionInWorld = target.Center,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)projectile.owner
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.TrueExcalibur, settings);

            if (Main.rand.NextBool(3))
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center - new Vector2(0, 130), Vector2.Zero,
                    ModContent.ProjectileType<EuphoriaSword>(), damageDone + damageDone / 2, 3, projectile.owner);
        }

        else if (ItemType == ModContent.ItemType<TrueEuphoria>())
        {
            var settings = new ParticleOrchestraSettings
            {
                PositionInWorld = target.Center,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)projectile.owner
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.TrueExcalibur, settings);

            if (Main.rand.NextBool(3))
            {
                var spawnPos = target.Center + new Vector2(120, 0).RotatedByRandom(Math.Tau);
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, spawnPos.DirectionTo(target.Center) * 9,
                    ModContent.ProjectileType<TrueEuphoriaSword>(), (int)(damageDone * 0.75f), 3, projectile.owner);
            }
        }

        else if (ItemType == ModContent.ItemType<StellarFrenzy>())
        {
            var settings = new ParticleOrchestraSettings
            {
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)projectile.owner
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StellarTune, settings);

            var spawnPos = target.Center - new Vector2(Main.rand.Next(-500, 500), 900);
            var velocity = spawnPos.DirectionTo(target.Center) * 10;

            if (Main.rand.NextBool(3))
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, velocity, ProjectileID.Starfury, damageDone, 3, projectile.owner);
        }
    }
}