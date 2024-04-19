using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using Steamworks;
using StramClasses.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Melee
{

    public abstract class BaseFlailProjectile : ModProjectile
    {

        public virtual string ChainTexturePath()
        {
            return "";
        }

        public virtual float MaxSpinDistance => 100;
        public virtual float SpinSpd => 1;
        public virtual float SpinDistanceIncrease => 1f;
        public virtual bool MeleeSpdToRange => true;
        public virtual float MeleeSpdEffectiveness => 0.5f;
        public virtual bool canHitThroughWalls => false;
        public virtual int spinHitCooldown => 40;
        public virtual float RetractSpd => 0.3f;
        public virtual float MaxRetractSpd => 10f;
        public virtual float flailHeadRotation => 0f;
        public virtual float flailHeadRotationRetract => 0f;



        private enum AIState
        {
            Spinning,
            Retracting,
            ForcedRetracting,
        }

        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
        private AIState CurrentAIState
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public ref float StateTimer => ref Projectile.ai[1];

        public ref float CollisionCounter => ref Projectile.localAI[0];
        public ref float SpinningStateTimer => ref Projectile.localAI[1];

        public float spinDistance;
        public bool atMaxRange;

        bool spawnEffectDone = false;

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // This ensures that the projectile is synced when other players join the world.
            Projectile.width = 24; // The width of your projectile
            Projectile.height = 24; // The height of your projectile
            Projectile.friendly = true; // Deals damage to enemies
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.DamageType = ModContent.GetInstance<FlailDamageClass>(); // Deals melee damage
            Projectile.usesLocalNPCImmunity = true; // Used for hit cooldown changes in the ai hook
            Projectile.localNPCHitCooldown = 10; // This facilitates custom hit cooldown logic
            Projectile.extraUpdates = 3;
            // Vanilla flails all use aiStyle 15, but the code isn't customizable so an adaption of that aiStyle is used in the AI method
        }

        // This AI code was adapted from vanilla code: Terraria.Projectile.AI_015_Flails() 

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (player.GetModPlayer<GlobalPlayer>().CobaltMelee && atMaxRange)
            {
                player.GetModPlayer<GlobalPlayer>().CobaltMeleeCooldown = 60 * 10;
                player.GetModPlayer<GlobalPlayer>().CobaltMeleeDefense += 1;
                if (player.GetModPlayer<GlobalPlayer>().CobaltMeleeDefense > 24)
                {
                    player.GetModPlayer<GlobalPlayer>().CobaltMeleeDefense = 24;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            // Kill the projectile if the player dies or gets crowd controlled
            if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 1200f)
            {
                Projectile.Kill();
                return;
            }
            if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
            {
                Projectile.Kill();
                return;
            }
            if (!spawnEffectDone)
            {
                spawnEffectDone = true;
                if (player.GetModPlayer<GlobalPlayer>().DoubleVisionBand && !Projectile.flailProjectile().isAClone)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Projectile.velocity, Projectile.type, (int)(Projectile.damage * 0.6f), Projectile.knockBack * 0.4f, player.whoAmI);
                    Main.projectile[proj].flailProjectile().isAClone = true;
                    Main.projectile[proj].flailProjectile().spinOffset = Projectile.flailProjectile().spinOffset;
                    Main.projectile[proj].flailProjectile().spinSpdMultiplier = -1;
                    Main.projectile[proj].flailProjectile().rangeMultiplier = 0.66f;
                    Main.projectile[proj].flailProjectile().distanceIncreaseMultiplier = 0.66f;
                }
            }
            Vector2 mountedCenter = player.MountedCenter;
            float retractAcceleration = RetractSpd; // How quickly the projectile will accelerate back towards the player while retracting
            float maxRetractSpeed = MaxRetractSpd; // The max speed the projectile will have while retracting
            float forcedRetractAcceleration = 1f; // How quickly the projectile will accelerate back towards the player while being forced to retract
            float maxForcedRetractSpeed = 20f; // The max speed the projectile will have while being forced to retract
            // Scaling these speeds and accelerations by the players melee speed makes the weapon more responsive if the player boosts it or general weapon speed
            float meleeSpeedMultiplier = player.GetTotalAttackSpeed(DamageClass.Melee);
            retractAcceleration *= meleeSpeedMultiplier;
            maxRetractSpeed *= meleeSpeedMultiplier;
            forcedRetractAcceleration *= meleeSpeedMultiplier;
            maxForcedRetractSpeed *= meleeSpeedMultiplier;
            Projectile.localNPCHitCooldown = (int)(spinHitCooldown / player.GetModPlayer<GlobalPlayer>().flailSpinSpd);
            Projectile.tileCollide = false;
            float maxSpinRange = MaxSpinDistance * Projectile.flailProjectile().rangeMultiplier * player.HeldItem.scale * player.GetModPlayer<GlobalPlayer>().flailRange;
            if (MeleeSpdToRange)
            {
                maxSpinRange *= ((meleeSpeedMultiplier - 1) * MeleeSpdEffectiveness + 1);
            }
           

            switch (CurrentAIState)
            {
                case AIState.Spinning:
                    {
                        StateTimer++;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
                            player.ChangeDir((unitVectorTowardsMouse.X > 0f).ToDirectionInt());
                            if (!player.channel) // If the player releases then change to moving forward mode
                            {
                                CurrentAIState = AIState.Retracting;
                                StateTimer = 0f;
                                Projectile.velocity = Vector2.Zero;
                                Projectile.netUpdate = true;
                                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                                break;
                            }
                        }
                        SpinningStateTimer += 1f;
                        // This line creates a unit vector that is constantly rotated around the player. 10f controls how fast the projectile visually spins around the player
                        float tempSpinSpd = SpinSpd * Projectile.flailProjectile().spinSpdMultiplier * 45 / player.HeldItem.useTime * player.GetModPlayer<GlobalPlayer>().flailSpinSpd;
                        if (!MeleeSpdToRange)
                        {
                            tempSpinSpd *= ((meleeSpeedMultiplier - 1) * MeleeSpdEffectiveness + 1);
                        }
                        DoFlailPositioning(player, tempSpinSpd, SpinningStateTimer / 60, out Vector2 offsetFromPlayer);
                        Projectile.Center = mountedCenter + offsetFromPlayer * spinDistance + new Vector2(0, player.gfxOffY);

                        SpinEffect(player, SpinningStateTimer, spinDistance, maxSpinRange);
                        AccessorySpinEffect(player, SpinningStateTimer, spinDistance, maxSpinRange);

                        Projectile.velocity = Vector2.Zero;
                        SetRotation(offsetFromPlayer, flailHeadRotation, player);
                        SetDistance();
                        atMaxRange = false;
                        if (spinDistance >= maxSpinRange)
                        {
                            atMaxRange = true;
                            spinDistance = maxSpinRange;
                        }
                        break;
                    }
                case AIState.Retracting:
                    {
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                        if (Projectile.Distance(mountedCenter) <= maxRetractSpeed)
                        {
                            Projectile.Kill(); // Kill the projectile once it is close enough to the player
                            return;
                        }
                        else
                        {
                            Projectile.velocity *= 0.98f;
                            Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
                            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(-90 + flailHeadRotationRetract);
                            player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        }
                        break;
                    }
                case AIState.ForcedRetracting:
                    {
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                        if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed)
                        {
                            Projectile.Kill(); // Kill the projectile once it is close enough to the player
                            return;
                        }
                        Projectile.velocity *= 0.98f;
                        Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
                        Vector2 target = Projectile.Center + Projectile.velocity;
                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(-90);

                        Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
                        if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f)
                        {
                            Projectile.Kill(); // Kill projectile if it will pass the player
                            return;
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
                        break;
                    }

            }

            // This is where Flower Pow launches projectiles. Decompile Terraria to view that code.

            Projectile.direction = (Projectile.velocity.X > 0f).ToDirectionInt();
            Projectile.spriteDirection = Projectile.direction;
            Projectile.ownerHitCheck = !canHitThroughWalls; // This prevents attempting to damage enemies without line of sight to the player. The custom Colliding code for spinning makes this necessary.

            // This rotation code is unique to this flail, since the sprite isn't rotationally symmetric and has tip.


            // If you have a ball shaped flail, you can use this simplified rotation code instead
            /*
			if (Projectile.velocity.Length() > 1f)
				Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
			else
				Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
			*/

            Projectile.timeLeft = 2; // Makes sure the flail doesn't die (good when the flail is resting on the ground)
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2); //Add a delay so the player can't button mash the flail
            player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
            if (Projectile.Center.X < mountedCenter.X)
            {
                player.itemRotation += (float)Math.PI;
            }
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            // Spawning dust. We spawn dust more often when in the LaunchingForward state
            Dusts();
        }

        public virtual void SetDistance()
        {
            spinDistance += SpinDistanceIncrease * Projectile.flailProjectile().distanceIncreaseMultiplier;
        }
        public virtual void SetRotation(Vector2 offsetFromPlayer, float rotation, Player player)
        {
            Projectile.rotation = offsetFromPlayer.ToRotation() + MathHelper.ToRadians(-90) + MathHelper.ToRadians(rotation) * player.direction * (Projectile.flailProjectile().spinSpdMultiplier >= 0 ? 1 : -1);
        }

        public virtual void DoFlailPositioning(Player player, float spd, float currentDir, out Vector2 offsetFromPlayer)
        {
            offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * (spd) * (currentDir) * player.direction + MathHelper.ToRadians(Projectile.flailProjectile().spinOffset));
        }

        public virtual void SpinEffect(Player player, float timer, float length, float maxLength)
        {

        }

        private void AccessorySpinEffect(Player player, float timer, float length, float maxLength)
        {
            if (player.GetModPlayer<GlobalPlayer>().jungleSporeFlail > 0)
            {
                if (length == maxLength && timer % 64 == 0)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileID.SporeCloud, player.GetModPlayer<GlobalPlayer>().jungleSporeFlail, 0, player.whoAmI);
                }
            }
        }

        public virtual void Dusts()
        {

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (canHitThroughWalls || Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height))
            {
                return base.Colliding(projHitbox, targetHitbox);
            }

            return false;
        }
        public override bool? CanDamage()
        {
            // Flails in spin mode won't damage enemies within the first 12 ticks. Visually this delays the first hit until the player swings the flail around for a full spin before damaging anything.
            if (SpinningStateTimer <= 60f)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Flails do a few custom things, you'll want to keep these to have the same feel as vanilla flails.

            // The hitDirection is always set to hit away from the player, even if the flail damages the npc while returning
            modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();
        }

        // PreDraw is used to draw a chain and trail before the projectile is drawn normally.
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

            // This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.
            playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath());

            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
            }
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (chainLengthRemainingToDraw > 0f)
            {
                // This code gets the lighting at the current tile coordinates
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

                var chainTextureToDraw = chainTexture;
                if (chainCount >= 4)
                {
                    // Use normal chainTexture and lighting, no changes
                }

                // Here, we draw the chain texture at the coordinates
                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;


            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, projectileTexture.Width, frameHeight);
            Color drawColor = Projectile.GetAlpha(lightColor);

            Vector2 drawOrigin = sourceRectangle.Size() / 2f;
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            // Add a motion trail when moving forward, like most flails do (don't add trail if already hit a tile)
            if (CurrentAIState == AIState.Spinning)
            {
                

                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width/2, Projectile.height/2);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
                }
            }
            Main.EntitySpriteDraw(projectileTexture,
               Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
               sourceRectangle, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            return false;
        }
    }

}