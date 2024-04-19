using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Transactions;
using Terraria.Audio;

namespace AwfulGarbageMod.Projectiles
{

    public class CarcassCadaverProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("nonexistant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 420;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        float spin = 0f;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft <= 300)
            {
                float collisionPoint6 = 0f;
                Vector2 vector3 = new Vector2(1, 0).RotatedBy(Projectile.rotation).RotatedBy(-1.5707963705062866) * Projectile.scale;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - vector3 * 88f, Projectile.Center + vector3 * 88f, 8f * Projectile.scale, ref collisionPoint6))
                {
                    return true;
                }
                vector3 = new Vector2(1, 0).RotatedBy(Projectile.rotation) * Projectile.scale;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - vector3 * 88f, Projectile.Center + vector3 * 88f, 8f * Projectile.scale, ref collisionPoint6))
                {
                    return true;
                }
            }
            return false;
        }
        public override void AI()
        {
            Projectile.damage = 16;
            if (Main.expertMode)
            {
                Projectile.damage = 15;
                if (Main.masterMode)
                {
                    Projectile.damage = 13;
                }
            }
            Projectile.rotation += MathHelper.ToRadians(spin);
            spin += 0.25f;
            if (spin == 180)
            {
                spin = 0;
            }


            int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,new Vector2(0, 0), Mod.Find<ModProjectile>("CarcassCadaverVisual").Type, 0, 0, Main.myPlayer);
            Main.projectile[proj].rotation = Projectile.rotation;
            int proj2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("CarcassCadaverVisual").Type, 0, 0, Main.myPlayer);
            Main.projectile[proj2].rotation = Projectile.rotation + MathHelper.ToRadians(90);

            if (Projectile.timeLeft % 8 == 0)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_00")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -4).RotatedBy(Projectile.rotation), Mod.Find<ModProjectile>("SeseNonGravProj").Type, Projectile.damage, 0, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -4).RotatedBy(Projectile.rotation + MathHelper.ToRadians(90)), Mod.Find<ModProjectile>("SeseNonGravProj").Type, Projectile.damage, 0, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -4).RotatedBy(Projectile.rotation + MathHelper.ToRadians(180)), Mod.Find<ModProjectile>("SeseNonGravProj").Type, Projectile.damage, 0, Main.myPlayer);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, -4).RotatedBy(Projectile.rotation + MathHelper.ToRadians(270)), Mod.Find<ModProjectile>("SeseNonGravProj").Type, Projectile.damage, 0, Main.myPlayer);
            }

            if (Projectile.timeLeft <= 300)
            {
                Projectile.velocity.X += Projectile.ai[0];
            }
        }
	}
}