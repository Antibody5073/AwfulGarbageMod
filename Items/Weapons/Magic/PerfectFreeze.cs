using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class PerfectFreeze : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Perfect Freeze"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Left click to shoot non-damaging projectiles, stopping mana regeneration\nProjectiles can not be shot while low on mana\nRight click to consume mana and freeze the projectiles in place and give them damage\nRight click again to scatter frozen projectiles\nSome scattered projectiles are aimed towards the cursor or a nearby enemy\nPenetrates 9 enemy armor\n\"I'll deep-freeze you with some English beef!!\"");
            Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 18;
			Item.mana = 18;
			Item.DamageType = DamageClass.Magic;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 3;
			Item.useAnimation = 3;
			Item.useStyle = 5;
			Item.knockBack = 2f;
			Item.value = 90000;
            Item.rare = 9;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.crit = 5;
			Item.shoot = Mod.Find<ModProjectile>("PerfectFreezeProj").Type;
			Item.shootSpeed = 9f;
			Item.noMelee = true;
		}

        public override bool AltFunctionUse(Player player)
        {
            
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.type == Mod.Find<ModProjectile>("PerfectFreezeProj").Type && projectile.owner == player.whoAmI)
                {
                    if (projectile.ai[0] == 0)
                    {
                        //Freeze
                        projectile.ai[0] = 1;
                        projectile.damage = (int)projectile.ai[1];
                        projectile.velocity = new Vector2(0, 0);
                        projectile.frame = 4;
                    }
                    else if (projectile.ai[0] == 1)
                    {
                        //Scatter

                        projectile.ai[0] = 2;
                        projectile.damage = (int)projectile.ai[1];
                        projectile.timeLeft = 600;

                        if (Main.rand.NextBool(3))
                        {
                            Vector2 targetCenter = projectile.position;
                            bool foundTarget = false;
                            float distanceFromTarget = 1500;

                            //Homing
                            if (Main.rand.NextBool(2))
                            {
                                for (int j = 0; j < Main.maxNPCs; j++)
                                {
                                    NPC npc = Main.npc[j];

                                    if (npc.CanBeChasedBy())
                                    {
                                        float between = Vector2.Distance(npc.Center, projectile.Center);
                                        bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
                                        bool inRange = between < distanceFromTarget;

                                        if ((closest && inRange) || !foundTarget)
                                        {
                                            distanceFromTarget = between;
                                            targetCenter = npc.Center;
                                            foundTarget = true;
                                        }
                                    }
                                }
                            }

                            //Aim towards mouse
                            if (!foundTarget)
                            {
                                Vector2 temp = Main.MouseWorld - projectile.Center;
                                temp.Normalize();
                                projectile.velocity = temp * 9f;
                            }
                            else
                            {
                                Vector2 temp;
                                temp = targetCenter - projectile.Center;
                                temp.Normalize();
                                projectile.velocity = temp * 9f;
                            }
                        }
                        else
                        {
                            //Random velociity
                            projectile.velocity = new Vector2(0, 9).RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360)));
                        }
                    }
                }
            }
            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                return false;
            }
            else
            {
                player.statMana += (int)(Item.mana * player.manaCost);
                int proj = Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360))), Mod.Find<ModProjectile>("PerfectFreezeProj").Type, 0, knockback, player.whoAmI, 0, damage);
                return false;
            }
        }
	}

    public class PerfectFreezeProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Perfect Freeeeeeeeeeeeeeeeeeeeze"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 450;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.ai[0] = 0;
            Projectile.ArmorPenetration = 9;
        }

        int counter;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(0, 4);
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 1)
            {
                Projectile.timeLeft++;
            }
            counter++;
            if (counter % 3 == 0)
            {
                int dust = Dust.NewDust(Projectile.Center - new Vector2(Projectile.width / 4, 0), 1, 1, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.5f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            float offsetX = 16f;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            float offsetY = 16f;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);


            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }
}