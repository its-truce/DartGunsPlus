using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class DysphoriaMagnet : ModProjectile
{
    private readonly Color _color = Color.Lerp(new Color(185, 133, 240), new Color(55, 224, 112), Main.masterColor) * 0.4f;
    private NPC Target => Main.npc[(int)Projectile.ai[0]];
    private ref float BeepTimer => ref Projectile.ai[1];
    private ref float BeepInterval => ref Projectile.ai[2];
    private int _initialDir;
    private Vector2 _offset;
    private float _initialRot;
    private int _magnetCount;
    
    public override void SetDefaults()
    {
        Projectile.tileCollide = false;
        Projectile.width = 24;
        Projectile.height = 20;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 610;
        Projectile.DamageType = DamageClass.Ranged;
    }

    public override void OnSpawn(IEntitySource source)
    {
        BeepInterval = 120;

        if (Projectile.ai[0] != 666)
        {
            _offset = Projectile.Center - Target.Center;
            _initialDir = Target.direction;
            _initialRot = Target.DirectionTo(Projectile.Center).ToRotation();
        }
    }

    public override void AI()
    {
        Projectile.velocity = Vector2.Zero;
        
        BeepTimer++;
        
        if (Projectile.timeLeft >= 600)
            FadingSystem.FadeIn(Projectile, 10);
        if (Projectile.timeLeft < 11)
            FadingSystem.FadeOut(Projectile, 10);
        
        if (BeepTimer % 6 == 0)
            BeepInterval--; // it'll go down to 30 frames by the time proj has been alive for 540 frames
        
        if (BeepTimer % BeepInterval == 0)
            SoundEngine.PlaySound(AudioSystem.ReturnSound("timebomb"), Projectile.Center);

        if (Projectile.timeLeft == 10)
            SoundEngine.PlaySound(AudioSystem.ReturnSound("magnet"), Projectile.Center);
        
        if (Projectile.ai[0] != 666) // arbitary number that says not attached to npc
        {
            int offsetFactor = Target.direction == -1 ? -1 : 1;
            Projectile.Center = Target.Center + new Vector2(Target.Size.X / 2 * offsetFactor, _offset.Y);
            
            int dir = Target.direction == _initialDir ? 1 : -1;
            Projectile.rotation = _initialRot * dir;

            if (!Target.active)
                Projectile.timeLeft = 1;
        }

        foreach (Projectile proj in Main.projectile)
        {
            if (proj.type == ModContent.ProjectileType<DysphoriaNail>() && proj.active && proj.owner == Projectile.owner && proj.Hitbox.Intersects(Projectile.Hitbox))
            {
                proj.Kill();
                Projectile.timeLeft -= 3;
                BeepInterval -= 0.5f;
                _magnetCount++;
            }
        }
    }
    
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D textureGlow = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Glowball").Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = _color;
        color.A = 0;

        SpriteEffects effects = Projectile.ai[0] == 666 || Target.direction == _initialDir ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        Main.EntitySpriteDraw(textureGlow, Projectile.Center - Main.screenPosition, null, color * Projectile.Opacity, Projectile.rotation,
            textureGlow.Size() / 2, Projectile.scale * 0.15f, effects);
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White * Projectile.Opacity, Projectile.rotation,
            texture.Size() / 2, Projectile.scale , effects);

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < _magnetCount; i++)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, new Vector2(4).RotatedByRandom(MathF.Tau),
                ModContent.ProjectileType<DysphoriaBolt>(), Projectile.damage, 3, Projectile.owner, ai1: 1);
        }

        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DysphoriaExplosion>(),
            Projectile.damage * 2 + _magnetCount / 2, 9, Projectile.owner);
        CameraSystem.Screenshake(8, 6);
    }
}