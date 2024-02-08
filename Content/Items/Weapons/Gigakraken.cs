using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Gigakraken : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 7, 14, true);
        Item.width = 58;
        Item.height = 30;
        Item.rare = ItemRarityID.Pink;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 24;
        Item.knockBack = 1;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);
        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(4));

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 5;
        }
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Dust.NewDustDirect(position, 20, 20, DustID.FishronWings);
        return true;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Lilkraken>()
            .AddIngredient(ItemID.IllegalGunParts)
            .AddIngredient(ItemID.BlackInk, 10)
            .AddIngredient(ItemID.SoulofMight)
            .AddTile(ItemID.MythrilAnvil)
            .Register();
    }
}