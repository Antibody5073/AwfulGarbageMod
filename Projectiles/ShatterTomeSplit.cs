using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Projectiles
{

    public class ShatterTomeSplit : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shard"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
            Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 4;
			Projectile.height = 4;
			Projectile.aiStyle = 0;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 60;
			Projectile.light = 1f;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = false;
			Projectile.ArmorPenetration = 12;
        }


        public override void AI()
        {
			int dust = Dust.NewDust(Projectile.Center, 1, 1, 15, 0f, 0f, 0, default(Color), 1f);
			Main.dust[dust].velocity *= 0.2f;
			Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
			Main.dust[dust].noGravity = true;
        }
	}
}