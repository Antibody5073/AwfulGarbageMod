using Microsoft.Xna.Framework;
using Mono.Cecil;
using Steamworks;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.DamageClasses;
using log4net.Core;
using Microsoft.Xna.Framework.Graphics;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class SlimyKnives : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Throw 2-3 knives at a time");
		}

		public override void SetDefaults()
		{
			Item.damage = 13;
			Item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
			Item.width = 60;
			Item.height = 60;
			Item.useTime = 25;
			Item.noMelee = true;
			Item.scale = 0f;
			Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 1.75f;
			Item.value = 10000;
			Item.rare = 1;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = Mod.Find<ModProjectile>("SlimyKnivesProj").Type;
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (var i = 0; i < Main.rand.Next(2, 4); i++)
            {
                int proj = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(24)) * Main.rand.NextFloat(0.8f, 1.2f), Mod.Find<ModProjectile>("SlimyKnivesProj").Type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    public class SlimyKnivesProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        float spinSpd;
        bool collide = false;
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 480;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;

            if (!collide)
            {
                collide = true;
            }

            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slimed, 180);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

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
            /*
            float offsetX = 0;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            float offsetY = 0;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);
            */

            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            if (collide)
            {
                if (Main.rand.NextBool(4))
                {
                    int dust = Dust.NewDust(Projectile.position, 4, 4, DustID.Water, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.35f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
            }
            else
            {
                int dust = Dust.NewDust(Projectile.position, 4, 4, DustID.Water, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
                if (Projectile.timeLeft == 450)
                {
                    spinSpd = Projectile.velocity.X * 0.06f;
                    if (spinSpd > 0.2f) { spinSpd = 0.2f; }
                    if (spinSpd < -0.2f) { spinSpd = -0.2f; }
                }
                else if (Projectile.timeLeft < 450)
                {
                    Projectile.rotation += spinSpd;
                    Projectile.velocity.Y += 0.66f;
                    Projectile.velocity.X *= 0.98f;
                    if (Projectile.velocity.Y > 17)
                    {
                        Projectile.velocity.Y = 17;
                    }
                }
                else
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.ToRadians(30);
                }
            }
        }
    }
}