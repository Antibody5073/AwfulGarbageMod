using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace AwfulGarbageMod.Projectiles
{
    public class SpiritReaperProjSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 1260;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        int StateTimer = 0;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint6 = 0f;
            Vector2 vector3 = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(-1.5707963705062866) * Projectile.scale;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - vector3 * 50f, Projectile.Center + vector3 * 50f, 16f * Projectile.scale, ref collisionPoint6))
            {
                return true;
            }
            return false;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;


            Texture2D texture2 = ModContent.Request<Texture2D>("AwfulGarbageMod/Projectiles/EoTSLightningTele").Value;
            DrawLaser(texture2, Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), 30, 800, lightColor, -1.57f, 1f, 2000f, Color.White, 0);

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;

            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(9f, 9f);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
                Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);
            }
            return false;
        }
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float step, float distance, Color lightColor, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
        {
            float r = unit.ToRotation() + rotation;

            Color drawColor = Projectile.GetAlpha(lightColor);

            float amt = 0;
            for (float max = 0; max <= distance; max += step)
            {
                amt += 1;
            }
            float current = 0;
            // Draws the laser 'body'
            for (float i = transDist; i <= distance; i += step)
            {
                Color c = drawColor * ((float)(amt - current) / (float)amt);
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 24, 28, 30), c, r,
                    new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
                current++;
            }
        }
        public override void AI()
        {
            StateTimer++;
            Projectile.rotation = Projectile.velocity.ToRotation();

        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool ShouldUpdatePosition()
        {
            if (Projectile.timeLeft < 1200)
            {
                return true;
            }
            else
            {
                Projectile.position += (Projectile.velocity / 2) * (((float)Projectile.timeLeft - 1200) / 60);
                return false;
            }
        }
    }
    public class SpiritReaperProjSlashSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 1260;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        int StateTimer = 0;


        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;


            Texture2D texture2 = ModContent.Request<Texture2D>("AwfulGarbageMod/Projectiles/EoTSLightningTele").Value;
            DrawLaser(texture2, Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero), 30, 400, lightColor, -1.57f, 1f, 2000f, Color.White, 0);

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;

            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(9f, 9f);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
                Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);
            }
            return false;
        }
        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float step, float distance, Color lightColor, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default(Color), int transDist = 50)
        {
            float r = unit.ToRotation() + rotation;

            Color drawColor = Projectile.GetAlpha(lightColor);

            float amt = 0;
            for (float max = 0; max <= distance; max += step)
            {
                amt += 1;
            }
            float current = 0;
            // Draws the laser 'body'
            for (float i = transDist; i <= distance; i += step)
            {
                Color c = drawColor * ((float)(amt - current) / (float)amt);
                var origin = start + i * unit;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition,
                    new Rectangle(0, 24, 28, 30), c, r,
                    new Vector2(28 * .5f, 26 * .5f), scale, 0, 0);
                current++;
            }
        }
        public override void AI()
        {
            StateTimer++;
            Projectile.rotation = Projectile.velocity.ToRotation();

        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool ShouldUpdatePosition()
        {
            if (Projectile.timeLeft < 1200)
            {
                return true;
            }
            else
            {
                Projectile.position += (Projectile.velocity / 1.5f) * (((float)Projectile.timeLeft - 1200) / 60);
                return false;
            }
        }
    }

    public class SpiritReaperProjScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.scale = 1.5f;
        }
        int StateTimer = 0;
        float RotateDir;
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
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(15f, 15f);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            StateTimer++;

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0, 0, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 0;
            Main.dust[dust].noGravity = true;
            if (StateTimer % 20 == 0)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileID.DeathSickle, Projectile.damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[proj].friendly = false;
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].DamageType = DamageClass.Default;
            }
            Projectile.rotation += Main.rand.NextFloat(0.35f, 0.5f);
            if (Projectile.timeLeft >= 1170)
            {
                RotateDir = Projectile.velocity.ToRotation();
            }
            else if (Projectile.timeLeft >= 1020)
            {
                RotateDir += AGUtils.TurnTowards(5, player.Center, RotateDir, Projectile.Center);
                Projectile.velocity = new Vector2(24, 0).RotatedBy(RotateDir);


                if (Projectile.timeLeft == 1110)
                {
                    Vector2 dir = Vector2.One.RotatedByRandom(MathHelper.TwoPi);

                    for (var i = 0; i < 16; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), owner.Center, dir.RotatedBy(MathHelper.TwoPi * i / 16) * 5.5f, ModContent.ProjectileType<SpiritReaperProjShadowBeam>(), Projectile.damage * 2 / 3, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
            else
            {
                if (owner.active)
                {
                    RotateDir += AGUtils.TurnTowards(8, owner.Center, RotateDir, Projectile.Center);
                    Projectile.velocity = new Vector2(24, 0).RotatedBy(RotateDir);

                    if (Vector2.DistanceSquared(owner.Center, Projectile.Center) < 36 * 36)
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
        }
    }
    public class SpiritReaperProjScytheChase : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 1100;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.scale = 1.5f;
        }
        int StateTimer = 0;
        float RotateDir;
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
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(15f, 15f);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            StateTimer++;

            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0, 0, 0, default(Color), 1f);
            Main.dust[dust].velocity *= 0;
            Main.dust[dust].noGravity = true;
            Player player = Main.player[Projectile.owner];
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            StateTimer++;
            Projectile.rotation += Main.rand.NextFloat(0.35f, 0.5f);
            if (StateTimer % 20 == 0)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileID.DeathSickle, Projectile.damage, Projectile.knockBack, Projectile.owner);
                Main.projectile[proj].friendly = false;
                Main.projectile[proj].hostile = true; 
                Main.projectile[proj].DamageType = DamageClass.Default;
            }
            if (owner.active)
            {
                Vector2 direction = player.Center - Projectile.Center;

                Projectile.velocity = direction * 0.025f + direction.SafeNormalize(Vector2.Zero) * 4;
            }
            else
            {
                Projectile.Kill();
            }
        }

    }
    public class SpiritReaperProjShadowBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Water"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 2;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        int StateTimer;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 450;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(0.7f, 1.2f);
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(8, 8);
                Main.dust[dust].noGravity = true;
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
        }
    }
}