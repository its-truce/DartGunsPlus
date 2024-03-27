using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class LuminiteTrail : ModProjectile
{
    private Vector2 _startPos;
    private Projectile ParentProj => Main.projectile[(int)Projectile.ai[0]];
    private bool ParentDead => Projectile.ai[1] != 0;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Bolt";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 64000;
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 240;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 10;
        Projectile.Opacity = 0;
    }

    public override void OnSpawn(IEntitySource source)
    {
        _startPos = Projectile.Center;
    }

    public override void AI()
    {
        Projectile.velocity = Vector2.Zero;

        Projectile.rotation = _startPos.DirectionTo(Projectile.Center).ToRotation();

        if (Projectile.timeLeft >= 220)
            FadingSystem.FadeIn(Projectile, 20);
        if (Projectile.timeLeft <= 20)
            FadingSystem.FadeOut(Projectile, 20);

        if (!ParentDead)
            Projectile.Center = ParentProj.Center;

        if (!ParentProj.active && Projectile.ai[1] == 0)
            Projectile.ai[1] = 1;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float point = 0f;

        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), _startPos,
            Projectile.Center, 6, ref point);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Deathray").Value;

        float scale = Vector2.Distance(_startPos, Projectile.Center) / texture.Width;
        Main.EntitySpriteDraw(texture, _startPos - Main.screenPosition, null, new Color(115, 245, 215, 0) * Projectile.Opacity, Projectile.rotation,
            Vector2.Zero, new Vector2(scale, 1f), SpriteEffects.None);

        return false;
    }
}