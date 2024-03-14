using System;
using System.Linq;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class AccessoryPlayer : ModPlayer
{
    private NPC[] _alreadySpawned = new NPC[1];
    public bool HasDartLicense;
    public bool HasSpyglass;
    public bool HasKite;
    public bool HasPlume;
    public bool HasShield;
    public bool IncrementShield;
    public int ShieldTimer;

    public override void ResetEffects()
    {
        HasDartLicense = false;
        HasSpyglass = false;
        HasKite = false;
        HasShield = false;
        _alreadySpawned = new NPC[1];
    }

    public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
    {
        if (!HasDartLicense)
            return;

        if (item.useAmmo == AmmoID.Dart || item.ammo == AmmoID.Dart) damage *= 1.1f;
    }

    public override void ModifyWeaponCrit(Item item, ref float crit)
    {
        if (!HasDartLicense)
            return;

        if (item.useAmmo == AmmoID.Dart || item.ammo == AmmoID.Dart)
            crit += 5;
    }

    public override void UpdateEquips()
    {
        if (HasShield && IncrementShield)
        {
            ShieldTimer++;
            
            if (ShieldTimer == 60)
            {
                IncrementShield = false;
                ShieldTimer = 0;
            }
        }

        if (HasSpyglass)
        {
            NPC closestTarget = DartUtils.FindClosestTarget(1600, _alreadySpawned, Player, 8);

            if (closestTarget != null && 
                !Main.projectile.Any(proj => proj.active && proj.owner == Player.whoAmI && proj.type == ModContent.ProjectileType<TargetCircle>() && 
                                            proj.ai[0] == closestTarget.whoAmI))
            {
                Projectile.NewProjectile(Player.GetSource_Misc("spyglass"), closestTarget.Center, Vector2.Zero,
                    ModContent.ProjectileType<TargetCircle>(), 0, 0, Player.whoAmI, closestTarget.whoAmI);
                
                Array.Resize(ref _alreadySpawned, _alreadySpawned.Length + 1);
                _alreadySpawned[^1] = closestTarget;
            }
        }
    }
}