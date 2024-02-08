using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class BumbleBarrage : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 11, 8, true);
        Item.width = 52;
        Item.height = 26;
        Item.rare = ItemRarityID.Orange;

        Item.UseSound = SoundID.Item97;

        Item.damage = 5;
        Item.knockBack = 2;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int numProjectiles = Main.rand.Next(1, 3);

        for (int i = 0; i < numProjectiles; i++)
        {
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(20));
            newVelocity *= 1f - Main.rand.NextFloat(0.1f);

            int projType = Main.rand.NextBool(2) ? ProjectileID.Bee : type;
            if (player.strongBees && Main.rand.NextBool(3))
                projType = ProjectileID.GiantBee;

            Projectile.NewProjectileDirect(source, position, newVelocity, projType, damage + Main.rand.Next(0, 5), knockback, player.whoAmI);
        }

        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= Main.rand.Next(-1, 3);
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }
}