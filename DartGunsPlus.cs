using DartGunsPlus.Content.Items.Weapons.Styx;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus;

public class DartGunsPlus : Mod
{
    public override void Load()
    {
        if (!Main.dedServ)
            if (Main.netMode != NetmodeID.Server)
                SkyManager.Instance["DartGunsPlus:StyxSky"] = new StyxSky();
    }
}