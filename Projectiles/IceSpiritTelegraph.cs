using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace AwfulGarbageMod.Projectiles
{

    public class IceSpiritTelegraph : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Telegraph"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 120;
            Projectile.alpha = 150;
        }

        public override void AI()
        {
            if (Vector2.Distance(Projectile.Center, Main.player[Main.myPlayer].Center) < 1000)
            {
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Ice, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
            
        }
	}
}