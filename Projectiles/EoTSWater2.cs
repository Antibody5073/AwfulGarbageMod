using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace AwfulGarbageMod.Projectiles
{

    public class EoTSWater2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Water"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
		{
            Projectile.width = 7;
            Projectile.height = 7;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            DrawOffsetX = -5;
            DrawOriginOffsetY = -25;


            Projectile.velocity.Y += Projectile.ai[0];
            if (Projectile.velocity.Y > Projectile.ai[1])
            {
                Projectile.velocity.Y = Projectile.ai[1];
            }
            Projectile.damage = 18;
            if (Main.expertMode)
            {
                Projectile.damage = 17;
                if (Main.masterMode)
                {
                    Projectile.damage = 16;
                }
            }
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
            Projectile.alpha = (int)(Projectile.alpha * 0.8);

            if (Projectile.timeLeft % 3 == 0)
            {
                int dust = Dust.NewDust(Projectile.position + new Vector2(-7, -8), 20, 20, DustID.Water, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1f, 1.5f);
                Main.dust[dust].velocity = Projectile.velocity * Main.rand.NextFloat(0.1f, 0.3f);
                Main.dust[dust].alpha = Projectile.alpha;
                Main.dust[dust].noGravity = true;
            }


        }
	}
}