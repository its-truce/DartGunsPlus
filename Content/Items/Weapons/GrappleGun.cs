using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class GrappleGun : ModItem
{

    public override void SetDefaults()
    {
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<Hook>(), 20, 12, true);
        Item.width = 32;
        Item.height = 24;
        Item.rare = ItemRarityID.Orange;

        Item.UseSound = AudioSystem.ReturnSound("shoot1");

        Item.damage = 15;
        Item.knockBack = 3;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 2;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }
}