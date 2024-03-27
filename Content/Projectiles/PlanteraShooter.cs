using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class PlanteraShooter : ModProjectile
{
    private readonly Texture2D _chainTexture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/PlanteraChain").Value;
    private Player Owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 30;
        Projectile.height = 30;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 600;
        Projectile.Opacity = 0;
    }

    public override void AI()
    {
        Projectile.Center = Owner.Center + new Vector2(75, 0).RotatedBy(Projectile.ai[0]);
        Projectile.ai[1]++;

        if (Projectile.ai[1] % 45 == 0 && Projectile.ai[1] != 0)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.Center.DirectionTo(Main.MouseWorld) * 8,
                ModContent.ProjectileType<PlanteraSeed>(), (int)Projectile.ai[2], 3, Owner.whoAmI);

            SoundEngine.PlaySound(SoundID.Grass with { MaxInstances = 2 }, Projectile.Center);
            VisualSystem.SpawnDustCircle(Projectile.Center, DustID.Grass);
        }


        Projectile.rotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation();

        if (Projectile.timeLeft > 570)
            FadingSystem.FadeIn(Projectile, 30);
        if (Projectile.timeLeft < 31)
            FadingSystem.FadeOut(Projectile, 30);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        VisualSystem.DrawLine(Main.spriteBatch, _chainTexture, Projectile.Center, Owner.Center, Color.White * Projectile.Opacity);
        return true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
}