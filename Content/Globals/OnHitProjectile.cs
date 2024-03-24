using System;
using System.Collections.Generic;
using System.Linq;
using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Items.Weapons;
using DartGunsPlus.Content.Items.Weapons.Styx;
using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using DartGunsPlus.Content.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DartGunsPlus.Content.Globals;

public class OnHitProjectile : GlobalProjectile
{
    private readonly int[] _shotguns =
    {
        ModContent.ItemType<DartStorm>(), ModContent.ItemType<CherryBlossom>(), ModContent.ItemType<GoreNGlory>(),
        ModContent.ItemType<OnyxStorm>(), ModContent.ItemType<Scatterhook>(), ModContent.ItemType<Scylla>()
    };

    private int _itemType;

    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse_WithAmmo use && ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart) _itemType = use.Item.type;
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (_shotguns.Contains(_itemType) && projectile.type != ModContent.ProjectileType<GoreHoldout>())
        {
            SoundEngine.PlaySound(AudioSystem.ReturnSound("hit", 0.4f), projectile.Center);

            int randDust = Main.rand.Next(20, 30);
            for (int i = 0; i < randDust; i++)
            {
                Dust dust = Dust.NewDustDirect(target.Center, 0, 0, DustID.Blood, 0f, 0f, 100, default, 0.8f);
                dust.velocity *= 1.6f;
                dust.velocity.Y -= 1f;
                dust.velocity += projectile.velocity;
                dust.noGravity = true;
            }
        }

        if (_itemType == ModContent.ItemType<DartZapper>())
            if (Main.rand.NextBool(6))
            {
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<ZapperLightning>(),
                    damageDone * 3, hit.Knockback, projectile.owner);
                SoundEngine.PlaySound(SoundID.MaxMana, target.Center);
            }

        if (_itemType == ModContent.ItemType<Blazebringer>())
        {
            target.AddBuff(BuffID.OnFire, 180);

            if (Main.rand.NextBool(3))
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(),
                    damageDone / 2, hit.Knockback, projectile.owner);
        }

        else if (_itemType == ModContent.ItemType<VerdantStalker>() && projectile.type != ProjectileID.BladeOfGrass)
        {
            target.AddBuff(BuffID.Poisoned, 60);

            if (Main.rand.NextBool(2))
            {
                Vector2 spawnPos = target.Center + new Vector2(Main.rand.Next(100, 200), 0).RotatedByRandom(Math.Tau);
                Vector2 velocity = spawnPos.DirectionTo(target.Center) * 8;
                
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, velocity, ModContent.ProjectileType<PlanteraSeed>(), 
                    (int)(damageDone * 0.75f), hit.Knockback / 2, projectile.owner);
                VisualSystem.SpawnDustPortal(spawnPos, velocity, DustID.GreenFairy, 0.8f);
            }
        }

        else if (_itemType == ModContent.ItemType<Katanakaze>())
        {
            Player owner = Main.player[projectile.owner];

            Vector2 randomPointWithinHitbox = Main.rand.NextVector2FromRectangle(target.Hitbox);
            Vector2 hitboxCenter = target.Hitbox.Center.ToVector2();
            Vector2 directionVector = (hitboxCenter - randomPointWithinHitbox).SafeNormalize(new Vector2(owner.direction, owner.gravDir)) * 8f;

            float randomRotationAngle = (Main.rand.Next(2) * 2 - 1) * ((float)Math.PI / 5f + (float)Math.PI * 4f / 5f * Main.rand.NextFloat());
            randomRotationAngle *= 0.5f;

            directionVector = directionVector.RotatedBy(0.7853981852531433);

            const int rotationSteps = 30;
            const int numIterations = 15;

            Vector2 currentPosition = hitboxCenter;

            for (int i = 0; i < numIterations; i++)
            {
                currentPosition -= directionVector;
                directionVector = directionVector.RotatedBy((0f - randomRotationAngle) / rotationSteps);
            }

            currentPosition += target.velocity * 3;

            Projectile.NewProjectile(projectile.GetSource_OnHit(target), currentPosition, directionVector, 977, projectile.damage / 2, 0f,
                projectile.owner, randomRotationAngle);
        }

        else if (_itemType == ModContent.ItemType<VioletVigilante>())
        {
            Vector2 velocity = new(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3));

            if (Main.rand.NextBool(4))
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, velocity, ProjectileID.LightsBane, damageDone,
                    hit.Knockback, projectile.owner, 1);
        }

        else if (_itemType == ModContent.ItemType<CrimsonCobra>())
        {
            if (Main.rand.NextBool(2))
            {
                Player player = Main.player[projectile.owner];
                target.AddBuff(BuffID.BloodButcherer, 540);
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ProjectileID.BloodButcherer, 0,
                    0, projectile.owner, 1, target.whoAmI);

                Vector2 pointPosition = target.Center;
                Vector2 alternatePoint = target.Center;

                player.LimitPointToPlayerReachableArea(ref pointPosition);
                Vector2 targetSpot = pointPosition + Main.rand.NextVector2Circular(8f, 8f);
                Vector2 spawnPos = DartUtils.FindSharpTearsSpot(targetSpot, player, alternatePoint).ToWorldCoordinates(Main.rand.Next(17),
                    Main.rand.Next(17));
                Vector2 velocity = (targetSpot - spawnPos).SafeNormalize(-Vector2.UnitY) * 16f;
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, velocity, ProjectileID.SharpTears,
                    projectile.damage / 4, 5, projectile.owner, ai1: Main.rand.NextFloat() * 0.5f + 0.6f);
            }

            CameraSystem.Screenshake(4, 4);

            for (int i = 0; i < 75; i++)
                Dust.NewDust(target.position, target.width, target.height, DustID.Blood, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
        }

        else if (_itemType == ModContent.ItemType<Dysphoria>() && projectile.type != ModContent.ProjectileType<DysphoriaBolt>())
        {
            VisualSystem.SpawnDustCircle(target.Center, ModContent.DustType<GlowFastDecelerate>(), 5, color: new Color(185, 133, 240), scale: 0.6f);

            if (Main.rand.NextBool(3))
                for (int i = 0; i < 4; i++)
                {
                    Vector2 position = target.Center + new Vector2(200, 200).RotatedByRandom(Math.Tau);
                    Vector2 velocity = position.DirectionTo(target.Center) * 6;

                    Projectile.NewProjectile(projectile.GetSource_OnHit(target), position, velocity, ModContent.ProjectileType<DysphoriaBolt>(),
                        projectile.damage, 3);

                    Dust.NewDust(position, projectile.height, projectile.width, DustID.WhiteTorch, newColor: new Color(128, 104, 207), Scale: 0.9f);
                }
        }

        else if (_itemType == ModContent.ItemType<TrueDysphoria>() && projectile.type != ModContent.ProjectileType<DysphoriaBolt>()
                                                                   && projectile.type != ModContent.ProjectileType<DysphoriaBoom>()
                                                                   && projectile.type != ModContent.ProjectileType<DysphoriaExplosion>())
        {
            bool purple = projectile.ai[2] % 2 == 0;
            Color color = purple ? new Color(185, 133, 240) : new Color(55, 224, 112);

            if (Main.rand.NextBool(2))
                for (int i = 0; i < 4; i++)
                {
                    VisualSystem.SpawnDustCircle(target.Center, ModContent.DustType<GlowFastDecelerate>(), 5, color: color, scale: 0.6f);

                    Vector2 position = target.Center + new Vector2(200, 200).RotatedByRandom(Math.Tau);
                    Vector2 velocity = position.DirectionTo(target.Center) * 6;
                    int projType = Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<DysphoriaMagnet>()] < 1
                        ? ModContent.ProjectileType<DysphoriaBolt>()
                        : ModContent.ProjectileType<DysphoriaNail>();

                    Projectile.NewProjectile(projectile.GetSource_OnHit(target), position, velocity, projType,
                        projectile.damage / 2, 3, projectile.owner, 1);

                    Dust.NewDust(position, projectile.height, projectile.width, DustID.WhiteTorch, newColor: new Color(128, 104, 207), Scale: 0.9f);
                }
        }

        else if (_itemType == ModContent.ItemType<RosemaryThyme>())
        {
            if (Main.rand.NextBool(7))
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<PlanteraShooter>(),
                    damageDone, 0, projectile.owner, Main.rand.NextFloat(0, MathF.Tau), ai2: damageDone);
        }

        else if (_itemType == ModContent.ItemType<Scatterhook>())
        {
            Player player = Main.player[projectile.owner];
            if (Main.rand.NextBool(7) && player.ownedProjectileCounts[ModContent.ProjectileType<GolemFist>()] < 16)
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<GolemFist>(),
                    damageDone, 0, projectile.owner, Main.rand.NextFloat(0, MathF.Tau));
        }

        else if (_itemType == ModContent.ItemType<Euphoria>())
        {
            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = target.Center,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)projectile.owner
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.TrueExcalibur, settings);

            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPos = target.Center + new Vector2(120, 0).RotatedByRandom(Math.Tau);
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, spawnPos.DirectionTo(target.Center) * 12,
                    ModContent.ProjectileType<EuphoriaSlash>(), (int)(damageDone * 0.75f), 3, projectile.owner);
            }
        }

        else if (_itemType == ModContent.ItemType<TrueEuphoria>())
        {
            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = target.Center,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)projectile.owner
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.TrueExcalibur, settings);

            if (Main.rand.NextBool(2))
            {
                Vector2 spawnPos = target.Center + new Vector2(120, 0).RotatedByRandom(Math.Tau);
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, spawnPos.DirectionTo(target.Center) * 12,
                    ModContent.ProjectileType<EuphoriaSlash>(), damageDone, 3, projectile.owner);
            }

            Player owner = Main.player[projectile.owner];
            if (owner.ownedProjectileCounts[ModContent.ProjectileType<RevolvingSword>()] != 0)
            {
                var rays = new List<Projectile>();
                foreach (Projectile proj in Main.projectile.AsSpan(0, Main.maxProjectiles))
                    if (proj.active && proj.type == ModContent.ProjectileType<EuphoriaRay>() && proj.owner == projectile.owner && proj.localAI[1] == target.whoAmI)
                    {
                        if (proj.ai[2] == 0)
                            rays.Add(proj);

                        if (proj.ai[1] < 160)
                            proj.ai[1] += 10; // size
                    }

                Projectile targetProj = Main.rand.NextFromCollection(rays);
                targetProj.ai[2] = 1; // expand

                foreach (Projectile proj in Main.projectile.AsSpan(0, Main.maxProjectiles))
                    if (proj.active && proj.type == ModContent.ProjectileType<RevolvingSword>() && proj.owner == projectile.owner && proj.ai[2] == target.whoAmI)
                        proj.ai[1] += 5; // rot dist
            }
        }

        else if (_itemType == ModContent.ItemType<StellarFrenzy>())
        {
            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = target.Center,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)projectile.owner
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StellarTune, settings);

            Vector2 spawnPos = target.Center - new Vector2(Main.rand.Next(-500, 500), 900);
            Vector2 velocity = spawnPos.DirectionTo(target.Center) * 10;

            if (Main.rand.NextBool(3))
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, velocity, ProjectileID.Starfury, damageDone, 3,
                    projectile.owner);
        }

        else if (_itemType == ModContent.ItemType<MartianMarksman>() && projectile.type != ModContent.ProjectileType<MartianLightning>())
        {
            MartianPlayer martianPlayer = Main.player[projectile.owner].GetModPlayer<MartianPlayer>();

            if (martianPlayer.MartianCurrent < 10)
                martianPlayer.MartianCurrent++;
        }

        else if (_itemType == ModContent.ItemType<Gigakraken>() && projectile.type != ModContent.ProjectileType<KrakenFish>())
        {
            Vector2 spawnPos = target.Center - new Vector2(Main.rand.Next(-50, 50), 200);

            if (Main.rand.NextBool(6))
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, spawnPos.DirectionTo(target.Center) * 12,
                    ModContent.ProjectileType<KrakenFish>(), damageDone * 2, 6, projectile.owner, Main.rand.Next(1, 27));
        }

        else if (_itemType == ModContent.ItemType<GlacialGeyser>() && projectile.type != ModContent.ProjectileType<Icicle>() &&
                 projectile.type != ModContent.ProjectileType<FreezeBolt>())
        {
            if (Main.rand.NextBool(2))
            {
                Player owner = Main.player[projectile.owner];
                Vector2 pointPosition = target.Center; // this is the target

                owner.LimitPointToPlayerReachableArea(ref pointPosition);
                Vector2 targetSpot = pointPosition + Main.rand.NextVector2Circular(8f, 8f);
                Vector2 spawnPos = DartUtils.FindSharpTearsSpot(targetSpot, owner, target.Center).ToWorldCoordinates(Main.rand.Next(17),
                    Main.rand.Next(17));
                Vector2 velocity = (targetSpot - spawnPos).SafeNormalize(-Vector2.UnitY) * 16f;
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, velocity, ModContent.ProjectileType<Icicle>(),
                    damageDone * 3 / 4, 5, projectile.owner, ai1: Main.rand.NextFloat() * 0.5f + 0.6f);
            }

            FrostPlayer frostPlayer = Main.player[projectile.owner].GetModPlayer<FrostPlayer>();

            if (frostPlayer.FrostCurrent < 10)
                frostPlayer.FrostCurrent++;
        }

        else if (_itemType == ModContent.ItemType<HalloweenHex>() && projectile.type != ModContent.ProjectileType<HalloweenJack>()
                                                                  && projectile.type != ModContent.ProjectileType<HalloweenRitual>() &&
                                                                  projectile.type != ModContent.ProjectileType<Explosion>())
        {
            HalloweenPlayer halloweenPlayer = Main.player[projectile.owner].GetModPlayer<HalloweenPlayer>();

            if (halloweenPlayer.HalloweenCurrent < 15)
                halloweenPlayer.HalloweenCurrent++;

            switch (halloweenPlayer.HalloweenCurrent)
            {
                case 5:
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, projectile.Center);
                    Projectile.NewProjectile(projectile.GetSource_OnHit(target), projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<HalloweenRitual>(), projectile.damage / 2, 1, projectile.owner);
                    break;

                case 10:
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack, projectile.Center);
                    for (int i = 0; i < 5; i++)
                        Projectile.NewProjectile(projectile.GetSource_OnHit(target), projectile.Center, Vector2.Zero,
                            ModContent.ProjectileType<HalloweenJack>(), projectile.damage, 4, projectile.owner, ai2: MathHelper.ToRadians(72 * i));
                    break;

                case 15:
                    SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, projectile.Center);
                    foreach (Projectile proj in Main.projectile.AsSpan(0, Main.maxProjectiles))
                    {
                        if (proj.type == ModContent.ProjectileType<HalloweenJack>() && proj.owner == Main.myPlayer && proj.active)
                        {
                            proj.ai[1] = 1; // enables to move
                            proj.velocity = proj.DirectionTo(Main.MouseWorld) * 12;
                        }

                        if (proj.type == ModContent.ProjectileType<HalloweenRitual>() && proj.owner == Main.myPlayer && proj.active)
                            proj.Kill();
                    }

                    halloweenPlayer.HalloweenCurrent = 0;
                    break;
            }
        }

        else if (_itemType == ModContent.ItemType<Serenity>() && projectile.type != ModContent.ProjectileType<SereneBolt>() && 
                 projectile.type != ModContent.ProjectileType<TerraBoom>())
        {
            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = target.Center,
                MovementVector = Vector2.One.RotatedByRandom(Math.Tau),
                IndexOfPlayerWhoInvokedThis = (byte)projectile.owner,
                UniqueInfoPiece = (int)VisualSystem.HueForParticle(Color.Green)
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.ChlorophyteLeafCrystalShot, settings);
            
            Player owner = Main.player[projectile.owner];

            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 spawnPos = owner.Center - owner.Center.DirectionTo(Main.MouseWorld) * 100;
                    Vector2 velocity = spawnPos.DirectionTo(Main.MouseWorld).RotatedByRandom(MathHelper.ToRadians(30)) * 7;
                    Dust.NewDust(spawnPos, projectile.width, projectile.height, ModContent.DustType<GlowFastDecelerate>(), Scale: 0.6f, newColor: Color.LawnGreen);
            
                    Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, velocity,
                        ModContent.ProjectileType<SereneBolt>(), projectile.damage / 3, 3, projectile.owner);
                }
            }
        }

        else if (_itemType == ModContent.ItemType<Luminescence>() && projectile.type != ModContent.ProjectileType<EmpressLaser>()
                                                                  && projectile.type != ModContent.ProjectileType<EmpressBolt>() &&
                                                                  projectile.type != ModContent.ProjectileType<LilEmpress>())
        {
            if (Main.rand.NextBool(2))
            {
                Vector2 spawnPos = target.Center + new Vector2(500, 0).RotatedByRandom(Math.Tau);
                Vector2 velocity = spawnPos.DirectionTo(target.Center) * 10;

                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, velocity,
                    ModContent.ProjectileType<EmpressBolt>(), projectile.damage, 3, projectile.owner);
            }

            if (Main.rand.NextBool(8))
            {
                Projectile minion = Projectile.NewProjectileDirect(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero,
                    ModContent.ProjectileType<LilEmpress>(), projectile.damage * 2, 3, projectile.owner);
                SoundEngine.PlaySound(SoundID.Shimmer1, minion.Center);
            }
        }

        else if (_itemType == ModContent.ItemType<CrownCascade>())
        {
            if (target.life <= 0 || !target.active)
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<DukeWhirlpool>(),
                    0, 0, projectile.owner);
        }

        else if (_itemType == ModContent.ItemType<LaserTherapy>() && projectile.type != ModContent.ProjectileType<LaserLightning>()
                                                                  && projectile.type != ModContent.ProjectileType<HomingLightning>())
        {
            LaserPlayer martianPlayer = Main.player[projectile.owner].GetModPlayer<LaserPlayer>();

            if (martianPlayer.LaserCurrent < 10)
                martianPlayer.LaserCurrent++;
        }

        else if (_itemType == ModContent.ItemType<Sporeflinger>() && projectile.type != ModContent.ProjectileType<BouncingShroom>())
        {
            if (Main.rand.NextBool(4))
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPos = target.Center + new Vector2(100, 100).RotatedBy(i * 90);
                    Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, spawnPos.DirectionTo(target.Center) * 5,
                        ModContent.ProjectileType<BouncingShroom>(), projectile.damage / 4, 3, projectile.owner);
                }
            }
        }

        else if (_itemType == ModContent.ItemType<GoreNGlory>())
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDust(target.position, target.width, target.height, DustID.Blood, Main.rand.NextFloat(projectile.direction * 1, projectile.direction * 3),
                    Main.rand.NextFloat(-3f, 3f));

            CameraSystem.Screenshake(5, 6);
        }

        else if (_itemType == ModContent.ItemType<StellarOutrage>())
        {
            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = target.Center,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)projectile.owner
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.StellarTune, settings);

            if (projectile.type != ModContent.ProjectileType<StellarSlash>())
            {
                Vector2 spawnPos = target.Center - new Vector2(Main.rand.Next(-500, 500), 500);
                Vector2 velocity = spawnPos.DirectionTo(target.Center) * 14;

                SoundEngine.PlaySound(SoundID.DD2_WitherBeastAuraPulse, target.Center);
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), spawnPos, velocity, ModContent.ProjectileType<LargeStar>(), damageDone,
                    3, projectile.owner);
            }
        }

        else if (_itemType == ModContent.ItemType<Styx>())
        {
            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, target.Center);
            Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<BlackHole>(), damageDone,
                3, projectile.owner, target.whoAmI);

            if (target.CanBeChasedBy())
                target.velocity = target.DirectionTo(Main.player[projectile.owner].Center) * 5;
        }
    }
}