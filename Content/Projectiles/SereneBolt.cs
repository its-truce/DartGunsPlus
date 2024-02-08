using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class SereneBolt : ModProjectile
{
    private Vector2 _mousePos;
    private float _scale = 0.2f;
    public override string Texture => "DartGunsPlus/Content/Projectiles/Glowball";

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20; // how long you want the trail to be
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // recording mode
        ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = 0;
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.penetrate = 1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 300;
        Projectile.damage = 1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        Projectile.ai[0]++;
        Projectile.ai[1] += 0.033f; // for color lerping

        if (_scale < 1.3f)
            _scale *= 1.03f;

        if (Projectile.ai[0] == 30)
            _mousePos = Main.MouseWorld;

        if (Projectile.ai[0] > 30)
        {
            if (Projectile.FindTargetWithLineOfSight(1000f) != -1)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.DirectionTo(Main.npc[Projectile.FindTargetWithLineOfSight(1000f)].Center) * 16,
                    Projectile.velocity, 0.95f);
            }
            else
            {
                Projectile.velocity = Vector2.Lerp(Projectile.DirectionTo(_mousePos) * 16, Projectile.velocity, 0.95f);
                if (Projectile.Center.Distance(_mousePos) < 40)
                    Projectile.Kill();
            }
        }

        Projectile.rotation = Projectile.velocity.ToRotation();

        Lighting.AddLight(Projectile.Center, new Color(62, 240, 93).ToVector3());
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
            Color color = Color.Lerp(new Color(233, 177, 82, 0), new Color(62, 240, 93, 0), Projectile.ai[1])
                          * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[k], frame.Size() / 2, sizec * 0.22f * new Vector2(_scale, 1),
                SpriteEffects.None);
        }

        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Main.rand.NextBool(3))
            Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, Vector2.Zero, ModContent.ProjectileType<SereneShock>(),
                Projectile.damage / 2, 4, Projectile.owner, target.whoAmI);
    }
}