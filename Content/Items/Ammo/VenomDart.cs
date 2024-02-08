using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class VenomDart : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 19;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 4.2f;
        Item.value = Item.sellPrice(copper: 18);
        Item.rare = ItemRarityID.Orange;
        Item.shoot = ModContent.ProjectileType<VenomDartProj>();
        Item.shootSpeed = 4.3f;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes()
    {
        CreateRecipe(35)
            .AddIngredient(ModContent.ItemType<Dart>(), 35)
            .AddIngredient(ItemID.VialofVenom)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}

public class VenomDartProj : DartProjectile
{
    public override string Texture => "DartGunsPlus/Content/Items/Ammo/VenomDart";
    protected override int GravityDelay => 22;
    protected override float GravityDecreaseY => 0.08f;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Venom, 600);

        if (Main.rand.NextBool(3))
            for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center + new Vector2(Main.rand.Next(-30, 30), Main.rand.Next(-30, 30)),
                    Vector2.Zero, ModContent.ProjectileType<VenomCloud>(), Projectile.damage / 2, 0, Projectile.owner);
    }
}