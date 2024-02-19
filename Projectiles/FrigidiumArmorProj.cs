using AwfulGarbageMod.Items.Weapons;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.DamageClasses;
using Microsoft.Xna.Framework.Graphics;

namespace AwfulGarbageMod.Projectiles
{


    // This minion shows a few mandatory things that make it behave properly.
    // Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
    // If the player targets a certain NPC with right-click, it will fly through tiles to it
    // If it isn't attacking, it will float near the player with minimal movement
    public class FrigidiumArmorProj : ModProjectile
    {

        public float dir;

        public virtual float ProjSpd => 0;
        public virtual int ProjType => -1;
        public virtual float OrbitDistance => 90;
        public virtual float OrbitSpd => 1;
        public virtual float OffsetDir => 0;

        float distance = 90;
        
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }


        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] == -1)
            {
                for (var i = 0; i < 10; i++)
                {
                    float xv = (float)Math.Sin(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                    float yv = (float)Math.Cos(MathHelper.ToRadians(Main.rand.Next(0, 360))) * Main.rand.Next(3, 18);
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ice, xv, yv, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }


        public virtual void GetOrbitDirection(out float tempdir, out Vector2 targetPos, float spd = 1, float distance = 120)
        {

            Player player = Main.player[Projectile.owner];
            tempdir = MathHelper.ToRadians((player.GetModPlayer<GlobalPlayer>().OrbitalDir * spd) + Projectile.ai[0] * 360 / player.ownedProjectileCounts[this.Type]);
            targetPos = player.MountedCenter + new Vector2(distance, 0).RotatedBy(tempdir) - new Vector2(Projectile.width / 2, Projectile.height / 2);
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

        public virtual void SetRotation(Player player, float direction, float offsetDir = 0)
        {
            Projectile.rotation = 0;
            dir = direction;

        }

        public virtual void SetPositionBasedOnDirection(float direction, Vector2 targetPos)
        {
            Projectile.position = targetPos;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.ai[1] == 0)
            {
                GetOrbitDirection(out float tempDir, out Vector2 targetPos, OrbitSpd, OrbitDistance);
                SetPositionBasedOnDirection(tempDir, targetPos);

                SetRotation(player, tempDir, OffsetDir);

                Projectile.timeLeft = 60;

                distance = 90;
                Visuals();
            }
            else if (Projectile.ai[1] == 1)
            {
                Projectile.position = player.MountedCenter + new Vector2(distance, 0).RotatedBy(dir) - new Vector2(Projectile.width / 2, Projectile.height / 2);
                distance /= 1.15f;
                if (distance < 32)
                {
                    Projectile.alpha += 20;
                }
            }

        }

        public virtual void Visuals()
        {

        }
    }
}