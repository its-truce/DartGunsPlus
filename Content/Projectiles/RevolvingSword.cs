using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public class RevolvingSword : ModProjectile
{
    public override string Texture => "DartGunsPlus/Content/Projectiles/EuphoriaSlash";
    private Projectile _deathray;

    private ref float DeathrayOffset => ref Projectile.localAI[0];
    private ref float RotationOffset => ref Projectile.ai[0];
    private ref float RotationDist => ref Projectile.ai[1];
    private NPC Target => Main.npc[(int)Projectile.ai[2]];

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 36;
        Projectile.height = 44;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
        Projectile.Opacity = 0;
    }

    public override void OnSpawn(IEntitySource source)
    {
        DeathrayOffset = MathHelper.ToRadians(Main.rand.Next(-45, 45));
        RotationDist = 90;
        _deathray = Projectile.NewProjectileDirect(Projectile.GetSource_ReleaseEntity(), Target.Center, new Vector2(-1, 0), 
            ModContent.ProjectileType<EuphoriaRay>(), Projectile.damage, 3, Projectile.owner, 0.1f, 140);
    }

    public override void AI()
    {
        if (Projectile.timeLeft > 579)
            FadingSystem.FadeIn(Projectile, 20);
        if (Projectile.timeLeft < 21)
            FadingSystem.FadeOut(Projectile, 20);
        
        RotationOffset += MathHelper.ToRadians(1.5f);
        Projectile.Center = Target.Center + new Vector2(RotationDist, 0).RotatedBy(RotationOffset);
        Projectile.velocity = Vector2.Zero;
        
        Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3());
        Projectile.rotation = RotationOffset + MathHelper.ToRadians(47);

        _deathray.velocity = new Vector2(-1, 0).RotatedBy(RotationOffset);
        _deathray.Center = Target.Center;
        _deathray.localAI[1] = Target.whoAmI;

        if (!Target.active)
        {
            Projectile.Kill();
            _deathray.Kill();
        }
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }
    
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawOrigin = texture.Size() / 2;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = new Color(255, 255, 200, 0) * Projectile.Opacity;

        Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale * 0.8f, SpriteEffects.None);
        return false;
    }
}