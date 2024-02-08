using DartGunsPlus.Content.Items.Ammo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Systems;

public class RecipeSystem : ModSystem
{
    public override void PostAddRecipes()
    {
        foreach (Recipe recipe in Main.recipe)
        {
            short[] dartItemIDs = { ItemID.PoisonDart, ItemID.CrystalDart, ItemID.IchorDart, ItemID.CursedDart };

            foreach (short dartItemID in dartItemIDs)
            {
                if (recipe.TryGetResult(dartItemID, out Item _))
                    recipe.AddIngredient<Dart>(100);
            }
        }
    }
}