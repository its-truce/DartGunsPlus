using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using DartGunsPlus.Content.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class GlacialGeyser : ModItem
{
    private int _freezeBoltCount;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 18, 12.5f, true);
        Item.width = 60;
        Item.height = 26;
        Item.rare = ItemRarityID.Lime;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 57;
        Item.knockBack = 2;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.25f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 3;
        }

        FrostPlayer frostPlayer = player.GetModPlayer<FrostPlayer>();

        if (frostPlayer.FrostCurrent == 10)
        {
            type = ModContent.ProjectileType<FreezeBolt>();
            velocity *= 0.2f;
            damage *= 2;
            _freezeBoltCount++;
        }

        if (_freezeBoltCount == 5)
            frostPlayer.FrostCurrent = 0;
    }

    public override bool CanUseItem(Player player)
    {
        FrostPlayer frostPlayer = player.GetModPlayer<FrostPlayer>();

        if (frostPlayer.FrostCurrent == 10)
        {
            Item.UseSound = SoundID.DD2_DarkMageCastHeal;
            Item.useTime = 40;
            Item.useAnimation = 40;
        }

        if (_freezeBoltCount == 5)
        {
            Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);
            Item.useTime = 18;
            Item.useAnimation = 18;
            _freezeBoltCount = 0;
        }
        
        return base.CanUseItem(player);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FrostCore, 3)
            .AddIngredient<Blazebringer>()
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}