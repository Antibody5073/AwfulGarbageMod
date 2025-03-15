using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;

namespace AwfulGarbageMod.NPCs.BossUnrealRework.DukeFishron
{
    public class DukeFishronProjWave : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Main.projFrames[Type] = 2;
            // DisplayName.SetDefault("Skill Issue"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }
        bool didit = false;
        Vector2 startpos; 
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 1000;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void AI()
        {
            NPC boss = Main.npc[(int)Projectile.ai[0]];
            if (boss.active)
            {
                Projectile.Center = boss.Center + new Vector2(0, 450);
                Projectile.timeLeft = 6;
            }
            else
            {
                Projectile.Kill();
            }
        }
    }
    public class DukeFishronProjFish : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        bool didit = false;
        Vector2 startpos;
        int StateTimer;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture;
            switch (Projectile.ai[0])
            {
                default:
                    texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.Tuna}");
                    break;
                case 0:
                    texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.Tuna}");
                    break;
                case 1:
                    texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.Tuna}");
                    break;
                case 2:
                    texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.Salmon}");
                    break;
                case 3:
                    texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.Salmon}");
                    break;
                case 4:
                    texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.Swordfish}");
                    break;
                case 5:
                    texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.ReaverShark}");
                    break;
            }

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color.A = (byte)(color.A * 0.75f);
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, Projectile.oldRot[k], origin, Projectile.scale - k * 0.06f, spriteEffects, 0f);
            }
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
            StateTimer++;

            if (Projectile.Bottom.Y < Main.player[Projectile.owner].Bottom.Y - 200)
            {
                didit = true;
            }
            if (didit)
            {
                Projectile.velocity.Y += 0.2f;
                if (Projectile.velocity.Y > 9)
                {
                    Projectile.velocity.Y = 9;
                }
            }
            Projectile.rotation += Projectile.ai[1];
        }
    }
    public class DukeFishronProjTyphoon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 3;
        }
        bool didit = false;
        Vector2 startpos;
        int StateTimer;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Projectile_{ProjectileID.Typhoon}");

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color.A = (byte)(color.A * 0.75f);
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, Projectile.oldRot[k], origin, Projectile.scale - k * 0.06f, spriteEffects, 0f);
            }
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
            StateTimer++;
            Player player = Main.player[Projectile.owner];

            if (Projectile.Center.Y > player.Center.Y + 720)
            {
                Projectile.position.Y -= 1440;
            }
            if (Projectile.Center.Y < player.Center.Y - 720)
            {
                Projectile.position.Y += 1440;
            }
            int frameSpeed = 4;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.rotation += 0.2f;
        }
    }
    public class DukeFishronProjTyphoon2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            Main.projFrames[Projectile.type] = 3;
        }
        bool didit = false;
        Vector2 startpos;
        int StateTimer;
        float spd = 1;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 540;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Projectile_{ProjectileID.Typhoon}");

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color.A = (byte)(color.A * 0.75f);
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, Projectile.oldRot[k], origin, Projectile.scale - k * 0.06f, spriteEffects, 0f);
            }
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
            StateTimer++;
            if (StateTimer > 30)
            {
                spd *= 0.99f;
            }
            Projectile.position -= Projectile.velocity * spd;
            int frameSpeed = 4;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.rotation += 0.2f;
        }
    }
    public class DukeFishronProjTornado : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }
        bool didit = false;
        Vector2 startpos;
        int StateTimer;

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 36;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 210;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Projectile_{ProjectileID.Sharknado}");

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height / 2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color.A = (byte)(color.A * 0.75f);
                Main.spriteBatch.Draw(texture, drawPos, sourceRectangle, color, Projectile.oldRot[k], origin, Projectile.scale - k * 0.06f, spriteEffects, 0f);
            }
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
            int frameSpeed = 5;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }
    }
    public class DukeFishronProjTornadoBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }
        bool didit = false;
        Vector2 startpos;
        int StateTimer;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            if (!didit)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (Projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                // Getting texture of projectile
                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>($"Terraria/Images/Projectile_{ProjectileID.SharknadoBolt}");

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
            }
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            startpos = Projectile.position;
        }
        public override void AI()
        {
            if (Projectile.position.Y < startpos.Y + 1500 && !didit)
            {
                Projectile.velocity.Y += 0.25f;
            }
            else
            {
                if (!didit)
                {
                    Projectile.position.Y = startpos.Y + 1050;
                    Projectile.velocity.Y = -42;
                    didit = true;
                }
                if (Projectile.position.Y < startpos.Y - 210)
                {
                    Projectile.Kill();
                }
                else
                {
                    DukeFishron boss = Main.npc[(int)Projectile.ai[0]].ModNPC as DukeFishron;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(boss.atkX * 3.5f, 0), ModContent.ProjectileType<DukeFishronProjTornado>(), 22, 0, Main.myPlayer);
                    Projectile.position.X += boss.atkX * 3.5f;
                }
            }

            int frameSpeed = 4;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }
    }
    public class DukeFishronProjTornadoBoltSpawner : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        bool didit = false;
        Vector2 startpos;
        int StateTimer;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            startpos = Projectile.position;
        }
        public override void AI()
        {
           if (Projectile.timeLeft % 12 == 6)
           {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, -2), ModContent.ProjectileType<DukeFishronProjTornadoBolt>(), 25, 0, Main.myPlayer, Projectile.ai[0]);

            }
        }
    }
}
