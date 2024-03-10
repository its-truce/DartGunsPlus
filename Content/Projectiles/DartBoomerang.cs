using DartGunsPlus.Content.Items.Weapons;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class DartBoomerang : ModProjectile
{
    private float _rotSpeed = 2;
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/Dartarang";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 36;
        Projectile.height = 22;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
        Projectile.light = 0.2f;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 5);

        Projectile.ai[0]++;
        if (_rotSpeed < 8)
            _rotSpeed *= 1.2f;

        Projectile.rotation += MathHelper.ToRadians(_rotSpeed);

        if (Projectile.Hitbox.Intersects(Owner.Hitbox) && Projectile.ai[0] > 35)
            Projectile.Kill();

        if (Projectile.ai[0] < 60 && Projectile.velocity.Length() < Owner.HeldItem.shootSpeed)
            Projectile.velocity *= 1.1f;
        if (Projectile.ai[0] == 30)
            Projectile.velocity = Vector2.Zero;
        if (Projectile.ai[0] > 35)
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Owner.Center) * Owner.HeldItem.shootSpeed,
                0.3f);

        if (Owner.HeldItem.ModItem is not Dartarang)
            Projectile.Kill();

        if (Projectile.ai[0] % 25 == 0 && Projectile.ai[0] != 0 && Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage,
                out float knockBack, out int _))
        {
            Vector2 spawnPos = Projectile.Center + new Vector2(Projectile.width / 2).RotatedBy(Projectile.rotation);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, new Vector2(speed, 0).RotatedBy(Projectile.rotation), projToShoot, damage,
                knockBack, Projectile.owner);

            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = spawnPos,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)Owner.whoAmI
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.SilverBulletSparkle, settings);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawOrigin = texture.Size() / 2;
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * Projectile.Opacity * 0.5f;
            color.A = 0;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.9f);
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, sizec, SpriteEffects.None);
        }

        return true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Collision.HitTiles(Projectile.Center, Projectile.velocity, Projectile.width, Projectile.height);
        Projectile.friendly = false;
        Projectile.ai[0] = 34;
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.friendly = false;
        Projectile.ai[0] = 34;
    }
}