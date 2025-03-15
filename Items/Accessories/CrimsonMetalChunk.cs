using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Accessories
{
    
    public class CrimsonMetalChunk : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Necropotence"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("34 damage\n5 defense\nTaking damage causes you to release bone toothpicks\n\"Argh, fine! I'll hit you with this and turn you into a couple of cremated reliquaries!\"");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 40000;
            Item.rare = 5;
			Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.GetModPlayer<GlobalPlayer>().CrimsonAxe == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<CrimsonMetalChunkProj>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<CrimsonMetalChunkProj>(), 0, 0, Main.myPlayer);
            }

            player.GetModPlayer<GlobalPlayer>().CrimsonAxe += 50;
        }
	}
    public class CrimsonMetalChunkProj : ModProjectile
    {
        Vector2 dashVel;

        bool isDashing;
        int dashCooldown;
        int shotnum = 0;
        int atkMod = 0;
        float targetDir;
        int StateTimer = 0;

        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 1;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            if (isDashing)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            StateTimer++;
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
            {
                return;
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, out NPC target);
            Movement(foundTarget, distanceFromTarget, targetCenter, target, distanceToIdlePosition, vectorToIdlePosition);
            Visuals(foundTarget, targetCenter, vectorToIdlePosition);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 180);
        }

        // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                return false;
            }

            if (owner.GetModPlayer<GlobalPlayer>().CrimsonAxe > 0)
            {
                Projectile.timeLeft = 2;
                Projectile.originalDamage = owner.GetModPlayer<GlobalPlayer>().CrimsonAxe;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 16 * 6f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (16 * 8) * owner.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter, out NPC target)
        {
            // Starting search distance
            distanceFromTarget = 600f;
            targetCenter = Projectile.position;
            foundTarget = false;
            target = null;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 1200f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                    target = npc;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if (((closest && inRange) || !foundTarget))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                            target = npc;
                        }
                    }
                }
            }

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            Projectile.friendly = foundTarget;
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, NPC target, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            // Default movement parameters (here for attacking)
            float speed = 8f;
            float inertia = 8f;
            Vector2 direction;
            if (foundTarget)
            {

                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget > 420f && !isDashing)
                {
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    direction = targetCenter - Projectile.Center;
                    Projectile.velocity = direction / 6 - direction.SafeNormalize(Vector2.Zero) * 4;
                    dashCooldown -= 1;
                }
                else if (!isDashing)
                {
                    dashCooldown = 42;
                    isDashing = true;
                    direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= 18;
                    Projectile.velocity = direction;
                    float origTargetDir = targetDir;
                    targetDir = direction.ToRotation();
                }
                if (dashCooldown > 0)
                {

                    Projectile.velocity *= 0.95f;
                    dashCooldown -= 1;
                }
                else
                {
                    isDashing = false;
                }
            }
            else
            {
                Projectile.velocity = vectorToIdlePosition / 3;
                dashCooldown -= 1;
                if (Main.player[Projectile.owner].direction == 1)
                {
                    targetDir = MathHelper.ToRadians(-45);
                }
                else
                {
                    targetDir = MathHelper.ToRadians(-135);
                }
            }
        }
       
        private void Visuals(bool foundTarget, Vector2 targetCenter, Vector2 vectorToIdlePosition)
        {
            Projectile.rotation += AGUtils.TurnTowards(180, Vector2.One.RotatedBy(targetDir), Projectile.rotation, Vector2.Zero, 8);

            // So it will lean slightly towards the direction it's moving
            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;
            if (Projectile.frame > 0)
            {
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
            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
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
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width / 2, Projectile.height/2);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2.5f);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }
            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }
}