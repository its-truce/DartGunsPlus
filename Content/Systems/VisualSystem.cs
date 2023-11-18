using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace DartGunsPlus.Content.Systems;

public static class VisualSystem
{
    public static Texture2D ToGrayscale(this Texture2D originalTexture)
    {
        var graphicsDevice = Main.instance.GraphicsDevice;

        var originalPixels = new Color[originalTexture.Width * originalTexture.Height];
        originalTexture.GetData(originalPixels);

        var grayscalePixels = new Color[originalPixels.Length];

        for (var i = 0; i < originalPixels.Length; i++)
        {
            var grayscaleValue = (originalPixels[i].R + originalPixels[i].G + originalPixels[i].B) / 3;
            grayscalePixels[i] = new Color(grayscaleValue, grayscaleValue, grayscaleValue, originalPixels[i].A);
        }

        var grayscaleTexture = new Texture2D(graphicsDevice, originalTexture.Width, originalTexture.Height);
        grayscaleTexture.SetData(grayscalePixels);

        return grayscaleTexture;
    }

    public static void SpawnDustCircle(Vector2 center, int dust, int numDust = 10, float radius = 0.8f, float scale = 1f)
    {
        for (var i = 0; i < numDust; i++)
        {
            var speed = Main.rand.NextVector2CircularEdge(radius, radius);
            var d = Dust.NewDustPerfect(center + speed * 32, dust, speed * 2, Scale: scale);
            d.noGravity = true;
        }
    }

    public static float HueForParticle(Color color)
    {
        return Main.rgbToHsl(color).X * 255;
    }
}