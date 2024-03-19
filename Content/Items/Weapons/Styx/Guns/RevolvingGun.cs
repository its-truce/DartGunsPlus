using System;
using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx.Guns;

public abstract class RevolvingGun : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = ProjTexture.Width;
        Projectile.height = ProjTexture.Height;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.timeLeft = 2;
        Projectile.Opacity = 0;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 10);
        int dirToMouse = (int)Math.Ceiling(Projectile.DirectionTo(Main.MouseWorld).X); // 1 = right, 0 = left;
        if (dirToMouse == 0)
            dirToMouse = -1;

        Projectile.direction = Projectile.spriteDirection = dirToMouse;

        if (Owner.HeldItem.ModItem is not Styx)
            Projectile.Kill();
        else
            Projectile.timeLeft += 2;

        float additionalRot = dirToMouse == -1 ? MathHelper.Pi * Projectile.direction : 0;
        Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation() + additionalRot;

        RotationOffset += MathHelper.ToRadians(0.5f);
        const int dist = 125;
        Projectile.Center = Owner.MountedCenter - new Vector2(0, dist).RotatedBy(RotationOffset);
        Projectile.velocity = Vector2.Zero;
        Lighting.AddLight(Projectile.Center, LightColor.ToVector3());

        if (Owner.ItemAnimationJustStarted && Owner.HeldItem.ModItem is Styx)
            Projectile.NewProjectile(Projectile.GetSource_Misc("styx"),
                Projectile.Center + new Vector2(ProjTexture.Width * Projectile.direction, 0).RotatedBy(Projectile.rotation) / 2,
                Projectile.DirectionTo(Main.MouseWorld) * ShotVelocity, ProjectileToShoot, Projectile.damage, Projectile.knockBack, Projectile.owner,
                InputsAI[0], InputsAI[1], InputsAI[2]);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Glow").Value;
        Color color = LightColor;
        color.A = 0;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Vector2 drawOrigin = texture.Size() / 2;

        Main.EntitySpriteDraw(texture, drawPos, null, color, 0, drawOrigin, 0.1f, SpriteEffects.None);
        return true;
    }

    // Resharper disable All
    public Texture2D ProjTexture => ModContent.Request<Texture2D>(Texture).Value;
    private Player Owner => Main.player[Projectile.owner];

    private float RotationOffset
    {
        get => Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    /// <summary>Determines the projectile to shoot when the player shoots with the Styx gun.</summary>
    protected virtual int ProjectileToShoot => ModContent.ProjectileType<DartProjectile>();

    /// <summary>Determines the velocity of the projectile which is shot when the player shoots with the Styx gun.</summary>
    protected virtual float ShotVelocity => 10f;

    /// <summary>Determines the AI array values used for spawning corresponding projectile.</summary>
    protected virtual float[] InputsAI => new float[] { 0, 0, 0 };

    /// <summary>Determines the color the projectile emits. EmitLight must be set to true for this to work.</summary>
    protected virtual Color LightColor => Color.White;
    // Resharper restore All
}