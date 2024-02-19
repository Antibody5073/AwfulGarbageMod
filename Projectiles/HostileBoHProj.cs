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
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace AwfulGarbageMod.Projectiles
{

   
    public class HostileBoHProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        bool hitTile;
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            hitTile = false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hitTile = true;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!hitTile)
            {
                Vector2 playerArmPosition = Main.npc[(int)Projectile.ai[0]].Center;

                Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("AwfulGarbageMod/Projectiles/HostileBoHProjectileChain");
                Rectangle? chainSourceRectangle = null;
                // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
                float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
                Vector2 chainDrawPosition = Projectile.Center;
                Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
                }
                float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

                // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
                while (chainLengthRemainingToDraw > 0f)
                {
                    // This code gets the lighting at the current tile coordinates
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                    // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                    // This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

                    var chainTextureToDraw = chainTexture;
                    if (chainCount >= 4)
                    {
                        // Use normal chainTexture and lighting, no changes
                    }

                    // Here, we draw the chain texture at the coordinates
                    Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                    chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }

        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust].scale = 1.35f;
            Main.dust[dust].noGravity = true;
            Main.dust[dust].alpha = 130;
            
            if (Projectile.timeLeft < 870)
            {
                Projectile.tileCollide = true;
                Projectile.velocity.Y += 0.4f;
                Projectile.velocity.X *= 0.95f;
                Projectile.hostile = true;
                if (Projectile.velocity.Y > 20)
                {
                    Projectile.velocity.Y = 20;
                }
            }
            if (!Main.npc[(int)Projectile.ai[0]].active)
            {
                Projectile.Kill();
            }
        }
    }
}