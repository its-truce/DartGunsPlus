using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using DartGunsPlus.Content.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class TrueEuphoria : ModItem
{
    private int _shootCount;
    private float _initialItemRot;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 24, 14, true);
        Item.width = 70;
        Item.height = 34;
        Item.rare = ItemRarityID.Yellow;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 68;
        Item.knockBack = 4.5f;
    }
    
    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        ParticleOrchestraSettings settings = new()
        {
            PositionInWorld = position,
            MovementVector = Vector2.Zero,
            IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
        };
        ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.Excalibur, settings);

        EuphoriaPlayer euphoriaPlayer = player.GetModPlayer<EuphoriaPlayer>();
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI,
            type == ModContent.ProjectileType<EuphoriaBoom>() ? 10 + euphoriaPlayer.EuphoriaCurrent * 20 : 0, 0, _shootCount);
        
        _shootCount++;
        _initialItemRot = player.itemRotation;
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y--;
        }

        if (Item.shoot == ModContent.ProjectileType<EuphoriaBoom>())
        {
            EuphoriaPlayer euphoriaPlayer = player.GetModPlayer<EuphoriaPlayer>();
            type = ModContent.ProjectileType<EuphoriaBoom>();
            damage += euphoriaPlayer.EuphoriaCurrent * 20;
            knockback *= 1.5f;
            velocity *= 0.9f;
            Item.shoot = ProjectileID.PurificationPowder;
        }
    }

    public override bool CanUseItem(Player player)
    {
        if (player.altFunctionUse == 2)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<RevolvingSword>()] == 0)
            {
                EuphoriaPlayer euphoriaPlayer = player.GetModPlayer<EuphoriaPlayer>();
                if (euphoriaPlayer.EuphoriaCurrent < 3)
                    euphoriaPlayer.EuphoriaCurrent++;
                Item.shoot = ModContent.ProjectileType<EuphoriaBoom>();
                Item.reuseDelay = 30;
            }
            else
                PopupSystem.PopUp("Swords already deployed!", new Color(255, 255, 200), player.Center - new Vector2(0, 70));
            
            return false;
        }

        Item.reuseDelay = 0;
        return base.CanUseItem(player);
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }
    
    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        if (player.ownedProjectileCounts[ModContent.ProjectileType<EuphoriaBoom>()] > 0)
            VisualSystem.RecoilAnimation(player, _initialItemRot, 20);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Euphoria>()
            .AddIngredient(ItemID.ChlorophyteBar, 24)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}