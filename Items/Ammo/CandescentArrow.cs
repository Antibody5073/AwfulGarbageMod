using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Ammo
{
    public class CandescentArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stone Pellet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Low damage, easy to obtain bullet");
        }

        public override void SetDefaults()
        {
            Item.damage = 10; // The damage for projectiles isn't actually 12, it actually is the damage combined with the projectile and the item together.
            Item.DamageType = DamageClass.Ranged;
            Item.width = 8;
            Item.height = 8;
            Item.maxStack = 9999;
            Item.consumable = true; // This marks the item as consumable, making it automatically be consumed when it's used as ammunition, or something else, if possible.
            Item.knockBack = 3f;
            Item.value = 15000;
            Item.rare = 5;
            Item.shoot = ModContent.ProjectileType<CandescentArrowProj>(); // The projectile that weapons fire when using this item as ammunition.
            Item.shootSpeed = 5.5f; // The speed of the projectile.
            Item.ammo = AmmoID.Arrow; // The ammo class this ammo belongs to.
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient<Placeable.OresBars.CandesciteBar>(2)
                .AddIngredient(ItemID.WoodenArrow, 100)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe(150)
                .AddIngredient<Placeable.OresBars.CandesciteBar>(2)
                .AddIngredient(ItemID.WoodenArrow, 125)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe(200)
                .AddIngredient<Placeable.OresBars.CandesciteBar>(2)
                .AddIngredient(ItemID.HellfireArrow, 150)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class CandescentArrowProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 420;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 17;
            Projectile.arrow = true;
        }
        Vector2 spawnPos;
        int temp;
        int StateTimer;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 1.2f;
            spawnPos = Projectile.Center;
            temp = 12;
        }
        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 6; i++)
            {
                float xv = Main.rand.NextFloat(-3, 3);
                float yv = Main.rand.NextFloat(-2, -5);
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.Torch, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.5f, 1.2f);
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
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2.5f);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
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
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
            temp--;
            if (temp == 0 || temp == 4 || temp == 8)
            {
                Vector2 projVel;
                if (Projectile.velocity.Length() * (Projectile.extraUpdates + 1) < 20)
                {
                    projVel = Projectile.velocity * (Projectile.extraUpdates + 1)/4;
                }
                else
                {
                    projVel = Projectile.velocity.SafeNormalize(Vector2.Zero) * 5;
                }
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, projVel.RotatedByRandom(MathHelper.ToRadians(1.5f)), ModContent.ProjectileType<CandescentArrowProj2>(), (int)(Projectile.damage * 0.25f), Projectile.knockBack / 3, Projectile.owner, 0);
                if (Main.player[Projectile.owner].magicQuiver)
                {
                    Main.projectile[proj].extraUpdates += 4;
                }
            }
        }
    }
    public class CandescentArrowProj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 36;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        int StateTimer = 0;
        float turn = 0;
        

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 750;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 3;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
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
            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);

            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(4f, 4f);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color.A = (byte)(color.A * 0.75f);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale - k * 0.02f, spriteEffects, 0f);
            }

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 vel = Projectile.oldVelocity;
            if (Projectile.velocity.X != oldVelocity.X) vel.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) vel.Y = -oldVelocity.Y;
            for (var i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, vel.X, vel.Y, 0, default(Color), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = Main.rand.NextFloat(0.5f, 1.5f);
                Main.dust[dust].velocity = vel.RotatedByRandom(MathHelper.ToRadians(16)) * Main.rand.NextFloat(0.5f, 1.5f);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(18))
            {
                target.AddBuff(BuffID.OnFire3, 120);
            }
        }

        public override void AI()
        {
            StateTimer++;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (StateTimer % 12 == 0)
            {
                float turnAmt = Main.rand.NextFloat(-15, 15);
                turn += turnAmt;
                if (Math.Abs(turn) > 24)
                {
                    turn -= turnAmt;
                    turnAmt *= -1f;
                    turn += turnAmt;
                }
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(turnAmt));
            }

        }
    }
}