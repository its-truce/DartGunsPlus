using DartGunsPlus.Content.Projectiles;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons;

public class DartStorm : ModItem
{
    private float _initialItemRot;

    public override void SetDefaults()
    {
        Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Dart, 40, 11, true);
        Item.width = 38;
        Item.height = 22;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.sellPrice(gold: 6);

        Item.UseSound = AudioSystem.ReturnSound("shotgun", 0.4f, 0.6f);

        Item.damage = 12;
        Item.knockBack = 6f;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        const int numProjectiles = 4;

        for (int i = 0; i < numProjectiles; i++)
        {
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
            newVelocity *= 1f - Main.rand.NextFloat(0.2f);

            Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
        }

        Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ShotgunMusket>(), 0, 0, player.whoAmI,
            velocity.ToRotation(), 14, 4);
        velocity.Normalize();
        player.velocity += velocity * -2;
        CameraSystem.Screenshake(4, 3);

        _initialItemRot = player.itemRotation;
        return false;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        Vector2 muzzleOffset = Vector2.Normalize(velocity) * Item.width;

        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) position += muzzleOffset;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-2f, -2f);
    }

    public override void UseStyle(Player player, Rectangle heldItemFrame)
    {
        VisualSystem.RecoilAnimation(player, _initialItemRot, 15);
    }
}

public class DartStormNPC : GlobalNPC
{
    public override void ModifyShop(NPCShop shop)
    {
        Condition downedSkeletronAndInGraveyard = new("Mods.DartGunsPlus.Conditions.downedSkeletronAndInGraveyard",
            () => Condition.DownedSkeletron.IsMet() && Condition.InGraveyard.IsMet());
        
        if (shop.NpcType == NPCID.ArmsDealer)
            shop.Add<DartStorm>(downedSkeletronAndInGraveyard);
    }
}