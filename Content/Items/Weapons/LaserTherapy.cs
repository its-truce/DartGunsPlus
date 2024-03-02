using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using DartGunsPlus.Content.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class LaserTherapy : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 22, 15, true);
        Item.width = 72;
        Item.height = 24;
        Item.rare = ItemRarityID.Red;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 144;
        Item.crit = 28;
        Item.knockBack = 6.5f;
        Item.channel = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 5;
        }

        if (player.GetModPlayer<LaserPlayer>().LaserCurrent == 10)
        {
            type = ModContent.ProjectileType<LaserLightning>();
            damage += damage / 2;
            player.GetModPlayer<LaserPlayer>().LaserCurrent = 0;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }
}