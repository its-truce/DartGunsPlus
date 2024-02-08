using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class AccessoryPlayer : ModPlayer
{
    public bool HasDartLicense;
    public bool HasSpyglass;

    public override void ResetEffects()
    {
        HasDartLicense = false;
        HasSpyglass = false;
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
}