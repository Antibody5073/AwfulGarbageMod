using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace AwfulGarbageMod.Projectiles
{

    public class IceSpiritBeam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Ice Beam"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

        public int counter;

        public int dir;

		public override void SetDefaults()
		{
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 240;
        }

        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].velocity *= 0f;
            Main.dust[dust].noGravity = true;

            counter++;
            if (counter % 1 == 0)
            {
                dir += 5;
                int dust2 = Dust.NewDust(Projectile.Center + new Vector2(0, 15).RotatedBy(MathHelper.ToRadians(dir)), 1, 1, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust2].scale = 1f;
                Main.dust[dust2].velocity *= 0f;
                Main.dust[dust2].noGravity = true;
                int dust3 = Dust.NewDust(Projectile.Center + new Vector2(0, 15).RotatedBy(MathHelper.ToRadians(dir + 120)), 1, 1, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust3].scale = 1f;
                Main.dust[dust3].velocity *= 0f;
                Main.dust[dust3].noGravity = true;
                int dust4 = Dust.NewDust(Projectile.Center + new Vector2(0, 15).RotatedBy(MathHelper.ToRadians(dir - 120)), 1, 1, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust4].scale = 1f;
                Main.dust[dust4].velocity *= 0f;
                Main.dust[dust4].noGravity = true;
                int dust5 = Dust.NewDust(Projectile.Center + new Vector2(0, 15).RotatedBy(MathHelper.ToRadians(-dir)), 1, 1, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust5].scale = 1f;
                Main.dust[dust5].velocity *= 0f;
                Main.dust[dust5].noGravity = true;
                int dust6 = Dust.NewDust(Projectile.Center + new Vector2(0, 15).RotatedBy(MathHelper.ToRadians(-dir + 120)), 1, 1, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust6].scale = 1f;
                Main.dust[dust6].velocity *= 0f;
                Main.dust[dust6].noGravity = true;
                int dust7 = Dust.NewDust(Projectile.Center + new Vector2(0, 15).RotatedBy(MathHelper.ToRadians(-dir - 120)), 1, 1, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust7].scale = 1f;
                Main.dust[dust7].velocity *= 0f;
                Main.dust[dust7].noGravity = true;
            }
        }
	}
}