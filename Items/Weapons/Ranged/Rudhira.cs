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
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class Rudhira : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spore Bow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Sometimes shoots a mushroom spore arrow");
        }

        public int counter;

        public override void SetDefaults()
        {
            Item.damage = 38;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 33;
            Item.noMelee = true;
            Item.useAnimation = 33;
            Item.useStyle = 5;
            Item.knockBack = 6f;
            Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 4f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = Mod.Find<ModProjectile>("RudhiraProj").Type;
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(0, 0);
            return offset;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("VeinJuice").Type, 30);
            recipe.AddIngredient(ItemID.DemonBow);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(Mod.Find<ModItem>("VeinJuice").Type, 30);
            recipe2.AddIngredient(ItemID.TendonBow);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }

        public class RudhiraProj : ModProjectile
        {
            public override void SetStaticDefaults()
            {
                // DisplayName.SetDefault("Spore Arrow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
                ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
                ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            }

            bool nearMouse = false;
            int StateTimer = 0;
            int bounceCount = 0;

            public override void SetDefaults()
            {
                Projectile.DamageType = DamageClass.Ranged;
                Projectile.width = 8;
                Projectile.height = 8;
                Projectile.aiStyle = -1;
                Projectile.friendly = true;
                Projectile.penetrate = 4;
                Projectile.timeLeft = 360;
                Projectile.light = 1f;
                Projectile.ignoreWater = true;
                Projectile.tileCollide = true;
                Projectile.extraUpdates = 0;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = -1;
                Projectile.arrow = true;

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

                Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawOrigin = origin;
                spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(4f, 4f);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);
                }

                Main.EntitySpriteDraw(texture,
                    Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                    sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
                
                // It's important to return false, otherwise we also draw the original texture.
                return false;
            }

            public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
            {
                for (var i = 0; i < 20; i++)
                {
                    float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                    float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                    int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Blood, xv, yv, 0, default(Color), 1f);
                    Main.dust[dust].scale = 2f;
                    Main.dust[dust].noGravity = true;
                }
            }

            public override bool OnTileCollide(Vector2 oldVelocity)
            {
                bounceCount++;
                for (var i = 0; i < 20; i++)
                {
                    float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                    float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                    int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Blood, xv, yv, 0, default(Color), 1f);
                    Main.dust[dust].scale = 2f;
                    Main.dust[dust].noGravity = true;
                }
                if (Projectile.timeLeft > 300 && !nearMouse)
                {
                    nearMouse = true;
                }
                if (bounceCount >= 3)
                {
                    return true;
                }
                if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
                return false;
            }


            public override void AI()
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
                StateTimer++;

                if (Projectile.timeLeft > 300 && !nearMouse)
                {
                    float projSpeed = 3f; // The speed at which the projectile moves towards the target

                    // Trying to find NPC closest to the projectile
                    Vector2 mousePos = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y);

                    // If found, change the velocity of the projectile and turn it in the direction of the target
                    // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                    Projectile.velocity += (mousePos - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
                    if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > 4f)
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 4;
                    }
                    if (Vector2.Distance(mousePos, Projectile.Center) < 16f)
                    {
                        nearMouse = true;
                    }
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
                }
                else if (Projectile.timeLeft == 300 || nearMouse)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 15;
                    Projectile.extraUpdates = 2;
                    nearMouse = false;
                    Projectile.timeLeft = 299;
                }
            }
        }
    }
}