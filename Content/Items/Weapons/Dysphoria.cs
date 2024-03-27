using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Dysphoria : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 18, 18, true);
        Item.width = 52;
        Item.height = 32;
        Item.rare = ItemRarityID.Orange;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 20;
        Item.knockBack = 4;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.4f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, 6f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<VioletVigilante>())
            .AddIngredient(ModContent.ItemType<Katanakaze>())
            .AddIngredient(ModContent.ItemType<VerdantStalker>())
            .AddIngredient(ModContent.ItemType<Blazebringer>())
            .AddTile(TileID.DemonAltar)
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<CrimsonCobra>())
            .AddIngredient(ModContent.ItemType<Katanakaze>())
            .AddIngredient(ModContent.ItemType<VerdantStalker>())
            .AddIngredient(ModContent.ItemType<Blazebringer>())
            .AddTile(TileID.DemonAltar)
            .Register();
    }
}