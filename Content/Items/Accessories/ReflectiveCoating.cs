using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class ReflectiveCoating : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 26;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 6);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        AccessoryPlayer accessoryPlayer = player.GetModPlayer<AccessoryPlayer>();

        accessoryPlayer.HasShield = true;
        Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Shield>(), 0, 0,
            player.whoAmI);
    }
}

public class ReflectProjectile : GlobalProjectile
{
    private bool _eligible;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse_WithAmmo use && ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart)
            _eligible = true;
    }

    public override void PostAI(Projectile projectile)
    {
        Player player = Main.player[projectile.owner];
        AccessoryPlayer accessoryPlayer = player.GetModPlayer<AccessoryPlayer>();

        if (!_eligible || !accessoryPlayer.HasShield)
            return;

        foreach (Projectile proj in Main.projectile.AsSpan(0, Main.maxProjectiles))
        {
            if (proj.hostile && proj.active &&
                new Rectangle((int)proj.position.X, (int)proj.position.Y, proj.width * 2, proj.height * 2).Intersects(projectile.Hitbox)
                && proj.Distance(player.Center) < 140 && accessoryPlayer.ShieldTimer == 0)
            {
                proj.velocity = proj.DirectionFrom(player.Center) * proj.velocity.Length() * 0.95f;
                PopupSystem.PopUp("Deflected!", Color.Pink, proj.Center - new Vector2(0, 50));
                accessoryPlayer.IncrementShield = true;
                
                ParticleOrchestraSettings settings = new()
                {
                    PositionInWorld = proj.Center,
                    MovementVector = Vector2.Zero,
                    IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
                };
                ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.ShimmerArrow, settings);
            }
        }
    }
}