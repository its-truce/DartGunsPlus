using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace DartGunsPlus.Content.Items.Weapons.Styx;

public class Styx : ModItem
{
    private CompactParticleManager _manager;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 16, 24, true);
        Item.width = 78;
        Item.height = 28;
        Item.rare = ItemRarityID.Red;

        Item.UseSound = AudioSystem.ReturnSound("styxshoot", 0.3f);

        Item.damage = 301;
        Item.crit = 32;
        Item.knockBack = 4;

        _manager = new CompactParticleManager(
            particle =>
            {
                particle.Rotation = 0;

                switch (particle.TimeAlive)
                {
                    case 0:
                        particle.Scale = 0f;
                        particle.Opacity = 1f;
                        break;

                    case < 30:
                        particle.Scale = MathHelper.Lerp(particle.Scale, 1f, 0.1f);
                        particle.Opacity = MathHelper.Lerp(particle.Opacity, 0f, 0.1f);
                        break;

                    case < 60:
                        particle.Scale = MathHelper.Lerp(particle.Scale, 0f, 0.1f);
                        particle.Opacity = MathHelper.Lerp(particle.Opacity, 1f, 0.1f);
                        break;

                    default:
                        particle.Dead = true;
                        break;
                }
            },
            (particle, spriteBatch, anchor) =>
            {
                Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.RainbowCrystalExplosion).Value;

                spriteBatch.Draw(texture, anchor + particle.Position, null, particle.Color * particle.Opacity, 0f,
                    texture.Size() / 2, particle.Scale * 0.5f, SpriteEffects.None, 0f);

                spriteBatch.Draw(texture, anchor + particle.Position, null, particle.Color * particle.Opacity, MathHelper.PiOver2,
                    texture.Size() / 2, particle.Scale * 0.5f, SpriteEffects.None, 0f);

                spriteBatch.Draw(texture, anchor + particle.Position, null, particle.Color * particle.Opacity * 0.5f, 0f,
                    texture.Size() / 2, particle.Scale * 0.75f, SpriteEffects.None, 0f);

                spriteBatch.Draw(texture, anchor + particle.Position, null, particle.Color * particle.Opacity * 0.5f,
                    MathHelper.PiOver2, texture.Size() / 2, particle.Scale * 0.75f, SpriteEffects.None, 0f);
            }
        );
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Dust.NewDustPerfect(position, DustID.WhiteTorch, newColor: Color.Purple);
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);
        velocity.RotatedByRandom(MathHelper.ToRadians(6));

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width * 0.2f;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= 2;
        }
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }

    public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
    {
        if (line.Name == "ItemName" && line.Mod == "Terraria")
        {
            Color color = Main.hslToRgb(Main.GlobalTimeWrappedHourly / 20f % 1f, 1f, 0.5f);

            Vector2 bounds = FontAssets.MouseText.Value.MeasureString(line.Text);
            Texture2D glow = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Spotlight").Value;
            Vector2 texSize = glow.Size();
            Color glowColor = Main.hslToRgb((Main.GlobalTimeWrappedHourly / 20f + 0.1f) % 1f, 1f, 0.5f) * 0.6f;
            glowColor.A = 0;
            Main.spriteBatch.Draw(glow, (line.X + bounds.X / 2) * Vector2.UnitX + (line.Y + bounds.Y / 2) * Vector2.UnitY - Vector2.UnitY * 3, null,
                glowColor, 0f, texSize / 2, new Vector2(1f, 0.3f), SpriteEffects.None, 0f);

            if (Main.GameUpdateCount % 10 == 0)
            {
                bounds.Y -= 10;
                Vector2 pos = new Vector2(0, 5) + new Vector2(Main.rand.NextFloat(bounds.X), Main.rand.NextFloat(bounds.Y));
                Color color2 = Main.hslToRgb((Main.GlobalTimeWrappedHourly / 20f + Main.rand.NextFloat(-0.1f, 0.1f)) % 1f, 1f, 0.5f);
                color2.A = 0;
                _manager.AddParticle(pos, (pos - bounds / 2).SafeNormalize(Vector2.UnitY) * 1, 0f, 5f, 1f, color2);
            }

            _manager.Update();

            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, FontAssets.MouseText.Value, line.Text,
                new Vector2(line.X, line.Y), Color.White, 0f, Vector2.Zero, line.BaseScale);

            ChatManager.DrawColorCodedString(Main.spriteBatch, FontAssets.MouseText.Value, line.Text,
                new Vector2(line.X, line.Y), color, 0f, Vector2.Zero, line.BaseScale);

            _manager.Draw(Main.spriteBatch, new Vector2(line.X, line.Y));

            return false;
        }

        return base.PreDrawTooltipLine(line, ref yOffset);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient<Serenity>()
            .AddIngredient<LaserTherapy>()
            .AddIngredient<StellarOutrage>()
            .AddIngredient<MartianMarksman>()
            .AddIngredient<HalloweenHex>()
            .AddIngredient<RosemaryThyme>()
            .AddIngredient<StellarFrenzy>()
            .AddIngredient<BumbleBarrage>()
            .AddIngredient<EnchantedDartcaster>()
            .AddIngredient<DartZapper>()
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}