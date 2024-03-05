using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DartGunsPlus.Content.Items.Weapons.Styx;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace DartGunsPlus;

public class DartGunsPlus : Mod
{
    public override void Load()
    {
        if (!Main.dedServ)
            if (Main.netMode != NetmodeID.Server)
            {
                SkyManager.Instance["DartGunsPlus:StyxSky"] = new StyxSky();

                MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance)?.GetGetMethod(true);

                if (info != null)
                {
                    TmodFile file = (TmodFile)info.Invoke(ModContent.GetInstance<DartGunsPlus>(), null);

                    if (file != null)
                    {
                        IEnumerable<TmodFile.FileEntry> shaders = file.Where(n => n.Name.StartsWith("DartGunsPlus/Content/Effects/") &&
                                                                                  n.Name.EndsWith(".xnb"));

                        foreach (TmodFile.FileEntry entry in shaders)
                        {
                            string name = entry.Name.Replace(".xnb", "").Replace("DartGunsPlus/Content/Effects/", "");
                            string path = entry.Name.Replace(".xnb", "");
                            LoadShader(name, path);
                        }
                    }
                }
            }
    }

    private static void LoadShader(string name, string path)
    {
        var screenRef = new Ref<Effect>(ModContent.Request<Effect>(path, AssetRequestMode.ImmediateLoad).Value);
        Filters.Scene[name] = new Filter(new ScreenShaderData(screenRef, name + "Pass"), EffectPriority.High);
        Filters.Scene[name].Load();
    }
}