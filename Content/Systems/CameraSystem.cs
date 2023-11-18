using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Systems;

public class CameraSystem : ModSystem
{
    public int screenshakeMagnitude;
    public int screenshakeTimer;

    public override void ModifyScreenPosition()
    {
        screenshakeTimer--;
        if (screenshakeTimer > 0)
            Main.screenPosition += new Vector2(Main.rand.Next(screenshakeMagnitude * -1, screenshakeMagnitude + 1),
                Main.rand.Next(screenshakeMagnitude * -1, screenshakeMagnitude + 1));
    }

    public static void Screenshake(int timer, int magnitude)
    {
        ModContent.GetInstance<CameraSystem>().screenshakeTimer = timer;
        ModContent.GetInstance<CameraSystem>().screenshakeMagnitude = magnitude;
    }
}