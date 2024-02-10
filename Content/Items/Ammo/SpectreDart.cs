using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class SpectreDart : ModItem
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
        Item.knockBack = 5f;
        Item.value = Item.sellPrice(copper: 14);
        Item.rare = ItemRarityID.Lime;
        Item.shoot = ModContent.ProjectileType<SpectreDartProj>();
        Item.shootSpeed = 5.5f;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes()
    {
        CreateRecipe(60)
            .AddIngredient(ModContent.ItemType<ChlorophyteDart>(), 60)
            .AddIngredient(ItemID.SpectreBar)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}

public class SpectreDartProj : DartProjectile
{
    private NPC[] _alreadyHit = new NPC[1];
    private float _velocityLength;

    public override string Texture => "DartGunsPlus/Content/Items/Ammo/SpectreDart";
    protected override bool EmitLight => true;
    protected override Color LightColor => Color.SkyBlue;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ModContent.ProjectileType<DartProjectile>());
        Projectile.penetrate = 3;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        if (Projectile.ai[0] == 4)
            _velocityLength = Projectile.velocity.Length() * 0.8f;

        NPC closestTarget = DartUtils.FindClosestTarget(1000, _alreadyHit, Projectile, 5);

        if (closestTarget is not null && Projectile.ai[0] >= 5)
            Projectile.velocity = Vector2.Lerp(Projectile.DirectionTo(closestTarget.Center),
                Vector2.Normalize(Projectile.velocity), 0.7f) * _velocityLength;

        Projectile.ai[0]++;

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
        Lighting.AddLight(Projectile.Center, LightColor.ToVector3());
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        for (int k = 0; k < Projectile.oldPos.Length; k++)
        for (float j = 0.0625f; j < 1; j += 0.0625f)
        {
            Vector2 oldPosForLerp = k > 0 ? Projectile.oldPos[k - 1] : Projectile.position;
            Vector2 lerpedPos = Vector2.Lerp(oldPosForLerp, Projectile.oldPos[k], j);
            lerpedPos += Projectile.Size / 2;
            lerpedPos -= Main.screenPosition;
            Color finalColor = Color.SkyBlue * 0.7f * (1 - k / (float)Projectile.oldPos.Length);
            finalColor.A = 0; // acts like additive blending without spritebatch stuff
            float scaleFactor = 0.65f - k * 0.01f;
            Main.EntitySpriteDraw(texture, lerpedPos, null, finalColor, Projectile.rotation, texture.Size() / 2, scaleFactor, SpriteEffects.None);
        }

        return true;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Array.Resize(ref _alreadyHit, _alreadyHit.Length + 1);
        _alreadyHit[^1] = target;
    }
}