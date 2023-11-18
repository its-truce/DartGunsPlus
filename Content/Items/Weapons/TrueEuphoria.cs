using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class TrueEuphoria : ModItem
{
    private int ShootCount;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 12, 18, true);
        Item.width = 66;
        Item.height = 40;
        Item.rare = ItemRarityID.Yellow;

        Item.UseSound = SoundID.Item98;

        Item.damage = 78;
        Item.knockBack = 4.5f;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        var settings = new ParticleOrchestraSettings
        {
            PositionInWorld = position,
            MovementVector = Vector2.Zero,
            IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
        };
        ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.Excalibur, settings);

        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: ShootCount);
        ShootCount++;
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        var muzzleOffset = Vector2.Normalize(velocity) * Item.width;

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
            .AddIngredient<Euphoria>()
            .AddIngredient(ItemID.ChlorophyteBar, 24)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}