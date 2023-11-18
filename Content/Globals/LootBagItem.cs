using System.Linq;
using DartGunsPlus.Content.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Globals;

public class LootBagItem : GlobalItem
{
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
    {
        switch (item.type)
        {
            case ItemID.QueenBeeBossBag:
                foreach (var rule in itemLoot.Get())
                    if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.BeeGun))
                    {
                        var original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ModContent.ItemType<BumbleBarrage>());
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }

                break;

            case ItemID.PlanteraBossBag:
                foreach (var rule in itemLoot.Get())
                    if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.Seedler))
                    {
                        var original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ModContent.ItemType<RosemaryThyme>());
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }

                break;

            case ItemID.GolemBossBag:
                foreach (var rule in itemLoot.Get())
                    if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.HeatRay))
                    {
                        var original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ModContent.ItemType<Scatterhook>());
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }

                break;
        }
    }
}