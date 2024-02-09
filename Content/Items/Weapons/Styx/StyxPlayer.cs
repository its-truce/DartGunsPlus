using DartGunsPlus.Content.Items.Weapons.Styx.Guns;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Weapons.Styx;

public class StyxPlayer : ModPlayer
{
    private readonly int[] _revolvingProjectiles =
    {
        ModContent.ProjectileType<SerenityRevolving>(), ModContent.ProjectileType<LaserRevolving>(),
        ModContent.ProjectileType<OutrageRevolving>(), ModContent.ProjectileType<MartianRevolving>(), ModContent.ProjectileType<HalloweenRevolving>(),
        ModContent.ProjectileType<RosemaryRevolving>(), ModContent.ProjectileType<FrenzyRevolving>(), ModContent.ProjectileType<BumbleRevolving>(),
        ModContent.ProjectileType<EnchantedRevolving>(), ModContent.ProjectileType<ZapperRevolving>()
    };

    public override void PostUpdate()
    {
        if (Player.HeldItem.ModItem is Styx && Player.ownedProjectileCounts[ModContent.ProjectileType<StyxRune>()] == 0)
        {
            SoundEngine.PlaySound(AudioSystem.ReturnSound("styx"), Player.Center);
            Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<StyxRune>(),
                Player.inventory[Player.selectedItem].damage, 4, Player.whoAmI);

            int i = 0;

            foreach (int projType in _revolvingProjectiles)
            {
                if (Player.ownedProjectileCounts[projType] == 0)
                    Projectile.NewProjectile(Player.GetSource_Misc("revolve"), Player.Center, Vector2.Zero, projType, 
                        Player.inventory[Player.selectedItem].damage, Player.inventory[Player.selectedItem].knockBack, Player.whoAmI, 
                        MathHelper.ToRadians(i * 360 / _revolvingProjectiles.Length));

                i++;
            }
        }
    }
}