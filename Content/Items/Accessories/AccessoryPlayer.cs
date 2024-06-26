using System;
using System.Linq;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class AccessoryPlayer : ModPlayer
{
    private NPC[] _alreadySpawned = new NPC[1];
    public bool HasDartLicense;
    public bool HasKite;
    public bool HasShield;
    public bool HasSpyglass;
    public bool HasTranq;
    public bool IncrementShield;
    public int ShieldTimer;
    public bool IncrementWeakpoint;
    public int WeakpointTimer;

    public override void ResetEffects()
    {
        HasDartLicense = false;
        HasSpyglass = false;
        HasKite = false;
        HasShield = false;
        HasTranq = false;
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

            if (ShieldTimer >= 30)
            {
                IncrementShield = false;
                ShieldTimer = 0;
            }
        }

        if (HasSpyglass)
        {
            NPC closestTarget = DartUtils.FindClosestTarget(3200, _alreadySpawned, Player, 20);

            if (closestTarget != null &&
                !Main.projectile.Any(proj => proj.active && proj.owner == Player.whoAmI && proj.type == ModContent.ProjectileType<TargetCircle>() &&
                                             proj.ai[0] == closestTarget.whoAmI))
            {
                Projectile.NewProjectile(Player.GetSource_Misc("spyglass"), closestTarget.Center, Vector2.Zero,
                    ModContent.ProjectileType<TargetCircle>(), 0, 0, Player.whoAmI, closestTarget.whoAmI);

                Array.Resize(ref _alreadySpawned, _alreadySpawned.Length + 1);
                _alreadySpawned[^1] = closestTarget;
            }

            Main.NewText(WeakpointTimer);
            if (IncrementWeakpoint)
            {
                WeakpointTimer++;

                if (WeakpointTimer >= 60)
                {
                    IncrementWeakpoint = false;
                    WeakpointTimer = 0;
                }
            }
        }
    }
}