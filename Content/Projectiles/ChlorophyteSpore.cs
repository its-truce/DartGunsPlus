using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class ChlorophyteSpore : ModProjectile
{
    private float _rot1;
    private float _rot2;
    private SlotId _soundSlot;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Smoke1";

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 512;
        Projectile.height = 512;
        Projectile.scale = 0.04f;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 80;
        Projectile.Opacity = 0;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 5;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Main.rand.NextFloat(MathF.Tau);
        _rot1 = Main.rand.NextFloat(MathF.Tau);
        _rot2 = Main.rand.NextFloat(MathF.Tau);

        _soundSlot = SoundEngine.PlaySound(AudioSystem.ReturnSound("pop", 0.5f), Projectile.Center);
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 10);

        if (Projectile.timeLeft > 70)
            FadingSystem.FadeOut(Projectile, 9);

        Lighting.AddLight(Projectile.Center, Color.LightGreen.ToVector3());

        Projectile.rotation += MathHelper.ToRadians(2);
        _rot1 += MathHelper.ToRadians(1);
        _rot2 += MathHelper.ToRadians(3);

        if (Projectile.timeLeft < 50 && Projectile.FindTargetWithinRange(600) != null)
            Projectile.velocity = Vector2.Lerp(Projectile.DirectionTo(Projectile.FindTargetWithinRange(600).Center),
                Projectile.velocity, 0.6f) * 1.2f;
        else
            Projectile.velocity = Vector2.Zero;

        if (Main.rand.NextBool(80))
            Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.JungleSpore);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke3").Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke4").Value;
        Texture2D texture3 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Sparkle2").Value;

        Main.EntitySpriteDraw(texture3, Projectile.Center - Main.screenPosition, null, new Color(143, 215, 29, 0) * Projectile.Opacity,
            _rot2, texture3.Size() / 2, 0.04f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 261, 0) * Projectile.Opacity,
            _rot1, texture.Size() / 2, 0.05f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, new Color(28, 216, 94, 0) * Projectile.Opacity,
            _rot2, texture2.Size() / 2, 0.15f, SpriteEffects.None);

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke5").Value;

        Color color = new Color(79, 245, 137, 0) * (1f - Projectile.alpha) * (Projectile.oldPos.Length / (float)Projectile.oldPos.Length) * Projectile.Opacity;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation,
            texture.Size() / 2, 0.15f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, new Color(205, 239, 88, 0) * Projectile.Opacity,
            Projectile.rotation, texture2.Size() / 2, 0.175f, SpriteEffects.None);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Poisoned, 600);
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.TryGetActiveSound(_soundSlot, out ActiveSound activeSound);
        activeSound?.Stop();
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
}