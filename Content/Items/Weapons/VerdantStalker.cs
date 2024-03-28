using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class VerdantStalker : ModItem
{
    private float _initialItemRot;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 60, 17, true);
        Item.width = 52;
        Item.height = 20;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(silver: 95);

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 50;
        Item.reuseDelay = 2;
        Item.knockBack = 8.5f;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        ParticleOrchestraSettings settings = new()
        {
            PositionInWorld = position,
            MovementVector = Vector2.Zero,
            IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
        };
        ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.SilverBulletSparkle, settings);

        velocity.Normalize();
        player.velocity += velocity * -3;
        CameraSystem.Screenshake(5, 4);

        _initialItemRot = player.itemRotation;
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Stinger, 12)
            .AddIngredient(ItemID.Vine, 3)
            .AddIngredient(ItemID.JungleSpores, 12)
            .AddTile(TileID.Anvils)
            .Register();
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        VisualSystem.RecoilAnimation(player, _initialItemRot, 30);
    }
}