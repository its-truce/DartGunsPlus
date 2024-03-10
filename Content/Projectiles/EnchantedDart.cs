using DartGunsPlus.Content.Items.Ammo;
using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class EnchantedDart : ModProjectile
{
    private float _scale = 0.4f;
    private static readonly Color[] Colors = { Color.SteelBlue, Color.PaleVioletRed, Color.Gold, Color.HotPink };

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ModContent.ProjectileType<DartProjectile>());
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.Opacity = 0;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, Color.AliceBlue.ToVector3());

        if (Main.rand.NextBool(20))
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, newColor: Main.rand.NextFromList(Colors));

        if (Projectile.velocity.Length() < 18)
            Projectile.velocity *= 1.02f;
        else if (Projectile.velocity.Length() > 6)
            Projectile.velocity *= 0.98f;

        if (_scale < 1.5f)
            _scale *= 1.01f;

        FadingSystem.FadeIn(Projectile, 45);
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Projectile.ai[0] == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 position;
                do
                {
                    position = target.position + new Vector2(Main.rand.Next(-300, 300), Main.rand.Next(-80, 80));
                } while (Framing.GetTileSafely(position).HasTile);

                Projectile.NewProjectile(Projectile.GetSource_OnHit(target), position, position.DirectionTo(target.Center) * 4, Projectile.type, 
                    Projectile.damage/2, Projectile.knockBack, Projectile.owner, 1);

                VisualSystem.SpawnDustPortal(position, position.DirectionTo(target.Center) * 4, DustID.TintableDustLighted, 
                    color: Main.rand.NextFromList(Colors));
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>("DartGunsPlus/Content/Projectiles/Bolt").Value;
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Vector2 offset = new(Projectile.width / 2f, Projectile.height / 2f);
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + offset;
            float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 0.8f);
            Color color = new(88, 101, 242, 0);

            if (Vector2.Distance(Projectile.oldPos[k], Projectile.Center) > 20)
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldVelocity.ToRotation(), frame.Size() / 2,
                    sizec * 0.15f * new Vector2(1, _scale), SpriteEffects.None);
        }

        return true;
    }
}