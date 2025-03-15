using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace AwfulGarbageMod.Projectiles
{

    public class BossTelegraph : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Telegraph"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 540;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 59;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void AI()
        {
            if (Vector2.Distance(Projectile.Center, Main.player[Main.myPlayer].Center) < 1000)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)Projectile.ai[0], 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
            
        }
	}
}