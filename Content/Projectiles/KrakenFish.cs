using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class KrakenFish : ModProjectile
{
    private int _currentFlop;
    private int _flopCount = 4;
    private int _flopDirection = 1;

    private int _gravityDelay = 10;

    private bool _isFlopping;
    private float _type = 1;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Fish/1b";
    private float Fish => Projectile.ai[0];

    private int GravityDelayTimer
    {
        get => (int)Projectile.ai[2];
        set => Projectile.ai[2] = value;
    }
    
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 34;
        Projectile.height = 34;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
    }

    public override void OnSpawn(IEntitySource source)
    {
        _type = Fish;
        _gravityDelay += (int)_type;
        VisualSystem.SpawnDustPortal(Projectile.Center, Projectile.velocity, DustID.FishronWings);
    }

    private void StartFlopBehavior()
    {
        _isFlopping = true;
        _flopCount = 3; // Number of flops
        _flopDirection = 1; // Initial flop direction (upward)
        Projectile.velocity.Y = -8f; // Initial upward velocity
    }

    private void Flopping()
    {
        if (_isFlopping && _currentFlop < _flopCount)
        {
            Projectile.velocity.X *= 0.2f;

            Projectile.velocity.Y += _flopDirection * 0.4f; // Adjust the vertical velocity

            if (_flopDirection == 1 && Projectile.velocity.Y > 32f)
            {
                Projectile.velocity.Y = 8f; // Cap the upward velocity
                _flopDirection *= -1; // Reverse the flop direction
            }
            else if (_flopDirection == -1 && Projectile.velocity.Y < 32F)
            {
                Projectile.velocity.Y = 8f; // Cap the downward velocity
            }

            if (Projectile.velocity.Y * _flopDirection > 0f && Projectile.oldVelocity.Y * _flopDirection <= 0f) _flopDirection *= -1; // Reverse the flop direction
        }

        if (_currentFlop == _flopCount) Projectile.Kill();
    }

    public override void AI()
    {
        if (_type == 0)
            _type = 1;

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);

        GravityDelayTimer++;

        if (GravityDelayTimer >= _gravityDelay)
        {
            GravityDelayTimer = _gravityDelay;
            Projectile.velocity.X *= 0.98f;
            Projectile.velocity.Y += 0.40f;
        }

        if (Main.rand.NextBool(12)) Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FishronWings, Scale: 2f);

        Flopping();
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        for (int i = 0; i < 30; i++) Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FishronWings, Scale: 1.3f);

        SoundEngine.PlaySound(AudioSystem.ReturnSound("fish", 4f, 0.5f));

        switch (_type)
        {
            case <= 8:
                target.AddBuff(BuffID.Stinky, 120);
                break;

            case 15:
                target.AddBuff(BuffID.Frostburn, 120);
                break;

            case 16:
                target.AddBuff(BuffID.BloodButcherer, 120);
                break;

            case 21:
                target.AddBuff(BuffID.OnFire3, 120);
                break;

            case 23:
                target.AddBuff(BuffID.Lovestruck, 120);
                break;

            case 25:
                target.AddBuff(BuffID.Confused, 120);
                break;

            case 26:
                target.AddBuff(BuffID.Midas, 120);
                break;
        }

        StartFlopBehavior();

        if (_isFlopping && _currentFlop < _flopCount) _currentFlop++;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        for (int i = 0; i < 50; i++) Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FishronWings, Scale: 1.3f);

        SoundEngine.PlaySound(AudioSystem.ReturnSound("fish", 4f, 0.5f));

        StartFlopBehavior();

        if (_isFlopping && _currentFlop < _flopCount) _currentFlop++;

        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>($"DartGunsPlus/Content/Projectiles/Fish/{_type}b");
        
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 offset = texture.Size() / 2;
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + offset;
            float sizec = 0.9f * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.8f);
            Color color = lightColor * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            color.A = 0;
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], offset, sizec, SpriteEffects.None);
        }
        
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() / 2,
            Projectile.scale, SpriteEffects.None);

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}