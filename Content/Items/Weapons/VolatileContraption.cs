using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class VolatileContraption : ModItem
{
    private int ShootCount;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 32, 12, true);
        Item.width = 66;
        Item.height = 28;
        Item.rare = ItemRarityID.Orange;

        Item.UseSound = SoundID.Item98;

        Item.damage = 55;
        Item.knockBack = 4;
        Item.channel = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        ShootCount++;
        if (ShootCount > 10)
        {
            CameraSystem.Screenshake(10, 4);
            ShootCount = 0;
        }

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

        if (ShootCount > 6 && ShootCount < 10)
            type = ModContent.ProjectileType<RedBomb>();
        else if (ShootCount == 10)
            type = ModContent.ProjectileType<RedLaser>();
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SoulofFright, 15)
            .AddIngredient(ItemID.HallowedBar, 10)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}