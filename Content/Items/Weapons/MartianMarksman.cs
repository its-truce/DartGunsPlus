using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using DartGunsPlus.Content.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class MartianMarksman : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 22, 12, true);
        Item.width = 60;
        Item.height = 24;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(gold: 9);

        Item.UseSound = AudioSystem.ReturnSound("scifishoot", 0.3f);

        Item.damage = 82;
        Item.crit = 20;
        Item.knockBack = 5;
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

        if (player.GetModPlayer<MartianPlayer>().MartianCurrent == 10)
        {
            type = ModContent.ProjectileType<MartianLightning>();
            damage *= 2;
            damage += damage / 4;
            player.GetModPlayer<MartianPlayer>().MartianCurrent = 0;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }
}