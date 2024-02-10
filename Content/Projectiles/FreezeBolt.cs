using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class FreezeBolt : ModProjectile
{
    private float _scale = 0.3f;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Bolt";
    private Player Owner => Main.player[Projectile.owner];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.penetrate = 1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        FadingSystem.FadeIn(Projectile, 5);

        if (Projectile.velocity.Length() < 20)
            Projectile.velocity *= 1.06f;

        if (_scale < 1)
            _scale *= 1.07f;

        Projectile.rotation = Projectile.velocity.ToRotation();

        Lighting.AddLight(Projectile.Center, Color.AliceBlue.ToVector3());
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
            Color color = new Color(93, 200, 255, 0) * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[k], frame.Size() / 2, sizec * 0.2f * new Vector2(_scale, 1),
                SpriteEffects.None);
        }

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void OnKill(int timeLeft)
    {
        VisualSystem.SpawnDustCircle(Projectile.Center, DustID.RainbowRod, 12, color: Color.CornflowerBlue, scale: 0.9f);

        Vector2[] points = DartUtils.GetInterpolatedPoints(Owner.Center, Projectile.Center, 15);

        foreach (Vector2 point in points)
        {
            Vector2 pointPosition = point;
            Vector2 alternatePoint = Projectile.Center + new Vector2(Main.rand.Next(-300, 300), Main.rand.Next(-100, 100));

            Owner.LimitPointToPlayerReachableArea(ref pointPosition);
            Vector2 targetSpot = pointPosition + Main.rand.NextVector2Circular(8f, 8f);
            Vector2 spawnPos = DartUtils.FindSharpTearsSpot(targetSpot, Owner, alternatePoint).ToWorldCoordinates(Main.rand.Next(17),
                Main.rand.Next(17));
            Vector2 velocity = (targetSpot - spawnPos).SafeNormalize(-Vector2.UnitY) * 16f;
            Projectile.NewProjectile(Projectile.GetSource_Death(), spawnPos, velocity, ModContent.ProjectileType<Icicle>(),
                Projectile.damage / 2, 5, Projectile.owner, ai1: Main.rand.NextFloat() * 0.5f + 0.6f);
        }

        SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.Center);
        CameraSystem.Screenshake(4, 7);
    }
}