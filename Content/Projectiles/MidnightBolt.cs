using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class MidnightBolt : ModProjectile
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = 1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.ai[0]++;

        Visuals();
    }

    private void Visuals()
    {
        Lighting.AddLight(Projectile.position, Color.Purple.ToVector3());
        if (Projectile.alpha > 0) Projectile.alpha--;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 64 - Projectile.alpha / 4);
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position); // Play a death sound
        var usePos = Projectile.position; // Position to use for dusts

        // Offset the rotation by 90 degrees because the sprite is oriented vertiacally.
        var rotationVector = (Projectile.rotation - MathHelper.ToRadians(90f)).ToRotationVector2(); // rotation vector to use for dust velocity
        usePos += rotationVector * 16f;

        // Spawn some dusts upon javelin death
        for (var i = 0; i < 20; i++)
        {
            // Create a new dust
            var dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.Shadowflame);
            dust.position = (dust.position + Projectile.Center) / 2f;
            dust.velocity += rotationVector * 2f;
            dust.velocity *= 0.5f;
            dust.noGravity = true;
            usePos -= rotationVector * 8f;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        var texture = TextureAssets.Projectile[Projectile.type].Value;

        for (var k = 0; k < Projectile.oldPos.Length; k++)
        for (var j = 0.0625f; j < 1; j += 0.0625f)
        {
            var oldPosForLerp = k > 0 ? Projectile.oldPos[k - 1] : Projectile.position;
            var lerpedPos = Vector2.Lerp(oldPosForLerp, Projectile.oldPos[k], j);
            lerpedPos += Projectile.Size / 2;
            lerpedPos -= Main.screenPosition;
            var finalColor = new Color(50, 47, 125) * (1 - k / (float)Projectile.oldPos.Length);
            finalColor.A = 0; //acts like additive blending without spritebatch stuff

            var scaleFactor = Projectile.scale - k * 0.1f;
            Main.EntitySpriteDraw(texture, lerpedPos, null, finalColor, Projectile.rotation, texture.Size() / 2, scaleFactor, SpriteEffects.None);
        }

        return true;
    }
}