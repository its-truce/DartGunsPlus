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

public class VenomCloud : ModProjectile
{
    private float _rot1;
    private float _rot2;
    private SlotId _soundSlot;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Smoke2";

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 512;
        Projectile.height = 512;
        Projectile.scale = 0.15f; // 30
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 80;
        Projectile.Opacity = 0;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 30;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Main.rand.NextFloat(MathF.Tau);
        _rot1 = Main.rand.NextFloat(MathF.Tau);
        _rot2 = Main.rand.NextFloat(MathF.Tau);

        _soundSlot = SoundEngine.PlaySound(AudioSystem.ReturnSound("poison"), Projectile.Center);
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 10);

        if (Projectile.timeLeft < 60)
            FadingSystem.FadeOut(Projectile, 19);

        Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3());
        Projectile.rotation += MathHelper.ToRadians(2);
        _rot1 += MathHelper.ToRadians(5);
        _rot2 += MathHelper.ToRadians(4);
        Projectile.velocity = Vector2.Zero;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke3").Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke4").Value;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(210, 164, 236, 0) * Projectile.Opacity,
            _rot1, texture.Size() / 2, 0.12f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, new Color(28, 15, 43, 0) * Projectile.Opacity,
            _rot2, texture2.Size() / 2, 0.1f, SpriteEffects.None);

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Smoke5").Value;
        Color color = new Color(59, 34, 79, 0) * (1f - Projectile.alpha) * (Projectile.oldPos.Length / (float)Projectile.oldPos.Length) * Projectile.Opacity;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation,
            texture.Size() / 2, 0.15f, SpriteEffects.None);

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, new Color(185, 131, 216, 0) * Projectile.Opacity,
            Projectile.rotation, texture2.Size() / 2, 0.15f, SpriteEffects.None);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Venom, 30);
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.TryGetActiveSound(_soundSlot, out ActiveSound activeSound);
        activeSound?.Stop();
    }
}