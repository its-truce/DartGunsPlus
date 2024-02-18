using Microsoft.Xna.Framework;
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
        float progress = (float)player.itemAnimation / player.itemAnimationMax;
        player.itemRotation = (float)Utils.Lerp(player.itemRotation, initialItemRot - MathHelper.ToRadians(degrees * player.direction),
            1 - progress);
    }
}