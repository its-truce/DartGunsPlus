using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class CrimsonCobra : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 23, 14, true);
        Item.width = 58;
        Item.height = 28;
        Item.rare = ItemRarityID.Blue;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 32;
        Item.knockBack = 5;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        ParticleOrchestraSettings settings = new()
        {
            PositionInWorld = position,
            MovementVector = Vector2.Zero,
            IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
        };
        ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.PrincessWeapon, settings);
        return true;
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
        return new Vector2(-2f, 4f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.CrimtaneBar, 12)
            .AddTile(TileID.Anvils)
            .Register();
    }
}