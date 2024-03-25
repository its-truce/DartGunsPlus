using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class HeavenlyDart : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 12;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 2;
        Item.value = Item.sellPrice(copper: 18);
        Item.rare = ItemRarityID.Orange;
        Item.shoot = ModContent.ProjectileType<HeavenlyDartProj>();
        Item.shootSpeed = 4;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes()
    {
        CreateRecipe(200)
            .AddIngredient(ModContent.ItemType<Dart>(), 200)
            .AddIngredient(ItemID.PixieDust, 3)
            .AddIngredient(ItemID.UnicornHorn)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}

public class HeavenlyDartProj : DartProjectile
{
    private float _velocityLength;
    public override string Texture => "DartGunsPlus/Content/Items/Ammo/HeavenlyDart";
    protected override int GravityDelay => 600;

    public override void SetDefaults()
    {
        Projectile.tileCollide = false;
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Ranged;
    }

    public override void OnSpawn(IEntitySource source)
    {
        _velocityLength = Projectile.velocity.Length();
    }

    public override void AI()
    {
        if (Projectile.velocity.Length() < _velocityLength * 1.5f)
            Projectile.velocity *= 1.01f;

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Dust.NewDust(target.Center, target.width, target.height, DustID.Dirt);

        for (int i = 0; i < 2; i++)
        {
            Vector2 spawnPos = target.Center - new Vector2(Main.rand.Next(-200, 200), 1000);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), spawnPos, spawnPos.DirectionTo(target.Center) * 11, ProjectileID.HallowStar,
                Projectile.damage / 3, 3, Projectile.owner);
        }
    }
}

public class HeavenlyDartItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        if (type == ModContent.ProjectileType<HeavenlyDartProj>())
        {
            position = Main.MouseWorld - new Vector2(Main.rand.Next(-250, 250), 400);
            velocity = position.DirectionTo(Main.MouseWorld) * velocity.Length();
        }

        base.ModifyShootStats(item, player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }

    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (type == ModContent.ProjectileType<HeavenlyDartProj>())
            player.itemRotation = Vector2.UnitY.ToRotation();


        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }
}