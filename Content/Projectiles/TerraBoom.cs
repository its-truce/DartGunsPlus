using System;
using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class TerraBoom : ModProjectile
{
    private NPC Target => Main.npc[(int)Projectile.ai[0]];  
    private Player Owner => Main.player[Projectile.owner];
    private bool Sticking => Projectile.ai[1] != 0;
    private bool Retreat => Projectile.localAI[1] != 0;
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.tileCollide = true;
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 1200;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.extraUpdates = 1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 20;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        // this is all taken from jeo

        Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Extra_89").Value;
        Rectangle frame = texture.Frame();
        Vector2 frameOrigin = frame.Size() / 2f;

        Color col = Color.Lerp(new Color(34, 177, 77), Color.YellowGreen, Main.masterColor) * 0.4f;
        col.A = 0;
        Vector2 stretchscale = new(Projectile.scale * 1.4f + Main.masterColor / 2);

        for (int i = 1; i < Projectile.oldPos.Length - 1; i++)
        {
            Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(Projectile.width / 2);
            
            if (!Sticking || Retreat)
                Main.EntitySpriteDraw(texture, drawPos + Main.rand.NextVector2Circular(i / 2, i / 2), frame,
                    new Color(col.R + i * 2, col.G, col.B, col.A) * (1 - i * 0.04f), Projectile.oldRot[i] + Main.rand.NextFloat(-i * 0.01f, i * 0.01f),
                    frameOrigin, new Vector2(stretchscale.X - i * 0.05f, stretchscale.Y * Main.rand.NextFloat(0.1f, 0.05f) * Vector2.Distance(Projectile.oldPos[i],
                        Projectile.oldPos[i + 1]) - i * 0.05f) * new Vector2(0.4f, Sticking && Projectile.velocity.Length() < 7 ? 10f : 3f), SpriteEffects.None);
        }
        
        return false;
    }

    public override void OnSpawn(IEntitySource source)
    {
        for (int k = 0; k < Projectile.oldPos.Length; k++)
            Projectile.oldPos[k] = Projectile.position;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, Color.LawnGreen.ToVector3());
        
        if (Projectile.timeLeft % 30 == 0 && (!Sticking || Retreat))
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(0, 600), new Vector2(Main.rand.Next(-3, 3), 11),
                ModContent.ProjectileType<TerraBolt>(), Projectile.damage, 2, Projectile.owner);
        }
        
        if (Sticking)
        {
            if (!Target.active || Target.dontTakeDamage || Projectile.localAI[0] > 480)
            {
                Projectile.localAI[1] = 1; // retreat

                for (int i = 0; i < 4; i++)
                {
                    Vector2 spawnPos = Target.Center + new Vector2(200, 0).RotatedByRandom(Math.Tau);
                    Vector2 velocity = spawnPos.DirectionTo(Target.Center) * 8;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPos, velocity, ModContent.ProjectileType<TerraSlash>(), Projectile.damage,
                        3, Projectile.owner, ai1: 1, ai2: Target.whoAmI);
                }
            }
            else
            {
                Projectile.Center = Target.Center;
                Projectile.velocity = Target.velocity;
                Projectile.gfxOffY = Target.gfxOffY;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] % 60 == 0)
                {
                    Vector2 spawnPos = Target.Center + new Vector2(200, 0).RotatedByRandom(Math.Tau);
                    Vector2 velocity = spawnPos.DirectionTo(Target.Center) * 8;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPos, velocity, ModContent.ProjectileType<TerraSlash>(), Projectile.damage/2,
                        3, Projectile.owner);
                }

                if (Projectile.localAI[0] % 30 == 0)
                {
                    ParticleOrchestraSettings settings = new()
                    {
                        PositionInWorld = Target.Center,
                        MovementVector = Vector2.Zero,
                        IndexOfPlayerWhoInvokedThis = (byte)Projectile.owner,
                        UniqueInfoPiece = (byte)VisualSystem.HueForParticle(Color.YellowGreen)
                    };
                    ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.TrueNightsEdge, settings);
                }
            }
        }

        if (Retreat)
        {
            SoundEngine.PlaySound(AudioSystem.ReturnSound("indicator"), Owner.Center);
            Projectile.ai[0] = -1;
            Projectile.ai[1] = 0;
            
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Owner.MountedCenter) * 13,
                0.05f);

            if (Owner.Hitbox.Intersects(Projectile.Hitbox))
            {
                Projectile.Kill();
                VisualSystem.SpawnDustCircle(Owner.MountedCenter, ModContent.DustType<GlowFastDecelerate>(), scale: 0.6f, color: Color.LawnGreen);
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (!Sticking)
        {
            int randDust = Main.rand.Next(15, 20);
            for (int i = 0; i < randDust; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<GlowFastDecelerate>(), 0f, 0f, 100, Color.GreenYellow,
                    0.6f);
                dust.velocity *= 1.6f;
                dust.velocity.Y -= 2f;
                dust.velocity += Projectile.velocity;
                dust.noGravity = true;
            }
            
            Projectile.ai[0] = target.whoAmI;
            Projectile.ai[1] = 1;
        }
    }
}