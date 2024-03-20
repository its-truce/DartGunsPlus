using System;
using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class ExplosiveDart : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 6;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 6.6f;
        Item.value = Item.sellPrice(copper: 8);
        Item.rare = ItemRarityID.Orange;
        Item.shoot = ModContent.ProjectileType<ExplosiveDartProj>();
        Item.shootSpeed = 4;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes()
    {
        CreateRecipe(50)
            .AddIngredient<Dart>(50)
            .AddIngredient(ItemID.ExplosivePowder)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}

public class ExplosiveDartProj : DartProjectile
{
    private static Color Color => Color.Lerp(new Color(255, 200, 48), new Color(255, 190, 65), Main.masterColor) * 0.4f;
    private Player Owner => Main.player[Projectile.owner];
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";
    protected override int GravityDelay => 40;
    protected override float GravityDecreaseY => 0.09f;
    protected override bool EmitLight => true;
    protected override Color LightColor => Color;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

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
        Projectile.extraUpdates = 1;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        // this is all taken from jeo

        Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Extra_98").Value;
        Rectangle frame = texture.Frame();
        Vector2 frameOrigin = frame.Size() / 2f;

        Texture2D texture2 = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle frame2 = texture2.Frame();
        Vector2 frameOrigin2 = frame2.Size() / 2f;

        Color col = Color;
        Color col2 = Color.Lerp(Color.White, Color.Black, Main.masterColor);
        Vector2 stretchscale = new(Projectile.scale * 1.4f + Main.masterColor / 2);

        for (int i = 1; i < Projectile.oldPos.Length - 1; i++)
        {
            col.A = 0;
            Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + new Vector2(Projectile.width / 2);
            Main.EntitySpriteDraw(texture, drawPos + Main.rand.NextVector2Circular(i / 2, i / 2), frame,
                new Color(col.R, col.G - i * 8, col.B, 0) * (1 - i * 0.04f), Projectile.oldRot[i] + Main.rand.NextFloat(-i * 0.01f, i * 0.01f),
                frameOrigin, new Vector2(stretchscale.X - i * 0.05f, stretchscale.Y * Main.rand.NextFloat(0.1f, 0.05f) * Vector2.Distance(Projectile.oldPos[i],
                    Projectile.oldPos[i + 1]) - i * 0.05f) * new Vector2(0.5f, 1.3f), SpriteEffects.None);
        }

        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, frame2, new Color(255, 0, 0, 0), Projectile.rotation, frameOrigin2,
            stretchscale * new Vector2(0.8f, 1.3f), SpriteEffects.None);
        col.A = 255;
        Main.EntitySpriteDraw(texture2, Projectile.Center - Main.screenPosition, frame2, col2 * Projectile.Opacity, Projectile.rotation, frameOrigin2,
            Projectile.scale * new Vector2(0.7f, 1.3f), SpriteEffects.None);

        return true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        for (int k = 0; k < Projectile.oldPos.Length; k++) Projectile.oldPos[k] = Projectile.position;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        VisualSystem.SpawnDustCircle(target.Center, ModContent.DustType<GlowFastDecelerate>(), color: Color, scale: 0.6f);
        target.velocity.Y -= 7 * target.knockBackResist;
    }

    public override void OnKill(int timeLeft)
    {
        if (Projectile.Center.Distance(Owner.Center) < 80)
        {
            CameraSystem.Screenshake(4, 6);
            Owner.maxRunSpeed += 2.5f;
            Owner.maxFallSpeed += 1000f;
            Owner.runAcceleration += 3f;

            float length = Projectile.oldVelocity.Length();
            if (Owner.velocity.Length() < new Vector2(length * 0.7f, 0).RotatedBy(Projectile.DirectionTo(Owner.Center).ToRotation()).Length() * 1.1f)
            {
                Owner.velocity += new Vector2(length * 0.7f, 0).RotatedBy(Projectile.DirectionTo(Owner.Center).ToRotation());
                Owner.velocity.Y -= 3f;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            Vector2 spawnPos = Projectile.Center + new Vector2(65, 0).RotatedByRandom(MathF.Tau);
            Projectile.NewProjectile(Projectile.GetSource_Death(), spawnPos, spawnPos.DirectionTo(Projectile.Center),
                ModContent.ProjectileType<SmallTrail>(), 0, 0, Projectile.owner, 255, 100, 55);
        }

        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RocketExplosion>(),
            Projectile.damage * 2, 8, Projectile.owner);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
}