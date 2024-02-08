using System.Collections.Generic;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class LilEmpress : ModProjectile
{
    private Player Owner => Main.player[Projectile.owner];

    private float RotationOffset
    {
        get => Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 6;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 36;
        Projectile.height = 44;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 1100;
        Projectile.Opacity = 0;
    }

    public override void OnSpawn(IEntitySource source)
    {
        var minionProjs = new List<Projectile>();

        foreach (Projectile proj in Main.projectile)
            if (proj.type == ModContent.ProjectileType<LilEmpress>() && proj.active && proj.owner == Projectile.owner)
                minionProjs.Add(proj);

        int i = 0;
        foreach (Projectile minion in minionProjs)
        {
            minion.ai[0] = i * MathHelper.ToRadians(360 / minionProjs.Count);
            i++;
        }
    }

    public override void OnKill(int timeLeft)
    {
        var minionProjs = new List<Projectile>();

        foreach (Projectile proj in Main.projectile)
            if (proj.type == ModContent.ProjectileType<LilEmpress>() && proj.active && proj.owner == Projectile.owner && proj.whoAmI != Projectile.whoAmI)
                minionProjs.Add(proj);

        int i = 0;
        foreach (Projectile minion in minionProjs)
        {
            minion.ai[0] = i * MathHelper.ToRadians(360 / minionProjs.Count);
            i++;
        }
    }

    public override void AI()
    {
        Projectile.direction = Projectile.spriteDirection = Owner.direction;

        if (Projectile.timeLeft > 1010)
            FadingSystem.FadeIn(Projectile, 60);
        if (Projectile.timeLeft < 61)
            FadingSystem.FadeOut(Projectile, 60);

        Projectile.ai[1]++;
        RotationOffset += MathHelper.ToRadians(1.5f);
        const int dist = 70;
        Projectile.Center = Owner.Center + new Vector2(dist, 0).RotatedBy(RotationOffset);
        Projectile.velocity = Vector2.Zero;

        if (Projectile.ai[1] % 300 == 0 && Projectile.ai[1] != 0)
        {
            SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
            VisualSystem.SpawnDustCircle(Projectile.Center, DustID.RainbowRod, 30, color: Main.DiscoColor);

            Vector2 spawnPos = Projectile.Center - new Vector2(10 * Owner.direction, 0);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), spawnPos, spawnPos.DirectionTo(Main.MouseWorld), ModContent.ProjectileType<EmpressLaser>(),
                Projectile.damage, 3, Projectile.owner, ai1: Projectile.whoAmI);
        }

        if (++Projectile.frameCounter >= 5)
        {
            Projectile.frameCounter = 0;
            Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
        }

        Color color = Color.Lerp(Main.DiscoColor, Color.White, 0.4f);
        Lighting.AddLight(Projectile.Center, color.ToVector3());
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Sparkle").Value;
        Color color = Color.Lerp(Main.DiscoColor, Color.White, 0.2f) * Projectile.Opacity;
        color.A = 0;
        Vector2 origin = texture.Size() / 2;
        Vector2 drawPos = Projectile.Center - new Vector2(0, 10) - Main.screenPosition;

        // sparkle
        Main.EntitySpriteDraw(texture, drawPos, null, color, MathHelper.ToRadians(Projectile.ai[1]), origin, 0.08f, SpriteEffects.None);

        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Glowball").Value;

        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 offset = Projectile.Size / 2;
            Vector2 drawPos2 = Projectile.oldPos[k] - Main.screenPosition + offset;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.8f);
            Color color2 = Color.Lerp(Main.DiscoColor, Color.White, 0.4f) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length)
                                                                          * Projectile.Opacity;
            color2.A = 0;

            if (Projectile.Center == Owner.Center + new Vector2(70, 0).RotatedBy(RotationOffset))
                Main.EntitySpriteDraw(texture2, drawPos2, null, color2, Projectile.oldRot[k], texture2.Size() / 2, sizec * 0.02f,
                    SpriteEffects.None);
        }

        return true;
    }
}