using DartGunsPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Buffs;

public class ElectricShock : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.pvpBuff[Type] = true; // This buff can be applied by other players in Pvp, so we need this to be true.
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        npc.GetGlobalNPC<LightningNPC>().Shocked = true;

        Projectile.NewProjectile(npc.GetSource_Buff(buffIndex), npc.Center - new Vector2(0, 20), Vector2.Zero,
            ModContent.ProjectileType<LightningIcon>(), 0, 0, ai0: npc.whoAmI, ai1: 50);
    }
}

public class LightningNPC : GlobalNPC
{
    private int _numAttacked;
    public bool Shocked;
    public override bool InstancePerEntity => true;

    public override void ResetEffects(NPC npc)
    {
        Shocked = false;
    }

    public override void HitEffect(NPC npc, NPC.HitInfo hit)
    {
        if (Shocked && npc.active && npc.life > 0)
        {
            _numAttacked++;
            for (int i = 0; i < 5; i++) Dust.NewDust(npc.Center, npc.width, npc.height, DustID.Electric);

            if (_numAttacked == 20)
                _numAttacked = 0;
        }
    }

    public override void UpdateLifeRegen(NPC npc, ref int damage)
    {
        if (Shocked)
        {
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;

            npc.lifeRegen -= _numAttacked * 6;
            damage = _numAttacked * 10;
        }
    }
}