using System.Collections.Generic;
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
                foreach (IItemDropRule rule in itemLoot.Get())
                    if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.BeeGun))
                    {
                        List<int> original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ModContent.ItemType<BumbleBarrage>());
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }

                break;

            case ItemID.PlanteraBossBag:
                foreach (IItemDropRule rule in itemLoot.Get())
                    if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.Seedler))
                    {
                        List<int> original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ModContent.ItemType<RosemaryThyme>());
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }

                break;

            case ItemID.GolemBossBag:
                foreach (IItemDropRule rule in itemLoot.Get())
                    if (rule is OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.HeatRay))
                    {
                        List<int> original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ModContent.ItemType<Scatterhook>());
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }

                break;
        }
    }
}