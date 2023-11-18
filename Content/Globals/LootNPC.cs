using System.Linq;
using DartGunsPlus.Content.Items.Weapons;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Globals;

// Resharper disable All
public class BossLootNPC : GlobalNPC // Reshaper restore All
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        switch (npc.type)
        {
            case NPCID.QueenBee:
                foreach (var rule in npcLoot.Get())
                {
                    if (rule is DropBasedOnExpertMode dropBasedOnExpertMode && dropBasedOnExpertMode.ruleForNormalMode is
                            OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.BeeGun))
                    {
                        var original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ModContent.ItemType<BumbleBarrage>());
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }
                }

                break;

            case NPCID.Plantera:
                foreach (var rule in npcLoot.Get())
                {
                    if (rule is DropBasedOnExpertMode dropBasedOnExpertMode && dropBasedOnExpertMode.ruleForNormalMode is
                            OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.Seedler))
                    {
                        var original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ModContent.ItemType<RosemaryThyme>());
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }
                }

                break;

            case NPCID.Golem:
                foreach (var rule in npcLoot.Get())
                {
                    if (rule is DropBasedOnExpertMode dropBasedOnExpertMode && dropBasedOnExpertMode.ruleForNormalMode is
                            OneFromOptionsNotScaledWithLuckDropRule oneFromOptionsDrop && oneFromOptionsDrop.dropIds.Contains(ItemID.HeatRay))
                    {
                        var original = oneFromOptionsDrop.dropIds.ToList();
                        original.Add(ModContent.ItemType<Scatterhook>());
                        oneFromOptionsDrop.dropIds = original.ToArray();
                    }
                }

                break;
        }
    }
}