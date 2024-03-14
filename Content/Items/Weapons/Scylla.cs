using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Scylla : ModItem
{
    private float _initialItemRot;
    
    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
    }

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 35, 18, true);
        Item.width = 64;
        Item.height = 24;
        Item.rare = ItemRarityID.Red;

        Item.UseSound = AudioSystem.ReturnSound("scyllashoot", 0.4f);

        Item.damage = 81;
        Item.knockBack = 8f;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse != 2)
        {
            int numProjectiles = Main.rand.Next(7, 10);

            for (int i = 0; i < numProjectiles; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                newVelocity *= 1f - Main.rand.NextFloat(0.4f);

                int projToShoot = Main.rand.NextBool(4) ? ModContent.ProjectileType<VortexBeam>() : type;

                Projectile.NewProjectileDirect(source, position - Vector2.Normalize(velocity) * Item.width * 0.6f, newVelocity, projToShoot, damage, knockback, player.whoAmI);
            }

            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ShotgunMusket>(),
                0, 0, player.whoAmI, velocity.ToRotation(), 5, 2);

            Dust.NewDustDirect(position, 20, 20, ModContent.DustType<GlowFastDecelerate>(), newColor: Color.Turquoise, Scale: 0.5f);

            velocity.Normalize();
            player.velocity += velocity * -5;
            CameraSystem.Screenshake(6, 6);
        }
        else
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Hook>(), damage, 0, player.whoAmI);

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
        return new Vector2(-2, -2);
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        if (player.altFunctionUse != 2)
            VisualSystem.RecoilAnimation(player, _initialItemRot, 30);
    }
}