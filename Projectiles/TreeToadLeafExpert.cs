using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace AwfulGarbageMod.Projectiles
{

    public class TreeToadLeafExpert : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Leaf"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 400;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.BrokenArmor, 1800);
        }

        public override void OnKill(int timeLeft)
        {
            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
            Main.projectile[proj].velocity = (Main.player[Main.myPlayer].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 8f;
        }

        public override void AI()
        {
            DrawOffsetX = -3;
            DrawOriginOffsetY = -4;
            Projectile.damage = 8;
            if (Main.expertMode)
            {
                Projectile.damage = 7;
                if (Main.masterMode)
                {
                    Projectile.damage = 6;
                }
            }

            if (Projectile.timeLeft % 3 == 0)
            {
                Projectile.aiStyle = 0;
                int dust = Dust.NewDust(Projectile.Center, 1, 1, DustID.Grass, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }

        }
	}
}