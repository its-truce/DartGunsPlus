using System;
using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class Serenity : ModItem
{
    private int _shootCount;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 10, 18, true);
        Item.width = 64;
        Item.height = 32;
        Item.rare = ItemRarityID.Yellow;

        Item.UseSound = SoundID.DD2_DarkMageAttack;

        Item.damage = 82;
        Item.knockBack = 5;
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
        ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.TerraBlade, settings);

        if (player.altFunctionUse != 2 || player.ownedProjectileCounts[ModContent.ProjectileType<TerraBoom>()] <= 0)
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: _shootCount);
        _shootCount++;

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
            type = ModContent.ProjectileType<TerraBoom>();
            damage *= 2;
            velocity *= 0.6f;

            SerenityPlayer serenityPlayer = player.GetModPlayer<SerenityPlayer>();
            serenityPlayer.SerenityCurrent++;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<TerraBoom>()] > 0)
            {
                foreach (Projectile proj in Main.projectile.AsSpan(0, Main.maxProjectiles))
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<TerraBoom>() && proj.owner == player.whoAmI && proj.ai[1] != 0)
                    {
                        NPC target = Main.npc[(int)proj.ai[0]];

                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 spawnPos = target.Center + new Vector2(200, 0).RotatedByRandom(Math.Tau);
                            Vector2 spawnVelocity = spawnPos.DirectionTo(target.Center) * 8;

                            Projectile.NewProjectile(proj.GetSource_FromAI(), spawnPos, spawnVelocity, ModContent.ProjectileType<TerraSlash>(), proj.damage * 2,
                                3, proj.owner, ai1: 1);
                        
                            proj.localAI[1] = 1; // retreat
                        }
                    }
                }
            }
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<TrueDysphoria>()
            .AddIngredient<TrueEuphoria>()
            .AddIngredient(ItemID.BrokenHeroSword)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}