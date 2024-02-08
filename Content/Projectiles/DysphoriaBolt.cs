using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class DysphoriaBolt : ModProjectile
{
    private float _scale = 0.2f;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Bolt";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.penetrate = 1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        Projectile.ai[0]++;

        if (_scale < 1)
            _scale *= 1.03f;

        if (Projectile.FindTargetWithLineOfSight(1000f) != -1)
            Projectile.velocity = Vector2.Lerp(Projectile.DirectionTo(Main.npc[Projectile.FindTargetWithLineOfSight(1000f)].Center) * 10,
                Projectile.velocity, 0.3f);
        else
            Projectile.velocity = Vector2.Lerp(Projectile.velocity.SafeNormalize(Projectile.velocity) * 10,
                Projectile.velocity, 0.3f);

        Projectile.rotation = Projectile.velocity.ToRotation();

        Lighting.AddLight(Projectile.Center, new Color(185, 133, 240).ToVector3());
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 offset = new(Projectile.width / 2f, Projectile.height / 2f);
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + offset;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.8f);
            Color color = new Color(185, 133, 240, 0) * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[k], frame.Size() / 2, sizec * 0.18f * new Vector2(_scale, 1),
                SpriteEffects.None);
        }

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
}