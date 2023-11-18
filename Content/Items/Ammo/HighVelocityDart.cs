using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class HighVelocityDart : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 11;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 5;
        Item.value = Item.sellPrice(copper: 8);
        Item.rare = ItemRarityID.Orange;
        Item.shoot = ModContent.ProjectileType<HighVelocityDartProj>();
        Item.shootSpeed = 4;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes()
    {
        CreateRecipe(50)
            .AddIngredient(ModContent.ItemType<Dart>(), 50)
            .AddIngredient(ItemID.Cog)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}

public class HighVelocityDartProj : DartProjectile
{
    public override string Texture => "DartGunsPlus/Content/Items/Ammo/HighVelocityDartProj";
    public override int GravityDelay => 300;
    public override bool EmitLight => true;
    public override Color LightColor => Color.Gold;

    public override void SetDefaults()
    {
        Projectile.tileCollide = true;
        Projectile.width = 4;
        Projectile.height = 4;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.extraUpdates = 6;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(3))
        {
            target.AddBuff(BuffID.BloodButcherer, 540);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ProjectileID.BloodButcherer, 0,
                0, Projectile.owner, 1, target.whoAmI);
        }

        Projectile.damage = (int)(Projectile.damage * 0.85f);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}