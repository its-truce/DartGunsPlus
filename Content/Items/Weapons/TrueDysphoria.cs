using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class TrueDysphoria : ModItem
{
    private int _shootCount;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 20, 16, true);
        Item.width = 60;
        Item.height = 36;
        Item.rare = ItemRarityID.Yellow;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 54;
        Item.knockBack = 4.5f;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        ParticleOrchestraSettings settings = new()
        {
            PositionInWorld = position,
            MovementVector = Vector2.Zero,
            IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
        };
        ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.NightsEdge, settings);

        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: _shootCount);
        _shootCount++;
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
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
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