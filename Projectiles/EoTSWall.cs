using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace AwfulGarbageMod.Projectiles
{

    public class EoTSWall : ModProjectile
	{
        public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Bone Wall"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
		{
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        int projFrame;

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }


        public override void AI()
        {
            DrawOffsetX = -10;
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
            if (Projectile.position.Y > Main.player[Main.myPlayer].Center.Y + 1800)
            {
                Projectile.position.Y += -3600;
            }

            if (Projectile.position.Y <= Main.player[Main.myPlayer].Center.Y - 1800)
            {
                Projectile.position.Y += 3600;
            }


            int frameSpeed = 5;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                projFrame++;

                if (projFrame >= 6)
                {
                    projFrame = 0;
                }
            }


            switch (projFrame)
            {
                case 0:
                    Projectile.frame = 0;
                    break;
                case 1:
                    Projectile.frame = 1;
                    break;
                case 2:
                    Projectile.frame = 2;
                    break;
                case 3:
                    Projectile.frame = 3;
                    break;
                case 4:
                    Projectile.frame = 4;
                    break;
                case 5:
                    Projectile.frame = 2;
                    break;
            }
        }
	}
}