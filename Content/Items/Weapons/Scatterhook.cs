using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Scatterhook : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 55, 16, true);
        Item.width = 70;
        Item.height = 24;
        Item.rare = ItemRarityID.LightRed;

        Item.UseSound = SoundID.Item98;

        Item.damage = 40;
        Item.knockBack = 8f;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        const int numProjectiles = 6;

        if (player.altFunctionUse != 2)
        {
            for (var i = 0; i < numProjectiles; i++)
            {
                var newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                newVelocity *= 1f - Main.rand.NextFloat(0.5f);

                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            velocity.Normalize();
            player.velocity = velocity * -4;
            CameraSystem.Screenshake(4, 5);
            return false;
        }

        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        var muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;

        if (player.altFunctionUse == 2)
        {
            type = ModContent.ProjectileType<GolemHook>();
            position += muzzleOffset;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }
}