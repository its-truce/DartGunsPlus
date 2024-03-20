using System;
using DartGunsPlus.Content.Dusts;
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

public class KatanaSwing : ModProjectile
{
    private int _hitTimer;
    private int _initialDirection;
    private bool _justHit;
    private int _parryTimer;
    private Vector2 _storedVelocity = Vector2.Zero;
    private Player Owner => Main.player[Projectile.owner];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 60;
        Projectile.friendly = true;
        Projectile.timeLeft = 2;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ownerHitCheck = true;
        Projectile.extraUpdates = 1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
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

        float timerAdd = 1.8f;

        if (_justHit)
        {
            _hitTimer++;
            timerAdd = 0f;
        }

        if (_hitTimer > 10)
        {
            _justHit = false;
            timerAdd = 1.8f;
        }

        Projectile.ai[0] += timerAdd;

        if (Projectile.ai[0] > 70f)
            Projectile.Kill();

        CheckForParry();
    }

    private void CheckForParry()
    {
        foreach (Projectile proj in Main.projectile.AsSpan(0, Main.maxProjectiles))
            if (proj.active && proj.hostile && proj.Hitbox.Intersects(Projectile.Hitbox) && _parryTimer == 0)
            {
                _parryTimer++;
                proj.velocity *= -1.25f;

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center - new Vector2(0, 30), Vector2.Zero,
                    ModContent.ProjectileType<Parry>(), 0, 0, Owner.whoAmI);

                SoundEngine.PlaySound(AudioSystem.ReturnSound("indicator"), Projectile.Center);

                for (int i = 0; i < 15; i++)
                    Dust.NewDust(proj.position, Projectile.width, Projectile.height, ModContent.DustType<GlowFastDecelerate>(),
                        Main.rand.NextFloat(Owner.direction * 1, Owner.direction * 3), Main.rand.NextFloat(-3f, 3f), newColor: Color.CornflowerBlue,
                        Scale: 0.4f);

                CameraSystem.Screenshake(8, 4);
                _justHit = true;

                Owner.AddBuff(BuffID.Swiftness, 60);
                Owner.AddBuff(BuffID.Regeneration, 120);

                const int numProjectiles = 2;

                Vector2 velocity = Owner.DirectionTo(Main.MouseWorld) * 8;
                Vector2 position = Owner.Center + new Vector2(40, 0).RotatedBy(velocity.ToRotation());

                float rotation = MathHelper.ToRadians(20);
                position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y));

                for (int i = 0; i < numProjectiles; i++)
                {
                    Vector2 perturbedSpeed =
                        new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation,
                            i / (numProjectiles - 1)));
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, perturbedSpeed, ModContent.ProjectileType<KatanaParry>(),
                        proj.damage + proj.damage / 2, 3, Owner.whoAmI);
                }

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<KatanaParry>(),
                    proj.damage + proj.damage / 2, 3, Owner.whoAmI);
            }

        if (_parryTimer != 0)
            _parryTimer++;
        if (_parryTimer == 30)
            _parryTimer = 0;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        SoundEngine.PlaySound(AudioSystem.ReturnSound("metal"), Projectile.Center);

        for (int i = 0; i < 15; i++)
            Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<GlowFastDecelerate>(),
                Main.rand.NextFloat(Owner.direction * 1, Owner.direction * 3), Main.rand.NextFloat(-3f, 3f), newColor: Color.CornflowerBlue, Scale: 0.4f);

        CameraSystem.Screenshake(4, 2);
        _justHit = true;

        Vector2 randomPointWithinHitbox = Main.rand.NextVector2FromRectangle(target.Hitbox);
        Vector2 hitboxCenter = target.Hitbox.Center.ToVector2();
        Vector2 directionVector = (hitboxCenter - randomPointWithinHitbox).SafeNormalize(new Vector2(Owner.direction, Owner.gravDir)) * 8f;

        float randomRotationAngle = (Main.rand.Next(2) * 2 - 1) * ((float)Math.PI / 5f + (float)Math.PI * 4f / 5f * Main.rand.NextFloat());
        randomRotationAngle *= 0.5f;

        directionVector = directionVector.RotatedBy(0.7853981852531433);

        const int rotationSteps = 30;
        const int numIterations = 15;

        Vector2 currentPosition = hitboxCenter;

        for (int i = 0; i < numIterations; i++)
        {
            currentPosition -= directionVector;
            directionVector = directionVector.RotatedBy((0f - randomRotationAngle) / rotationSteps);
        }

        currentPosition += target.velocity * 3;

        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), currentPosition, directionVector, 977, Projectile.damage / 3, 0f,
            Owner.whoAmI, randomRotationAngle);
    }

    public override bool? CanHitNPC(NPC target)
    {
        return Projectile.ai[0] <= 40 ? null : false;
    }

    public override bool? CanCutTiles()
    {
        return Projectile.ai[0] <= 40 ? null : false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D trail = (Texture2D)ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/KatanaTrail");

        Vector2 drawOrigin = projectileTexture.Size() / 2;
        Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
        Color color = Projectile.GetAlpha(lightColor);

        SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        for (int j = 0; j < Projectile.oldPos.Length; j++)
        {
            Vector2 drawPosEffect = Projectile.oldPos[j] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color colorAfterEffect = Projectile.GetAlpha(Color.Lerp(new Color(162, 180, 255, 50), new Color(50, 90, 255, 0),
                Projectile.ai[0] / 70f)) * ((float)(Projectile.oldPos.Length - j) / Projectile.oldPos.Length) * 0.5f;
            float afterAffectScale = Projectile.scale - (float)j / Projectile.oldPos.Length;
            Main.spriteBatch.Draw(trail, drawPosEffect, null, colorAfterEffect, Projectile.oldRot[j], drawOrigin, afterAffectScale, effects,
                0);
        }

        Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

        return false;
    }

    public override void OnKill(int timeLeft)
    {
        if (Projectile.ai[2] == 0) // spawn another
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Owner.Center, Owner.DirectionTo(Main.MouseWorld), Projectile.type, Projectile.damage,
                Projectile.knockBack, Owner.whoAmI, ai1: Projectile.ai[1] * -1, ai2: 1);
            SoundEngine.PlaySound(AudioSystem.ReturnSound("swing"), Owner.Center);
        }
    }
}