using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class EmpressLaser : ModProjectile
{
    private const float MoveDistance = 60f;

    // The actual distance is stored in the ai0 field
    // By making a property to handle this it makes our life easier, and the accessibility more readable
    private float Distance
    {
        get => Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    private Projectile ParentProj => Main.projectile[(int)Projectile.ai[1]];

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        //Projectile.hide = true;
        Projectile.timeLeft = 120;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 5;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Color color = Color.Lerp(Main.DiscoColor, Color.White, 0.4f);
        color.A = 0;
        // We start drawing the laser if we have charged up
        DrawLaser(Main.spriteBatch, TextureAssets.Projectile[Projectile.type].Value, ParentProj.Center,
            Projectile.velocity, 10, -1.57f, 1f, color, (int)MoveDistance);

        return false;
    }

    // The core function of drawing a laser
    private void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, float rotation = 0f, float scale = 1f,
        Color color = default, int transDist = 50)
    {
        float r = unit.ToRotation() + rotation;

        // Draws the laser 'body'
        for (float i = transDist; i <= Distance; i += step)
        {
            Vector2 origin = start + i * unit;
            spriteBatch.Draw(texture, origin - Main.screenPosition,
                new Rectangle(0, 22, 20, 22), i < transDist ? Color.Transparent : color, r,
                texture.Size() / 2, scale, 0, 0);
        }

        // Draws the laser 'tail'
        spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
            new Rectangle(0, 0, 20, 22), color, r, texture.Size() / 2, scale, 0, 0);

        // Draws the laser 'head'
        spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
            new Rectangle(0, 56, 20, 22), color, r, texture.Size() / 2, scale, 0, 0);
    }

    // Change the way of collision check of the Projectile
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Vector2 unit = Projectile.velocity;
        float point = 0f;
        // Run an AABB versus Line check to look for collisions, look up AABB collision first to see how it works
        // It will look for collisions on the given line using AABB
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), ParentProj.Center,
            ParentProj.Center + unit * Distance, 22, ref point);
    }

    // Set custom immunity time on hitting an NPC
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.immune[Projectile.owner] = 5;
    }

    // The AI of the Projectile
    public override void AI()
    {
        Projectile.position = ParentProj.Center + Projectile.velocity * MoveDistance;

        UpdatePlayer();
        SetLaserPosition();
        SpawnDusts();
        CastLights();
    }

    private void SpawnDusts()
    {
        Vector2 dustPos = ParentProj.Center + Projectile.velocity * Distance;

        for (int i = 0; i < 2; ++i)
        {
            float num1 = Projectile.velocity.ToRotation() + (Main.rand.NextBool(2) ? -1.0f : 1.0f) * 1.57f;
            float num2 = (float)(Main.rand.NextDouble() * 0.8f + 1.0f);
            Vector2 dustVel = new((float)Math.Cos(num1) * num2, (float)Math.Sin(num1) * num2);
            Dust dust = Main.dust[Dust.NewDust(dustPos, 0, 0, DustID.RainbowRod, dustVel.X, dustVel.Y, newColor: Main.DiscoColor)];
            dust.noGravity = true;
            dust.scale = 1.2f;
        }

        if (Main.rand.NextBool(5))
        {
            Vector2 offset = Projectile.velocity.RotatedBy(1.57f) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
            Dust dust = Main.dust[Dust.NewDust(dustPos + offset - Vector2.One * 4f, 8, 8, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f)];
            dust.velocity *= 0.5f;
            dust.velocity.Y = -Math.Abs(dust.velocity.Y);
            Vector2 unit = dustPos - ParentProj.Center;
            unit.Normalize();
            dust = Main.dust[Dust.NewDust(ParentProj.Center + 55 * unit, 8, 8, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f)];
            dust.velocity *= 0.5f;
            dust.velocity.Y = -Math.Abs(dust.velocity.Y);
        }
    }

    // Sets the end of the laser position based on where it collides with something
    private void SetLaserPosition()
    {
        for (Distance = MoveDistance; Distance <= 2200f; Distance += 5f)
        {
            Vector2 start = ParentProj.Center + Projectile.velocity * Distance;
            if (!Collision.CanHit(ParentProj.Center, 1, 1, start, 1, 1))
            {
                Distance -= 5f;
                break;
            }
        }
    }

    private void UpdatePlayer()
    {
        // Multiplayer support here, only run this code if the client running it is the owner of the Projectile
        if (Projectile.owner == Main.myPlayer)
        {
            Vector2 diff = Main.MouseWorld - ParentProj.Center;
            diff.Normalize();
            Projectile.velocity = diff;
            Projectile.direction = Main.MouseWorld.X > ParentProj.position.X ? 1 : -1;
            Projectile.netUpdate = true;
        }

        //player.heldProj = Projectile.whoAmI; // Update player's held Projectile
    }

    private void CastLights()
    {
        // Cast a light along the line of the laser
        DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
        Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * (Distance - MoveDistance), 26, DelegateMethods.CastLight);
    }

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void CutTiles()
    {
        DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
        Vector2 unit = Projectile.velocity;
        Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Distance, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        Color red = new(241, 57, 57, 0);
        Color col = Color.Lerp(Color.White, red, 0.4f);
        return col;
    }
}