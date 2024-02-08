using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class RedBomb : ModProjectile
{
    private const int DefaultWidthHeight = 22;
    private const int ExplosionWidthHeight = 250;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = DefaultWidthHeight;
        Projectile.height = DefaultWidthHeight;
        Projectile.friendly = true;
        Projectile.penetrate = -1;

        Projectile.timeLeft = 180;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Main.expertMode)
            if (target.type >= NPCID.EaterofWorldsHead && target.type <= NPCID.EaterofWorldsTail)
                modifiers.FinalDamage /= 5;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.soundDelay == 0) SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
        Projectile.soundDelay = 30;

        // This code makes the projectile very bouncy.
        if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f) Projectile.velocity.X = oldVelocity.X * -0.9f;
        if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f) Projectile.velocity.Y = oldVelocity.Y * -0.9f;
        return false;
    }

    public override void AI()
    {
        if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            Projectile.Resize(ExplosionWidthHeight, ExplosionWidthHeight);

            Projectile.damage = 250;
            Projectile.knockBack = 10f;
        }
        else
        {
            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100);
                dust.scale = 0.1f + Main.rand.Next(5) * 0.1f;
                dust.fadeIn = 1.5f + Main.rand.Next(5) * 0.1f;
                dust.noGravity = true;
                dust.position = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation - 2.1f) * 10f;

                dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100);
                dust.scale = 1f + Main.rand.Next(5) * 0.1f;
                dust.noGravity = true;
                dust.position = Projectile.Center + new Vector2(1, 0).RotatedBy(Projectile.rotation - 2.1f) * 10f;
            }
        }

        Projectile.ai[0] += 1f;
        if (Projectile.ai[0] > 10f)
        {
            Projectile.ai[0] = 10f;
            if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.96f;

                if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01)
                {
                    Projectile.velocity.X = 0f;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
        }

        Projectile.rotation += Projectile.velocity.X * 0.1f;

        int frameSpeed = 4;

        Projectile.frameCounter++;

        if (Projectile.frameCounter >= frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= Main.projFrames[Projectile.type]) Projectile.frame = 0;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.timeLeft = 3;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        for (int i = 0; i < 50; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
            dust.velocity *= 1.4f;
        }

        for (int i = 0; i < 30; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
            dust.noGravity = true;
            dust.velocity *= 5f;
            dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
            dust.velocity *= 3f;
        }

        for (int g = 0; g < 2; g++)
        {
            Vector2 goreSpawnPosition = new(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
            Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64));
            gore.scale = 1.5f;
            gore.velocity.X += 1.5f;
            gore.velocity.Y += 1.5f;
            gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64));
            gore.scale = 1.5f;
            gore.velocity.X -= 1.5f;
            gore.velocity.Y += 1.5f;
            gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64));
            gore.scale = 1.5f;
            gore.velocity.X += 1.5f;
            gore.velocity.Y -= 1.5f;
            gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64));
            gore.scale = 1.5f;
            gore.velocity.X -= 1.5f;
            gore.velocity.Y -= 1.5f;
        }
    }
}