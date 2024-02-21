using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class DysphoriaBoom : ModProjectile
{
    private readonly Color _color = Color.Lerp(new Color(185, 133, 240), new Color(55, 224, 112), Main.masterColor) * 0.4f;
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.tileCollide = true;
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 600;
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

        Color col = _color;
        Color col2 = Color.Lerp(Color.White, Color.Black, Main.masterColor);
        Vector2 stretchscale = new(Projectile.scale * 1.4f + Main.masterColor / 2);

        for (int i = 1; i < Projectile.oldPos.Length - 1; i++)
        {
            col.A = 0;
            Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(Projectile.width / 2);
            
            if (drawPos.Distance(Projectile.Center) > 15)
                Main.EntitySpriteDraw(texture, drawPos + Main.rand.NextVector2Circular(i / 2, i / 2), frame,
                    new Color(col.R, col.G - i * 8, col.B, 0) * (1 - i * 0.04f), Projectile.oldRot[i] + Main.rand.NextFloat(-i * 0.01f, i * 0.01f),
                    frameOrigin, new Vector2(stretchscale.X - i * 0.05f, stretchscale.Y * Main.rand.NextFloat(0.1f, 0.05f) * Vector2.Distance(Projectile.oldPos[i],
                        Projectile.oldPos[i + 1]) - i * 0.05f) * new Vector2(1, 0.5f), SpriteEffects.None);
        }

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, frame2, new Color(255, 0, 0, 0), Projectile.rotation, frameOrigin2,
            stretchscale * new Vector2(1, 0.5f), SpriteEffects.None);
        col.A = 255;
        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, frame2, col2 * Projectile.Opacity, Projectile.rotation, frameOrigin2,
            Projectile.scale * new Vector2(1, 0.5f), SpriteEffects.None);

        return true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        for (int k = 0; k < Projectile.oldPos.Length; k++) 
            Projectile.oldPos[k] = Projectile.position;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center + Projectile.oldVelocity, Vector2.Zero, ModContent.ProjectileType<DysphoriaMagnet>(),
            Projectile.damage, 8, Projectile.owner, 666);
        proj.rotation = Projectile.oldVelocity.ToRotation() + MathF.PI;
        
        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DysphoriaMagnet>(),
            Projectile.damage, 8, Projectile.owner, target.whoAmI);
        proj.rotation = target.DirectionTo(Projectile.Center).ToRotation();
    }
}