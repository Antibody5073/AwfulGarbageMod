using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace AwfulGarbageMod.Projectiles
{

    public class SeseBoneProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bone"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
            Projectile.width = 18;
            Projectile.height = 400;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.velocity.Y > 0f)
            {
                float collisionPoint6 = 0f;
                Vector2 vector3 = new Vector2(1, 0).RotatedBy(Projectile.rotation).RotatedBy(-1.5707963705062866) * Projectile.scale;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - vector3 * 160f, Projectile.Center + vector3 * 160f, 5f * Projectile.scale, ref collisionPoint6))
                {
                    return true;
                }
            }
            return false;
        }
        public override void AI()
        {
            Projectile.damage = 14;
            if (Main.expertMode)
            {
                Projectile.damage = 13;
                if (Main.masterMode)
                {
                    Projectile.damage = 11;
                }
            }
            Projectile.velocity.Y += 0.15f;
            Projectile.rotation += MathHelper.ToRadians(Projectile.ai[0]);


            if (Projectile.velocity.Y > 0f)
            {
                if (Projectile.velocity.Y > 6f)
                {
                    Projectile.velocity.Y = 6f;
                }
            }
        }
	}
}