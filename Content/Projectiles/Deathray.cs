using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace DartGunsPlus.Content.Projectiles;

public abstract class Deathray : ModProjectile
{
	protected float MoveDistance = 100f;
	protected float RealMaxDistance = 100f;
	protected float MaxDistance = 100f;
	protected Rectangle BodyRect = new();
	protected Rectangle TailRect = new();
	protected Rectangle HeadRect = new();
	protected Color DrawColor = Color.White;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 100000;
	}
	
	public override void PostDraw(Color lightColor)
	{
		DrawLaser(Main.spriteBatch, ModContent.Request<Texture2D>(Texture).Value, Position(), Projectile.velocity, BodyRect.Height, -1.57f, 1f, 
			MaxDistance, (int)MoveDistance);
	}

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.hide = true;
		Projectile.timeLeft = 1;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.velocity = oldVelocity;
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	protected void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 startPoint, Vector2 unit, float step, float rotation = 0f, float scale = 1f, 
		float maxDist = 2000f, int transDist = 50)
	{
		float r = unit.ToRotation() + rotation;

		for (float i = transDist; i <= maxDist; i += step)
		{
			Color c = DrawColor;
			Vector2 origin = startPoint + i * unit;
			Rectangle newBodyRectangle = BodyRect;
			spriteBatch.Draw(texture, origin - Main.screenPosition, newBodyRectangle, i < transDist ? Color.Transparent : c, r, 
				newBodyRectangle.Size() / 2, new Vector2(scale, 1), 0, 0);
		}

		Rectangle newTailRectangle = TailRect;
		spriteBatch.Draw(texture, startPoint + unit * (transDist - step + 2) + unit - Main.screenPosition, newTailRectangle, DrawColor, r, 
			newTailRectangle.Size() / 2, new Vector2(scale * 2, 1), SpriteEffects.FlipVertically, 0);

		if (Projectile.type == ModContent.ProjectileType<VolatileLaser>())
		{
			Rectangle newHeadRectangle = HeadRect;
			spriteBatch.Draw(texture, startPoint + maxDist * unit - Main.screenPosition, newHeadRectangle, DrawColor, r, newHeadRectangle.Size() / 2,
				new Vector2(scale * 2, 1), SpriteEffects.FlipVertically, 0);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		Vector2 unit = Projectile.velocity;
		float point = 0f;

		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Position(),
			Position() + unit * MaxDistance, 22, ref point);
	}
	
	public override void AI()
	{
		SetLaserPosition();
		Projectile.velocity.Normalize();
	}

	protected virtual Vector2 Position()
	{
		return Main.player[Projectile.owner].Center;
	}

	protected virtual void SetLaserPosition()
	{
		if (!Projectile.tileCollide)
			MaxDistance = RealMaxDistance;
		else
		{
			for (MaxDistance = MoveDistance; MaxDistance <= RealMaxDistance; MaxDistance += 5f)
			{
				Vector2 start = Position() + Projectile.velocity * MaxDistance;
				if (Collision.SolidCollision(start, 1, 1))
				{
					MaxDistance -= 5f;
					break;
				}
			}
		}
	}

	public override bool ShouldUpdatePosition() => false;

	public override void CutTiles()
	{
		DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
		Vector2 unit = Projectile.velocity;
		Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * MaxDistance, (Projectile.width + 16) * Projectile.scale, DelegateMethods.CutTiles);
	}
}
