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
    private readonly int[] _excludedProjectiles =
    {
        ModContent.ProjectileType<DysphoriaBolt>(), ModContent.ProjectileType<AstralBolt>(), ModContent.ProjectileType<VolatileLaser>(),
        ModContent.ProjectileType<EuphoriaSlash>(), ModContent.ProjectileType<MartianLightning>(), ModContent.ProjectileType<HalloweenRitual>(),
        ModContent.ProjectileType<HalloweenJack>(), ModContent.ProjectileType<Explosion>(), ModContent.ProjectileType<Icicle>(),
        ModContent.ProjectileType<FreezeBolt>(), ModContent.ProjectileType<DysphoriaBolt>(), ModContent.ProjectileType<DysphoriaBoom>(),
        ModContent.ProjectileType<DysphoriaExplosion>(), ModContent.ProjectileType<DysphoriaNail>(), ModContent.ProjectileType<SereneBolt>(),
        ModContent.ProjectileType<SereneShock>(), ModContent.ProjectileType<TerraBoom>(), ModContent.ProjectileType<EmpressLaser>(),
        ModContent.ProjectileType<EmpressBolt>(), ModContent.ProjectileType<LilEmpress>(), ModContent.ProjectileType<LaserLightning>(),
        ModContent.ProjectileType<HomingLightning>(), ModContent.ProjectileType<EuphoriaBoom>(), ModContent.ProjectileType<ShotgunMusket>(),
        ModContent.ProjectileType<LuminiteStrike>(), ModContent.ProjectileType<Hook>()
    };

    private readonly int[] _validItems =
    {
        ModContent.ItemType<Dysphoria>(), ModContent.ItemType<TrueDysphoria>(), ModContent.ItemType<AstralAmalgam>(), ModContent.ItemType<VolatileContraption>(),
        ModContent.ItemType<Euphoria>(), ModContent.ItemType<TrueEuphoria>(), ModContent.ItemType<MartianMarksman>(), ModContent.ItemType<HalloweenHex>(),
        ModContent.ItemType<GlacialGeyser>(), ModContent.ItemType<Serenity>(), ModContent.ItemType<Luminescence>(), ModContent.ItemType<LaserTherapy>(),
        ModContent.ItemType<Scylla>()
    };

    private int _itemType;
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return !_excludedProjectiles.Contains(entity.type);
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse_WithAmmo use && ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart &&
            _validItems.Contains(use.Item.type) && !_excludedProjectiles.Contains(projectile.type))
        {
            _itemType = use.Item.type;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10; // how long you want the trail to be
            ProjectileID.Sets.TrailingMode[projectile.type] = 2; // recording mode
        }
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        float defaultScale = 1;
        Color drawColor = Color.White;

        if (_itemType == ModContent.ItemType<Dysphoria>())
        {
            drawColor = new Color(128, 104, 207);
        }
        else if (_itemType == ModContent.ItemType<TrueDysphoria>() && projectile.ai[2] % 2 == 0)
        {
            drawColor = new Color(128, 104, 207);
        }
        else if (_itemType == ModContent.ItemType<TrueDysphoria>() && projectile.ai[2] % 2 != 0)
        {
            drawColor = new Color(55, 224, 112);
        }
        else if (_itemType == ModContent.ItemType<AstralAmalgam>())
        {
            drawColor = new Color(0, 255, 10);
            defaultScale = 0.6f;
        }
        else if (_itemType == ModContent.ItemType<VolatileContraption>())
        {
            drawColor = new Color(241, 57, 57);
            defaultScale = 0.75f;
        }
        else if (_itemType == ModContent.ItemType<Euphoria>())
        {
            drawColor = new Color(233, 210, 65);
        }
        else if (_itemType == ModContent.ItemType<TrueEuphoria>() && projectile.ai[2] % 2 == 0)
        {
            drawColor = new Color(233, 210, 65);
        }
        else if (_itemType == ModContent.ItemType<TrueEuphoria>() && projectile.ai[2] % 2 != 0)
        {
            drawColor = new Color(236, 110, 180);
        }
        else if (_itemType == ModContent.ItemType<MartianMarksman>())
        {
            drawColor = Color.Cyan;
        }
        else if (_itemType == ModContent.ItemType<HalloweenHex>())
        {
            drawColor = new Color(255, 82, 30);
        }
        else if (_itemType == ModContent.ItemType<GlacialGeyser>())
        {
            drawColor = new Color(93, 200, 255);
        }
        else if (_itemType == ModContent.ItemType<Serenity>() && projectile.ai[2] % 2 == 0)
        {
            drawColor = new Color(62, 240, 93);
        }
        else if (_itemType == ModContent.ItemType<Serenity>() && projectile.ai[2] % 2 != 0)
        {
            drawColor = new Color(233, 177, 82, 0);
        }
        else if (_itemType == ModContent.ItemType<Luminescence>())
        {
            drawColor = Color.Lerp(Main.DiscoColor, Color.White, 0.4f);
        }
        else if (_itemType == ModContent.ItemType<LaserTherapy>())
        {
            drawColor = Color.PaleVioletRed;
        }
        else if (_itemType == ModContent.ItemType<Scylla>())
        {
            drawColor = Color.Turquoise;
            defaultScale = 0.4f;
        }

        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("DartGunsPlus/Content/Globals/DartGrayscale");

        for (int k = 0; k < projectile.oldPos.Length; k++)
        for (float j = 0.0625f; j < 1; j += 0.0625f)
        {
            Vector2 oldPosForLerp = k > 0 ? projectile.oldPos[k - 1] : projectile.position;
            Vector2 lerpedPos = Vector2.Lerp(oldPosForLerp, projectile.oldPos[k], j);
            lerpedPos += projectile.Size / 2;
            lerpedPos -= Main.screenPosition;
            Color finalColor = drawColor * 0.4f * (1 - k / (float)projectile.oldPos.Length);
            finalColor.A = 0; //acts like additive blending without spritebatch stuff
            float scaleFactor = defaultScale - k * 0.02f;
            if (_validItems.Contains(_itemType) && !_excludedProjectiles.Contains(projectile.type))
                Main.EntitySpriteDraw(texture, lerpedPos, null, finalColor, projectile.rotation, texture.Size() / 2,
                    scaleFactor, SpriteEffects.None);
        }

        return true;
    }

    public override Color? GetAlpha(Projectile projectile, Color lightColor)
    {
        if (_validItems.Contains(_itemType) && !_excludedProjectiles.Contains(projectile.type))
            return Color.White;
        return default;
    }
}