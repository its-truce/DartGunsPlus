using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace DartGunsPlus.Content.Systems;

public static class VisualSystem
{
    public static void SpawnDustCircle(Vector2 center, int dust, int numDust = 10, float radius = 0.8f, float scale = 1f, Color color = default)
    {
        for (int i = 0; i < numDust; i++)
        {
            Vector2 speed = Main.rand.NextVector2CircularEdge(radius, radius);
            Dust d = Dust.NewDustPerfect(center + speed * 32, dust, speed * 2, newColor: color, Scale: scale);
            d.noGravity = true;
        }
    }

    private static Vector2 PositionOffset(Vector2 position, Vector2 velocity, float offset)
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

    public static void RecoilAnimation(Player player, float initialItemRot, float degrees)
    {
        float progress = Math.Clamp((float)player.itemAnimation / player.itemAnimationMax * 0.7f, 0, 1);

        player.itemRotation = (float)Utils.Lerp(player.itemRotation, initialItemRot - MathHelper.ToRadians(degrees * player.direction),
            1 - progress);

        if (player.itemAnimation < player.itemAnimationMax * 0.3f)
        {   
            float downProgress = Math.Clamp((player.itemAnimationMax * 0.7f - player.itemAnimation) / player.itemAnimationMax * 1.5f, 0, 1);
            player.itemRotation = (float)Utils.Lerp(player.itemRotation, initialItemRot,
                downProgress);
        }
    }

    public static void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color color, float scale = 1f, bool spriteFacingUpwards = true, 
        float bendingFactor = 0f)
    {
        float offset = spriteFacingUpwards ? MathF.PI / 2 : 0;
        float rotation = start.DirectionTo(end).ToRotation() + offset;
        
        Vector2 direction = end - start;
        float bendingRotation = MathF.Atan2(direction.Y, direction.X) + bendingFactor;

        float distance = Vector2.Distance(start, end);

        IEnumerable<Vector2> points = DartUtils.GetInterpolatedPoints(start, end, (int)(distance / (texture.Height * scale)) + 1);

        foreach (Vector2 point in points)
        {
            spriteBatch.Draw(texture, point - Main.screenPosition, texture.Bounds, color, rotation + bendingRotation, texture.Size() / 2,
                scale, SpriteEffects.None, 0);
        }
    }

}