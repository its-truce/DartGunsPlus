using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DartGunsPlus.Content.Projectiles;

public class Spaz : ModProjectile
{
    private NPC Target => Main.npc[(int)Projectile.ai[0]];
    private ref float RotationOffset => ref Projectile.localAI[0];
    private ref float RelaxTimer => ref Projectile.ai[1];
    private ref float RotationGoal => ref Projectile.ai[2]; // passed in as random from 0 to 2 pi

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 36;
        Projectile.height = 22;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        const int frameSpeed = 4;

        Projectile.frameCounter++;

        if (Projectile.frameCounter >= frameSpeed)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= Main.projFrames[Projectile.type]) Projectile.frame = 0;
        }

        Projectile.localAI[1]++; // general timer;

        if (Projectile.timeLeft > 580)
            FadingSystem.FadeIn(Projectile, 20);
        if (Projectile.timeLeft < 21)
            FadingSystem.FadeOut(Projectile, 20);

        Projectile.rotation = Projectile.DirectionTo(Target.Center).ToRotation();

        if (!Target.active)
        {
            if (Projectile.FindTargetWithinRange(2000) is not null)
                Projectile.ai[0] = Projectile.FindTargetWithinRange(2000).whoAmI; // target
            else
                Projectile.Kill();
        }

        const int dist = 120;

        if (RelaxTimer != 0)
        {
            RelaxTimer++;

            if (RelaxTimer % 3 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.DirectionTo(Target.Center) * 22,
                    Projectile.DirectionTo(Target.Center) * 6, ModContent.ProjectileType<SpazFlame>(), Projectile.damage, 2, Projectile.owner);
        }

        if (RelaxTimer == 30)
            RelaxTimer = 0;

        if (Projectile.localAI[1] % 180 == 0)
        {
            VisualSystem.SpawnDustCircle(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), 12, color: Color.LightGreen, scale: 0.9f);
            SoundEngine.PlaySound(AudioSystem.ReturnSound("flame"), Projectile.Center);
            RelaxTimer++;
            RotationGoal *= -1;
        }

        if (RelaxTimer == 0)
        {
            Projectile.Center = Vector2.Lerp(Projectile.Center, Target.Center + new Vector2(dist, 0).RotatedBy(RotationOffset), 0.04f);
            Projectile.velocity = Vector2.Zero;
            RotationOffset = (float)Utils.Lerp(RotationOffset, RotationGoal, 0.04f);
        }
    }
}