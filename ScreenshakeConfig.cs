using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;

namespace DartGunsPlus;

public class ScreenshakeConfig : ModConfig
{
    [Header("Screenshake")] [Increment(0.1f)] [Range(0f, 5f)] [DefaultValue(1f)] [Slider] [DrawTicks]
    public float ScreenshakeMultiplier;

    public override ConfigScope Mode => ConfigScope.ClientSide;

    [OnDeserialized]
    internal void OnDeserializedMethod(StreamingContext context)
    {
        ScreenshakeMultiplier = Utils.Clamp(ScreenshakeMultiplier, 0f, 10f);
    }
}