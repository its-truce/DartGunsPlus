using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class DartLicense : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 50;
        Item.height = 34;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 2);
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
        Condition downedSkeletronOrDeerclopsOrHardmode = new Condition("Mods.DartGunsPlus.Conditions.downedSkeletronOrDeerclopsOrHardmode",
            () => Condition.DownedSkeletron.IsMet() || Condition.DownedDeerclops.IsMet() || Condition.Hardmode.IsMet());
        
        if (shop.NpcType == NPCID.Merchant)
            shop.Add<DartLicense>(downedSkeletronOrDeerclopsOrHardmode);
    }
}