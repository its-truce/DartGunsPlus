using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class Dart : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 1;
        Item.value = Item.sellPrice(copper: 1);
        Item.rare = ItemRarityID.White;
        Item.shoot = ModContent.ProjectileType<DartProjectile>();
        Item.shootSpeed = 2;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes()
    {
        CreateRecipe(100)
            .AddIngredient(ItemID.TinBar)
            .Register();

        CreateRecipe(100)
            .AddIngredient(ItemID.CopperBar)
            .Register();
    }
}

public class DartProjectile : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Items/Ammo/Dart";

    protected int GravityDelayTimer
    {
        get => (int)Projectile.ai[0];
        set => Projectile.ai[0] = value;
    }

    /// <summary>
    ///     Determines the number of ticks after which the projectile starts to be affected by gravity. Defaults to 12
    ///     ticks.
    /// </summary>
    protected virtual int GravityDelay => 12;

    /// <summary>
    ///     Multiplier which determines how much the X component of the velocity of the projectile is decreased by every tick.
    ///     Defaults to 0.
    ///     Every tick after the projectile starts being affected by gravity, its velocity's X component is multiplied by (1 -
    ///     this property).
    /// </summary>
    protected virtual float GravityDecreaseX => 0f;

    /// <summary>
    ///     Determines how much the Y component of the velocity of the projectile is increased by every tick. Defaults to 0.1f.
    ///     Every tick after the projectile starts being affected by gravity, this is added to its velocity's Y component.
    /// </summary>
    protected virtual float GravityDecreaseY => 0.1f;

    /// <summary>Determines whether this projectile emits light. Set LightColor to a custom color if needed.</summary>
    protected virtual bool EmitLight => false;

    /// <summary>Determines the color the projectile emits. EmitLight must be set to true for this to work.</summary>
    protected virtual Color LightColor => Color.White;

    public override void SetDefaults()
    {
        Projectile.tileCollide = true;
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Ranged;
    }

    public override void AI()
    {
        GravityDelayTimer++;

        if (GravityDelayTimer >= GravityDelay)
        {
            GravityDelayTimer = GravityDelay;
            Projectile.velocity.X -= GravityDecreaseX;
            Projectile.velocity.Y += GravityDecreaseY;
        }

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

        if (EmitLight)
            Lighting.AddLight(Projectile.Center, LightColor.ToVector3());
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Collision.HitTiles(Projectile.Center, oldVelocity, Projectile.width, Projectile.height);
        SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Dust.NewDust(target.Center, target.width, target.height, DustID.Dirt);

        // zenith
        if (Projectile.type == ModContent.ProjectileType<DartProjectile>() && Projectile.ai[2] == -1)
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<ZapperLightning>(),
                damageDone * 3, hit.Knockback, Projectile.owner);
    }
}

public class DartNPC : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        if (shop.NpcType == NPCID.WitchDoctor)
            shop.Add<Dart>();
    }
}