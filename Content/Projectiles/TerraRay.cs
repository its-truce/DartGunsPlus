using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class TerraRay : Deathray
{
    private float _rayAlpha;
    private ref float Scale => ref Projectile.ai[0];
    private NPC Target => Main.npc[(int)Projectile.ai[1]];
    private ref float Expand => ref Projectile.ai[2];
    private static Color Color => Color.Lerp(new Color(34, 177, 77), Color.YellowGreen, Main.masterColor) * 0.4f;

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
        Projectile.localNPCHitCooldown = 15;
        MoveDistance = 0f;
        RealMaxDistance = 600f;
        BodyRect = new Rectangle(0, 0, Projectile.width, Projectile.height);
        HeadRect = new Rectangle(0, 0, Projectile.width, Projectile.height);
        TailRect = new Rectangle(0, 0, Projectile.width, Projectile.height);
    }

    public override void OnSpawn(IEntitySource source)
    {
        Scale = 0.075f;
        RealMaxDistance = 1600;
    }

    public override void PostDraw(Color lightColor)
    {
        float projScale = (float)(5 + Math.Sin(Projectile.localAI[0] / 1.5f)) / 5f;
        _rayAlpha = MathHelper.Lerp(_rayAlpha, Projectile.timeLeft < 10 ? 0 : 1, 0.1f);

        DrawColor = Color * _rayAlpha * projScale;
        DrawColor.A = 0;

        for (int i = 0; i < 3; i++)
            DrawLaser(Main.spriteBatch, ModContent.Request<Texture2D>(Texture).Value, Position(), Projectile.velocity,
                BodyRect.Height, -1.57f, Scale, MaxDistance, (int)MoveDistance);

        DrawColor = new Color(255, 255, 255, 0);
        DrawLaser(Main.spriteBatch, ModContent.Request<Texture2D>(Texture).Value, Position(), Projectile.velocity,
            BodyRect.Height, -1.57f, Scale / 5, MaxDistance, (int)MoveDistance);
    }

    public override void PostAI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.localAI[0]++;

        RealMaxDistance = MaxDistance = 1600;

        if (Expand != 0)
        {
            Projectile.friendly = true;
            Scale = 0.3f;
        }

        if (!Target.active)
            Projectile.Kill();
    }

    protected override Vector2 Position()
    {
        return Projectile.Center - new Vector2(RealMaxDistance / 2, 0).RotatedBy(Projectile.rotation);
    }
}