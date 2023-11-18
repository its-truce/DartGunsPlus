using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Lilkraken : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 8, 9, true);
        Item.width = 54;
        Item.height = 24;
        Item.rare = ItemRarityID.Green;

        Item.UseSound = SoundID.Item98;

        Item.damage = 4;
        Item.knockBack = 0;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        var muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 5;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }
}