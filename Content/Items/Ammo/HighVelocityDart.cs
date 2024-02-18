using System;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
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
    private NPC[] _alreadyHit = new NPC[1];
    public override string Texture => "DartGunsPlus/Content/Projectiles/Bolt";
    protected override int GravityDelay => 300;
    protected override bool EmitLight => true;
    protected override Color LightColor => Color.Gold;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

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
        Projectile.localNPCHitCooldown = 8;
        Projectile.usesLocalNPCImmunity = true;
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

        Array.Resize(ref _alreadyHit, _alreadyHit.Length + 1);
        _alreadyHit[^1] = target;

        NPC closestTarget = DartUtils.FindClosestTarget(600, _alreadyHit, Projectile, 4);
        if (closestTarget is not null)
            Projectile.velocity = Projectile.DirectionTo(closestTarget.Center) * Projectile.velocity.Length() * 0.85f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 offset = new(Projectile.width / 2f, Projectile.height / 2f);
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + offset;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.8f);
            Color color = new Color(210, 172, 71, 0) * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[k] - MathHelper.ToRadians(90), frame.Size() / 2, sizec * 0.18f,
                SpriteEffects.None);
        }

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}