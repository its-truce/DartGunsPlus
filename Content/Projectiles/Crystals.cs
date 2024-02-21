using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class Crystals : ModProjectile
{
    private readonly Color[] _colors = { new(218, 84, 255), new(56, 230, 150), new(200, 86, 135), new(122, 82, 204) };
    private Color _color;
    private int _initialDir;
    private Vector2 _offset;
    private Vector2 _scale;
    private NPC Target => Main.npc[(int)Projectile.ai[0]];

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.friendly = false;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 300;
        Projectile.width = 24;
        Projectile.height = 22;
        Projectile.Opacity = 0;
    }

    public override void OnSpawn(IEntitySource source)
    {
        int crystal = Main.rand.Next(0, 4);
        Projectile.frame = crystal;
        _color = _colors[crystal];
        _color.A = 0;

        _scale = new Vector2(Main.rand.NextFloat(0.8f, 1.2f), Main.rand.NextFloat(0.8f, 1.2f));

        _offset = Projectile.Center - Target.Center;
        _initialDir = Target.direction;
    }

    public override void AI()
    {
        if (Projectile.timeLeft > 279)
            FadingSystem.FadeIn(Projectile, 20);
        if (Projectile.timeLeft < 21)
            FadingSystem.FadeOut(Projectile, 20);

        Projectile.velocity = Vector2.Zero;

        int offsetFactor = Target.direction == -1 ? -1 : 1;
        Projectile.Center = Target.Center + new Vector2(Target.Size.X / 2 * offsetFactor, _offset.Y);
        Lighting.AddLight(Projectile.Center, _color.ToVector3());

        if (Main.rand.NextBool(120))
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SparkForLightDisc, newColor: _color);

        int dir = Target.direction == _initialDir ? 1 : -1;
        Projectile.rotation = Projectile.ai[1] * dir;

        if (!Target.active)
            Projectile.Kill();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D textureGlow = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/CrystalGlow").Value;

        Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        SpriteEffects effects = Target.direction == _initialDir ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Main.EntitySpriteDraw(textureGlow, Projectile.Center - Main.screenPosition, null, _color * 0.1f, Projectile.rotation,
            textureGlow.Size() / 2, Projectile.scale * 0.12f * _scale, effects);
        Main.EntitySpriteDraw(texture, drawPos, frame, Color.White * Projectile.Opacity, Projectile.rotation,
            frame.Size() / 2, Projectile.scale * _scale, effects);

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
}