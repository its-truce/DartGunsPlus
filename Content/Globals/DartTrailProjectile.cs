using System.Linq;
using DartGunsPlus.Content.Items.Weapons;
using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Globals;

public class DartTrailProjectile : GlobalProjectile
{
    private readonly int[] ExcludedProjectiles =
    {
        ModContent.ProjectileType<MidnightBolt>(), ModContent.ProjectileType<AstralBolt>(), ModContent.ProjectileType<RedLaser>(),
        ModContent.ProjectileType<RedBomb>(), ModContent.ProjectileType<TrueEuphoriaSword>()
    };

    private readonly int[] ValidItems =
    {
        ModContent.ItemType<MidnightMarauder>(), ModContent.ItemType<AstralAmalgam>(), ModContent.ItemType<VolatileContraption>(),
        ModContent.ItemType<Euphoria>(), ModContent.ItemType<TrueEuphoria>()
    };

    private int ItemType;
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return !ExcludedProjectiles.Contains(entity.type);
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse_WithAmmo use && ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart &&
            ValidItems.Contains(use.Item.type) && !ExcludedProjectiles.Contains(projectile.type))
        {
            ItemType = use.Item.type;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8; // how long you want the trail to be
            ProjectileID.Sets.TrailingMode[projectile.type] = 2; // recording mode
        }
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        float defaultScale = 1;
        var drawColor = Color.White;

        if (ItemType == ModContent.ItemType<MidnightMarauder>())
        {
            drawColor = new Color(50, 47, 125);
        }
        else if (ItemType == ModContent.ItemType<AstralAmalgam>())
        {
            drawColor = new Color(0, 255, 10);
            defaultScale = 0.6f;
        }
        else if (ItemType == ModContent.ItemType<VolatileContraption>())
        {
            drawColor = new Color(241, 57, 57);
            defaultScale = 0.75f;
        }
        else if (ItemType == ModContent.ItemType<Euphoria>())
        {
            drawColor = new Color(233, 210, 65);
        }
        else if (ItemType == ModContent.ItemType<TrueEuphoria>() && projectile.ai[2] % 2 == 0)
        {
            drawColor = new Color(233, 210, 65);
        }
        else if (ItemType == ModContent.ItemType<TrueEuphoria>() && projectile.ai[2] % 2 != 0)
        {
            drawColor = new Color(236, 110, 180);
        }

        var texture = (Texture2D)ModContent.Request<Texture2D>("DartGunsPlus/Content/Globals/DartGrayscale");

        for (var k = 0; k < projectile.oldPos.Length; k++)
        for (var j = 0.0625f; j < 1; j += 0.0625f)
        {
            var oldPosForLerp = k > 0 ? projectile.oldPos[k - 1] : projectile.position;
            var lerpedPos = Vector2.Lerp(oldPosForLerp, projectile.oldPos[k], j);
            lerpedPos += projectile.Size / 2;
            lerpedPos -= Main.screenPosition;
            var finalColor = drawColor * 0.4f * (1 - k / (float)projectile.oldPos.Length);
            finalColor.A = 0; //acts like additive blending without spritebatch stuff
            var scaleFactor = defaultScale - k * 0.02f;
            if (ValidItems.Contains(ItemType) && !ExcludedProjectiles.Contains(projectile.type))
                Main.EntitySpriteDraw(texture, lerpedPos, null, finalColor, projectile.rotation, texture.Size() / 2,
                    scaleFactor, SpriteEffects.None);
        }

        return true;
    }

    public override Color? GetAlpha(Projectile projectile, Color lightColor)
    {
        if (ValidItems.Contains(ItemType) && !ExcludedProjectiles.Contains(projectile.type))
            return Color.White;
        return default;
    }
}