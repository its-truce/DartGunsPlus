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
        Main.NewText(player.GetModPlayer<AccessoryPlayer>().HasDartLicense);
    }
}

public class DartLicenseNPC : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        var eligibleForLicense = NPC.downedBoss3 || NPC.downedDeerclops || Main.hardMode;
        Main.NewText(eligibleForLicense);

        if (shop.NpcType == NPCID.Merchant && eligibleForLicense)
        {
            Main.NewText("ye");
            shop.Add<DartLicense>();
        }
    }
}