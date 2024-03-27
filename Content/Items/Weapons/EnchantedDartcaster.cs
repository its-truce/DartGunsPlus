using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class EnchantedDartcaster : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 22, 8, true);
        Item.width = 42;
        Item.height = 20;
        Item.rare = ItemRarityID.Green;

        Item.UseSound = SoundID.DD2_DarkMageHealImpact;

        Item.damage = 10;
        Item.knockBack = 4;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 5;
        }

        if (type == ModContent.ProjectileType<DartProjectile>())
        {
            velocity *= 0.8f;
            damage += damage / 2;
            type = ModContent.ProjectileType<EnchantedDart>();
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<DartZapper>())
            .AddIngredient(ItemID.FallenStar, 5)
            .AddTile(TileID.Anvils)
            .Register();
    }
}