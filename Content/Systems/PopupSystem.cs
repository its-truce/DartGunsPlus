using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Systems;

public class PopupSystem : ModSystem
{
    public static void PopUp(string text, Color color, Vector2 position)
    {
        AdvancedPopupRequest request = default;
        request.Text = text;
        float seconds = Math.Min(request.Text.Length / 7.5f, 4);
        request.DurationInFrames = (int)(seconds * 60);
        request.Color = color;
        PopupText.NewText(request, position);
    }
}