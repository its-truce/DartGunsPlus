using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class EmpressBolt : ModProjectile
{
    private bool _hit;

    //public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.FairyQueenRangedItemShot}"; // eventide
    private float _scale = 0.3f;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Bolt";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.FairyQueenRangedItemShot);
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        FadingSystem.FadeIn(Projectile, 10);

        if (!_hit)
        {
            if (Projectile.velocity.Length() < 20)
                Projectile.velocity *= 1.06f;

            if (_scale < 1)
                _scale *= 1.07f;
        }
        else
        {
            if (Projectile.velocity.Length() > 5)
                Projectile.velocity *= 0.98f;

            if (_scale > 0.4f)
                _scale *= 0.97f;
        }

        Projectile.rotation = Projectile.velocity.ToRotation();

        Color color = Color.Lerp(Main.DiscoColor, Color.White, 0.4f);
        Lighting.AddLight(Projectile.Center, color.ToVector3());
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
            Color color = Color.Lerp(Main.DiscoColor, Color.White, 0.4f) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length)
                                                                         * Projectile.Opacity;
            color.A = 0;
            Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[k], frame.Size() / 2, sizec * 0.25f * new Vector2(_scale, 1),
                SpriteEffects.None);
        }

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        VisualSystem.SpawnDustCircle(Projectile.Center, DustID.RainbowRod, 12, color: Main.DiscoColor, scale: 0.9f);
        SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, Projectile.Center);
        _hit = true;
    }
}