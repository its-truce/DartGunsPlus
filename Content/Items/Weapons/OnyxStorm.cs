using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class OnyxStorm : ModItem
{
    private float _initialItemRot;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 48, 13, true);
        Item.width = 44;
        Item.height = 18;
        Item.rare = ItemRarityID.LightRed;

        Item.UseSound = AudioSystem.ReturnSound("onyx");

        Item.damage = 40;
        Item.knockBack = 6.5f;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectileDirect(source, position, velocity, ProjectileID.BlackBolt, damage * 2, knockback, player.whoAmI);

        const int numProjectiles = 4;

        for (int i = 0; i < numProjectiles; i++)
        {
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
            newVelocity *= 1f - Main.rand.NextFloat(0.5f);

            Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
        }

        Projectile.NewProjectile(source, position + Vector2.Normalize(velocity) * Item.width * 0.06f, Vector2.Zero, ModContent.ProjectileType<OnyxMusket>(),
            0, 0, player.whoAmI, velocity.ToRotation());
        Dust.NewDustDirect(position, 20, 20, DustID.PurpleCrystalShard);
        Dust.NewDustDirect(position, 20, 20, DustID.PurpleMoss);

        velocity.Normalize();
        player.velocity = velocity * -3;
        CameraSystem.Screenshake(5, 4);

        _initialItemRot = player.itemRotation;
        return false;
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

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        VisualSystem.RecoilAnimation(player, _initialItemRot, 20);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<DartStorm>())
            .AddIngredient(ItemID.DarkShard, 2)
            .AddIngredient(ItemID.SoulofNight, 10)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}