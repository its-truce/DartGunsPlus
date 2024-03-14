using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class SkywardPlume : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 44;
        Item.height = 60;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<AccessoryPlayer>().HasPlume = true;
    }
}

public class PlumeProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    private bool _eligible;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse_WithAmmo use && ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart)
            _eligible = true;
    }

    public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
    {
        Player player = Main.player[projectile.owner];

        if (!player.GetModPlayer<AccessoryPlayer>().HasPlume || !_eligible)
            return;

        if (player.velocity.Y != 0)
            modifiers.FinalDamage *= 1.1f;
    }
}