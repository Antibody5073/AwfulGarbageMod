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

    public class Sprinkler : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sprinkler"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

        int counter = 0;
		public override void SetDefaults()
		{
			Item.damage = 16;
            Item.mana = 6;
			Item.DamageType = DamageClass.Magic;
			Item.width = 30;
			Item.height = 30;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.knockBack = 0.1f;
			Item.value = 10000;
			Item.rare = 1;
			Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SprinklerProj>();
			Item.shootSpeed = 9f;
			Item.noMelee = true;
            Item.noUseGraphic = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Projectile.NewProjectile(source, position, new Vector2(0, Main.rand.NextFloat(-4f, -7f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 31))), type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<SprinklerHoldout>(), 0, 0, player.whoAmI, counter);

            for (int i = 0; i < 12; i++)
            {
                int dust = Dust.NewDust(position - new Vector2(3, 3), 4, 4, DustID.Water, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(3, 3) + new Vector2(0, -8);
                Main.dust[dust].scale = Main.rand.NextFloat(1, 1.5f);
                Main.dust[dust].alpha = 100;
                Main.dust[dust].noGravity = false;
            }
            counter = (counter + 1) % 2;
            return false;
        }
        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Waterleaf, 3);
            recipe.AddIngredient(ItemID.SilverBar, 5);
            recipe.AddIngredient(ItemID.WaterBucket, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Waterleaf, 3);
            recipe2.AddIngredient(ItemID.TungstenBar, 5);
            recipe2.AddIngredient(ItemID.WaterBucket, 5);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
    public class SprinklerHoldout : ModProjectile
    {
        public override string Texture => "AwfulGarbageMod/Items/Weapons/Magic/Sprinkler";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[base.Type] = true;
            ProjectileID.Sets.CanDistortWater[base.Type] = false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public int duration;

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = 0.5f;
            }
            else
            {
                Projectile.rotation = -0.5f;
            }
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
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(0, -20).RotatedBy(Projectile.rotation),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[base.Projectile.owner];
            duration = player.itemAnimationMax;
            player.heldProj = base.Projectile.whoAmI;
            if (base.Projectile.timeLeft > duration)
            {
                base.Projectile.timeLeft = duration;
            }
            Projectile.Center = player.HandPosition.Value;
            Projectile.spriteDirection = player.direction;
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation += ((-0.5f) - Projectile.rotation) / 8; 
            }
            else
            {
                Projectile.rotation += (0.5f - Projectile.rotation) / 8;
            }
        }
    }


    public class SprinklerProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 480;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.extraUpdates = 2;
        }
        bool spawnedProj = false;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 300)
            {
                if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;
            }
            else
            {
                Projectile.penetrate = -1;
                Projectile.damage = 0;
            }
            return false;
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
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void AI()
        {
            if (Projectile.penetrate == -1)
            {
                if (!spawnedProj)
                {
                    spawnedProj = true;
                    Projectile.frame = 2;
                    Projectile.frameCounter = 0;
                    Projectile.rotation = 0;
                    for (int i = 0; i < 12; i++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].velocity = Main.rand.NextVector2CircularEdge(8, 8) * Main.rand.NextFloat(0.5f, 1f);
                        Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
                        Main.dust[dust].noGravity = false;
                    }
                }
                Projectile.velocity = Vector2.Zero;
                Projectile.timeLeft++;

                if (++Projectile.frameCounter >= 16)
                {
                    Projectile.frameCounter = 0;
                    // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                    if (++Projectile.frame >= 4)
                        Projectile.Kill();
                }

            }
            else
            {
                if (Projectile.timeLeft > 300)
                {
                    Projectile.frame = 0;
                    Projectile.velocity *= 0.98f;
                }
                else if (Projectile.timeLeft == 300)
                {
                    Vector2 vel = new Vector2((float)Main.mouseX + Main.screenPosition.X - Projectile.position.X, (float)Main.mouseY + Main.screenPosition.Y - Projectile.position.Y);
                    vel.Normalize();
                    Projectile.velocity = vel * 10f;
                    Projectile.frame = 1;
                }
                else
                {
                    Projectile.velocity.Y += 0.05f;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
                }
                if (Main.rand.NextBool())
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].velocity *= 0.25f;
                    Main.dust[dust].scale = (float)Main.rand.Next(50, 85) * 0.013f;
                    Main.dust[dust].noGravity = true;
                }

            }
        }
    }
}