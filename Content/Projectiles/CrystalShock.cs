using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class CrystalShock : ModProjectile
{
    public override string Texture => "Terraria/Images/Projectile_950"; // princess weapon

    public override void SetDefaults()
    {
        Projectile.width = 104;
        Projectile.height = 104;
        Projectile.aiStyle = ProjAIStyleID.PrincessWeapon;
        Projectile.friendly = true;
        Projectile.Opacity = 0;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 180;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 25;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle frame = texture.Frame();
        Vector2 drawOrigin = frame.Size() / 2;
        float scaleFactor = Projectile.localAI[0];

        Color color1 = Color.LightPink;
        color1.A = 0;
        Color color2 = new(200, 86, 135, 0);

        Main.EntitySpriteDraw(texture, drawPos, frame, color2, Projectile.rotation, drawOrigin, scaleFactor, SpriteEffects.None);
        Main.EntitySpriteDraw(texture, drawPos, frame, color1, Projectile.rotation, drawOrigin, scaleFactor * 0.8f, SpriteEffects.None);
        Main.EntitySpriteDraw(texture, drawPos, frame, color2, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(texture, drawPos, frame, color1, Projectile.rotation, drawOrigin, Projectile.scale * 0.8f, SpriteEffects.None);

        return false;
    }
}