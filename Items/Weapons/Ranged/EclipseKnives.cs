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
using AwfulGarbageMod.Tiles.Furniture;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Weapons.Ranged
{

    public class EclipseKnives : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knives"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Throw 2-3 knives at a time");
        }

        public override void SetDefaults()
        {
            Item.damage = 155;
            Item.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Item.width = 45;
            Item.height = 45;
            Item.useTime = 24;
            Item.noMelee = true;
            Item.scale = 0f;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1.75f;
            Item.value = 10000;
            Item.rare = 9;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BouncyKnivesProj>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (Main.dayTime)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(2)) * Main.rand.NextFloat(0.95f, 1.05f), ModContent.ProjectileType<EclipseKnivesProjSun>(), damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(2)) * Main.rand.NextFloat(0.95f, 1.05f), ModContent.ProjectileType<EclipseKnivesProjMoon>(), damage, knockback, player.whoAmI);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(2)) * Main.rand.NextFloat(0.95f, 1.05f), ModContent.ProjectileType<EclipseKnivesProjMoon>(), damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(2)) * Main.rand.NextFloat(0.95f, 1.05f), ModContent.ProjectileType<EclipseKnivesProjSun>(), damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    public class EclipseKnivesProjSun : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        float spinSpd;
        Vector2 centerPos;

        float offsetPos = 0;
        float offsetDistance = 18;
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 450;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (var i = 0; i < 9; i++)
            {
                Vector2 dustvel = new Vector2(Main.rand.NextFloat(8, 14.5f), 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.ToRadians(Main.rand.NextFloat(-18, 18)));
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.SolarFlare, dustvel.X, dustvel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.2f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (var i = 0; i < 9; i++)
            {
                Vector2 dustvel = new Vector2(Main.rand.NextFloat(8, 14.5f), 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.ToRadians(Main.rand.NextFloat(-18, 18)));
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.SolarFlare, dustvel.X, dustvel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.2f;
                Main.dust[dust].noGravity = true;
            }
            return true;
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
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 3.5f);
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

            if (Projectile.timeLeft == 420)
            {
                spinSpd = Projectile.velocity.X * 0.06f;
                if (spinSpd > 0.2f) { spinSpd = 0.2f; }
                if (spinSpd < -0.2f) { spinSpd = -0.2f; }
            }
            else if (Projectile.timeLeft < 420)
            {
                Projectile.rotation += spinSpd;
                Projectile.velocity.Y -= 0.25f;
                Projectile.velocity.X *= 0.99f;
                if (Projectile.velocity.Y < -12)
                {
                    Projectile.velocity.Y = -12;
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.ToRadians(30);
            }


            offsetPos += 24;
            centerPos = Projectile.position + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(90)) * (float)Math.Sin((double)MathHelper.ToRadians(offsetPos)) * offsetDistance;
            offsetDistance -= 0.25f;
            if (offsetDistance < 0)
            {
                offsetDistance = 0;
            }
            for (var i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(centerPos, Projectile.width, Projectile.height, DustID.SolarFlare, 0, 0);
                Main.dust[dust].scale = Main.rand.NextFloat(1f, 1.25f);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }
        }
    }
    public class EclipseKnivesProjMoon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimy Knife"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        float spinSpd;
        Vector2 centerPos;

        float offsetPos = 0;
        float offsetDistance = 18;
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<KnifeDamageClass>();
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 450;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (var i = 0; i < 9; i++)
            {
                Vector2 dustvel = new Vector2(Main.rand.NextFloat(8, 14.5f), 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.ToRadians(Main.rand.NextFloat(-18, 18)));
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.LunarRust, dustvel.X, dustvel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.2f;
                Main.dust[dust].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (var i = 0; i < 9; i++)
            {
                Vector2 dustvel = new Vector2(Main.rand.NextFloat(8, 14.5f), 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.ToRadians(Main.rand.NextFloat(-18, 18)));
                int dust = Dust.NewDust(Projectile.position, 1, 1, DustID.LunarRust, dustvel.X, dustvel.Y, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.2f;
                Main.dust[dust].noGravity = true;
            }
            return true;
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
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 3.5f);
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

            if (Projectile.timeLeft == 420)
            {
                spinSpd = Projectile.velocity.X * 0.06f;
                if (spinSpd > 0.2f) { spinSpd = 0.2f; }
                if (spinSpd < -0.2f) { spinSpd = -0.2f; }
            }
            else if (Projectile.timeLeft < 420)
            {
                Projectile.rotation += spinSpd;
                Projectile.velocity.Y += 0.25f;
                Projectile.velocity.X *= 0.99f;
                if (Projectile.velocity.Y > 12)
                {
                    Projectile.velocity.Y = 12;
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.ToRadians(30);
            }

            offsetPos -= 24;
            centerPos = Projectile.position + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(90)) * (float)Math.Sin((double)MathHelper.ToRadians(offsetPos)) * offsetDistance;

            offsetDistance -= 0.25f;
            if (offsetDistance < 0)
            {
                offsetDistance = 0;    
            }
            for (var i = 0; i < 2; i++)
            {
                int dust = Dust.NewDust(centerPos, Projectile.width, Projectile.height, DustID.LunarRust, 0, 0);
                Main.dust[dust].scale = Main.rand.NextFloat(1f, 1.25f);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;

            }
        }
    }
}