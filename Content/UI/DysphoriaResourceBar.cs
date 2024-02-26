using System.Collections.Generic;
using DartGunsPlus.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace DartGunsPlus.Content.UI;

internal class DysphoriaResourceBar : UIState
{
    private UIElement _area;
    private UIImage _barFrame;
    private Color _gradientA;
    private Color _gradientB;
    private UIText _text;

    public override void OnInitialize()
    {
        _area = new UIElement();
        _area.Top.Set(90, 0f); // Placing it just a bit below the top of the screen.
        _area.Width.Set(182, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
        _area.Height.Set(60, 0f);
        _area.HAlign = _area.VAlign = 0.5f; // 1

        _barFrame = new UIImage(ModContent.Request<Texture2D>("DartGunsPlus/Content/UI/DysphoriaFrame")); // Frame of our resource bar
        _barFrame.Left.Set(22, 0f);
        _barFrame.Top.Set(0, 0f);
        _barFrame.Width.Set(138, 0f);
        _barFrame.Height.Set(34, 0f);

        _text = new UIText("", 0.8f); // text to show stat
        _text.Width.Set(138, 0f);
        _text.Height.Set(34, 0f);
        _text.Top.Set(40, 0f);
        _text.Left.Set(0, 0f);

        _gradientA = new Color(217, 103, 240);
        _gradientB = new Color(82, 183, 132);

        _area.Append(_text);
        _area.Append(_barFrame);
        Append(_area);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Main.LocalPlayer.HeldItem.ModItem is not TrueDysphoria)
            return;

        base.Draw(spriteBatch);
    }

    // Here we draw our UI
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        DysphoriaPlayer modPlayer = Main.LocalPlayer.GetModPlayer<DysphoriaPlayer>();

        float quotient = (float)modPlayer.DysphoriaCurrent / modPlayer.DysphoriaMax2;
        quotient = Utils.Clamp(quotient, 0f, 1f);

        Rectangle hitbox = _barFrame.GetInnerDimensions().ToRectangle();
        hitbox.X += 12;
        hitbox.Width -= 24;
        hitbox.Y += 8;
        hitbox.Height -= 16;

        int left = hitbox.Left;
        int right = hitbox.Right;
        int steps = (int)((right - left) * quotient);
        for (int i = 0; i < steps; i += 1)
        {
            float percent = (float)i / (right - left);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.Lerp(_gradientA, _gradientB, percent));
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (Main.LocalPlayer.HeldItem.ModItem is not TrueDysphoria)
            return;

        base.Update(gameTime);
    }
}

// This class will only be autoloaded/registered if we're not loading on a server
[Autoload(Side = ModSide.Client)]
internal class DysphoriaUISystem : ModSystem
{
    private DysphoriaResourceBar _dysphoriaBar;
    private UserInterface _dysphoriaResourceUserInterface;

    public override void Load()
    {
        _dysphoriaBar = new DysphoriaResourceBar();
        _dysphoriaResourceUserInterface = new UserInterface();
        _dysphoriaResourceUserInterface.SetState(_dysphoriaBar);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        _dysphoriaResourceUserInterface?.Update(gameTime);
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
        if (resourceBarIndex != -1)
            layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                "DartGunsPlus: Dysphoria Bar",
                delegate
                {
                    _dysphoriaResourceUserInterface.Draw(Main.spriteBatch, new GameTime());
                    return true;
                },
                InterfaceScaleType.UI)
            );
    }
}