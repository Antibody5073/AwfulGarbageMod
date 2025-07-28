using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace AwfulGarbageMod.Projectiles
{

    public class TreeToadTelegraph : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Telegraph"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}
        int hit = 0;
		public override void SetDefaults()
		{
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 390;
            Projectile.alpha = 255;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hit++;
            return false;
        }
        public override void AI()
        {
            if (hit < 2)
            {
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Grass, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
                Projectile.velocity.Y -= Projectile.ai[0];
            }
            if (hit > 0)
            {
                hit++;
                if (hit == 3)
                {
                    Projectile.Kill();
                }
            }
        }
	}
}