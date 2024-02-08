using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class SeekersSpyglass : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 72;
        Item.height = 46;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 7);
        Item.rare = ItemRarityID.Yellow;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<AccessoryPlayer>().HasSpyglass = true;
    }
}

public class SpyglassItem : GlobalItem
{
    private bool _seeking;
    public override bool InstancePerEntity => true;

    public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        NPC target = FindClosestTarget(800);
        if (target != null && player.GetModPlayer<AccessoryPlayer>().HasSpyglass)
        {
            float velocityLength = velocity.Length();
            velocity = position.DirectionTo(target.Center) * velocityLength;
            _seeking = true;
            int dir = velocity.X > 0 ? 1 : -1;
            player.ChangeDir(dir);
            position = player.MountedCenter + new Vector2(item.width, 0).RotatedBy(velocity.ToRotation());
        }
    }

    public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (_seeking)
        {
            NPC target = FindClosestTarget(800);
            Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, Vector2.Zero,
                ModContent.ProjectileType<TargetCircle>(), 0, 0, player.whoAmI, target.whoAmI);
            _seeking = false;
        }

        return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
    }

    private NPC FindClosestTarget(float maxDetectDistance)
    {
        NPC closestTarget = null;
        float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

        for (int k = 0; k < Main.maxNPCs; k++)
        {
            NPC target = Main.npc[k];
            if (target.CanBeChasedBy())
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Main.MouseWorld);

                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestTarget = target;
                }
            }
        }

        return closestTarget;
    }
}