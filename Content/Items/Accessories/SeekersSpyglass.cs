using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class SeekersSpyglass : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 32;
        Item.height = 28;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 2, silver: 67);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<AccessoryPlayer>().HasSpyglass = true;
    }
}

public class SpyglassProjectile : GlobalProjectile
{
    private bool _eligible;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse_WithAmmo use && ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart)
            _eligible = true;
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        Player player = Main.player[projectile.owner];
        AccessoryPlayer accessoryPlayer = player.GetModPlayer<AccessoryPlayer>();

        if (!_eligible || !accessoryPlayer.HasSpyglass)
            return;

        foreach (Projectile proj in Main.projectile.AsSpan(0, Main.maxProjectiles))
            if (proj.active && proj.type == ModContent.ProjectileType<TargetCircle>() && proj.owner == projectile.owner && proj.Hitbox.Intersects(projectile.Hitbox) &&
                proj.ai[0] == target.whoAmI)
            {
                modifiers.FinalDamage *= 2.5f;
                modifiers.Knockback *= 1.5f;
                PopupSystem.PopUp("Weakpoint!", Color.Red, proj.Center - new Vector2(0, 30));
            }
    }
}