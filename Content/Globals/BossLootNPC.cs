using DartGunsPlus.Content.Items.Accessories;
using DartGunsPlus.Content.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Globals;

public class BossLootItem : GlobalItem
{
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) // Using ModifyItemLoot to change the loot from certain boss bags
    {
        switch (item.type)
        {
            case ItemID.QueenBeeBossBag:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BumbleBarrage>(), 2));
                break;

            case ItemID.PlanteraBossBag:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<RosemaryThyme>(), 2));
                break;

            case ItemID.GolemBossBag:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Scatterhook>(), 2));
                break;

            case ItemID.FairyQueenBossBag:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Luminescence>(), 2));
                break;

            case ItemID.MoonLordBossBag:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<LaserTherapy>(), 3));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<StellarOutrage>(), 3));
                break;

            case ItemID.SkeletronPrimeBossBag:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<VolatileContraption>(), 2));
                break;

            case ItemID.FishronBossBag:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrownCascade>(), 2));
                break;

            case ItemID.WallOfFleshBossBag:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<GoreNGlory>(), 2));
                break;
            
            case ItemID.TwinsBossBag:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SeekersSpyglass>()));
                break;
        }
    }
}

public class BossBagNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        switch (npc.type)
        {
            case NPCID.Pumpking:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HalloweenHex>(), 2));
                break;

            case NPCID.IceQueen:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GlacialGeyser>(), 2));
                break;

            case NPCID.MartianSaucer:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MartianMarksman>(), 2));
                break;
        }
    }
}