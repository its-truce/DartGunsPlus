using System.Collections.Generic;
using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class ZapperLightning : ModProjectile
{
    private Vector2 _startLocation;

    public override void SetDefaults()
    {
        Projectile.width = 40;
        Projectile.height = 40;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 10;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 5;
    }

    public override void OnSpawn(IEntitySource source)
    {
        _startLocation = Projectile.Center - new Vector2(0, 600);
    }

    public override bool? CanCutTiles()
    {
        return true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Glowball").Value;
        Color col = new Color(255, 246, 72, 0);
        
        DrawLightning(texture, texture2, col, _startLocation, Projectile.Center, 0.022f);

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Vector2 drawOrigin = texture.Size() / 2;

        Main.EntitySpriteDraw(texture, drawPos, null, col, 0, drawOrigin, 0.1f, SpriteEffects.None);
        return false;
    }

    private static void DrawLightning(Texture2D texture, Texture2D texture2, Color col, Vector2 source, Vector2 dest, float scale, float sway = 80f,
        float jaggednessNumerator = 1f)
    {
        List<Vector2> points = LightningSystem.CreateBolt(source, dest, sway, jaggednessNumerator);

        for (int i = 1; i < points.Count; i++)
        {
            Vector2 start = points[i - 1];
            Vector2 end = points[i];
            float numPoints = (end - start).Length() * 0.4f;

            for (int j = 0; j < numPoints; j++)
            {
                float lerp = j / numPoints;
                Vector2 drawPos = Vector2.Lerp(start, end, lerp);
                Lighting.AddLight(drawPos, Color.Yellow.ToVector3());
                Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, col, 0, texture.Size() / 2, scale,
                    SpriteEffects.None);
                Main.EntitySpriteDraw(texture2, drawPos - Main.screenPosition, null, new Color(255, 255, 255, 0), 0,
                    texture2.Size() / 2, scale * 0.3f, SpriteEffects.None); // white center
            }
        }
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float point = 0f;

        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), _startLocation,
            Projectile.Center, 22, ref point);
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(AudioSystem.ReturnSound("boom"), Projectile.Center);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        int randDust = Main.rand.Next(15, 20);
        for (int i = 0; i < randDust; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<GlowFastDecelerate>(), 0f, 0f, 100, Color.Gold, 
                0.6f);
            dust.velocity *= 3.2f;
            dust.velocity.Y -= 1f;
            Vector2 velocity = new Vector2(0, 10);
            dust.velocity += velocity;
            dust.noGravity = true;
        }
    }
}