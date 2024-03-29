using System.Collections.Generic;
using DartGunsPlus.Content.Items.Weapons;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class GolemFist : ModProjectile
{
    private const int LaunchPhaseIndicator = 1;
    private readonly Texture2D _chainTexture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/GolemChain").Value;
    private Vector2 _initialPos;
    private Player Owner => Main.player[Projectile.owner];
    private bool LaunchPhase => Projectile.ai[2] == LaunchPhaseIndicator;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 14; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // recording mode
    }

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
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.hide = true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        _initialPos = Owner.Center + new Vector2(75, 0).RotatedBy(Projectile.ai[0]);
        Projectile.Center = _initialPos;
    }

    public override void AI()
    {
        _initialPos = Owner.Center + new Vector2(75, 0).RotatedBy(Projectile.ai[0]);

        if (Projectile.timeLeft > 570)
            FadingSystem.FadeIn(Projectile, 30);
        if (Projectile.timeLeft < 31)
            FadingSystem.FadeOut(Projectile, 30);

        if (Owner.ItemAnimationJustStarted && Owner.HeldItem.ModItem.Type == ModContent.ItemType<Scatterhook>())
            Projectile.ai[2] = LaunchPhaseIndicator;

        if (LaunchPhase)
        {
            Projectile.ai[1]++;
            Projectile.friendly = true;

            switch (Projectile.ai[1])
            {
                case 1:
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * -2;
                    break;

                case 25:
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<FistFlash>(), 0,
                        0, Projectile.owner, Projectile.whoAmI);
                    break;

                case 30:
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * 10;
                    break;

                case 60:
                    Projectile.velocity = Projectile.DirectionTo(_initialPos) * 5;
                    Projectile.friendly = false;
                    Projectile.ai[1] = 0;
                    Projectile.ai[2] = 0;
                    break;
            }
        }
        else
        {
            Projectile.rotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation();
            Projectile.velocity = Projectile.DirectionTo(_initialPos) * 10;
            Projectile.friendly = false;

            if (Vector2.Distance(Projectile.Center, _initialPos) < 5)
                Projectile.velocity = Vector2.Zero;
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers,
        List<int> overWiresUI)
    {
        behindProjectiles.Add(index);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        VisualSystem.DrawLine(Main.spriteBatch, _chainTexture, Projectile.Center, Owner.Center, Color.White * Projectile.Opacity);

        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        // Redraw the projectile with the color not influenced by light
        Vector2 drawOrigin = texture.Size() / 2;
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color color = Color.Lerp(Color.Orange, Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 
                                                   Projectile.Opacity * 0.5f, 0.75f);
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.9f);

            if (LaunchPhase)
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, sizec, SpriteEffects.None);
        }

        return true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.velocity = Projectile.DirectionTo(_initialPos) * 5;
        VisualSystem.SpawnDustCircle(target.Center, DustID.Torch, 8);
        CameraSystem.Screenshake(2, 3);

        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(),
            Projectile.damage, 7, Projectile.owner);
        Projectile.ai[1] = 0;
        Projectile.ai[2] = 0;

        int randDust = Main.rand.Next(10, 20);
        for (int i = 0; i < randDust; i++)
        {
            Dust dust = Dust.NewDustDirect(target.Center, 0, 0, DustID.Torch);
            dust.velocity *= 3.2f;
            dust.velocity.Y -= 1f;
            dust.velocity += Projectile.velocity;
            dust.noGravity = true;
        }
    }
}