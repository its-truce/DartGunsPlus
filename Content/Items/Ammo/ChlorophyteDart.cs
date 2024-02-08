using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Ammo;

public class ChlorophyteDart : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 99;
    }

    public override void SetDefaults()
    {
        Item.damage = 10;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 4.5f;
        Item.value = Item.sellPrice(copper: 10);
        Item.rare = ItemRarityID.Lime;
        Item.shoot = ModContent.ProjectileType<ChlorophyteDartProj>();
        Item.shootSpeed = 5;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes()
    {
        CreateRecipe(60)
            .AddIngredient(ModContent.ItemType<Dart>(), 60)
            .AddIngredient(ItemID.ChlorophyteBar)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}

public class ChlorophyteDartProj : DartProjectile
{
    private float _velocityLength;
    public override string Texture => "DartGunsPlus/Content/Items/Ammo/ChlorophyteDart";
    protected override int GravityDelay => 30;
    protected override bool EmitLight => true;
    protected override Color LightColor => Color.Green;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
    }

    public override void AI()
    {
        if (Projectile.ai[0] == 7)
            _velocityLength = Projectile.velocity.Length() * 0.9f;

        if (Projectile.FindTargetWithLineOfSight(1000f) != -1 && Projectile.ai[0] >= 8) // homes in after 8 ticks have passed
        {
            Projectile.velocity = Vector2.Lerp(Projectile.DirectionTo(Main.npc[Projectile.FindTargetWithLineOfSight(1000f)].Center),
                Vector2.Normalize(Projectile.velocity), 0.75f) * _velocityLength;
        }
        else
        {
            if (GravityDelayTimer >= GravityDelay)
            {
                GravityDelayTimer = GravityDelay;
                Projectile.velocity.X -= GravityDecreaseX;
                Projectile.velocity.Y += GravityDecreaseY;
            }
        }

        GravityDelayTimer++;

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
            Color finalColor = Color.Green * 0.7f * (1 - k / (float)Projectile.oldPos.Length);
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

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Collision.HitTiles(Projectile.Center, oldVelocity, Projectile.width, Projectile.height);
        SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        return true;
    }
}