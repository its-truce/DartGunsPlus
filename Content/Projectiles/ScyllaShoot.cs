using DartGunsPlus.Content.Dusts;
using DartGunsPlus.Content.Items.Weapons;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class ScyllaShoot : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Projectiles/EmptyTexture";
    private Player Owner => Main.player[Projectile.owner];
    private NPC Target => Main.npc[(int)Projectile.ai[0]];
    private ref float InitialItemRot => ref Projectile.ai[2];
    private const int Degrees = 30;

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.width = 22;
        Projectile.height = 22;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 600;
    }

    public override void OnSpawn(IEntitySource source)
    {
        if (!Owner.noItems && !Owner.CCed && Owner.PickAmmo(Owner.HeldItem, out int proj, out float speed, out int damage,
                out float knockback, out int _))
        {
            Item scyllaItem = Owner.HeldItem;

            Owner.itemAnimationMax = scyllaItem.useTime + scyllaItem.reuseDelay;
            Owner.itemAnimation = Owner.itemAnimationMax;
            InitialItemRot = Owner.itemRotation;
            Projectile.timeLeft = Owner.itemAnimationMax;
        
            int numProjectiles = Main.rand.Next(7, 10);

            Vector2 velocity = Owner.DirectionTo(Target.active ? Target.Center : Main.MouseWorld) * speed;
            Vector2 position = Owner.Center + new Vector2(scyllaItem.width * 0.8f, 0).RotatedBy(velocity.ToRotation());
        
            for (int i = 0; i < numProjectiles; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(30));
                newVelocity *= 1f - Main.rand.NextFloat(0.4f);

                int projToShoot = Main.rand.NextBool(4) ? ModContent.ProjectileType<LuminiteStrike>() : proj;

                Projectile.NewProjectileDirect(Owner.GetSource_ItemUse(scyllaItem), position, newVelocity, projToShoot, damage, knockback, 
                    Owner.whoAmI);
            }

            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<ShotgunMusket>(),
                0, 0, Owner.whoAmI, velocity.ToRotation(), 5, 2);

            Dust.NewDustDirect(position, 20, 20, ModContent.DustType<GlowFastDecelerate>(), newColor: Color.Turquoise, Scale: 0.5f);

            velocity.Normalize();
            Owner.velocity += velocity * -5;
            CameraSystem.Screenshake(6, 6);

            SoundEngine.PlaySound(AudioSystem.ReturnSound("scyllashoot"), Owner.Center);
        }
        else
            Projectile.Kill();
    }

    public override void AI()
    {
        Owner.itemTime = Owner.itemAnimation;
        Owner.heldProj = Projectile.whoAmI;

        VisualSystem.RecoilAnimation(Owner, InitialItemRot, Degrees);
        
        if (Owner.HeldItem.ModItem is not Scylla)
            Projectile.Kill();
    }
}