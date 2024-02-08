using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class FistFlash : ModProjectile
{
    private Projectile ParentProj => Main.projectile[(int)Projectile.ai[0]];

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 11;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        Projectile.Center = ParentProj.Center;
        Projectile.rotation = ParentProj.rotation;

        if (Projectile.timeLeft > 5)
            FadingSystem.FadeIn(Projectile, 5);
        if (Projectile.timeLeft < 6)
            FadingSystem.FadeOut(Projectile, 5);

        if (Projectile.timeLeft == 6)
            SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(255, 140, 12, 0) * Projectile.Opacity;
    }
}