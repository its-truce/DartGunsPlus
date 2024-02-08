using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class EndlessDartCase : ModItem
{
    public override void SetDefaults()
    {
        Item.CloneDefaults(ModContent.ItemType<Dart>());
        Item.width = 36;
        Item.height = 30;
        Item.consumable = false;
        Item.maxStack = 1;
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<Dart>(), 3996)
            .AddTile(TileID.CrystalBall)
            .Register();
    }
}