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
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace AwfulGarbageMod.Projectiles
{


    // This minion shows a few mandatory things that make it behave properly.
    // Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
    // If the player targets a certain NPC with right-click, it will fly through tiles to it
    // If it isn't attacking, it will float near the player with minimal movement
    public class GlacialEyeProj : ModProjectile
    {
        public float dir;
        public float pupilDir;
        public float pupilMagnitude;
        public int[] frames = {0, 1, 2, 1};

        public virtual float ProjSpd => 0;
        public virtual int ProjType => -1;
        public virtual float OrbitDistance => 75;
        public virtual float OrbitSpd => -0.9f;
        public virtual float OffsetDir => 0;

        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 3;
            // This is necessary for right-click targeting

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
        }

        // Here you can decide if your minion breaks things like grass or pots
        int timer = 0;

        public override bool? CanCutTiles()
        {
            return false;
        }

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
            Texture2D pupil = (Texture2D)ModContent.Request<Texture2D>("AwfulGarbageMod/Projectiles/GlacialEyePupil");


            // Calculating frameHeight and current Y pos dependence of frame
            // If texture without animation frameHeight is always texture.Height and startY is always 0
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * frames[Projectile.frame];

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // If image isn't centered or symmetrical you can specify origin of the sprite
            // (0,0) for the upper-left corner
            
            float offsetX = 17;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);

            float offsetY = 17;
            origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);
            

            // Applying lighting and draw current frame
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(-3f, Projectile.gfxOffY + 12),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.

            frameHeight = pupil.Height;
            startY = 0;

            // Get this frame on pupil
            sourceRectangle = new Rectangle(0, startY, pupil.Width, frameHeight);

            // Alternatively, you can skip defining frameHeight and startY and use this:
            // Rectangle sourceRectangle = pupil.Frame(1, Main.projFrames[Projectile.type], frameY: Projectile.frame);

            origin = sourceRectangle.Size() / 2f;


            Main.EntitySpriteDraw(pupil,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(pupilMagnitude, 0).RotatedBy(pupilDir),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }

        public virtual void SetRotation(Player player, float direction, float offsetDir = 0)
        {
            float maxDetectRadius = 500f;
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
            {
                dir = direction;
                return;
            }
            dir = (closestNPC.Center - Projectile.Center).ToRotation();

            pupilDir = dir;
            pupilMagnitude = 5.5f;
        }

        public virtual void SetPositionBasedOnDirection(float direction, Vector2 targetPos)
        {
            Projectile.position = targetPos;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            pupilDir = 0;
            pupilMagnitude = 0;
            if (!CheckActive(player))
            {
                return;
            }
            GetOrbitDirection(out float tempDir, out Vector2 targetPos, OrbitSpd, OrbitDistance);
            SetPositionBasedOnDirection(tempDir, targetPos);

            SetRotation(player, tempDir, OffsetDir);

            if (!player.GetModPlayer<GlobalPlayer>().GlacialEyePassive)
            {
                Shoot();
            }
            Visuals();

        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                return false;
            }

            if (owner.GetModPlayer<GlobalPlayer>().GlacialEye == true)
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }
        public virtual void Visuals()
        {
            int frameSpeed = 10;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= frames.Length)
                {
                    Projectile.frame = 0;
                }
            }


        }

        public virtual void Shoot()
        {
            if (timer > 0)
            {
                timer--;
                return;
            }

            float maxDetectRadius = 500f; // The maximum radius at which a projectile can detect a target
            // Trying to find NPC closest to the projectile
            NPC closestNPC = FindClosestNPC(maxDetectRadius);
            if (closestNPC == null)
                return;

            Vector2 direction;


            direction = closestNPC.Center - Projectile.Center;
            direction.Normalize();
            int proj = Projectile.NewProjectile(Projectile.GetSource_ReleaseEntity(), Projectile.Center, direction.RotatedByRandom(MathHelper.ToRadians(8)) * 13, ProjectileID.SnowBallFriendly, 22, 2, Projectile.owner);
            Main.projectile[proj].DamageType = DamageClass.Default;
            Main.projectile[proj].ArmorPenetration = 8;
            timer = 20;
        }
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance && lineOfSight)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}