using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class GoreNGlory : ModItem
{
    private float _initialItemRot;
    private int _shootCount;
    private int _swingDirection = 1;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 30, 14, true);
        Item.width = 84;
        Item.height = 30;
        Item.rare = ItemRarityID.Lime;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 90;
        Item.knockBack = 6;
        Item.reuseDelay = 5;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (_shootCount % 3 != 0)
        {
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI, 0f,
                _swingDirection); // sword
            _swingDirection *= -1;
        }
        else
        {
            int numProjectiles = Main.rand.Next(5, 8);

            for (int i = 0; i < numProjectiles; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(20));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);

                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            Projectile.NewProjectile(source, position + Vector2.Normalize(velocity) * Item.width * 0.7f, Vector2.Zero, ModContent.ProjectileType<ShotgunMusket>(),
                0, 0, player.whoAmI, velocity.ToRotation(), -2);

            velocity.Normalize();
            player.velocity = velocity * -4;
            CameraSystem.Screenshake(4, 5);
        }

        _initialItemRot = player.itemRotation;
        _shootCount++;
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (_shootCount % 3 != 0)
        {
            type = ModContent.ProjectileType<GoreHoldout>();
        }
        else
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset * 0.3f; // so u can hit while swinging
                position.Y += 2;
            }
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }

    public override bool CanUseItem(Player player)
    {
        if (_shootCount % 3 != 0)
        {
            Item.knockBack = 8;
            Item.UseSound = AudioSystem.ReturnSound("slash", 0.3f);
            Item.noUseGraphic = true;
            Item.reuseDelay = 6;
        }
        else
        {
            Item.knockBack = 6;
            Item.UseSound = AudioSystem.ReturnSound("ping", 0.3f);
            Item.noUseGraphic = false;
            Item.reuseDelay = 0;
        }

        return base.CanUseItem(player);
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    { 
        VisualSystem.RecoilAnimation(player, _initialItemRot, 40);
    }
}