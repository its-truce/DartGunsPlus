using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class ReflectiveCoating : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 36;
        Item.height = 40;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 6);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        AccessoryPlayer accessoryPlayer = player.GetModPlayer<AccessoryPlayer>();
        
        accessoryPlayer.HasShield = true;
        Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<Shield>(), 0, 0, 
            player.whoAmI);
    }
}

public class ReflectProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;
    private bool _eligible;
    
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse_WithAmmo use && ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart)
            _eligible = true;
    }

    public override void PostAI(Projectile projectile)
    {
        Player player = Main.player[projectile.owner];
        AccessoryPlayer accessoryPlayer = player.GetModPlayer<AccessoryPlayer>();
        
        if (!_eligible || !accessoryPlayer.HasShield)
            return;

        foreach (Projectile proj in Main.projectile)
        {
            if (proj.hostile && proj.active && 
                new Rectangle((int)proj.position.X, (int)proj.position.Y, proj.width + proj.width/2, proj.height + proj.height/2).Intersects(projectile.Hitbox)
                && proj.Distance(player.Center) < 140 && accessoryPlayer.ShieldTimer == 0)
            {
                proj.velocity = proj.DirectionFrom(player.Center) * proj.velocity.Length() * 0.95f;
                PopupSystem.PopUp("Deflected!", Color.CornflowerBlue, proj.Center - new Vector2(0, 50));
                accessoryPlayer.IncrementShield = true;
            }
        }
    }
}