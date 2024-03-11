using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Katanakaze : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 22, 7, true);
        Item.width = 72;
        Item.height = 18;
        Item.rare = ItemRarityID.Green;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 24;
        Item.knockBack = 3;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Dust.NewDustDirect(position, Item.width, Item.height, DustID.BlueFairy);
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }
}