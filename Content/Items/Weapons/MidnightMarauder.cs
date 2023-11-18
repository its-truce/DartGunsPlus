using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class MidnightMarauder : ModItem
{
    private readonly Color PurpleCol = new(50, 47, 125, 0);

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 18, 18, true);
        Item.width = 84;
        Item.height = 34;
        Item.rare = ItemRarityID.Orange;

        Item.UseSound = SoundID.Item98;

        Item.damage = 36;
        Item.knockBack = 4;
    }

    public static Vector2 PositionOffset(Vector2 position, Vector2 velocity, float offset)
    {
        var offsetVector = velocity.SafeNormalize(Vector2.Zero) * offset;
        if (Collision.CanHitLine(position, 0, 0, position + offsetVector, 0, 0)) return position + offsetVector;
        return position;
    }


    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        var settings = new ParticleOrchestraSettings
        {
            PositionInWorld = position,
            MovementVector = player.velocity,
            IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
        };
        ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.NightsEdge, settings);

        for (var i = 0; i < 30; i++)
        {
            var dust = Dust.NewDust(PositionOffset(position, velocity, 20), 0, 0, DustID.Shadowflame, newColor: PurpleCol);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].position += Main.rand.NextVector2CircularEdge(5, 3.5f).RotatedBy(velocity.ToRotation() + MathHelper.PiOver2) * 2;
            Main.dust[dust].velocity = velocity * .5f;
            Main.dust[dust].fadeIn = 1f;
        }

        for (var i = 0; i < 60; i++)
        {
            var dust1 = Dust.NewDust(position, 0, 0, DustID.Shadowflame);
            Main.dust[dust1].noGravity = true;
            Main.dust[dust1].position += Main.rand.NextVector2CircularEdge(12.5f, 4.5f).RotatedBy(velocity.ToRotation() + MathHelper.PiOver2) * 2;
            Main.dust[dust1].velocity = velocity * .35f;
            Main.dust[dust1].fadeIn = 1f;
            var dust2 = Dust.NewDust(PositionOffset(position, velocity, -10), 0, 0, DustID.Shadowflame, newColor: PurpleCol);
            Main.dust[dust2].noGravity = true;
            Main.dust[dust2].position += Main.rand.NextVector2CircularEdge(20, 5.5f).RotatedBy(velocity.ToRotation() + MathHelper.PiOver2) * 2;
            Main.dust[dust2].velocity = velocity * .2f;
            Main.dust[dust2].fadeIn = 1f;
        }

        // for (int i = 0; i < 100; i++)
        // {
        //     Vector2 rotate = Main.rand.NextVector2CircularEdge(10, 3.5f).RotatedBy(velocity.ToRotation() + MathHelper.PiOver2);
        //     int dust2 = Dust.NewDust(PositionOffset(position, velocity, -20), 0, 0, DustID.Shadowflame, newColor: PurpleCol);
        //     Main.dust[dust2].noGravity = true;
        //     Main.dust[dust2].velocity = rotate;
        //     Main.dust[dust2].fadeIn = 1f;
        // }
        return true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.4f);

        var muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<VioletVigilante>())
            .AddIngredient(ModContent.ItemType<Katanakaze>())
            .AddIngredient(ModContent.ItemType<VerdantStalker>())
            .AddIngredient(ModContent.ItemType<Blazebringer>())
            .AddTile(TileID.DemonAltar)
            .Register();

        CreateRecipe()
            .AddIngredient(ModContent.ItemType<CrimsonCobra>())
            .AddIngredient(ModContent.ItemType<Katanakaze>())
            .AddIngredient(ModContent.ItemType<VerdantStalker>())
            .AddIngredient(ModContent.ItemType<Blazebringer>())
            .AddTile(TileID.DemonAltar)
            .Register();
    }
}