using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class LuminiteDart : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 22;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 6;
        Item.value = Item.sellPrice(copper: 20);
        Item.rare = ItemRarityID.Cyan;
        Item.shoot = ModContent.ProjectileType<LuminiteDartProj>();
        Item.shootSpeed = 8;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes()
    {
        CreateRecipe(333)
            .AddIngredient(ModContent.ItemType<Dart>(), 333)
            .AddIngredient(ItemID.LunarBar)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}

public class LuminiteDartProj : DartProjectile
{
    public override string Texture => "DartGunsPlus/Content/Items/Ammo/LuminiteDart";
    protected override int GravityDelay => 600;
    protected override bool EmitLight => true;
    protected override Color LightColor => Color.DarkTurquoise;
    private Player Owner => Main.player[Projectile.owner];

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LuminiteTrail>(),
            Projectile.damage / 2, 2, Projectile.owner, Projectile.whoAmI);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }

    public override void OnKill(int timeLeft)
    {
        if (Main.rand.NextBool(2))
            for (int i = 0; i < 3; i++)
            {
                Vector2 position;
                do
                {
                    position = Owner.position + new Vector2(Main.rand.Next(-70, 70) * Owner.direction * -1, Main.rand.Next(-30, 30));
                } while (Framing.GetTileSafely(position).HasTile);

                Projectile.NewProjectile(Projectile.GetSource_Death(), position, position.DirectionTo(Projectile.Center) * 2,
                    ModContent.ProjectileType<LuminiteStrike>(), Projectile.damage / 3, Projectile.knockBack);
            }
    }
}