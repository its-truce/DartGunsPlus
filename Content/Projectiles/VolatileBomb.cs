using System;
using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class VolatileBomb : ModProjectile
{
    private const int ExplosionWidthHeight = 125;
    
    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.timeLeft = 300;
        Projectile.Opacity = 0;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 5;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.soundDelay == 0)
            SoundEngine.PlaySound(AudioSystem.ReturnSound("metal", 0.4f), Projectile.Center);
        Projectile.soundDelay = 30;

        // This code makes the projectile very bouncy.
        if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f) 
            Projectile.velocity.X = oldVelocity.X * -0.9f;
        if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
            Projectile.velocity.Y = oldVelocity.Y * -0.9f;
        
        return false;
    }

    public override void AI()
    {
        if (Projectile.timeLeft >= 280)
            FadingSystem.FadeIn(Projectile, 20);
        
        if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 5)
        {
            Projectile.tileCollide = false;
            FadingSystem.FadeOut(Projectile, 5);
            Projectile.Resize(ExplosionWidthHeight, ExplosionWidthHeight);

            Projectile.damage += Projectile.damage / 5;
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

        Projectile.ai[0]++;
        Projectile.ai[1]++; // draw
        
        if (Projectile.ai[0] > 10f)
        {
            Projectile.ai[0] = 10f;
            if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
            {
                Projectile.velocity.X *= 0.96f;

                if (Projectile.velocity.X > -0.01 && Projectile.velocity.X < 0.01)
                {
                    Projectile.velocity.X = 0f;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.velocity.Y += 0.2f;
        }

        Projectile.rotation += Projectile.velocity.X * 0.1f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {       
        int[] possibleProjs = { ModContent.ProjectileType<Probe>(), ModContent.ProjectileType<Retinazer>(), ModContent.ProjectileType<Spaz>() };
        int projToSpawn = possibleProjs[(int)Projectile.ai[2] - 1];

        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, projToSpawn, Projectile.damage, 3,
            Projectile.owner, target.whoAmI, ai2: Main.rand.NextFloat(MathF.Tau));
        
        Projectile.Kill();
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(AudioSystem.ReturnSound("mechaboom", volume: 0.6f));
        VisualSystem.SpawnDustCircle(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), scale: 0.6f, color: Color.Red);
    }
    
    public override void PostDraw(Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/VolatileFlash").Value;
        Color color = new(255, 50, 50, 0);
        
        if (Projectile.ai[1] % 5 == 0)
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size()/2, 
                Projectile.scale, SpriteEffects.None);
    }
}