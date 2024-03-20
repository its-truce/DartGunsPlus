using System.Collections.Generic;
using DartGunsPlus.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace DartGunsPlus.Content.UI;

public class SerenityUIState : ModUIState
{
    private static Texture2D _textureBorder;
    private static Texture2D _textureBar;
    private static SerenityPlayer SerenityPlayer => Main.LocalPlayer.GetModPlayer<SerenityPlayer>();
    private static float Segment => SerenityPlayer.SerenityMax2 / 38f; // Player max flail charge divided by the flail bar texture width

    public override int InsertionIndex(List<GameInterfaceLayer> layers)
    {
        return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
    }

    public override void OnInitialize()
    {
        _textureBorder ??= ModContent.Request<Texture2D>("DartGunsPlus/Content/UI/EuphoriaBar", AssetRequestMode.ImmediateLoad).Value;
        _textureBar ??= ModContent.Request<Texture2D>("DartGunsPlus/Content/UI/SerenityFill", AssetRequestMode.ImmediateLoad).Value;

        Width.Set(0f, 0f);
        Height.Set(0f, 0f);
        Left.Set(Main.screenWidth / 2, 0f);
        Top.Set(Main.screenHeight / 2, 0f);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Player player = Main.LocalPlayer;

        if (!player.dead && player.HeldItem.ModItem is Serenity)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null,
                Main.Transform);

            Vector2 position = (player.position + new Vector2(player.width * 0.5f, player.gravDir > 0 ? player.height + player.gfxOffY : 10 + player.gfxOffY)).Floor();
            position = Vector2.Transform(position - Main.screenPosition, Main.GameViewMatrix.EffectMatrix * Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(_textureBorder, position, null, Color.White, 0f, _textureBorder.Size() * 0.5f, 1f, SpriteEffects.None,
                0f);

            Rectangle rectangle = _textureBar.Bounds;
            rectangle.Width = (int)(SerenityPlayer.SerenityCurrent / Segment);

            spriteBatch.Draw(_textureBar, position, rectangle, Color.White, 0f, _textureBar.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null,
                Main.UIScaleMatrix);
        }
    }
}