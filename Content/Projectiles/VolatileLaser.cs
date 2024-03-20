using System;
using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class VolatileLaser : Deathray
{
    private const float MaxCharge = 130f;

    private float _rayAlpha;
    private SlotId _soundSlot;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Deathray";
    private Player Owner => Main.player[Projectile.owner];
    private ref float Charge => ref Projectile.localAI[0];
    private bool IsAtMaxCharge => Charge == MaxCharge;

    private Vector2 Nozzle => Owner.MountedCenter + new Vector2(Owner.HeldItem.width * Owner.direction, -8).RotatedBy
        ((float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction));

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 1;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
        Projectile.friendly = false;
        Projectile.timeLeft = 430;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 5;
        MoveDistance = 0f;
        RealMaxDistance = 0f;
        BodyRect = new Rectangle(0, 0, Projectile.width, Projectile.height);
        HeadRect = new Rectangle(0, 0, Projectile.width, Projectile.height);
        TailRect = new Rectangle(0, 0, Projectile.width, Projectile.height);
    }

    public override void OnSpawn(IEntitySource source)
    {
        _soundSlot = SoundEngine.PlaySound(AudioSystem.ReturnSound("lasercharge", volume: 0.6f));
    }

    public override void PostDraw(Color lightColor)
    {
        if (IsAtMaxCharge)
        {
            _rayAlpha = MathHelper.Lerp(_rayAlpha, Projectile.timeLeft < 10 ? 0 : 1, 0.1f);

            int drawAlpha = Main.dayTime ? 50 : 0;
            DrawColor = new Color(255, 100, 100, drawAlpha) * _rayAlpha;

            DrawLaser(Main.spriteBatch, ModContent.Request<Texture2D>(Texture).Value, Position(), Projectile.velocity,
                BodyRect.Height, -1.57f, 0.4f, MaxDistance, (int)MoveDistance);

            Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Spotlight").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Glowball").Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, DrawColor, Projectile.ai[1], texture.Size() / 2,
                0.7f, SpriteEffects.None);

            DrawColor = new Color(255, 255, 255, 0) * _rayAlpha;

            DrawLaser(Main.spriteBatch, ModContent.Request<Texture2D>(Texture).Value, Position(), Projectile.velocity,
                BodyRect.Height, -1.57f, 0.1f, MaxDistance, (int)MoveDistance);
            Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, null, DrawColor,
                0, texture2.Size() / 2, 0.06f, SpriteEffects.None);
        }
    }

    public override void PostAI()
    {
        Owner.itemTime = 2;
        Owner.itemAnimation = 2;
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.Center = Position() + Projectile.velocity * MaxDistance;

        RealMaxDistance = Vector2.Distance(Main.MouseWorld, Position());

        UpdatePlayer(Owner);
        ChargeLaser(Owner);

        Projectile.ai[1] += MathHelper.ToRadians(2);

        if (IsAtMaxCharge)
        {
            Projectile.ai[2] = 1;
            CastLights();
            Projectile.friendly = true;
        }

        switch (Projectile.timeLeft)
        {
            case 300:
                SoundEngine.PlaySound(SoundID.MaxMana);
                VisualSystem.SpawnDustCircle(Nozzle, ModContent.DustType<GlowFastDecelerate>(), 14, color: Color.Red, scale: 0.6f);
                CameraSystem.Screenshake(5, 4);
                break;

            case 310:
                _soundSlot = SoundEngine.PlaySound(AudioSystem.ReturnSound("laserbeam", volume: 0.6f));
                break;
        }
    }

    protected override Vector2 Position()
    {
        return Owner.MountedCenter + new Vector2(Owner.HeldItem.width * Owner.direction * 0.2f, 0).RotatedBy
            ((float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction));
    }

    private void UpdatePlayer(Player player)
    {
        if (Projectile.owner == Main.myPlayer)
        {
            Vector2 diff = Main.MouseWorld - player.Center;
            diff.Normalize();
            Projectile.velocity = diff;
            Projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
            Projectile.netUpdate = true;
        }

        int dir = Projectile.direction;
        player.ChangeDir(dir); // Set player direction to where we are shooting
        player.heldProj = Projectile.whoAmI; // Update player's held Projectile
        player.itemTime = 2; // Set item time to 2 frames while we are used
        player.itemAnimation = 2; // Set item animation time to 2 frames while we are used
        player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * dir, Projectile.velocity.X * dir); // Set the item rotation to where we are shooting
    }

    private void ChargeLaser(Player player)
    {
        if (!player.channel)
        {
            Projectile.Kill();
        }
        else
        {
            if (Charge < MaxCharge) Charge++;
            int chargeFact = (int)(Charge / 20f);

            for (int k = 0; k < chargeFact + 1; k++)
            {
                Vector2 spawn = Nozzle + ((float)Main.rand.NextDouble() * MathF.Tau).ToRotationVector2() * (12f - chargeFact * 2);
                Dust dust = Dust.NewDustDirect(Nozzle, 20, 20, ModContent.DustType<GlowFastDecelerate>(), Projectile.velocity.X / 2f,
                    Projectile.velocity.Y / 2f, newColor: Color.Red);
                dust.velocity = Vector2.Normalize(Nozzle - spawn) * 1.5f * (10f - chargeFact * 2f) / 10f;
                dust.noGravity = true;
                dust.scale = Main.rand.Next(10, 20) * 0.03f;
            }
        }
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Projectile.Center.Distance(target.Center) < 30)
            modifiers.FinalDamage *= 2f;
    }

    private void CastLights()
    {
        // Cast a light along the line of the laser
        DelegateMethods.v3_1 = Color.HotPink.ToVector3();
        Utils.PlotTileLine(Projectile.Center, Position(), 26, DelegateMethods.CastLight);
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.TryGetActiveSound(_soundSlot, out ActiveSound activeSound);
        activeSound?.Stop();
        VisualSystem.SpawnDustCircle(Projectile.Center, ModContent.DustType<GlowFastDecelerate>(), 14, color: Color.Red, scale: 0.6f);
        SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, Projectile.Center);
    }
}