using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace AwfulGarbageMod.Projectiles
{
    public class EquilibriumArmorFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        bool isDashing;
        int dashCooldown;
        int shotnum = 0;
        int atkMod = 0;
        float targetDir;
        int StateTimer = 0;
        Vector2 targetArea;
        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            if (isDashing && dashCooldown > 60)
            {
                StateTimer++;
            }
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

        }

        // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                return false;
            }

            if (owner.GetModPlayer<GlobalPlayer>().EquilibriumMinionBonus)
            {
                Projectile.timeLeft = 2;
                Projectile.originalDamage = 47;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 16 * 8f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (16 * 12) * owner.direction;
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
                if (owner.direction == Math.Sign(npc.Center.X - owner.Center.X))
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);

                    // Reasonable distance away so it doesn't target across multiple screens
                    if (between < 600f)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                        target = npc;
                    }
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (owner.direction == Math.Sign(npc.Center.X - owner.Center.X))
                    {
                        if (npc.CanBeChasedBy())
                        {
                            float between = Vector2.Distance(npc.Center, Projectile.Center);
                            if (between < 600f)
                            {
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
            if (foundTarget || isDashing)
            {

                // Minion has a target: attack (here, fly towards the enemy)
                if (!isDashing)
                {
                    dashCooldown = 90;
                    isDashing = true;
                    StateTimer = 0;
                    direction = targetCenter - Projectile.Center;
                    direction = direction.SafeNormalize(Vector2.Zero);
                    direction *= 160;
                    targetArea = targetCenter + direction;
                }
                if (dashCooldown > 0)
                {
                    if (dashCooldown > 60)
                    {
                        speed = Vector2.Distance(Projectile.Center, targetArea) / 4f;
                        if (speed > 33)
                        {
                            speed = 33;
                        }
                        inertia = 12f;


                        direction = targetArea - Projectile.Center;
                        direction = direction.SafeNormalize(Vector2.Zero);
                        direction *= speed;

                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    }
                    else
                    {
                        StateTimer = 0;
                        speed = vectorToIdlePosition.Length() / 6f;
                        if (speed > 27)
                        {
                            speed = 27;
                        }
                        inertia = 12f;

                        direction = vectorToIdlePosition;
                        direction = direction.SafeNormalize(Vector2.Zero);
                        direction *= speed;

                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    }
                    dashCooldown -= 1;
                }
                else
                {
                    isDashing = false;
                }
            }
            else
            {
                isDashing = false;
                StateTimer = 0;

                speed = vectorToIdlePosition.Length() / 6f;
                if (speed > 27)
                {
                    speed = 27;
                }
                inertia = 12f;

                direction = vectorToIdlePosition;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia; dashCooldown -= 1;
            }
        }

        private void Visuals(bool foundTarget, Vector2 targetCenter, Vector2 vectorToIdlePosition)
        {
            // So it will lean slightly towards the direction it's moving
            // This is a simple "loop through all frames from top to bottom" animation
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

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 playerArmPosition = player.Center;

            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("AwfulGarbageMod/Projectiles/FireChain");
            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);

            float chainRotation = 0;//unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;

            float gravStrength = 800 / MathHelper.Clamp(Vector2.Distance(Projectile.Center, playerArmPosition), 100, 9999);

            for (int i = 1; i < 8; i++)
            {
                var chainTextureToDraw = chainTexture;
                chainDrawPosition = Vector2.Lerp(Projectile.Center, playerArmPosition, i / 8f);
                //Main.NewText(i.ToString() + new Vector2(0, 2 * 20));
                chainDrawPosition -= new Vector2(0, MathF.Pow(i - 4, 2) * gravStrength);
                chainDrawPosition += new Vector2(0, 16 * gravStrength);
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));
                // Here, we draw the chain texture at the coordinates
                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor * 0.35f, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);
            }
        
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
    }
    public class EquilibriumArmorIce : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        bool isDashing;
        int dashCooldown;
        int shotnum = 0;
        int atkMod = 0;
        float targetDir;
        int StateTimer = 0;
        Vector2 targetArea;
        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            if (isDashing && dashCooldown > 60)
            {
                StateTimer++;
            }
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

        }

        // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                return false;
            }

            if (owner.GetModPlayer<GlobalPlayer>().EquilibriumMinionBonus)
            {
                Projectile.timeLeft = 2;
                Projectile.originalDamage = 47;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 16 * 8f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (16 * 12) * owner.direction;
            idlePosition.X -= minionPositionOffsetX; // Go behind the player

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
                if (owner.direction != Math.Sign(npc.Center.X - owner.Center.X))
                {
                    float between = Vector2.Distance(npc.Center, Projectile.Center);

                    // Reasonable distance away so it doesn't target across multiple screens
                    if (between < 600f)
                    {
                        distanceFromTarget = between;
                        targetCenter = npc.Center;
                        foundTarget = true;
                        target = npc;
                    }
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (owner.direction != Math.Sign(npc.Center.X - owner.Center.X))
                    {
                        if (npc.CanBeChasedBy())
                        {
                            float between = Vector2.Distance(npc.Center, Projectile.Center);
                            if (between < 600f)
                            {
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
            if (foundTarget || isDashing)
            {

                // Minion has a target: attack (here, fly towards the enemy)
                if (!isDashing)
                {
                    dashCooldown = 90;
                    isDashing = true;
                    StateTimer = 0;
                    direction = targetCenter - Projectile.Center;
                    direction = direction.SafeNormalize(Vector2.Zero);
                    direction *= 160;
                    targetArea = targetCenter + direction;
                }
                if (dashCooldown > 0)
                {
                    if (dashCooldown > 60)
                    {
                        speed = Vector2.Distance(Projectile.Center, targetArea) / 4f;
                        if (speed > 33)
                        {
                            speed = 33;
                        }
                        inertia = 12f;


                        direction = targetArea - Projectile.Center;
                        direction = direction.SafeNormalize(Vector2.Zero);
                        direction *= speed;

                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    }
                    else
                    {
                        StateTimer = 0;
                        speed = vectorToIdlePosition.Length() / 6f;
                        if (speed > 27)
                        {
                            speed = 27;
                        }
                        inertia = 12f;

                        direction = vectorToIdlePosition;
                        direction = direction.SafeNormalize(Vector2.Zero);
                        direction *= speed;

                        Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                    }
                    dashCooldown -= 1;
                }
                else
                {
                    isDashing = false;
                }
            }
            else
            {
                isDashing = false;
                StateTimer = 0;

                speed = vectorToIdlePosition.Length() / 6f;
                if (speed > 27)
                {
                    speed = 27;
                }
                inertia = 12f;

                direction = vectorToIdlePosition;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia; dashCooldown -= 1;
            }
        }

        private void Visuals(bool foundTarget, Vector2 targetCenter, Vector2 vectorToIdlePosition)
        {
            // So it will lean slightly towards the direction it's moving
            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 8;
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

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 playerArmPosition = player.Center;

            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("AwfulGarbageMod/Projectiles/IceChain");
            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);

            float chainRotation = 0;//unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;

            float gravStrength = 800 / MathHelper.Clamp(Vector2.Distance(Projectile.Center, playerArmPosition), 100, 9999);

            for (int i = 1; i < 8; i++)
            {
                var chainTextureToDraw = chainTexture;
                chainDrawPosition = Vector2.Lerp(Projectile.Center, playerArmPosition, i / 8f);
                //Main.NewText(i.ToString() + new Vector2(0, 2 * 20));
                chainDrawPosition -= new Vector2(0, MathF.Pow(i - 4, 2) * gravStrength);
                chainDrawPosition += new Vector2(0, 16 * gravStrength);
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));
                // Here, we draw the chain texture at the coordinates
                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor * 0.35f, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);
            }
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
    }

}