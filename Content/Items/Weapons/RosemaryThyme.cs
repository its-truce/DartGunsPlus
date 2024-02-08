using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class RosemaryThyme : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 18, 11, true);
        Item.width = 64;
        Item.height = 26;
        Item.rare = ItemRarityID.Lime;

        Item.UseSound = SoundID.DD2_JavelinThrowersAttack;

        Item.damage = 42;
        Item.knockBack = 6;
        Item.consumeAmmoOnLastShotOnly = true;

        Item.useTime = 6; // 1/3rd of useAnimation
        Item.reuseDelay = 10;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 2;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }
}