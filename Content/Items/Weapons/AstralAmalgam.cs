using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class AstralAmalgam : ModItem
{
    private int ShootCount;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 20, 11, true);
        Item.width = 58;
        Item.height = 26;
        Item.rare = ItemRarityID.Orange;

        Item.UseSound = SoundID.Item98;

        Item.damage = 15;
        Item.knockBack = 3;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        ShootCount++;
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        var muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 2;
        }

        if (ShootCount % 4 == 0)
        {
            type = ModContent.ProjectileType<AstralBolt>();
            knockback = 0.75f;
            damage *= 2;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.MeteoriteBar, 25)
            .AddTile(TileID.Anvils)
            .Register();
    }
}