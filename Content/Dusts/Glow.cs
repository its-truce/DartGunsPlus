using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Dusts;

public class Glow : ModDust
{
    public override string Texture => "DartGunsPlus/Content/Dusts/GlowingDust";

    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.frame = new Rectangle(0, 0, 64, 64);

        dust.shader = new ArmorShaderData(new Ref<Effect>(ModContent.Request<Effect>("DartGunsPlus/Content/Effects/GlowingDust",
            AssetRequestMode.ImmediateLoad).Value), "GlowingDustPass");
        dust.scale *= 0.75f;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor)
    {
        return dust.color;
    }

    public override bool Update(Dust dust)
    {
        if (dust.customData is null)
        {
            dust.position -= Vector2.One * 32 * dust.scale;
            dust.customData = true;
        }

        Vector2 currentCenter = dust.position + Vector2.One.RotatedBy(dust.rotation) * 32 * dust.scale;

        dust.scale *= 0.95f;
        Vector2 nextCenter = dust.position + Vector2.One.RotatedBy(dust.rotation + 0.06f) * 32 * dust.scale;

        dust.rotation += 0.06f;
        dust.position += currentCenter - nextCenter;

        dust.shader.UseColor(dust.color);

        dust.position += dust.velocity;

        if (!dust.noGravity)
            dust.velocity.Y += 0.1f;

        dust.velocity *= 0.99f;
        dust.color *= 0.95f;

        if (!dust.noLight)
            Lighting.AddLight(dust.position, dust.color.ToVector3());

        if (dust.scale < 0.05f)
            dust.active = false;

        return false;
    }
}

public class GlowFastDecelerate : Glow
{
    public override bool Update(Dust dust)
    {
        dust.velocity *= 0.95f;
        return base.Update(dust);
    }
}

public class GlowFollowPlayer : Glow
{
    public override bool Update(Dust dust)
    {
        base.Update(dust);

        if (dust.customData is object[] pair)
        {
            Player player = pair[0] as Player;
            Vector2 pos = (Vector2)pair[1];

            if (player != null) dust.position = player.Center + Vector2.UnitY * player.gfxOffY + pos - Vector2.One * 32 * dust.scale;
            pair[1] = pos + dust.velocity;
        }

        return false;
    }
}