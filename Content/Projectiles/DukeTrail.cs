using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class DukeTrail : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => "DartGunsPlus/Content/Projectiles/Trail";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 154;
        Projectile.height = 82;
        Projectile.penetrate = 1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.damage = 0;
        Projectile.friendly = false;
        Projectile.timeLeft = 11;
    }

    public override void AI()
    {
        Projectile.Center = Owner.Center;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Vector2 drawOrigin = texture.Size() / 2;

        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] + Projectile.Size / 2 - Main.screenPosition;
            Color col = Color.Turquoise * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * Projectile.Opacity;
            col.A = 0;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.9f);

            if (Owner.Center.Distance(Projectile.oldPos[k] + Projectile.Size / 2) > 30)
                Main.EntitySpriteDraw(texture, drawPos, null, col, Projectile.velocity.ToRotation(), drawOrigin, sizec * 0.4f, SpriteEffects.None);
        }

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}