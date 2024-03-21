using DartGunsPlus.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class AltitudeAnnihilator : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 32;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 2);
        Item.rare = ItemRarityID.Orange;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<AccessoryPlayer>().HasKite = true;
    }
}

public class AltitudeProjectile : GlobalProjectile
{
    private bool _eligible;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse_WithAmmo use && (ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart || use.Item.ModItem is Charybdis))
            _eligible = true;
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        Player player = Main.player[projectile.owner];

        if (!player.GetModPlayer<AccessoryPlayer>().HasKite || !_eligible)
            return;

        if (target.Center.Y > player.MountedCenter.Y && target.Center.Y - player.Center.Y > 10)
        {
            ParticleOrchestraSettings settings = new()
            {
                PositionInWorld = projectile.Center,
                MovementVector = Vector2.Zero,
                IndexOfPlayerWhoInvokedThis = (byte)player.whoAmI
            };
            ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.SilverBulletSparkle, settings);

            modifiers.Knockback *= 1.333f;
            modifiers.FinalDamage *= 1.25f;
        }
    }
}