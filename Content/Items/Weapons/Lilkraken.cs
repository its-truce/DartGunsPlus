using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Lilkraken : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 8, 9, true);
        Item.width = 52;
        Item.height = 30;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 4);

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 4;
        Item.knockBack = 0;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);
        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(1));

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 5;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }
}

public class LilkrakenNPC : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    { 
        if (shop.NpcType == NPCID.ArmsDealer)
            shop.Add<Lilkraken>();
    }
}