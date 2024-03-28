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

            case NPCID.MartianSaucerCore:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MartianMarksman>(), 2));
                break;
            
            case NPCID.DarkCaster:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Katanakaze>(), 4));
                break;
        }

        switch (npc.type) // for boss drops in non expert
        {
            case NPCID.QueenBee:
                LeadingConditionRule notExpertRuleQueenBee = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRuleQueenBee.OnSuccess(ItemDropRule.Common(ModContent.ItemType<BumbleBarrage>(), 2));
                npcLoot.Add(notExpertRuleQueenBee);
                break;

            case NPCID.Plantera:
                LeadingConditionRule notExpertRulePlantera = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRulePlantera.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RosemaryThyme>(), 2));
                npcLoot.Add(notExpertRulePlantera);
                break;

            case NPCID.Golem:
                LeadingConditionRule notExpertRuleGolem = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRuleGolem.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Scatterhook>(), 2));
                npcLoot.Add(notExpertRuleGolem);
                break;

            case NPCID.HallowBoss:
                LeadingConditionRule notExpertRuleHallowBoss = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRuleHallowBoss.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Luminescence>(), 2));
                npcLoot.Add(notExpertRuleHallowBoss);
                break;

            case NPCID.MoonLordCore:
                LeadingConditionRule notExpertRuleMoonLord = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRuleMoonLord.OnSuccess(ItemDropRule.Common(ModContent.ItemType<LaserTherapy>(), 3));
                npcLoot.Add(notExpertRuleMoonLord);
                notExpertRuleMoonLord.OnSuccess(ItemDropRule.Common(ModContent.ItemType<StellarOutrage>(), 3));
                npcLoot.Add(notExpertRuleMoonLord);
                break;

            case NPCID.DukeFishron:
                LeadingConditionRule notExpertRuleDukeFishron = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRuleDukeFishron.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CrownCascade>(), 2));
                npcLoot.Add(notExpertRuleDukeFishron);
                break;

            case NPCID.WallofFlesh:
                LeadingConditionRule notExpertRuleWallOfFlesh = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRuleWallOfFlesh.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GoreNGlory>(), 2));
                npcLoot.Add(notExpertRuleWallOfFlesh);
                break;

            case NPCID.Spazmatism:
                LeadingConditionRule notExpertRuleSpazmatism = new LeadingConditionRule(new Conditions.NotExpert());
                notExpertRuleSpazmatism.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SeekersSpyglass>()));
                npcLoot.Add(notExpertRuleSpazmatism);
                break;
        }
    }
}