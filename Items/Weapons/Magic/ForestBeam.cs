using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Magic
{

    public class ForestBeam : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acorn Staff"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Hitting an ememy restores all used mana");
            Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.mana = 8;
			Item.DamageType = DamageClass.Magic;
			Item.width = 42;
			Item.height = 46;
			Item.useTime = 33;
			Item.useAnimation = 33;
			Item.useStyle = 5;
			Item.knockBack = 0.1f;
			Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item8;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<ForestBeamProj>();
			Item.shootSpeed = 8f;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }
	}

    public class ForestBeamProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 2;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        int StateTimer;
        bool dust = true;
        float projSpd = 2f;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 750;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage /= 2;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            dust = false;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                if (k > 0)
                {
                    int dust = Dust.NewDust(Projectile.oldPos[k], Projectile.width, Projectile.height, DustID.GreenFairy, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
                    Main.dust[dust].noGravity = true;

                    Main.dust[dust].velocity = oldVelocity * 1.5f;
                    Main.dust[dust].alpha = (int)(255 * (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                }
            }
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft)
        {
            if (dust)
            {
                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    if (k > 0)
                    {
                        int dust = Dust.NewDust(Projectile.oldPos[k], Projectile.width, Projectile.height, DustID.GreenFairy, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
                        Main.dust[dust].noGravity = true;

                        Main.dust[dust].velocity = Projectile.velocity * 1.5f;
                        Main.dust[dust].alpha = (int)(255 * (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    }
                }
            }
            base.OnKill(timeLeft);
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
            int startY = frameHeight * (Projectile.frame + 1);


            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, (int)Math.Ceiling(frameHeight * Vector2.Distance(Projectile.velocity, Vector2.Zero) / 12));

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;
            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);

            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                if (k > 0)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(4f, 4f);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    color.A = (byte)(color.A * 0.75f);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
                }
            }
            startY = frameHeight * Projectile.frame;
            sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            StateTimer++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);

            float maxDetectRadius = 300; // The maximum radius at which a projectile can detect a target

            // Trying to find NPC closest to the projectile
            if (Projectile.ai[0] <= 0)
            {
                return;
            }
            NPC closestNPC = AGUtils.GetClosestNPC(Projectile.Center, maxDetectRadius);
            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity += (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpd;
            projSpd += 0.1f;
            if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > Projectile.ai[0])
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= Projectile.ai[0];
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
        }
    }
}