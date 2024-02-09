using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Systems;

public class FadingSystem : ModSystem
{
    public static void FadeIn(Projectile entity, int frames)
    {
        float amount = 1f / frames;
        if (entity.Opacity < 1f) entity.Opacity += amount;
    }

    public static void FadeOut(Projectile entity, int frames)
    {
        float amount = 1f / frames;
        if (entity.Opacity > 0f) entity.Opacity -= amount;
    }
}