using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Accessories
{

    public class SwampyGrip : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mandible Gauntlet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("3 defense\n8% increased melee speed");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 12500;
            Item.rare = 2;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().SwampyGrip += 25;
            player.GetModPlayer<GlobalPlayer>().flailRetractSpeed += 0.1f;

        }
    }
    public class SwampyGripSludge : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        Vector2 center;
        bool exploded = false;
        int StateTimer;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 40; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(3, 3);
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptSpray, vel.X, vel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.2f;


                if (Main.rand.NextBool())
                {
                    dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, vel.X, -1f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!exploded)
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

                Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawOrigin = origin;
                spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(6f, 6f);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
                }

                Main.EntitySpriteDraw(texture,
                    Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                    sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Explode();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Explode();
            return false;
        }

        public void Explode()
        {
            if (!exploded)
            {
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Zero;
                center = Projectile.Center;
                exploded = true;

                Projectile.width = 100;
                Projectile.height = 100;
                Projectile.Center = center;
                Projectile.timeLeft = 2;
            }
        }

        public override void AI()
        {
            StateTimer++;
            if (Main.rand.NextBool())
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }
    }
}