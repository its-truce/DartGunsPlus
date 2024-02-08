using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx;

public class StyxSky : CustomSky
{
    private bool _active;
    private float _intensity;
    private float _vel;

    public override void Update(GameTime gameTime)
    {
        const float alpha = 0.01f;
        if (_active)
        {
            _intensity += alpha;
            if (_intensity > 1f) _intensity = 1f;
            _vel = 0;
        }
        else
        {
            _intensity -= alpha;
            if (_intensity < 0f)
            {
                _intensity = 0f;
                Deactivate();
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        string path = Main.dayTime ? "StyxSkyDay" : "StyxSky";
        Texture2D skyTexture = ModContent.Request<Texture2D>($"DartGunsPlus/Content/Items/Weapons/Styx/{path}").Value;

        if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
        {
            Rectangle rect = new(0, /*Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 2400.) * 0.10000000149011612))*/ 0, Main.screenWidth,
                Main.screenHeight);
            spriteBatch.Draw(skyTexture, rect, null, Color.White * _intensity * 0.7f, 0f, new Vector2(_vel % 5184, 0),
                SpriteEffects.None, 0f);
            spriteBatch.Draw(skyTexture, rect, null, Color.White * _intensity * 0.7f, 0f, new Vector2(5184 + _vel % 5184, 0),
                SpriteEffects.None, 0f);
        }
    }

    public override float GetCloudAlpha()
    {
        return 1f - _intensity;
    }

    public override void Activate(Vector2 position, params object[] args)
    {
        _active = true;
    }

    public override void Deactivate(params object[] args)
    {
        _active = false;
        _vel = 0;
    }

    public override void Reset()
    {
        _active = false;
        _vel = 0;
    }

    public override bool IsActive()
    {
        return _active;
    }
}

public class AbosluleSky : ModSceneEffect
{
    private const SceneEffectPriority SetPriority = SceneEffectPriority.BossLow;

    public override SceneEffectPriority Priority => SetPriority;

    public override bool IsSceneEffectActive(Player player)
    {
        return player.HeldItem.ModItem is Styx;
    }

    public override void SpecialVisuals(Player player, bool isActive)
    {
        if (player.HeldItem.ModItem is Styx && isActive)
        {
            Main.numClouds = 10;
            SkyManager.Instance.Activate("DartGunsPlus:StyxSky");
            Main.BackgroundEnabled = true;
        }
        else
        {
            SkyManager.Instance.Deactivate("DartGunsPlus:StyxSky");
        }
    }
}