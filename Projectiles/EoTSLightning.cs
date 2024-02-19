using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.GameContent;
using static System.Formats.Asn1.AsnWriter;
using static Terraria.ModLoader.ModContent;
using Terraria.Enums;
using Terraria.Audio;

namespace AwfulGarbageMod.Projectiles
{

    public class EoTSLightningTele : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laser");
            ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type] = true;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2000;
        }

        // Use a different style for constant so it is very clear in code when a constant is used

        //The distance charge particle from the player center
        private const float MOVE_DISTANCE = 60f;

        // The actual distance is stored in the ai0 field
        // By making a property to handle this it makes our life easier, and the accessibility more readable

        Vector2 origin;
        int startingTimeLeft;
        float distanceToPlayer;
        public float Distance
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        // The actual charge value is stored in the localAI0 field

        // Are we at max charge? With c#6 you can simply use => which indicates this is a get only property


        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
            Projectile.tileCollide = false;
            Projectile.alpha = 160;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override void OnSpawn(IEntitySource source)
        {
            origin = Projectile.position;
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            SetLaserPosition(npc);
            startingTimeLeft = Projectile.timeLeft;
            SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);
            distanceToPlayer = Vector2.Distance(Projectile.Center, Main.LocalPlayer.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Request<Texture2D>("AwfulGarbageMod/Projectiles/EoTSLightningTele").Value;
            DrawLaser(texture, origin - Projectile.velocity * 42, Projectile.velocity, 15, Projectile.damage, lightColor, -1.57f, 1f, 2000f, Color.White, (int)MOVE_DISTANCE);
            return false;
        }
        /*
		// The core function of drawing a laser
		public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50) {
			float r = unit.ToRotation() + rotation;

			// Draws the laser 'body'
			for (float i = transDist; i <= Distance; i += step) {
				Color c = Color.White;
				var origin = start + i * unit;
				spriteBatch.Draw(texture, origin - Main.screenPosition,
					new Rectangle(0, 26, 28, 26), i < transDist ? Color.Transparent : c, r,
					new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
			}
			
			// Draws the laser 'tail'
			spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
				new Rectangle(0, 0, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
			
			// Draws the laser 'head'
			spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
				new Rectangle(0, 52, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
		}
		*/

        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, Color lightColor, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
        {
            float r = unit.ToRotation() + rotation;

            Color drawColor = Projectile.GetAlpha(lightColor);

            // Draws the laser 'body'
            for (float i = transDist; i <= Distance; i += step)
            {
                Color c = drawColor;
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 24, 28, 30), c, r,
                    new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
            }
            

            // Draws the laser 'tail'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 28, 24), drawColor, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

            /*
            Dust dust = Dust.NewDustDirect(start + unit * (transDist - step) - new Vector2(16, 16), 32, 32, DustID.YellowTorch);
            dust.fadeIn = 0f;
            dust.noGravity = true;
            dust.scale = 2f;
            */
            // Draws the laser 'head'
            Main.EntitySpriteDraw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 52, 28, 24), drawColor, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
            /*
            dust = Dust.NewDustDirect(start + (Distance + step) * unit - new Vector2(16, 16), 32, 32, DustID.YellowTorch);
            dust.fadeIn = 0f;
            dust.noGravity = true;
            dust.scale = 2f;
            */
        }


        // Change the way of collision check of the Projectile
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), origin, origin + unit * Distance, 6f, ref point);

        }

        // The AI of the Projectile
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Projectile.position = origin + Projectile.velocity * MOVE_DISTANCE;


            // By separating large AI into methods it becomes very easy to see the flow of the AI in a broader sense
            // First we update player variables that are needed to channel the laser
            // Then we run our charging laser logic
            // If we are fully charged, we proceed to update the laser's position
            // Finally we spawn some effects like dusts and light
            // If laser is not charged yet, stop the AI here.

            Projectile.alpha = 255 - Projectile.timeLeft * 200 / startingTimeLeft;

            SetLaserPosition(npc);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), Projectile.Center - Projectile.velocity * MOVE_DISTANCE, Projectile.velocity, Mod.Find<ModProjectile>("EoTSLightningStrike").Type, 17, 0, Main.myPlayer);

        }

        /*
		 * Sets the end of the laser position based on where it collides with something
		 */
        private void SetLaserPosition(NPC npc)
        {
            for (Distance = MOVE_DISTANCE; Distance <= 2000f; Distance += 5f)
            {
                var start = origin + Projectile.velocity * Distance;
                if ((!Collision.CanHit(origin + Projectile.velocity * (Distance - 5f), 1, 1, start, 1, 1)) && Distance > distanceToPlayer)
                {
                    Distance += 16f;
                    break;
                }
            }
        }

        public override bool ShouldUpdatePosition() => false;

    }
    public class EoTSLightningStrike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning");
            ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type] = true;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2000;
        }

        // Use a different style for constant so it is very clear in code when a constant is used

        //The distance charge particle from the player center
        private const float MOVE_DISTANCE = 60f;

        // The actual distance is stored in the ai0 field
        // By making a property to handle this it makes our life easier, and the accessibility more readable

        Vector2 origin;
        bool dusts = true;
        List<int> frames = new List<int>();
        bool hasFrames = false;
        float distanceToPlayer;

        public float Distance
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        // The actual charge value is stored in the localAI0 field

        // Are we at max charge? With c#6 you can simply use => which indicates this is a get only property

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.alpha = 160;
        }

        public override void OnSpawn(IEntitySource source)
        {
            origin = Projectile.position;

            Projectile.position = origin + Projectile.velocity * MOVE_DISTANCE;
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            SetLaserPosition(npc);
            distanceToPlayer = Vector2.Distance(Projectile.Center, Main.LocalPlayer.Center);

            SoundEngine.PlaySound(SoundID.Item94, Projectile.Center);
            dusts = true;

            Projectile.damage = 24;
            if (Main.expertMode)
            {
                Projectile.damage = 22;
                if (Main.masterMode)
                {
                    Projectile.damage = 20;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Request<Texture2D>("AwfulGarbageMod/Projectiles/EoTSLightningStrike").Value;
            DrawLaser(texture, origin - Projectile.velocity * 42, Projectile.velocity, 26, Projectile.damage, lightColor, -1.57f, 1f, 2000f, Color.White, (int)MOVE_DISTANCE);
            return false;
        }
        /*
		// The core function of drawing a laser
		public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50) {
			float r = unit.ToRotation() + rotation;

			// Draws the laser 'body'
			for (float i = transDist; i <= Distance; i += step) {
				Color c = Color.White;
				var origin = start + i * unit;
				spriteBatch.Draw(texture, origin - Main.screenPosition,
					new Rectangle(0, 26, 28, 26), i < transDist ? Color.Transparent : c, r,
					new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
			}
			
			// Draws the laser 'tail'
			spriteBatch.Draw(texture, start + unit * (transDist - step) - Main.screenPosition,
				new Rectangle(0, 0, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
			
			// Draws the laser 'head'
			spriteBatch.Draw(texture, start + (Distance + step) * unit - Main.screenPosition,
				new Rectangle(0, 52, 28, 26), Color.White, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
		}
		*/

        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, Color lightColor, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
        {
            float r = unit.ToRotation() + rotation;

            Color drawColor = Projectile.GetAlpha(lightColor);

            // Draws the laser 'body'
            int j = 0;
            for (float i = transDist; i <= Distance; i += step)
            {
                if (!hasFrames)
                {
                    frames.Add(Main.rand.Next(1, 4));
                }

                Color c = drawColor;
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 26 * frames[j], 28, 26), c, r,
                    new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
                j++;
                if (dusts && j % 4 == 0)
                {
                    int dust = Dust.NewDust(origin, 1, 1, DustID.Electric, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.5f;
                }
            }
            hasFrames = true;


            // Draws the laser 'tail'
            Main.EntitySpriteDraw(texture, start + unit * (transDist - step) - Main.screenPosition,
                new Rectangle(0, 0, 28, 26), drawColor, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

            /*
            Dust dust = Dust.NewDustDirect(start + unit * (transDist - step) - new Vector2(16, 16), 32, 32, DustID.YellowTorch);
            dust.fadeIn = 0f;
            dust.noGravity = true;
            dust.scale = 2f;
            */
            // Draws the laser 'head'
            Main.EntitySpriteDraw(texture, start + (Distance + step) * unit - Main.screenPosition,
                new Rectangle(0, 104, 28, 26), drawColor, r, new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);

            if (dusts)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustDirect(start + (Distance + step) * unit - new Vector2(16, 16), 32, 32, DustID.Electric);
                    dust.scale = 1.5f;
                }
            }
            dusts = false;

        }


        // Change the way of collision check of the Projectile
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = Projectile.velocity;
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), origin, origin + unit * Distance, 6f, ref point);
        }

        // The AI of the Projectile
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[1]];
            Projectile.position = origin + Projectile.velocity * MOVE_DISTANCE;


            // By separating large AI into methods it becomes very easy to see the flow of the AI in a broader sense
            // First we update player variables that are needed to channel the laser
            // Then we run our charging laser logic
            // If we are fully charged, we proceed to update the laser's position
            // Finally we spawn some effects like dusts and light
            // If laser is not charged yet, stop the AI here.

            Projectile.alpha = 255 - Projectile.timeLeft * 255 / 10;

            SetLaserPosition(npc);
            CastLights();

            if (Projectile.timeLeft < 8)
            {
                Projectile.hostile = false;
            }
        }

        /*
		 * Sets the end of the laser position based on where it collides with something
		 */
        private void SetLaserPosition(NPC npc)
        {
            for (Distance = MOVE_DISTANCE; Distance <= 2000f; Distance += 5f)
            {
                var start = origin + Projectile.velocity * Distance;
                if ((!Collision.CanHit(origin + Projectile.velocity * (Distance - 5f), 1, 1, start, 1, 1)) && Distance > distanceToPlayer)
                {
                    Distance += 16f;
                    break;
                }
            }
        }
        private void CastLights()
        {
            // Cast a light along the line of the laser
            DelegateMethods.v3_1 = new Vector3(0.8f, 0.2f, 0.2f);
            Terraria.Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * (Distance - MOVE_DISTANCE), 26, DelegateMethods.CastLight);
        }

        public override bool ShouldUpdatePosition() => false;

    }
}