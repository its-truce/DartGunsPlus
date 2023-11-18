using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class MartianMarksman : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 22, 11, true);
        Item.width = 62;
        Item.height = 24;
        Item.rare = ItemRarityID.Lime;

        Item.UseSound = SoundID.Item98;

        Item.damage = 70;
        Item.knockBack = 5;
        Item.channel = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        var muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 5;
        }

        if (Main.rand.NextBool(3))
        {
            type = ModContent.ProjectileType<MartianLightning>();
            damage *= 2;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CopperBar, 12)
            .AddTile(TileID.Anvils)
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.TinBar, 12)
            .AddTile(TileID.Anvils)
            .Register();
    }
}