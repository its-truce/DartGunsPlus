using DartGunsPlus.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Items.Accessories;

public class TargetCircle : ModProjectile
{
    private NPC Target => Main.npc[(int)Projectile.ai[0]];
    private Vector2 _offset;

    public override void SetDefaults()
    {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.width = 512;
        Projectile.height = 512;
        Projectile.penetrate = -1;
        Projectile.friendly = false;
        Projectile.Opacity = 0;
        Projectile.scale = 0.15f;
        Projectile.timeLeft = 600;
        Projectile.Hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, (int)(Projectile.width * 0.07f),
            (int)(Projectile.height * 0.07f));
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = MathHelper.ToRadians(-45);
        
        _offset = Main.rand.NextBool(2)
            ? new Vector2(Main.rand.NextBool(2) ? Target.Hitbox.Width / -2 : Target.Hitbox.Width / 2,
                Main.rand.Next(Target.Hitbox.Height / -2, Target.Hitbox.Height / 2)) :
            new Vector2(Main.rand.Next(Target.Hitbox.Width / -2, Target.Hitbox.Width / 2),
                Main.rand.NextBool(2) ? Target.Hitbox.Height / -2 : Target.Hitbox.Height / 2);
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White * Projectile.Opacity;
    }

    public override void AI()
    {
        FadingSystem.FadeIn(Projectile, 15);

        if (Projectile.scale > 0.07f)
            Projectile.scale -= 0.006f;
        
        Projectile.Center = Target.Center + _offset;

        if (Projectile.rotation < 0)
            Projectile.rotation += MathHelper.ToRadians(3);

        if (Projectile.timeLeft < 11)
            FadingSystem.FadeOut(Projectile, 10);

        if (Projectile.timeLeft == 580)
            SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
        
        if (!Target.active)
            Projectile.Kill();
        
        Projectile.Hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, (int)(Projectile.width * 0.07f), 
            (int)(Projectile.height * 0.07f));
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Color color = Color.Red;
        color.A = 0;

        for (int i = 0; i < 2; i++)
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2,
                Projectile.scale, SpriteEffects.None);
        return false;
    }
}