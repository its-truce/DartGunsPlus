using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class HarpyQuillstorm : ModItem
{
    private float _numProjectiles = 1;
    private int _shootCount;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 22, 11, true);
        Item.width = 52;
        Item.height = 20;
        Item.rare = ItemRarityID.LightRed;

        Item.UseSound = SoundID.DD2_BetsyWindAttack;

        Item.damage = 30;
        Item.knockBack = 3;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        _shootCount++;
        if (_numProjectiles < 4)
            _numProjectiles++;
        else
            _numProjectiles = 1;

        if (_numProjectiles > 1)
        {
            float rotation = MathHelper.ToRadians(15);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y));
            for (int i = 0; i < _numProjectiles; i++)
            {
                Vector2 perturbedSpeed =
                    new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation,
                        i / (_numProjectiles - 1))); // Watch out for dividing by 0 if there is only 1 Projectile.
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback);
            }

            return false;
        }

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

        if (_shootCount % 2 == 0)
        {
            type = ModContent.ProjectileType<Feather>();
            velocity *= 1.3f;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.GiantHarpyFeather)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}