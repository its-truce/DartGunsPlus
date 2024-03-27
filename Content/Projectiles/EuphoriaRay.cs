using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class EuphoriaRay : Deathray
{
    private Color _color = Color.White;
    private float _rayAlpha;
    private ref float Scale => ref Projectile.ai[0];
    private ref float Size => ref Projectile.ai[1];
    private ref float Expand => ref Projectile.ai[2];

    public override string Texture => "DartGunsPlus/Content/Projectiles/Deathray";

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 1;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 420;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
        MoveDistance = 0f;
        RealMaxDistance = 140f;
        BodyRect = new Rectangle(0, 0, Projectile.width, Projectile.height);
        HeadRect = new Rectangle(0, 0, Projectile.width, Projectile.height);
        TailRect = new Rectangle(0, 0, Projectile.width, Projectile.height);
    }

    public override void OnSpawn(IEntitySource source)
    {
        Scale = 0.1f;
        RealMaxDistance = Size;

        Color[] colors = { new(255, 238, 80, 50), new(255, 122, 200, 50), new(90, 90, 200, 50) };
        _color = Main.rand.NextFromList(colors);
    }

    public override void PostDraw(Color lightColor)
    {
        float projScale = (float)(5 + Math.Sin(Projectile.localAI[0] / 1.5f)) / 5f;
        _rayAlpha = MathHelper.Lerp(_rayAlpha, Projectile.timeLeft < 10 ? 0 : 1, 0.1f);

        DrawColor = _color * _rayAlpha * projScale;
        DrawLaser(Main.spriteBatch, ModContent.Request<Texture2D>(Texture).Value, Position(), Projectile.velocity,
            BodyRect.Height, -1.57f, Scale, MaxDistance, (int)MoveDistance);
    }

    public override void PostAI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.localAI[0]++;
        RealMaxDistance = MaxDistance = Size;

        if (Expand != 0)
        {
            Projectile.friendly = true;
            Scale = 0.3f;
        }
    }

    protected override Vector2 Position()
    {
        return Projectile.Center - new Vector2(RealMaxDistance / 2, 0).RotatedBy(Projectile.rotation);
    }
}