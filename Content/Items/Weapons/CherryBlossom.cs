using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class CherryBlossom : ModItem
{
    private float _initialItemRot;
    private int _shootCount;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 42, 13, true);
        Item.width = 68;
        Item.height = 26;
        Item.rare = ItemRarityID.Orange;

        Item.UseSound = AudioSystem.ReturnSound("shotgun", 0.4f, 0.75f);

        Item.damage = 22;
        Item.knockBack = 6.5f;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        const int numProjectiles = 3;

        for (int i = 0; i < numProjectiles; i++)
        {
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(25));
            newVelocity *= 1f - Main.rand.NextFloat(0.3f);

            Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
        }

        Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ShotgunMusket>(), 0, 0, player.whoAmI,
            velocity.ToRotation(), 5);
        velocity.Normalize();
        player.velocity += velocity * -2.5f;
        CameraSystem.Screenshake(4, 3);

        _shootCount++;
        _initialItemRot = player.itemRotation;
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;

        if (_shootCount % 3 == 0)
        {
            type = ModContent.ProjectileType<Blossom>();
            velocity *= 1.3f;
            damage *= 2;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-8f, -2f);
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        VisualSystem.RecoilAnimation(player, _initialItemRot, 30);
    }
}