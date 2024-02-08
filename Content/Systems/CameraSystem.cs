using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Systems;

public class CameraSystem : ModSystem
{
    public int ScreenshakeMagnitude;
    public int ScreenshakeTimer;

    public override void ModifyScreenPosition()
    {
        ScreenshakeTimer--;
        if (ScreenshakeTimer > 0)
            Main.screenPosition += new Vector2(Main.rand.Next(ScreenshakeMagnitude * -1, ScreenshakeMagnitude + 1),
                Main.rand.Next(ScreenshakeMagnitude * -1, ScreenshakeMagnitude + 1));
    }

    public static void Screenshake(int timer, int magnitude)
    {
        ModContent.GetInstance<CameraSystem>().ScreenshakeTimer = timer;
        ModContent.GetInstance<CameraSystem>().ScreenshakeMagnitude = magnitude;
    }
}