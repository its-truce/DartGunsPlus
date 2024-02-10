using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Systems;

public class DartUtils : ModSystem
{
    public static NPC FindClosestTarget(float maxDetectDistance, NPC[] alreadyHit, Projectile proj, int maxLength)
    {
        NPC closest = null;

        float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

        for (int k = 0; k < Main.maxNPCs; k++)
        {
            NPC target = Main.npc[k];
            if (target.CanBeChasedBy() && !alreadyHit.Contains(target) && alreadyHit.Length <= maxLength)
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, proj.Center);

                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closest = target;
                }
            }
        }

        return closest;
    }
    
    public static Vector2[] GetInterpolatedPoints(Vector2 start, Vector2 end, int numberOfPoints)
    {
        // Ensure numberOfPoints is at least 2 to include start and end points
        numberOfPoints = Math.Max(numberOfPoints, 2);

        var points = new Vector2[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            float t = i / (float)(numberOfPoints - 1); // Calculate interpolation factor

            // Use Vector2.Lerp to calculate the interpolated point
            points[i] = Vector2.Lerp(start, end, t);
        }

        return points;
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