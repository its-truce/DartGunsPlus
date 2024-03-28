using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using DartGunsPlus.Content.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class TrueDysphoria : ModItem
{
    private float _initialItemRot;
    private int _shootCount;
    
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 14, 16, true);
        Item.width = 60;
        Item.height = 36;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(gold: 8);

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 30;
        Item.knockBack = 4.5f;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse != 2 || player.ownedProjectileCounts[ModContent.ProjectileType<DysphoriaMagnet>()] <= 0)
        {
            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = position,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.NightsEdge, settings);

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: _shootCount);
        }

        _shootCount++;
        _initialItemRot = player.itemRotation;
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y--;
        }

        if (player.altFunctionUse == 2)
        {
            DysphoriaPlayer dysphoriaPlayer = player.GetModPlayer<DysphoriaPlayer>();

            if (dysphoriaPlayer.DysphoriaCurrent == dysphoriaPlayer.DysphoriaMax2)
            {
                type = ModContent.ProjectileType<DysphoriaBoom>();
                velocity *= 0.8f;
                dysphoriaPlayer.DysphoriaCurrent = 0;
            }
            else
            {
                PopupSystem.PopUp("Not enough energy!", new Color(55, 224, 112), player.Center - new Vector2(0, 50));
            }
        }

        if (player.ownedProjectileCounts[ModContent.ProjectileType<RevolvingSword>()] > 0)
            damage -= damage / 3;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, 2f);
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        if (player.ownedProjectileCounts[ModContent.ProjectileType<DysphoriaMagnet>()] == 0 && player.altFunctionUse == 2)
            VisualSystem.RecoilAnimation(player, _initialItemRot, 20);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Dysphoria>()
            .AddIngredient(ItemID.SoulofFright, 20)
            .AddIngredient(ItemID.SoulofMight, 20)
            .AddIngredient(ItemID.SoulofSight, 20)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}