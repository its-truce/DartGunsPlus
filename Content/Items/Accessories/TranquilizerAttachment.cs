using DartGunsPlus.Content.Items.Weapons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class TranquilizerAttachment : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 38;
        Item.maxStack = 1;
        Item.accessory = true;
        Item.value = Item.sellPrice(gold: 5);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<AccessoryPlayer>().HasTranq = true;
    }
}

public class TranqProjectile : GlobalProjectile
{
    private bool _eligible;
    public override bool InstancePerEntity => true;

    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (source is EntitySource_ItemUse_WithAmmo use && (ContentSamples.ItemsByType[use.AmmoItemIdUsed].ammo == AmmoID.Dart || use.Item.ModItem is Charybdis))
            _eligible = true;
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
    {
        Player player = Main.player[projectile.owner];

        if (!player.GetModPlayer<AccessoryPlayer>().HasTranq || !_eligible)
            return;

        if (Main.rand.NextBool(5))
        {
            float quotient = target.life / (float)target.lifeMax;
            int debuffTime = (int)(90 * quotient * (target.boss ? 0.333f : 1f));

            target.AddBuff(ModContent.BuffType<TranquilizerDebuff>(), debuffTime);
        }
    }
}

public class TranquilizerDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.pvpBuff[Type] = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.AddBuff(BuffID.Poisoned, 2);
        npc.velocity = Vector2.Zero;

        if (Main.rand.NextBool(5))
            Dust.NewDust(npc.position, npc.width, npc.height, DustID.Poisoned);
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.AddBuff(BuffID.Poisoned, 2);
        player.velocity *= 0.5f;
    }
}

public class TranqNPC : GlobalNPC
{
    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (npc.HasBuff<TranquilizerDebuff>())
            drawColor.G = 255;
    }
}