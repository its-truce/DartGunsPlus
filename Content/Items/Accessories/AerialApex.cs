using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class AerialApex : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 48;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 4);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<AccessoryPlayer>().HasKite = true;
        player.GetModPlayer<AccessoryPlayer>().HasPlume = true;
    }
}