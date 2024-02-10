using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class Meteor : ModProjectile
{
    private int _projTextureID = 424;
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }
    
    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 36;
        Projectile.height = 36;
        Projectile.penetrate = 1;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.timeLeft = 300;
        Projectile.scale = 0.7f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        _projTextureID += Main.rand.Next(0, 3); // 425 to 426
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, Color.Red.ToVector3());
        Projectile.rotation += MathHelper.ToRadians(4);

        Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * -1, Projectile.velocity.Y * -1);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>($"Terraria/Images/Projectile_{_projTextureID}").Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Glowball").Value;

        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 offset = new(Projectile.width / 2f, Projectile.height / 2f);
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + offset;
            float sizec = 1.3f * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.8f);
            Color color = new Color(255, 44, 44, 0) * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(texture2, drawPos, null, color, Projectile.oldRot[k], texture2.Size()/2,
                sizec * 0.07f, SpriteEffects.None);
        }

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size()/2, 
            Projectile.scale, SpriteEffects.None);
        
        return true;
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

        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(),
            Projectile.damage / 2, 4, Projectile.owner);
    }
}