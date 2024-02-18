using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class ShotgunMusket : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => "DartGunsPlus/Content/Projectiles/GoreMusket";

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 102;
        Projectile.height = 102;
        Projectile.scale = 0.4f; // 30
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 10;
        Projectile.Opacity = 0;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.timeLeft = Owner.inventory[Owner.selectedItem].useTime;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 5);

        if (Projectile.timeLeft < 5)
            FadingSystem.FadeOut(Projectile, 4);

        Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3());

        float offset = Owner.direction == -1 ? MathF.PI : 0;
        Projectile.rotation = Owner.itemRotation + offset;
        Projectile.Center = Owner.MountedCenter + new Vector2(Owner.inventory[Owner.selectedItem].width * Owner.direction, 0).RotatedBy(Owner.itemRotation -
            MathHelper.ToRadians(Projectile.ai[1] * Owner.direction));
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Musket2").Value;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 140, 0, 0) * Projectile.Opacity,
            Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None);
        
        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, new Color(255, 112, 0, 0) * Projectile.Opacity,
            Projectile.rotation, texture2.Size() / 2, Projectile.scale * 0.7f, SpriteEffects.None);

        return false;
    }
}