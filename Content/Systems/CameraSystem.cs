using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Systems;

public class CameraSystem : ModSystem
{
    private int _screenshakeMagnitude;
    private int _screenshakeTimer;

    public override void ModifyScreenPosition()
    {
        _screenshakeTimer--;
        if (_screenshakeTimer > 0)
            Main.screenPosition += new Vector2(Main.rand.Next(_screenshakeMagnitude * -1, _screenshakeMagnitude + 1),
                Main.rand.Next(_screenshakeMagnitude * -1, _screenshakeMagnitude + 1));
    }

    public static void Screenshake(int timer, int magnitude)
    {
        ModContent.GetInstance<CameraSystem>()._screenshakeTimer = timer;
        ModContent.GetInstance<CameraSystem>()._screenshakeMagnitude = magnitude;
    }
}