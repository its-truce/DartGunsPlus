using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class TerraBoom : ModProjectile
{
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
        Projectile.penetrate = 1;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.extraUpdates = 1;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        // this is all taken from jeo

        Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Extra_89").Value;
        Rectangle frame = texture.Frame();
        Vector2 frameOrigin = frame.Size() / 2f;

        Color col = Color.Lerp(new Color(34, 177, 77), Color.YellowGreen, Main.masterColor) * 0.4f;
        Vector2 stretchscale = new (Projectile.scale * 1.4f + Main.masterColor / 2);

        for (int i = 1; i < Projectile.oldPos.Length - 1; i++)
        {
            col.A = 0;
            Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(Projectile.width / 2);
            Main.EntitySpriteDraw(texture, drawPos + Main.rand.NextVector2Circular(i / 2, i / 2), frame,
                new Color(col.R + i * 2, col.G, col.B, 0) * (1 - i * 0.04f), Projectile.oldRot[i] + Main.rand.NextFloat(-i * 0.01f, i * 0.01f),
                frameOrigin, new Vector2(stretchscale.X - i * 0.05f, stretchscale.Y * Main.rand.NextFloat(0.1f, 0.05f) * Vector2.Distance(Projectile.oldPos[i],
                    Projectile.oldPos[i + 1]) - i * 0.05f) * new Vector2(0.4f, 3f), SpriteEffects.None);
        }
        
        return true;
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
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

        if (Projectile.timeLeft % 30 == 0)
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(0, 600), new Vector2(Main.rand.Next(-3, 3), 11),
                ModContent.ProjectileType<TerraBolt>(), Projectile.damage, 2, Projectile.owner);
    }
}