using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace AwfulGarbageMod.Projectiles
{

    public class IceSpiritFrigidius: ModProjectile
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spirit"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 540;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

        }

        public override void AI()
        {
            Projectile.damage = 25;
            if (Main.expertMode)
            {
                Projectile.damage = 20;
                if (Main.masterMode)
                {
                    Projectile.damage = 18;
                }
            }

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, Projectile.velocity.X / 2, Projectile.velocity.Y / 2, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].noGravity = true;
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SpectreStaff, 0, 0 / 2, 0, default(Color), 1f);
            Main.dust[dust2].scale = 1.35f;
            Main.dust[dust2].noGravity = true;
        }
    }
}