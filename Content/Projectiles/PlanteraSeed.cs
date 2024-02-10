using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class PlanteraSeed : ModProjectile
{
    private NPC[] _alreadyHit = new NPC[1];
    private float _velocityLength;
    
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.width = 22;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.penetrate = 3;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }
    
    public override void AI()
    {
        if (Projectile.ai[0] == 4)
            _velocityLength = Projectile.velocity.Length() * 0.7f;

        NPC closestTarget = DartUtils.FindClosestTarget(800, _alreadyHit, Projectile, 3);

        if (closestTarget is not null && Projectile.ai[0] >= 5)
            Projectile.velocity = Vector2.Lerp(Projectile.DirectionTo(closestTarget.Center),
                Vector2.Normalize(Projectile.velocity), 0.7f) * _velocityLength;

        Projectile.ai[0]++;

        Projectile.rotation = Projectile.velocity.ToRotation();
        Lighting.AddLight(Projectile.Center, Color.DarkGreen.ToVector3());
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawOrigin = texture.Size() / 2;
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
            Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * Projectile.Opacity * 0.5f;
            color.A = 0;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.9f);
            
            for (int i = 0; i < 2; i++)
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, sizec, SpriteEffects.None);
        }

        return true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
    
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Array.Resize(ref _alreadyHit, _alreadyHit.Length + 1);
        _alreadyHit[^1] = target;
    }
}