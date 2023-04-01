using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Projectiles
{

    public class GlockSlime : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glock Slime"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 4;
			Projectile.height = 20;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 400;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = true;
			Projectile.scale = 0.7f;
			Projectile.extraUpdates = 1;
        }


        public override void Kill(int timeLeft)
        {
            for (var i = 0; i < 6; i++)
            {
                float xv = (0f - Projectile.velocity.X) * (float)Main.rand.Next(35, 50) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.3f;
                float yv = (0f - Projectile.velocity.Y) * (float)Main.rand.Next(35, 50) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.3f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.position.X + xv, Projectile.position.Y + xv), new Vector2(xv, yv), Mod.Find<ModProjectile>("GlockSplatter").Type, Projectile.damage / 3, 0f, Projectile.owner);
            }
            for (var i = 0; i < 20; i++)
            {
                float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 10);
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Cloud, xv, yv, 0, Colors.RarityBlue, 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].noGravity = true;

            }
        }

        public override void AI()
        {
			int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Cloud, 0f, 0f, 0, Colors.RarityBlue, 1f);
			Main.dust[dust].velocity *= 0.2f;
			Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
			Main.dust[dust].noGravity = true;
			Projectile.aiStyle = 0;
        }
	}
}