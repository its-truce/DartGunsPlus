using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class BumbleBarrage : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 11, 8, true);
        Item.width = 52;
        Item.height = 26;
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.sellPrice(gold: 2);

        Item.UseSound = SoundID.Item97;

        Item.damage = 4;
        Item.knockBack = 2;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int numProjectiles = Main.rand.Next(1, 4);

        for (int i = 0; i < numProjectiles; i++)
        {
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(20));
            newVelocity *= 1f - Main.rand.NextFloat(0.1f);

            int projType = Main.rand.NextBool(4, 9) ? player.beeType() : type;
            int projDamage = projType == player.beeType() ? player.beeDamage(damage) : damage;
            float projKnockback = projType == player.beeType() ? player.beeKB(knockback) : knockback;

            Projectile.NewProjectileDirect(source, position, newVelocity, projType, projDamage + Main.rand.Next(0, 5), projKnockback, player.whoAmI);
        }

        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.8f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= Main.rand.Next(-1, 3);
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }
}

public class BeeTrail : GlobalProjectile
{
    private bool _active;
    public override bool InstancePerEntity => true;

    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
    {
        return entity.type is ProjectileID.Bee or ProjectileID.GiantBee;
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if ((source is EntitySource_ItemUse_WithAmmo use && use.Item.type == ModContent.ItemType<BumbleBarrage>()) || source is EntitySource_Misc { Context: "styx" })
        {
            _active = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10; // how long you want the trail to be
            ProjectileID.Sets.TrailingMode[projectile.type] = 2; // recording mode
        }
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor)
    {
        Main.instance.LoadProjectile(projectile.type);
        Texture2D texture = TextureAssets.Projectile[projectile.type].Value;

        for (int k = 0; k < projectile.oldPos.Length; k++)
        {
            Vector2 offset = new(projectile.width / 2f, projectile.height / 2f);
            Rectangle frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + offset;
            float sizec = projectile.scale * (projectile.oldPos.Length - k) / (projectile.oldPos.Length * 0.8f);
            Color color = new Color(255, 172, 12, 0) * (1f - projectile.alpha) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);

            if (_active)
                Main.EntitySpriteDraw(texture, drawPos, frame, color, projectile.oldRot[k], frame.Size() / 2,
                    sizec, SpriteEffects.None);
        }

        return true;
    }

    public override Color? GetAlpha(Projectile projectile, Color lightColor)
    {
        return _active ? Color.White : base.GetAlpha(projectile, lightColor);
    }
}