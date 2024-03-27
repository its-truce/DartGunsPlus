using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class Shield : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 48;
        Projectile.height = 100;
        Projectile.penetrate = -1;
        Projectile.friendly = false;
        Projectile.timeLeft = 2;
        Projectile.aiStyle = -1;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        Projectile.Center = Owner.Center + new Vector2(100, 0).RotatedBy(Owner.DirectionTo(Main.MouseWorld).ToRotation());
        Projectile.rotation = Owner.DirectionTo(Main.MouseWorld).ToRotation();
        Projectile.velocity = Vector2.Zero;

        if (!Owner.GetModPlayer<AccessoryPlayer>().HasShield)
            Projectile.Kill();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        AccessoryPlayer accessoryPlayer = Owner.GetModPlayer<AccessoryPlayer>();

        float multiplier = accessoryPlayer.ShieldTimer == 0 ? 1.2f : 0.5f;
        Color color = Color.Lerp(new Color(219, 178, 69, 0), new Color(178, 37, 230, 0), Main.masterColor / 2) * 0.6f;
        Color color2 = new(185, 134, 255, 0);

        Main.EntitySpriteDraw(texture, drawPos, null, color * multiplier, Projectile.rotation, texture.Size() / 2,
            Projectile.scale, SpriteEffects.None);

        Main.EntitySpriteDraw(texture, drawPos, null, color2 * multiplier, Projectile.rotation, texture.Size() / 2,
            Projectile.scale * 0.5f, SpriteEffects.None);
        return false;
    }
}