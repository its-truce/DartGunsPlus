using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class CrownCascade : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 16, 13, true);
        Item.width = 80;
        Item.height = 30;
        Item.rare = ItemRarityID.Yellow;

        Item.UseSound = AudioSystem.ReturnSound("dart", 0.3f);

        Item.damage = 36;
        Item.knockBack = 3;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse != 2)
        {
            int numProjectiles = Main.rand.Next(4, 7);
            int[] possibleProjectiles = { ProjectileID.MiniSharkron, ProjectileID.FlaironBubble, ProjectileID.Bubble };

            for (int i = 0; i < numProjectiles; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(25));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);

                int projType = Main.rand.NextBool(2) ? Main.rand.NextFromList(possibleProjectiles) : type;

                Projectile.NewProjectileDirect(source, position, newVelocity, projType, damage, knockback, player.whoAmI);
            }
        }
        else
        {
            int whirlpoolType = ModContent.ProjectileType<DukeWhirlpool>();
            if (player.ownedProjectileCounts[whirlpoolType] == 1)
            {
                foreach (Projectile proj in Main.projectile)
                    if (proj.type == whirlpoolType && proj.active && proj.owner == player.whoAmI)
                    {
                        if (!Framing.GetTileSafely(proj.Center).HasTile)
                        {
                            player.Teleport(proj.Center - new Vector2(0, 20), TeleportationStyleID.MagicConch);
                            SoundEngine.PlaySound(AudioSystem.ReturnSound("bubble", 0.3f), proj.Center);
                            proj.Kill();
                        }
                        else
                        {
                            PopupSystem.PopUp("Can't teleport!", Color.Turquoise, player.Center - new Vector2(0, 40));
                        }
                    }
            }
            else
            {
                PopupSystem.PopUp("Can't teleport!", Color.Turquoise, player.Center - new Vector2(0, 40));
            }
        }

        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        velocity *= 1f - Main.rand.NextFloat(0.2f);

        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
        {
            position += muzzleOffset;
            position.Y -= Main.rand.Next(-1, 3);
        }
    }

    public override bool CanConsumeAmmo(Item ammo, Player player)
    {
        return player.altFunctionUse != 2;
    }

    public override bool CanUseItem(Player player)
    {
        Item.UseSound = player.altFunctionUse != 2 ? AudioSystem.ReturnSound("dart", 0.3f) : SoundID.QueenSlime;
        return base.CanUseItem(player);
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -1f);
    }
}