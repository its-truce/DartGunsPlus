using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Blazebringer : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 14, 9, true);
        Item.width = 48;
        Item.height = 36;
        Item.rare = ItemRarityID.Orange;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 25;
        Item.knockBack = 2;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 4;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.HellstoneBar, 15)
            .AddTile(TileID.Anvils)
            .Register();
    }
}