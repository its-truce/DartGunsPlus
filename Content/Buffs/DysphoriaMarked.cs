using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Buffs;

public class DysphoriaMarked : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.pvpBuff[Type] = true; // This buff can be applied by other players in Pvp, so we need this to be true.
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.GetGlobalNPC<DysphoriaNPC>().HitByPurpleDart = true;

        Dust.NewDust(npc.Center, npc.width, npc.height, DustID.WhiteTorch, newColor: new Color(185, 133, 240));
    }
}

public class DysphoriaNPC : GlobalNPC
{
    public bool HitByPurpleDart;
    public override bool InstancePerEntity => true;

    public override void ResetEffects(NPC npc)
    {
        HitByPurpleDart = false;
    }
}