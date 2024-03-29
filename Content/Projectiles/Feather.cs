using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class Feather : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = ProjAIStyleID.Arrow;
        Projectile.width = 34;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.tileCollide = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.timeLeft = 600;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Collision.HitTiles(Projectile.Center, Projectile.velocity, Projectile.width, Projectile.height);
        return true;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawOrigin = texture.Size() / 2;
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * Projectile.Opacity * 0.5f;
            color.A = 0;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.9f);
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, sizec, SpriteEffects.None);
        }

        return true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}