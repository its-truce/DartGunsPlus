using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DartGunsPlus.Content.Projectiles;

public class Probe : ModProjectile
{
    private NPC Target => Main.npc[(int)Projectile.ai[0]];
    private ref float RotationOffset => ref Projectile.localAI[0];
    private ref float RecoilTimer => ref Projectile.ai[1];
    private ref float RotationGoal => ref Projectile.ai[2]; // passed in as random from 0 to 2 pi
    
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 14; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // recording mode
    }
    
    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 22;
        Projectile.height = 22;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        Projectile.localAI[1]++; // general timer;
        
        if (Projectile.timeLeft > 580)
            FadingSystem.FadeIn(Projectile, 20);
        if (Projectile.timeLeft < 21)
            FadingSystem.FadeOut(Projectile, 20);

        Projectile.rotation = Projectile.DirectionTo(Target.Center).ToRotation();

        if (!Target.active)
            Projectile.Kill();
        
        const int dist = 150;
        
        if (RecoilTimer != 0)
            RecoilTimer++;
        if (RecoilTimer == 15)
            RecoilTimer = 0;
        
        if (Projectile.localAI[1] % 140 == 0)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.DirectionTo(Target.Center) * 6,
                ModContent.ProjectileType<VolatileBolt>(), Projectile.damage, 2, Projectile.owner);
            VisualSystem.SpawnDustCircle(Projectile.Center, DustID.RainbowRod, 12, color: Color.HotPink, scale: 0.9f);
            SoundEngine.PlaySound(AudioSystem.ReturnSound("shoot1"), Projectile.Center);
            
            Projectile.velocity = Projectile.DirectionFrom(Target.Center) * 6;
            RecoilTimer++;
            RotationGoal *= Main.rand.Next(-3, -1);
        }

        if (RecoilTimer == 0)
        {
            Projectile.Center = Vector2.Lerp(Projectile.Center, Target.Center + new Vector2(dist, 0).RotatedBy(RotationOffset), 0.04f);
            Projectile.velocity = Vector2.Zero;
            RotationOffset = (float)Utils.Lerp(RotationOffset, RotationGoal, 0.01f);
        }
    }
    
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        
        Vector2 drawOrigin = texture.Size() / 2;
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color color = new Color(255, 100, 100, 0) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * Projectile.Opacity * 0.5f;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.9f);
            
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, sizec, SpriteEffects.None);
        }

        return true;
    }
}