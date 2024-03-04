using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class RetinazerDash : ModProjectile
{
    private float _scale = 0.05f;
    private Vector2 _direction;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Bolt";
    
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
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 25;
        Projectile.Opacity = 0;
    }

    public override void OnSpawn(IEntitySource source)
    {
        _direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);
        Projectile.rotation = Projectile.ai[2];
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        
        if (_scale < 0.25f)
            _scale *= 1.2f;

        Projectile.rotation = Projectile.ai[2];

        Lighting.AddLight(Projectile.Center, Color.HotPink.ToVector3());
        
        if (Projectile.timeLeft > 14)
            FadingSystem.FadeIn(Projectile, 10);
        if (Projectile.timeLeft < 11)
            FadingSystem.FadeOut(Projectile, 10);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Vector2 drawOrigin = texture.Size() / 2;
        Color color = new(255, 100, 100, 0);
        
        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, new Vector2(_scale, 0.15f),
            SpriteEffects.None);

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
    
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float point = 0f;

        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - _direction * 90,
            Projectile.Center + _direction * 90, 50, ref point);
    }

    public override bool? CanCutTiles()
    {
        return true;
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }
}