using System;
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
    
    public static void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color col)
    {
        float rotation = start.DirectionTo(end).ToRotation();
        float scale = Vector2.Distance(start, end) / texture.Height;
        Vector2 origin = Vector2.Zero;

        spriteBatch.Draw(texture, start + new Vector2(0, 2) - Main.screenPosition, texture.Bounds, col, rotation, origin, new Vector2(scale, 1f),
            SpriteEffects.None, 0);
    }
}