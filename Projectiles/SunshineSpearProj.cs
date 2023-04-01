using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Projectiles
{

    public class SunshineSpearProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sunshine Spear"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Melee;
			Projectile.scale = 1.25f;
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.aiStyle = 19;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 600;
			Projectile.light = 0f;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.hide = true;
			

		}

        public float movementFactor
		{
			get => Projectile.ai[0] - 1.25f;
			set => Projectile.ai[0] = value;

		}

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = player.RotatedRelativePoint(player.MountedCenter, true);

			Projectile.direction = player.direction;
			player.heldProj = Projectile.whoAmI;
			player.itemTime = player.itemAnimation;

			Projectile.position.X = (ownerMountedCenter.X) - (float)Projectile.width / 2;
            Projectile.position.Y = (ownerMountedCenter.Y) - (float)Projectile.height/ 2;

			Projectile.ai[1] += 1f;

			if (!player.frozen)
			{
				if (movementFactor == 0)
				{
					movementFactor = 2.6f;
					Projectile.netUpdate = true;
				}
				if (player.itemAnimation < player.itemAnimationMax / 3)
				{
					movementFactor -= 3.2f;
				}
				else
				{
					movementFactor += 2.7f;
				}
			}

			Projectile.position += Projectile.velocity * movementFactor;

			if (player.itemAnimation == 0)
			{
				Projectile.Kill();
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

			if (Projectile.spriteDirection == -1)
			{
				Projectile.rotation -= MathHelper.ToRadians(90f);
			}

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + new Vector2(0, -600), new Vector2(0, 5), Mod.Find<ModProjectile>("SunshineProj").Type, damage / 2, knockback / 2, Projectile.owner);
        }
    }
}