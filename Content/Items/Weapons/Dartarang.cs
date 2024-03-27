using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Dartarang : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 25, 9, true);
        Item.width = 32;
        Item.height = 22;
        Item.rare = ItemRarityID.Blue;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 24;
        Item.knockBack = 3;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse != 2)
        {
            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = position,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.SilverBulletSparkle, settings);
        }

        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;
        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            type = ModContent.ProjectileType<DartBoomerang>();
            velocity *= 0.7f;
            damage += 3 * damage / 4;
        }
    }

    public override bool CanUseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.DD2_JavelinThrowersAttack;
        }
        else
        {
            Item.noUseGraphic = false;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);
        }

        return player.ownedProjectileCounts[ModContent.ProjectileType<DartBoomerang>()] == 0 && base.CanUseItem(player);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, 4f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.WoodenBoomerang)
            .AddIngredient<Dart>(99)
            .AddTile(TileID.Anvils)
            .Register();
    }
}