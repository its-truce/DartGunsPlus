using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class DukeDash : ModProjectile
{
    private Vector2 _initialPos;
    private CompactParticleManager _manager;
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => "DartGunsPlus/Content/Projectiles/Glowball";

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 10;
        
        _manager = new CompactParticleManager(
            particle =>
            {
                particle.Rotation = 0;
                
                switch (particle.TimeAlive)
                {
                    case 0:
                        particle.Scale = 0f;
                        particle.Opacity = 1f;
                        break;
                    
                    case < 30:
                        particle.Scale = MathHelper.Lerp(particle.Scale, 1f, 0.1f);
                        particle.Opacity = MathHelper.Lerp(particle.Opacity, 0f, 0.1f);
                        break;
                    
                    case < 60:
                        particle.Scale = MathHelper.Lerp(particle.Scale, 0f, 0.1f);
                        particle.Opacity = MathHelper.Lerp(particle.Opacity, 1f, 0.1f);
                        break;
                    
                    default:
                        particle.Dead = true;
                        break;
                }
            },
            
            (particle, spriteBatch, _) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.RainbowCrystalExplosion).Value;
                
                spriteBatch.Draw(texture, particle.Position - Main.screenPosition, null, particle.Color * particle.Opacity, 0f, 
                    texture.Size() / 2, particle.Scale * 0.25f, SpriteEffects.None, 0f);
                
                spriteBatch.Draw(texture, particle.Position - Main.screenPosition, null, particle.Color * particle.Opacity, MathHelper.PiOver2,
                    texture.Size() / 2, particle.Scale * 0.25f, SpriteEffects.None, 0f);
                
                spriteBatch.Draw(texture, particle.Position - Main.screenPosition, null, particle.Color * particle.Opacity * 0.5f, 0f,
                    texture.Size() / 2, particle.Scale * 0.5f, SpriteEffects.None, 0f);
                
                spriteBatch.Draw(texture, particle.Position - Main.screenPosition, null, particle.Color * particle.Opacity * 0.5f, 
                    MathHelper.PiOver2, texture.Size() / 2, particle.Scale * 0.5f, SpriteEffects.None, 0f);
            }
        );
    }

    public override void OnSpawn(IEntitySource source)
    {
        _initialPos = Owner.Center;
        
        Owner.velocity = Owner.DirectionTo(Main.MouseWorld - new Vector2(0, 20)) * 25;
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Owner.velocity, ModContent.ProjectileType<DukeTrail>(), 0, 0,
            Owner.whoAmI);

        Color color = Color.Turquoise;
        color.A = 0;
        Vector2[] points = VisualSystem.GetInterpolatedPoints(_initialPos, Owner.Center, 10);

        foreach (Vector2 point in points)
        {
            Dust.NewDust(point, Owner.width, Owner.height, DustID.FishronWings, (Owner.DirectionTo(Projectile.Center) * 2).X, 
                (Owner.DirectionTo(Projectile.Center) * 2).Y, newColor: color);
        }

        SoundEngine.PlaySound(AudioSystem.ReturnSound("bubble", 0.3f), Projectile.Center);
        VisualSystem.SpawnDustCircle(Projectile.Center, DustID.FishronWings, 20, 1.2f, color: color);
        
        Color color2 = new (24, 200, 119, 0);

        for (int i = 0; i < 6; i++)
        {
            Vector2 position = Owner.Center + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians(i * 60));
            _manager.AddParticle(position, Vector2.UnitX, 0f, 2f, 1f, color2);
        }
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, Color.Turquoise.ToVector3());
        Projectile.Center = Owner.MountedCenter;
        Projectile.velocity = Vector2.Zero;

        DukePlayer dukePlayer = Owner.GetModPlayer<DukePlayer>();
        dukePlayer.DukeDash = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Color color = new (24, 200, 119, 0);
        
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2,
            0.4f, SpriteEffects.None);
        
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * Projectile.Opacity,
            Projectile.rotation, texture.Size() / 2, 0.25f, SpriteEffects.None); // white center
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        _manager.Update();
        _manager.Draw(Main.spriteBatch, Vector2.Zero);
    }

    public override void OnKill(int timeLeft)
    {
        Owner.velocity = Vector2.Zero;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float point = 0f;
        
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), _initialPos,
            Owner.Center, 50, ref point);
    }
    
    public override bool? CanCutTiles()
    {
        return true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
}

public class DukePlayer : ModPlayer
{
    public bool DukeDash;
    
    public override void ResetEffects()
    {
        DukeDash = false;
    }

    public override void PostUpdateRunSpeeds()
    {
        if (DukeDash)
        {
            Player.maxRunSpeed += 2.5f;
            Player.accRunSpeed += 2.5f;
        }
    }
}