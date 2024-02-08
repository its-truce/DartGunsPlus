using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Drawing;
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
        Item.shootSpeed = 7;
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
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";
    protected override int GravityDelay => 600;
    protected override bool EmitLight => true;
    protected override Color LightColor => Color.DarkTurquoise;
    private Player Owner => Main.player[Projectile.owner];

    public override void SetDefaults()
    {
        Projectile.netImportant = true;
        Projectile.tileCollide = true;
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.aiStyle = 43;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.hide = true;
        Projectile.extraUpdates = 180;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 6;
    }

    public override void AI()
    {
        ParticleOrchestraSettings settings = new()
        {
            PositionInWorld = Projectile.Center,
            MovementVector = Projectile.velocity,
            UniqueInfoPiece = 140
        };

        ParticleOrchestrator.BroadcastOrRequestParticleSpawn(ParticleOrchestraType.ChlorophyteLeafCrystalShot, settings);

        Lighting.AddLight(Projectile.Center, LightColor.ToVector3());
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        int randDust = Main.rand.Next(15, 31);
        for (int i = 0; i < randDust; i++)
        {
            Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Vortex, 0f, 0f, 100, Color.DarkTurquoise, 0.8f);
            dust.velocity *= 1.6f;
            dust.velocity.Y -= 1f;
            dust.velocity += Projectile.velocity;
            dust.noGravity = true;
        }

        Projectile.damage = (int)(Projectile.damage * 0.85f);
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

                Projectile.NewProjectile(Projectile.GetSource_Death(), position, position.DirectionTo(Main.MouseWorld) * 2, ModContent.ProjectileType<LuminiteStrike>(),
                    Projectile.damage / 3, Projectile.knockBack);
            }
    }
}