using System;
using System.Collections.Generic;
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
        Projectile.timeLeft = (int)(Owner.HeldItem.useTime * 0.7f);
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 5);

        if (Projectile.timeLeft < 5)
            FadingSystem.FadeOut(Projectile, 4);

        Lighting.AddLight(Projectile.Center, Color.White.ToVector3());

        float offset = Owner.direction == -1 ? MathF.PI : 0;
        Projectile.rotation = Owner.itemRotation + offset;
        Projectile.Center = Owner.MountedCenter + new Vector2((Owner.HeldItem.width + Owner.MountXOffset) * Owner.direction, 0).RotatedBy(
            Owner.itemRotation - MathHelper.ToRadians(Projectile.ai[1] * Owner.direction));
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Musket2").Value;

        Color color1;
        Color color2;

        switch (Projectile.ai[2])
        {
            case 1: // gore n glory
                color1 = new Color(255, 204, 20, 0);
                color2 = new Color(255, 231, 143, 0);
                break;

            case 2: // scylla
                color1 = new Color(20, 255, 203, 0);
                color2 = new Color(143, 255, 206, 0);
                break;
            
            case 3: // styx
                color1 = new Color(42, 34, 80, 0);
                color2 = new Color(85, 105, 181, 0);
                break;

            default:
                color1 = Color.Violet;
                color2 = Color.DarkBlue;
                break;
        }

        float scaleFactor = Projectile.ai[2] == 1 ? 1.3f : 1f;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color1 * Projectile.Opacity,
            Projectile.rotation, texture.Size() / 2, Projectile.scale * scaleFactor, SpriteEffects.None);
        
        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, color2 * Projectile.Opacity,
            Projectile.rotation, texture2.Size() / 2, Projectile.scale * 0.7f * scaleFactor, SpriteEffects.None);

        return false;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindProjectiles.Add(index);
    }
}