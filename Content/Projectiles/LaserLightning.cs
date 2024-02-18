using System;
using System.Collections.Generic;
using System.Linq;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class LaserLightning : ModProjectile
{
    private SlotId _soundSlot;
    private Vector2 _startLocation;
    private Player Owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.width = 40;
        Projectile.height = 40;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 60;
        Projectile.extraUpdates = 10;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        _startLocation = Projectile.Center;
    }

    public override void AI()
    {
        Projectile.Center = Main.MouseWorld;

        if (FindClosestTargets(600, 4) != null && Projectile.ai[0] % 5 == 0)
            foreach (NPC target in FindClosestTargets(600, 4))
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<HomingLightning>(),
                    Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI);

        Projectile.ai[0]++;

        if (Owner.channel)
            Projectile.timeLeft += 20;
        else if (Projectile.ai[0] > 60)
            Projectile.Kill();

        if (Projectile.ai[0] > 3000)
            Projectile.Kill();

        if (Projectile.ai[0] % 1500 == 0 || Projectile.ai[0] == 10f)
            _soundSlot = SoundEngine.PlaySound(AudioSystem.ReturnSound("electricity"), Projectile.Center);

        if (Projectile.ai[1] == 0)
            UpdatePlayer(Owner);

        _startLocation = Projectile.ai[1] == 0
            ? Owner.MountedCenter + new Vector2(66 * Owner.direction, 0).RotatedBy
                ((float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction))
            : new Vector2(Projectile.ai[1], Projectile.ai[2]);
    }

    public override bool? CanCutTiles()
    {
        return true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Texture2D texture2 = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Glowball").Value;
        Color col = Color.HotPink;
        col.A = 0;
        DrawLightning(texture, texture2, col, _startLocation, Projectile.Center, 0.022f, 60, 0.8f);
        return false;
    }

    private static void DrawLightning(Texture2D texture, Texture2D texture2, Color col, Vector2 source, Vector2 dest, float scale, float sway = 80f,
        float jaggednessNumerator = 1f)
    {
        List<Vector2> points = LightningSystem.CreateBolt(source, dest, sway, jaggednessNumerator);

        for (int i = 1; i < points.Count; i++)
        {
            Vector2 start = points[i - 1];
            Vector2 end = points[i];
            float numPoints = (end - start).Length() * 0.4f;

            for (int j = 0; j < numPoints; j++)
            {
                float lerp = j / numPoints;
                Vector2 drawPos = Vector2.Lerp(start, end, lerp);
                Lighting.AddLight(drawPos, Color.HotPink.ToVector3());
                Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, col, 0, texture.Size() / 2, scale,
                    SpriteEffects.None);
                Main.EntitySpriteDraw(texture2, drawPos - Main.screenPosition, null, new Color(255, 255, 255, 0), 0,
                    texture2.Size() / 2, scale * 0.3f, SpriteEffects.None); // white center
            }
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.immune[Projectile.owner] = 10;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float point = 0f;

        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center,
            Projectile.Center, 22, ref point);
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

    public override bool ShouldUpdatePosition()
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.TryGetActiveSound(_soundSlot, out ActiveSound activeSound);
        activeSound?.Stop();
    }

    private NPC[] FindClosestTargets(float maxDetectDistance, int numTargets)
    {
        var closestTargetsList = new SortedSet<KeyValuePair<float, NPC>>(Comparer<KeyValuePair<float, NPC>>.Create((x, y)
            => x.Key.CompareTo(y.Key)));

        float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

        for (int k = 0; k < Main.maxNPCs; k++)
        {
            NPC target = Main.npc[k];
            if (target.CanBeChasedBy())
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    closestTargetsList.Add(new KeyValuePair<float, NPC>(sqrDistanceToTarget, target));

                    // Remove the farthest target if the size exceeds numTargets
                    if (closestTargetsList.Count > numTargets) closestTargetsList.Remove(closestTargetsList.Last());
                }
            }
        }

        NPC[] closestTargets = closestTargetsList.Select(kvp => kvp.Value).ToArray();

        return closestTargets.Length > 0 ? closestTargets : null;
    }
}