using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class GoreHoldout : ModProjectile
{
    private int _hitTimer;
    private int _initialDirection;
    private bool _justHit;
    private Vector2 _storedVelocity = Vector2.Zero;
    private Player Owner => Main.player[Projectile.owner];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 42;
        Projectile.height = 44;
        Projectile.friendly = true;
        Projectile.timeLeft = 2;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ownerHitCheck = true;
        Projectile.extraUpdates = 1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 40;
        Projectile.light = 0.2f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        _storedVelocity = Projectile.velocity;
        _initialDirection = Owner.direction;
    }

    public override void AI()
    {
        if (Owner.active)
            Projectile.timeLeft = 2;

        Owner.ChangeDir(_initialDirection);
        Owner.heldProj = Projectile.whoAmI;
        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - (float)Math.PI / 2f);
        Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, 0f);
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;

        Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true) +
                            new Vector2((Projectile.width + Projectile.height) * 0.35f, 0f).RotatedBy(Projectile.velocity.ToRotation());
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.direction = _initialDirection;
        Projectile.spriteDirection = _initialDirection;

        if (Projectile.spriteDirection == -1)
            Projectile.rotation += (float)Math.PI - MathHelper.ToRadians(45f);
        else
            Projectile.rotation += MathHelper.ToRadians(45f);

        Projectile.velocity = _storedVelocity;

        if (Projectile.ai[0] <= 40f)
            Projectile.velocity = _initialDirection == -1
                ? Projectile.velocity.RotatedBy(MathHelper.ToRadians
                    (MathHelper.SmoothStep(-95f * Projectile.ai[1], 90f * Projectile.ai[1], Projectile.ai[0] / 40f)))
                : Projectile.velocity.RotatedBy(MathHelper.ToRadians
                    (MathHelper.SmoothStep(95f * Projectile.ai[1], -90f * Projectile.ai[1], Projectile.ai[0] / 40f)));
        else if (_initialDirection == -1)
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(90f * Projectile.ai[1],
                100f * Projectile.ai[1], (Projectile.ai[0] - 40f) / 30f)));
        else
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(-90f * Projectile.ai[1],
                -100f * Projectile.ai[1], (Projectile.ai[0] - 40f) / 30f)));

        Projectile.velocity.Normalize();

        float timerAdd = 1.65f;
        
        if (_justHit)
        {
            _hitTimer++;
            timerAdd = 0f;
        }
        if (_hitTimer > 10)
        {
            _justHit = false;
            timerAdd = 1.65f;
        }

        Projectile.ai[0] += timerAdd;

        if (Projectile.ai[0] > 70f)
            Projectile.Kill();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundStyle hitSound = AudioSystem.ReturnSound("metal", 1f, 0.3f);
        SoundEngine.PlaySound(hitSound, Projectile.Center);

        for (int i = 0; i < 70; i++)
            Dust.NewDust(target.position, target.width, target.height, DustID.Blood, Main.rand.NextFloat(Owner.direction * 1, Owner.direction * 3),
                Main.rand.NextFloat(-3f, 3f));

        CameraSystem.Screenshake(4, 4);
        _justHit = true;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (Projectile.ai[0] <= 40f)
            return null;
        return false;
    }

    public override bool? CanCutTiles()
    {
        if (Projectile.ai[0] <= 40f)
            return null;
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D trail = (Texture2D)ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/GoreTrail");

        Vector2 drawOrigin = projectileTexture.Size() / 2;
        Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
        Color color = Projectile.GetAlpha(lightColor);

        SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        for (int j = 0; j < Projectile.oldPos.Length; j++)
        {
            Vector2 drawPosEffect = Projectile.oldPos[j] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color colorAfterEffect = Projectile.GetAlpha(Color.Lerp(new Color(230, 211, 130, 50), new Color(194, 145, 85, 0),
                Projectile.ai[0] / 70f)) * ((float)(Projectile.oldPos.Length - j) / Projectile.oldPos.Length) * 0.5f;
            float afterAffectScale = Projectile.scale - (float)j / Projectile.oldPos.Length;
            Main.spriteBatch.Draw(trail, drawPosEffect, null, colorAfterEffect, Projectile.oldRot[j], drawOrigin, afterAffectScale, effects,
                0);
        }

        Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

        return false;
    }
}