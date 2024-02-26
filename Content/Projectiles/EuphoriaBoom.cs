using DartGunsPlus.Content.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class EuphoriaBoom : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";
    private ref float TimeLeft => ref Projectile.ai[0];
    private Player Owner => Main.player[Projectile.owner];
    
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
        Projectile.penetrate = 1;
        Projectile.timeLeft = 30;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.extraUpdates = 1;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        // this is all taken from jeo

        Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Extra_98").Value;
        Rectangle frame = texture.Frame();
        Vector2 frameOrigin = frame.Size() / 2f;

        Texture2D texture2 = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle frame2 = texture2.Frame();
        Vector2 frameOrigin2 = frame2.Size() / 2f;

        Color col = Color.Lerp(new Color(255, 255, 125), Color.LightBlue, Main.masterColor) * 0.4f;
        Color col2 = Color.Lerp(Color.White, Color.Black, Main.masterColor);
        Vector2 stretchscale = new(Projectile.scale * 1.4f + Main.masterColor / 2);

        for (int i = 1; i < Projectile.oldPos.Length - 1; i++)
        {
            col.A = 0;
            Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(Projectile.width / 2);
            Main.EntitySpriteDraw(texture, drawPos + Main.rand.NextVector2Circular(i / 2, i / 2), frame,
                new Color(col.R, col.G - i * 8, col.B, 0) * (1 - i * 0.04f), Projectile.oldRot[i] + Main.rand.NextFloat(-i * 0.01f, i * 0.01f),
                frameOrigin, new Vector2(stretchscale.X - i * 0.05f, stretchscale.Y * Main.rand.NextFloat(0.1f, 0.05f) * Vector2.Distance(Projectile.oldPos[i],
                    Projectile.oldPos[i + 1]) - i * 0.05f) * new Vector2(0.5f, 1.3f), SpriteEffects.None);
        }

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, frame2, new Color(255, 0, 0, 0), Projectile.rotation, frameOrigin2,
            stretchscale * new Vector2(0.8f, 1.3f), SpriteEffects.None);
        col.A = 255;
        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, frame2, col2 * Projectile.Opacity, Projectile.rotation, frameOrigin2,
            Projectile.scale * new Vector2(0.7f, 1.3f), SpriteEffects.None);

        return true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        for (int k = 0; k < Projectile.oldPos.Length; k++)
            Projectile.oldPos[k] = Projectile.position;
        
        EuphoriaPlayer euphoriaPlayer = Owner.GetModPlayer<EuphoriaPlayer>();
        Projectile.timeLeft = (int)TimeLeft;
        euphoriaPlayer.EuphoriaCurrent = 0;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
    
    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
    }

    public override void OnKill(int timeLeft)
    { 
        for (int i = 0; i < 30; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, 100, 100, DustID.WhiteTorch, 0f, 0f, 100,
                new Color(255, 255, 125), 2f);
            dust.noGravity = true;
            dust.velocity *= 5f;
            dust = Dust.NewDustDirect(Projectile.position, 100, 100, DustID.WhiteTorch, 0f, 0f, 100,
                Color.CornflowerBlue);
            dust.velocity *= 3f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        for (int i = 0; i < 6; i++)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, Vector2.Zero, ModContent.ProjectileType<RevolvingSword>(),
                Projectile.damage, 7, Projectile.owner, MathHelper.ToRadians(60 * i), 70, target.whoAmI);
        }
    }
}