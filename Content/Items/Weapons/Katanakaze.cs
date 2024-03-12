using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Katanakaze : ModItem
{
    private int _swingDirection = 1;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
    }

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

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage - damage/3, knockback, player.whoAmI, 0, _swingDirection);
            _swingDirection *= -1;
            
            return false;
        }

        for (int i = 0; i < 3; i++)
        {
            Dust.NewDustDirect(position, Item.width, Item.height, ModContent.DustType<GlowFastDecelerate>(), newColor: Color.CornflowerBlue, Scale: 0.5f);
        }
        
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;

        if (player.altFunctionUse == 2)
            type = ModContent.ProjectileType<KatanaSwing>();
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }
    
    public override bool CanUseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            Item.knockBack = 6;
            Item.UseSound = AudioSystem.ReturnSound("swing", 0.3f);
            Item.noUseGraphic = true;
            Item.reuseDelay = 10;
        }
        else
        {
            Item.knockBack = 3;
            Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);
            Item.noUseGraphic = false;
            Item.reuseDelay = 0;
        }

        return base.CanUseItem(player);
    }
}