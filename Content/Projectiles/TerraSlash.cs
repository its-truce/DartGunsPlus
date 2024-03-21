using System;
using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class TerraSlash : ModProjectile
{
    private bool _hit;
    private float _scale = 0.3f;
    public override string Texture => "DartGunsPlus/Content/Items/Weapons/Styx/Spike";
    private Vector2 _center;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 30;
        Projectile.Opacity = 0;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 5;
    }

    public override void OnSpawn(IEntitySource source)
    {
        VisualSystem.SpawnDustPortal(Projectile.Center, Projectile.velocity, ModContent.DustType<GlowFastDecelerate>(), scale: 0.6f, Color.LawnGreen);
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
        
        Lighting.AddLight(Projectile.Center, Color.YellowGreen.ToVector3());
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
            Color col = Color.Lerp(new Color(34, 177, 77), Color.YellowGreen, Main.masterColor) * 0.4f;
            col.A = 0;

            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(texture, drawPos, frame, col, Projectile.oldRot[k], frame.Size() / 2, sizec * 0.3f * new Vector2(_scale, 1),
                    SpriteEffects.None);
            }
        }

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        VisualSystem.SpawnDustCircle(target.Center, ModContent.DustType<GlowFastDecelerate>(), 4, 0.6f, scale: 0.6f, color: Color.LawnGreen);
        _hit = true;
        _center = target.Center;

        SoundEngine.PlaySound(AudioSystem.ReturnSound("hit", 0.5f, 0.6f), target.Center);
    }

    public override void OnKill(int timeLeft)
    {
        if (Projectile.ai[1] != 0 && Projectile.ai[1] < 5 && _hit) // upto 4 times
        {
            Vector2 spawnPos = _center + new Vector2(200, 0).RotatedByRandom(Math.Tau);
            Vector2 velocity = spawnPos.DirectionTo(_center) * 8;

            Projectile.NewProjectile(Projectile.GetSource_Death(), spawnPos, velocity, Projectile.type, Projectile.damage,
                2, Projectile.owner, ai1: Projectile.ai[1] + 1);
        }
    }
}