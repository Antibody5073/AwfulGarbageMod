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
    public class CandescenceProjFireBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skill Issue"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 720;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
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
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust2].scale = 1f;
            Main.dust[dust2].velocity *= 0f;
            Main.dust[dust2].noGravity = true;
            Projectile.damage = 29;
            if (Main.expertMode)
            {
                Projectile.damage = 28;
                if (Main.masterMode)
                {
                    Projectile.damage = 27;
                }
            }
        }
        public override bool ShouldUpdatePosition()
        {
            if (Projectile.timeLeft < 600)
            {
                return true;
            }
            else
            {
                Projectile.position += (Projectile.velocity / 3) * (((float)Projectile.timeLeft - 600) / 120);
                return false;
            }
        }
    }
    public class CandescenceProjFireBlastUnreal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skill Issue"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
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
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust2].scale = 1f;
            Main.dust[dust2].velocity *= 0f;
            Main.dust[dust2].noGravity = true;
            Projectile.damage = 29;
            if (Main.expertMode)
            {
                Projectile.damage = 28;
                if (Main.masterMode)
                {
                    Projectile.damage = 27;
                }
            }

            if (Projectile.ai[0]-- <= 0)
            {
                Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.05f;

                if (Projectile.timeLeft % 24 == 0)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.1f, ModContent.ProjectileType<CandescenceProjFireball>(), 17, 0, Main.myPlayer, 0, 0, 0.22f);
                    CandescenceProjFireball fireballProj = Main.projectile[proj].ModProjectile as CandescenceProjFireball;
                    fireballProj.counter = 115;
                    Main.projectile[proj].timeLeft = 150;
                }
            }
        }
    }
    public class CandescenceProjTwinFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        Vector2 centerPos;
        float offsetPos = 0;
        float offsetDistance = 12;
        Vector2 startingVel;
        int StateTimer;
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

            Texture2D projectileTexture = texture;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color.A = (byte)(color.A * 0.75f);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale - k * 0.06f, spriteEffects, 0f);
            }

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            centerPos = Projectile.position;
            offsetPos = Projectile.ai[1];
            startingVel = Projectile.velocity;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), centerPos, new Vector2(10, 0).RotatedBy(Projectile.ai[2]), ModContent.ProjectileType<CandescenceProjTwinFireShot>(), 17, 0, Main.myPlayer);

            base.OnKill(timeLeft);
        }

        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];

            StateTimer++;
            Projectile.damage = 32;
            if (Main.expertMode)
            {
                Projectile.damage = 31;
                if (Main.masterMode)
                {
                    Projectile.damage = 30;
                }
            }
            if (owner.globalEnemyBossInfo().killOrbitals || !owner.active)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.timeLeft = 3;
            }

            centerPos += Projectile.velocity;
            offsetPos += 6;
            Vector2 normalizedVel = startingVel.SafeNormalize(Vector2.One);
            Projectile.Center = centerPos + normalizedVel.RotatedBy(MathHelper.ToRadians(offsetPos)) * offsetDistance;

            Projectile.velocity *= 0.97f;

        }
    }
    public class CandescenceProjTwinFireShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }
        int StateTimer;
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint6 = 0f;
            Vector2 vector3 = new Vector2(1, 0).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.scale;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - vector3 * 96f, Projectile.Center, 8f * Projectile.scale, ref collisionPoint6))
            {
                return true;
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
            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);

            Texture2D projectileTexture = texture;
            Vector2 drawOrigin = origin;
            spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color.A = (byte)(color.A * 0.75f);
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
            Projectile.damage = 32;
            if (Main.expertMode)
            {
                Projectile.damage = 31;
                if (Main.masterMode)
                {
                    Projectile.damage = 30;
                }
            }
        }
    }
    public class CandescenceProjWarning : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 90;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
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


            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            Projectile.frame = Projectile.timeLeft % 30 < 10 ? 1 : 0;
            Projectile.Center = owner.Center + Projectile.velocity;
        }
        public override void OnKill(int timeLeft)
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (Projectile.ai[1] == 0)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center, Projectile.velocity.SafeNormalize(Vector2.One) * 16, ProjectileID.Flames, 25, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false; 
                proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center, Projectile.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.ToRadians(6)) * 16, ProjectileID.Flames, 25, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false; 
                proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), owner.Center, Projectile.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.ToRadians(-6)) * 16, ProjectileID.Flames, 25, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
            }
        }
    }
    public class CandescenceProjFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skill Issue"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        public int counter = 0;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 720;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
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
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust2].scale = 1f;
            Main.dust[dust2].velocity *= 0f;
            Main.dust[dust2].noGravity = true;
            Projectile.damage = 31;
            if (Main.expertMode)
            {
                Projectile.damage = 29;
                if (Main.masterMode)
                {
                    Projectile.damage = 28;
                }
            }
            counter++;
            if (counter > 120)
            {
                Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.ai[2];
            }
            if (Vector2.Distance(Vector2.Zero, Projectile.velocity) > 15)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 15;
            }

            Projectile.ai[1]--;
            if (Projectile.ai[1] > 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0]);
            }
        }
    }
    public class CandescenceProjFireballSlow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skill Issue"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        int counter;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 720;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
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
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust2].scale = 1f;
            Main.dust[dust2].velocity *= 0f;
            Main.dust[dust2].noGravity = true;
            Projectile.damage = 24;
            if (Main.expertMode)
            {
                Projectile.damage = 23;
                if (Main.masterMode)
                {
                    Projectile.damage = 22;
                }
            }
        }
    }

    public class CandescenceProjLavaOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 4;
        }
        int counter;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
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
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint6 = 0f;
            Vector2 vector3 = new Vector2(1, 0).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.scale;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.oldPos[0] + new Vector2(Projectile.width / 2, Projectile.height / 2), 8f * Projectile.scale, ref collisionPoint6))
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
            Projectile.damage = 33;
            NPC owner = Main.npc[(int)Projectile.ai[0]];

            Projectile.Center = owner.Center + new Vector2(owner.globalEnemyBossInfo().OrbitDistance * Projectile.ai[2], 0).RotatedBy(MathHelper.ToRadians(owner.globalEnemyBossInfo().OrbitDirection + Projectile.ai[1]));
            if (owner.globalEnemyBossInfo().killOrbitals || !owner.active)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.timeLeft = 3;
            }

            Projectile.hostile = owner.globalEnemyBossInfo().orbitalsDealDamage;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }
    }
    public class CandescenceProjLavaOrb2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 4;
        }
        int counter;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
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
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint6 = 0f;
            Vector2 vector3 = new Vector2(1, 0).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.scale;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.oldPos[0] + new Vector2(Projectile.width / 2, Projectile.height / 2), 8f * Projectile.scale, ref collisionPoint6))
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
            Projectile.damage = 33;
            NPC owner = Main.npc[(int)Projectile.ai[0]];

            Projectile.Center = owner.Center + new Vector2(owner.globalEnemyBossInfo().OrbitDistance * Projectile.ai[2], 0).RotatedBy(MathHelper.ToRadians(-owner.globalEnemyBossInfo().OrbitDirection + Projectile.ai[1]));
            if (owner.globalEnemyBossInfo().killOrbitals || !owner.active)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.timeLeft = 3;
            }

            Projectile.hostile = owner.globalEnemyBossInfo().orbitalsDealDamage;
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }
    }

    public class CandescenceProjHomingFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skill Issue"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        float turned;
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
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
            int dust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default(Color), 1f);
            Main.dust[dust2].scale = 1f;
            Main.dust[dust2].velocity *= 0f;
            Main.dust[dust2].noGravity = true;
            Projectile.damage = 37;
            if (Main.expertMode)
            {
                Projectile.damage = 35;
                if (Main.masterMode)
                {
                    Projectile.damage = 34;
                }
            }
            if (turned < MathHelper.Pi)
            {
                float turn = AGUtils.TurnTowards(Projectile.ai[0], Main.player[Projectile.owner].Center, Projectile.velocity.ToRotation(), Projectile.Center);
                turned += Math.Abs(turn);
                Projectile.velocity = Projectile.velocity.RotatedBy(turn);
            }
        }
    }
}