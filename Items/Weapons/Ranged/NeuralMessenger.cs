using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class NeuralMessenger : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spore Bow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Sometimes shoots a mushroom spore arrow");
        }

        public int counter;

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 5;
            Item.noMelee = true;
            Item.useAnimation = 10;
            Item.reuseDelay = 19;
            Item.useStyle = 5;
            Item.knockBack = 0f;
            Item.value = 10000;
            Item.rare = 1;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = 1;
            Item.useAmmo = AmmoID.Arrow;
            Item.shootSpeed = 10f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
            {
                type = Mod.Find<ModProjectile>("NeuralMessengerProj").Type;
            }
        }

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new Vector2(2, 0);
            return offset;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CrimtaneBar, 16);
            recipe.AddIngredient(ItemID.TissueSample, 20);
            recipe.AddIngredient(ItemID.Vertebrae, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public class NeuralMessengerProj : ModProjectile
        {
            public override void SetStaticDefaults()
            {
                // DisplayName.SetDefault("Spore Arrow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            }

            public override void SetDefaults()
            {
                Projectile.DamageType = DamageClass.Ranged;
                Projectile.width = 8;
                Projectile.height = 8;
                Projectile.aiStyle = -1;
                Projectile.friendly = true;
                Projectile.penetrate = 1;
                Projectile.timeLeft = 150;
                Projectile.light = 1f;
                Projectile.ignoreWater = true;
                Projectile.tileCollide = true;
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
                Main.EntitySpriteDraw(texture,
                    Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                    sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

                // It's important to return false, otherwise we also draw the original texture.
                return false;
            }

            public override void AI()
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);


                if (Projectile.timeLeft < 135)
                {
                    float projSpeed = 1f; // The speed at which the projectile moves towards the target

                    Projectile.velocity += new Vector2(Main.mouseX + Main.screenPosition.X - Projectile.Center.X, Main.mouseY + Main.screenPosition.Y - Projectile.Center.Y).SafeNormalize(Vector2.Zero) * projSpeed;
                    if (Vector2.Distance(new Vector2(0, 0), Projectile.velocity) > 12f)
                    {
                        Projectile.velocity *= 0.85f;
                    }
                }
            }
        }
    }
}