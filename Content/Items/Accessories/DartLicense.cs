using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class DartLicense : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 38;
        Item.height = 24;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 1, silver: 50);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<AccessoryPlayer>().HasDartLicense = true;
    }
}

public class DartLicenseNPC : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        Condition downedSkeletronOrDeerclopsOrHardmode = new("Mods.DartGunsPlus.Conditions.downedSkeletronOrDeerclopsOrHardmode",
            () => Condition.DownedSkeletron.IsMet() || Condition.DownedDeerclops.IsMet() || Condition.Hardmode.IsMet());

        if (shop.NpcType == NPCID.Merchant)
            shop.Add<DartLicense>(downedSkeletronOrDeerclopsOrHardmode);
    }
}