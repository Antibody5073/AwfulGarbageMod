using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using static Terraria.ModLoader.PlayerDrawLayer;
using AwfulGarbageMod.NPCs.Boss;


namespace AwfulGarbageMod.Projectiles
{
    public class EoTSWallCloudTemporary : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cloud"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            Main.projFrames[Projectile.type] = 6;
        }
        int counter;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 40;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 480;
            Projectile.light = 1f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
        }



        public override void AI()
        {
            if (Projectile.timeLeft >= 450)
            {
                Projectile.alpha = (Projectile.timeLeft - 450) * 255 / 30;
                Projectile.hostile = false;
            }
            else if (Projectile.timeLeft < 30)
            {
                Projectile.alpha = 255 - Projectile.timeLeft * 255 / 30;
                Projectile.hostile = false;
            }
            else
            {
                Projectile.hostile = true;
                if (counter % 30 == 0)
                {
                    Projectile.NewProjectile(Projectile.GetSource_NaturalSpawn(), Projectile.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                }
            }
            EyeOfTheStorm boss = Main.npc[(int)Projectile.ai[0]].ModNPC as EyeOfTheStorm;
            if (Projectile.timeLeft > 30 && (!Main.npc[(int)Projectile.ai[0]].active || (boss.AI_State != 10 && boss.AI_State != 12 && boss.AI_State != 14 && boss.AI_State != 2)))
            {
                Projectile.timeLeft = 30;
            }

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                // Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
            counter++;
           
            if (Main.player[Main.myPlayer].Center.Y < Projectile.Center.Y)
            {
                Projectile.position.Y = Main.player[Main.myPlayer].Center.Y - Projectile.height / 2;
            }
            if (Main.player[Main.myPlayer].Center.Y - 480 > Projectile.Center.Y)
            {
                Projectile.position.Y = Main.player[Main.myPlayer].Center.Y - 480 - Projectile.height / 2;
            }
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
            float offsetX = 27f;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            float offsetY = 12f;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);


            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }
}