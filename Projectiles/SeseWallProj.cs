using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace AwfulGarbageMod.Projectiles
{

    public class SeseWallProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bone Wall"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
            Projectile.width = 32;
            Projectile.height = 1200;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override void AI()
        {
            DrawOriginOffsetY = -600;
            Projectile.damage = 12;
            if (Main.expertMode)
            {
                Projectile.damage = 11;
                if (Main.masterMode)
                {
                    Projectile.damage = 10;
                }
            }
            Projectile.position.Y = Main.player[Main.myPlayer].Center.Y;
        }
	}
}