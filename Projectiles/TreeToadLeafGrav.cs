using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AwfulGarbageMod.Systems;

namespace AwfulGarbageMod.Projectiles
{

    public class TreeToadLeafGrav : ModProjectile
	{
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }

        public override void SetDefaults()
		{
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.BrokenArmor, 1800);
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        int StateTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)TextureAssets.Projectile[206];

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
            Texture2D projectileTexture = texture;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }
            Main.spriteBatch.Draw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (DifficultyModes.Difficulty > 0)
            {
                if (Projectile.ai[0] == 0 || DifficultyModes.Difficulty == 2)
                {
                    if (Math.Abs(Projectile.Center.X - Main.player[Projectile.owner].Center.X) < 640 || DifficultyModes.Difficulty == 2)
                    {
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, 0), ModContent.ProjectileType<TreeToadLeaf>(), 17, 0, Main.myPlayer);
                        Main.projectile[proj].velocity = (Main.player[Main.myPlayer].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 8f;
                    }
                } 
            }
        }
        public override void AI()
        {
            Projectile.damage = 8;
            if (Main.expertMode)
            {
                Projectile.damage = 7;
                if (Main.masterMode)
                {
                    Projectile.damage = 6;
                }
            }

            Projectile.velocity.Y += 0.1f;
            if (Projectile.velocity.Y > 8)
            {
                Projectile.velocity.Y = 8;
            }

            StateTimer++;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (StateTimer % 5 == 0)
            {
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            if (DifficultyModes.Difficulty > 0)
            {
                if (Projectile.ai[0] == 0 || DifficultyModes.Difficulty == 2)
                {
                    if (Projectile.velocity.Y > 0 && Projectile.Center.Y > Main.player[Projectile.owner].Center.Y)
                    {
                        Projectile.tileCollide = true;
                    }
                }
            }
        }
    }
}