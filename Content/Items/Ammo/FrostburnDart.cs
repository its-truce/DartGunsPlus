using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class FrostburnDart : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 7;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 3;
        Item.value = Item.sellPrice(copper: 3);
        Item.rare = ItemRarityID.White;
        Item.shoot = ModContent.ProjectileType<FrostburnDartProj>();
        Item.shootSpeed = 3;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes()
    {
        CreateRecipe(10)
            .AddIngredient(ModContent.ItemType<Dart>(), 10)
            .AddIngredient(ItemID.IceTorch)
            .Register();
    }
}

public class FrostburnDartProj : DartProjectile
{
    public override string Texture => "DartGunsPlus/Content/Items/Ammo/FrostburnDart";
    protected override int GravityDelay => 15;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Dust.NewDust(target.Center, target.width, target.height, DustID.Dirt);

        if (Main.rand.NextBool(3))
        {
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<Explosion>(),
                damageDone / 2, hit.Knockback, Projectile.owner, 1); // ai0 = 1 for blue
            target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}