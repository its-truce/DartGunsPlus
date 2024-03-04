using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DartGunsPlus.Content.Projectiles;

public class Retinazer : ModProjectile
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
        Projectile.width = 48;
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
            Projectile.Kill();
        
        const int dist = 120;
        
        if (RelaxTimer != 0)
            RelaxTimer++;
        if (RelaxTimer == 25)
            RelaxTimer = 0;
        
        if (Projectile.localAI[1] % 90 == 0)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.DirectionTo(Target.Center) * 90, 
                Vector2.Zero, ModContent.ProjectileType<RetinazerDash>(), Projectile.damage, 2, Projectile.owner, 
                Projectile.DirectionTo(Target.Center).X, Projectile.DirectionTo(Target.Center).Y, ai2: Projectile.rotation);
            VisualSystem.SpawnDustCircle(Projectile.Center, DustID.RainbowRod, 12, color: Color.HotPink, scale: 0.9f);
            SoundEngine.PlaySound(AudioSystem.ReturnSound("dash"), Projectile.Center);

            Projectile.Center += Projectile.DirectionTo(Target.Center) * 240;
            RelaxTimer++;
            RotationGoal *= Main.rand.Next(-3, -1);
        }

        if (RelaxTimer == 0)
        {
            Projectile.Center = Vector2.Lerp(Projectile.Center, Target.Center + new Vector2(dist, 0).RotatedBy(RotationOffset), 0.04f);
            Projectile.velocity = Vector2.Zero;
            RotationOffset = (float)Utils.Lerp(RotationOffset, RotationGoal, 0.01f);
        }
    }
}