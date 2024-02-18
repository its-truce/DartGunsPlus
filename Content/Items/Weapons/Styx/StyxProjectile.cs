using System.Collections.Generic;
using System.Linq;
using DartGunsPlus.Content.Items.Ammo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx;

public class StyxProjectile : GlobalProjectile
{
    private readonly int[] _excludedProjectiles =
    {
        ModContent.ProjectileType<HighVelocityDartProj>(),
        ModContent.ProjectileType<LuminiteDartProj>(), ModContent.ProjectileType<ExplosiveDartProj>()
    };

    private bool _increase = true;
    private int _itemType;

    private float _lerpTimer;
    private bool _styx;
    private Color LerpedColor => Color.Lerp(new Color(85, 105, 181, 0), new Color(42, 34, 80, 0), _lerpTimer);

    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return !_excludedProjectiles.Contains(entity.type);
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        switch (source)
        {
            case EntitySource_ItemUse_WithAmmo use when ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart &&
                                                        use.Item.type == ModContent.ItemType<Styx>() && !_excludedProjectiles.Contains(projectile.type):
                _itemType = use.Item.type;
                ProjectileID.Sets.TrailCacheLength[projectile.type] = 16; // how long you want the trail to be
                ProjectileID.Sets.TrailingMode[projectile.type] = 0; // recording mode
                break;

            case EntitySource_Misc { Context: "styx" }:
                _styx = true;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = -1;
                break;
        }
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (_styx)
            target.immune[projectile.owner] = 0;
        base.OnHitNPC(projectile, target, hit, damageDone);
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        Vector2 ownerCenter = Main.player[projectile.owner].Center;
        float distance = Vector2.Distance(ownerCenter, target.Center);

        var distanceMultipliers = new Dictionary<float, float>
        {
            { 100f, 1.8f },
            { 200f, 1.5f },
            { 1600f, 0.8f },
            { 3200f, 0.6f }
        };

        foreach (float threshold in distanceMultipliers.Keys.OrderByDescending(d => d))
            if (distance < threshold)
                modifiers.SourceDamage *= distanceMultipliers[threshold];
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        // for color lerping
        const float lerpingIncrement = 1f / 30;

        _increase = _lerpTimer switch
        {
            >= 1 => false,
            <= 0 => true,
            _ => _increase
        };

        if (_increase)
            _lerpTimer += lerpingIncrement;
        else
            _lerpTimer -= lerpingIncrement;

        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Bolt").Value;
        for (int k = 0; k < projectile.oldPos.Length; k++)
        {
            Vector2 offset = new(projectile.width / 2f, projectile.height / 2f);
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + offset;
            float sizec = projectile.scale * (projectile.oldPos.Length - k) / (projectile.oldPos.Length * 0.8f);
            Color color = LerpedColor * (1f - projectile.alpha) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);

            if (Vector2.Distance(projectile.oldPos[k], projectile.Center) > 90 && _itemType == ModContent.ItemType<Styx>())
                Main.EntitySpriteDraw(texture, drawPos, frame, color, projectile.oldVelocity.ToRotation(), frame.Size() / 2,
                    sizec * 0.18f, SpriteEffects.None);
        }

        return true;
    }

    public override Color? GetAlpha(Projectile projectile, Color lightColor)
    {
        if (_itemType == ModContent.ItemType<Styx>() && !_excludedProjectiles.Contains(projectile.type))
            return Color.White;

        return base.GetAlpha(projectile, lightColor);
    }
}