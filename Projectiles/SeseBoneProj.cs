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

    public class SeseBoneProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Bone"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
            Projectile.width = 18;
            Projectile.height = 600;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.velocity.Y > 0f)
            {
                float collisionPoint6 = 0f;
                Vector2 vector3 = new Vector2(1, 0).RotatedBy(Projectile.rotation).RotatedBy(-1.5707963705062866) * Projectile.scale;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - vector3 * 160f, Projectile.Center + vector3 * 160f, 5f * Projectile.scale, ref collisionPoint6))
                {
                    return true;
                }
            }
            return false;
        }
        int StateTimer = 0;
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
            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);

            Texture2D projectileTexture = texture;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) * 0.2f / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale - k * 0.06f, spriteEffects, 0f);
            }

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            StateTimer++;
            Projectile.damage = 14;
            if (Main.expertMode)
            {
                Projectile.damage = 13;
                if (Main.masterMode)
                {
                    Projectile.damage = 11;
                }
            }
            Projectile.velocity.Y += 0.15f;
            Projectile.rotation += MathHelper.ToRadians(Projectile.ai[0]);


            if (Projectile.velocity.Y > 0f)
            {
                if (Projectile.velocity.Y > 6f)
                {
                    Projectile.velocity.Y = 6f;
                }
            }
        }
	}
}