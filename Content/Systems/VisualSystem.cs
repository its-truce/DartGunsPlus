using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace DartGunsPlus.Content.Systems;

public static class VisualSystem
{
    public static Texture2D ToGrayscale(this Texture2D originalTexture)
    {
        GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;

        var originalPixels = new Color[originalTexture.Width * originalTexture.Height];
        originalTexture.GetData(originalPixels);

        var grayscalePixels = new Color[originalPixels.Length];

        for (int i = 0; i < originalPixels.Length; i++)
        {
            int grayscaleValue = (originalPixels[i].R + originalPixels[i].G + originalPixels[i].B) / 3;
            grayscalePixels[i] = new Color(grayscaleValue, grayscaleValue, grayscaleValue, originalPixels[i].A);
        }

        Texture2D grayscaleTexture = new(graphicsDevice, originalTexture.Width, originalTexture.Height);
        grayscaleTexture.SetData(grayscalePixels);

        return grayscaleTexture;
    }

    public static void SpawnDustCircle(Vector2 center, int dust, int numDust = 10, float radius = 0.8f, float scale = 1f, Color color = default)
    {
        for (int i = 0; i < numDust; i++)
        {
            Vector2 speed = Main.rand.NextVector2CircularEdge(radius, radius);
            Dust d = Dust.NewDustPerfect(center + speed * 32, dust, speed * 2, newColor: color, Scale: scale);
            d.noGravity = true;
        }
    }

    public static Vector2 PositionOffset(Vector2 position, Vector2 velocity, float offset)
    {
        Vector2 offsetVector = velocity.SafeNormalize(Vector2.Zero) * offset;
        if (Collision.CanHitLine(position, 0, 0, position + offsetVector, 0, 0)) return position + offsetVector;
        return position;
    }

    public static void SpawnDustPortal(Vector2 position, Vector2 velocity, int dust, float scale = 1f, Color color = default)
    {
        for (int j = 0; j < 30; j++)
        {
            Dust dust1 = Dust.NewDustDirect(PositionOffset(position, velocity, 20), 0, 0, dust, newColor: color, Scale: scale);
            dust1.noGravity = true;
            dust1.position += Main.rand.NextVector2CircularEdge(5, 3.5f).RotatedBy(velocity.ToRotation() + MathHelper.PiOver2) * 2;
            dust1.velocity = velocity * .5f;
            dust1.fadeIn = 1f;
        }
    }

    public static float HueForParticle(Color color)
    {
        return Main.rgbToHsl(color).X * 255;
    }

    public static Point FindSharpTearsSpot(Vector2 targetSpot, Player player, Vector2 alternatePoint)
    {
        targetSpot.ToTileCoordinates();
        Point tileCoordinates = targetSpot.ToTileCoordinates();

        Vector2 center = player.Center;
        Vector2 endPoint = targetSpot;

        const int samplesToTake = 3;
        const float samplingWidth = 4f;
        Collision.AimingLaserScan(center, endPoint, samplingWidth, samplesToTake, out Vector2 vectorTowardsTarget, out float[] samples);

        float minSample = samples.Min();

        targetSpot = center + vectorTowardsTarget.SafeNormalize(Vector2.Zero) * minSample;

        Rectangle searchRectangle = new(tileCoordinates.X, tileCoordinates.Y, 1, 1);
        searchRectangle.Inflate(6, 16);
        Rectangle rectangle2 = new(0, 0, Main.maxTilesX, Main.maxTilesY);
        rectangle2.Inflate(-40, -40);

        searchRectangle = Rectangle.Intersect(searchRectangle, rectangle2);

        var solidPoints = new List<Point>();
        var nonSolidPoints = new List<Point>();

        for (int left = searchRectangle.Left; left <= searchRectangle.Right; ++left)
        for (int top = searchRectangle.Top; top <= searchRectangle.Bottom; ++top)
            if (WorldGen.SolidTile2(left, top))
            {
                Vector2 tileCenter = new(left * 16 + 8, top * 16 + 8);

                if (Vector2.Distance(targetSpot, tileCenter) <= 200.0)
                {
                    if (FindSharpTearsOpening(left, top, left > tileCoordinates.X, left < tileCoordinates.X, top > tileCoordinates.Y, top < tileCoordinates.Y))
                        solidPoints.Add(new Point(left, top));
                    else
                        nonSolidPoints.Add(new Point(left, top));
                }
            }

        if (solidPoints.Count == 0 && nonSolidPoints.Count == 0)
            solidPoints.Add((alternatePoint.ToTileCoordinates().ToVector2() + Main.rand.NextVector2Square(-2f, 2f)).ToPoint());

        List<Point> finalPoints = solidPoints.Count > 0 ? solidPoints : nonSolidPoints;
        int randomIndex = Main.rand.Next(finalPoints.Count);

        return finalPoints[randomIndex];
    }

    private static bool FindSharpTearsOpening(int x, int y, bool acceptLeft, bool acceptRight, bool acceptUp, bool acceptDown)
    {
        return (acceptLeft && !WorldGen.SolidTile(x - 1, y)) ||
               (acceptRight && !WorldGen.SolidTile(x + 1, y)) ||
               (acceptUp && !WorldGen.SolidTile(x, y - 1)) ||
               (acceptDown && !WorldGen.SolidTile(x, y + 1));
    }
}