using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class StellarOutrage : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 20, 18, true);
        Item.width = 74;
        Item.height = 44;
        Item.rare = ItemRarityID.Red;

        Item.UseSound = SoundID.Item105;

        Item.damage = 121;
        Item.knockBack = 4;
        Item.consumeAmmoOnLastShotOnly = true;

        Item.useTime = 4; // 1/5th of useAnimation
        Item.reuseDelay = 10;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);
        velocity.RotatedByRandom(MathHelper.ToRadians(4));

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 2;
        }

        if (Main.rand.NextBool(4))
        {
            type = ModContent.ProjectileType<LargeStar>();
            velocity *= 0.7f;
            damage += damage / 2;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }
}