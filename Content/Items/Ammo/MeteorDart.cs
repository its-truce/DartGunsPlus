using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class MeteorDart : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 9;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 1.3f;
        Item.value = Item.sellPrice(copper: 1);
        Item.rare = ItemRarityID.Blue;
        Item.shoot = ModContent.ProjectileType<MeteorDartProj>();
        Item.shootSpeed = 3.5f;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes()
    {
        CreateRecipe(75)
            .AddIngredient(ModContent.ItemType<Dart>(), 70)
            .AddIngredient(ItemID.MeteoriteBar)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class MeteorDartProj : DartProjectile
{
    public override string Texture => "DartGunsPlus/Content/Items/Ammo/MeteorDart";
    protected override int GravityDelay => 30;
    protected override float GravityDecreaseY => 0.03f;

    public override void SetDefaults()
    {
        Projectile.tileCollide = true;
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = 2;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.penetrate > 1)
        {
            Projectile.velocity.Y *= -1;
            Projectile.velocity *= 1.3f;
            Projectile.penetrate--;
            return false;
        }
        
        Collision.HitTiles(Projectile.Center, oldVelocity, Projectile.width, Projectile.height);
        SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

        return true;
    }
}