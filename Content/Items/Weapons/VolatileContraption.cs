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
    private int _bombCount;
    private int _shootCount;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 32, 14, true);
        Item.width = 72;
        Item.height = 42;
        Item.rare = ItemRarityID.Pink;
        Item.value = Item.sellPrice(gold: 6);

        Item.UseSound = AudioSystem.ReturnSound("volatileshoot");

        Item.damage = 36;
        Item.crit = 20;
        Item.knockBack = 4;
        Item.channel = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        _shootCount++;

        if (_shootCount > 10)
        {
            CameraSystem.Screenshake(10, 4);
            _shootCount = 0;
        }

        if (type == ModContent.ProjectileType<VolatileBomb>())
        {
            _bombCount++;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: _bombCount);
            return false;
        }

        _bombCount = 0;
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 4;
        }

        type = _shootCount is > 6 and < 10 ? ModContent.ProjectileType<VolatileBomb>() : _shootCount == 10 ? ModContent.ProjectileType<VolatileLaser>() : type;

        if (_shootCount > 6)
            damage += damage / 2;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, 5);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SoulofFright, 10)
            .AddIngredient(ItemID.SoulofMight, 10)
            .AddIngredient(ItemID.SoulofSight, 10)
            .AddIngredient(ItemID.HallowedBar, 10)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}