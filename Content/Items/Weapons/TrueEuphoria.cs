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
    private float _initialItemRot;
    private int _shootCount;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 24, 16, true);
        Item.width = 70;
        Item.height = 34;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(gold: 8);

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 54;
        Item.knockBack = 4.5f;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse != 2 || player.ownedProjectileCounts[ModContent.ProjectileType<TerraBoom>()] <= 0)
        {
            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = position,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.Excalibur, settings);

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: _shootCount);
        }

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

        if (player.altFunctionUse == 2)
        {
            EuphoriaPlayer euphoriaPlayer = player.GetModPlayer<EuphoriaPlayer>();

            if (euphoriaPlayer.EuphoriaCurrent == euphoriaPlayer.EuphoriaMax2)
            {
                type = ModContent.ProjectileType<EuphoriaBoom>();
                damage *= 2;
                velocity *= 0.8f;
                euphoriaPlayer.EuphoriaCurrent = 0;
            }
            else
            {
                PopupSystem.PopUp("Not enough energy!", new Color(255, 255, 200), player.Center - new Vector2(0, 50));
            }
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, 4f);
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        if (player.ownedProjectileCounts[ModContent.ProjectileType<RevolvingSword>()] == 0 && player.altFunctionUse == 2)
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