using System;
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

public class EuphoriaSlash : ModProjectile
{ 
    private Player Owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.width = 44;
        Projectile.height = 44;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 30;
    }

    public override void OnSpawn(IEntitySource source)
    {
        SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, Projectile.Center);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeOut(Projectile, 10);

        if (Projectile.timeLeft <= 7)
            FadingSystem.FadeOut(Projectile, 7);

        Projectile.velocity *= 0.97f;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(47);

        Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3());

        if (Main.rand.NextBool(30))
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.CrystalPulse2, Projectile.velocity.X * .2f,
                Projectile.velocity.Y * .2f, 200, Scale: 1.2f);
            dust.noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawOrigin = texture.Size() / 2;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = new Color(255, 255, 100) * Projectile.Opacity;

        color.A = 0;

        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(3))
        {
            Vector2 spawnPos = target.Center + new Vector2(120, 0).RotatedByRandom(Math.Tau);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), spawnPos, spawnPos.DirectionTo(target.Center) * 9, Projectile.type,
                Projectile.damage, Projectile.knockBack, Owner.whoAmI);
        }

        for (int i = 0; i < 4; i++)
        {
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width / 3, Projectile.height / 3, DustID.CrystalPulse2, Projectile.velocity.X, Projectile.velocity.Y,
                100, default, 1.2f);
            Main.dust[dust2].noGravity = true;
            Main.dust[dust2].velocity *= 2f;
            Main.dust[dust2].fadeIn = 1.3f;
        }
    }
}