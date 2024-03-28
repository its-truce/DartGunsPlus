using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Luminescence : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 25, 10, true);
        Item.width = 52;
        Item.height = 44;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(gold: 5);        Item.value = Item.sellPrice(gold: 5);


        Item.UseSound = SoundID.DD2_DarkMageHealImpact;

        Item.damage = 54;
        Item.knockBack = 3;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2);
    }
}