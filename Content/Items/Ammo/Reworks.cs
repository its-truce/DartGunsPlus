using System;
using DartGunsPlus.Content.Effects;
using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class Reworks : GlobalProjectile
{
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return lateInstantiation && entity.type == ProjectileID.CrystalDart;
    }

    public override void SetDefaults(Projectile entity)
    {
        entity.penetrate = 1;
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player owner = Main.player[projectile.owner];

        if (Main.rand.NextBool(3) && owner.ownedProjectileCounts[ModContent.ProjectileType<Crystals>()] < 3)
            Projectile.NewProjectile(projectile.GetSource_OnHit(target), projectile.Center, Vector2.Zero, ModContent.ProjectileType<Crystals>(), projectile.damage,
                0, projectile.owner, target.whoAmI, target.DirectionTo(projectile.Center).ToRotation());

        if (owner.ownedProjectileCounts[ModContent.ProjectileType<Crystals>()] >= 3)
        {
            int numCrystals = 0;

            foreach (Projectile proj in Main.projectile)
                if (proj.active && proj.owner == projectile.owner && proj.type == ModContent.ProjectileType<Crystals>() && proj.ai[0] == target.whoAmI)
                    numCrystals++;

            if (numCrystals >= 3)
            {
                SoundEngine.PlaySound(AudioSystem.ReturnSound("shatter", 0.4f), projectile.Center);
                CameraSystem.Screenshake(4, 6);
                VisualSystem.SpawnDustCircle(target.Center, DustID.RainbowRod, 30, color: new Color(200, 86, 135));

                Projectile.NewProjectile(projectile.GetSource_OnHit(target), projectile.Center, Vector2.Zero, ModContent.ProjectileType<CrystalShock>(),
                    projectile.damage * 2, 4, projectile.owner);
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShockwaveProjectile>(),
                    0, 0, projectile.owner);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 spawnPos = projectile.Center + new Vector2(65, 0).RotatedByRandom(MathF.Tau);
                    Projectile.NewProjectile(projectile.GetSource_Death(), spawnPos, spawnPos.DirectionTo(projectile.Center),
                        ModContent.ProjectileType<SmallTrail>(), 0, 0, projectile.owner, 200, 86, 135);
                }

                int randDust = Main.rand.Next(20, 30);
                for (int i = 0; i < randDust; i++)
                {
                    Dust dust = Dust.NewDustDirect(target.Center, 0, 0, DustID.RainbowRod, 0f, 0f, 100, Color.Pink, 0.8f);
                    dust.velocity *= 1.6f;
                    dust.velocity.Y -= 1f;
                    dust.velocity += projectile.velocity;
                    dust.noGravity = true;
                }

                foreach (Projectile proj in Main.projectile)
                    if (proj.active && proj.owner == projectile.owner && proj.type == ModContent.ProjectileType<Crystals>() && proj.ai[0] == target.whoAmI)
                    {
                        proj.Kill();

                        for (int i = 0; i < 5; i++)
                        {
                            Projectile shard = Projectile.NewProjectileDirect(proj.GetSource_Death(), proj.Center,
                                new Vector2(7, 2).RotatedBy(MathHelper.ToRadians(i * 72)), ProjectileID.QueenSlimeMinionBlueSpike, projectile.damage,
                                3, proj.owner);
                            shard.friendly = true;
                            shard.hostile = false;
                        }
                    }
            }
        }
    }
}