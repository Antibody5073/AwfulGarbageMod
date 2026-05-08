using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.NPCs.BossUnrealRework.KingSlime;
using AwfulGarbageMod.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Drawing.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.Melee.Hardmode
{

    public class CursedFlametongue : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cloud Cleaver"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Launches a shockwave that speeds up on enemy hits");
        }

        public int counter;

        public override void SetDefaults()
        {
            Item.damage = 666;
            Item.mana = 20;
            Item.DamageType = ModContent.GetInstance<MageMeleeClass>();
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 24 * 3;
            Item.useAnimation = 24 * 3;
            Item.useStyle = 1;
            Item.knockBack = 5;
            Item.value = 15000;
            Item.rare = 4;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CursedFlametongueProj>();
            Item.shootSpeed = 14f;
            Item.crit = 0;
            Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, counter);
            counter = (counter + 1) % 2; // Increment attackType to make sure next swing is different

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CursedFlames)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.Bladetongue)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
        }
    }

    public class CursedFlametongueProj : ModProjectile
    {
        // We define some constants that determine the swing range of the sword
        // Not that we use multipliers here since that simplifies the amount of tweaks for these interactions
        // You could change the values or even replace them entirely, but they are tweaked with looks in mind
        private const float SWINGRANGE = 1.67f * (float)Math.PI; // The angle a swing attack covers (300 deg)
        private const float FIRSTHALFSWING = 0.45f; // How much of the swing happens before it reaches the target angle (in relation to swingRange)
        private const float SPINRANGE = 3.5f * (float)Math.PI; // The angle a spin attack covers (630 degrees)
        private const float WINDUP = 0.15f; // How far back the player's hand goes when winding their attack (in relation to swingRange)
        private const float UNWIND = 0.4f; // When should the sword start disappearing
        private const float SPINTIME = 2.5f; // How much longer a spin is than a swing

        private enum AttackType // Which attack is being performed
        {
            // Swings are normal sword swings that can be slightly aimed
            // Swings goes through the full cycle of animations
            Swing,
            // Spins are swings that go full circle
            // They are slower and deal more knockback
            ReverseSwing,
        }

        private enum AttackStage // What stage of the attack is being executed, see functions found in AI for description
        {
            Prepare,
            Execute,
            Unwind
        }

        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
        private AttackType CurrentAttack
        {
            get => (AttackType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private AttackStage CurrentStage
        {
            get => (AttackStage)Projectile.localAI[0];
            set
            {
                Projectile.localAI[0] = (float)value;
                Timer = 0; // reset the timer when the projectile switches states
            }
        }

        // Variables to keep track of during runtime
        private ref float InitialAngle => ref Projectile.ai[1]; // Angle aimed in (with constraints)
        private ref float Timer => ref Projectile.ai[2]; // Timer to keep track of progression of each stage
        private ref float Progress => ref Projectile.localAI[1]; // Position of sword relative to initial angle
        private ref float Size => ref Projectile.localAI[2]; // Size of sword

        // We define timing functions for each stage, taking into account melee attack speed
        // Note that you can change this to suit the need of your projectile
        private float prepTime => 24f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float execTime => 24f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
        private float hideTime => 24f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
    
        private Player Owner => Main.player[Projectile.owner];
        bool reset = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 96; // Hitbox width of projectile
            Projectile.height = 96; // Hitbox height of projectile
            Projectile.friendly = true; // Projectile hits enemies
            Projectile.timeLeft = 10000; // Time it takes for projectile to expire
            Projectile.penetrate = -1; // Projectile pierces infinitely
            Projectile.tileCollide = false; // Projectile does not collide with tiles
            Projectile.usesLocalNPCImmunity = true; // Uses local immunity frames
            Projectile.localNPCHitCooldown = -1; // We set this to -1 to make sure the projectile doesn't hit twice
            Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
            Projectile.DamageType = ModContent.GetInstance<MageMeleeClass>();
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

            if (CurrentAttack == AttackType.ReverseSwing)
            {

                if (Projectile.spriteDirection == 1)
                {
                    // However, we limit the rangle of possible directions so it does not look too ridiculous
                    targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
                }
                else
                {
                    if (targetAngle < 0)
                    {
                        targetAngle += 2 * (float)Math.PI; // This makes the range continuous for easier operations
                    }

                    targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
                }

                InitialAngle = targetAngle + FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // Otherwise, we calculate the angle
            }
            else
            {
                if (Projectile.spriteDirection == 1)
                {
                    // However, we limit the rangle of possible directions so it does not look too ridiculous
                    targetAngle = MathHelper.Clamp(targetAngle, (float)-Math.PI * 1 / 3, (float)Math.PI * 1 / 6);
                }
                else
                {
                    if (targetAngle < 0)
                    {
                        targetAngle += 2 * (float)Math.PI; // This makes the range continuous for easier operations
                    }

                    targetAngle = MathHelper.Clamp(targetAngle, (float)Math.PI * 5 / 6, (float)Math.PI * 4 / 3);
                }

                InitialAngle = targetAngle - FIRSTHALFSWING * SWINGRANGE * Projectile.spriteDirection; // Otherwise, we calculate the angle
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            // Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually. 
            writer.Write((sbyte)Projectile.spriteDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.spriteDirection = reader.ReadSByte();
        }

        public override void AI()
        {
            // Extend use animation until projectile is killed
            Owner.itemAnimation = 2;
            Owner.itemTime = 2;

            // Kill the projectile if the player dies or gets crowd controlled
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
            {
                Projectile.Kill();
                return;
            }

            // AI depends on stage and attack
            // Note that these stages are to facilitate the scaling effect at the beginning and end
            // If this is not desirable for you, feel free to simplify
            switch (CurrentStage)
            {
                case AttackStage.Prepare:
                    PrepareStrike();
                    break;
                case AttackStage.Execute:
                    ExecuteStrike();
                    break;
                default:
                    UnwindStrike();
                    break;
            }

            SetSwordPosition();

            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(16, 0).RotatedBy(Projectile.rotation), new Vector2(5f + 7.5f * Size, 0).RotatedBy(Projectile.rotation), ModContent.ProjectileType<CursedFlametongueFlame>(), Projectile.damage, 0, Projectile.owner, reset ? 0 : 1);
            if (CurrentStage != AttackStage.Prepare) {
                proj.friendly = true;
            }
            
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.spriteDirection > 0)
            {
                origin = new Vector2(0, Projectile.height);
                rotationOffset = MathHelper.ToRadians(45f);
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Projectile.width, Projectile.height);
                rotationOffset = MathHelper.ToRadians(135f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

            // Since we are doing a custom draw, prevent it from normally drawing
            return false;
        }

        // Find the start and end of the sword and use a line collider to check for collision with enemies
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 40f * Projectile.scale, ref collisionPoint);
        }

        // Do a similar collision check for tiles
        public override void CutTiles()
        {
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
            Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
        }

        // We make it so that the projectile can only do damage in its release and unwind phases
        public override bool? CanDamage()
        {
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Make knockback go away from player
            modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;

        }

        // Function to easily set projectile and arm position
        public void SetSwordPosition()
        {
            Projectile.rotation = InitialAngle + Projectile.spriteDirection * (CurrentAttack == AttackType.Swing ? Progress : -Progress); // Set projectile rotation

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            //Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)

            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Projectile.Center = armPosition; // Set projectile to arm position
            Projectile.scale = 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers
            Projectile.position.X -= Owner.direction * 8;
            Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
            if (Owner.direction > 0)
            {
                if (Projectile.rotation > MathHelper.ToRadians(80))
                {
                    Owner.bodyFrame.Y = 4 * Owner.bodyFrame.Height;
                }
                else if (Projectile.rotation > MathHelper.ToRadians(20))
                {
                    Owner.bodyFrame.Y = 3 * Owner.bodyFrame.Height;
                }
                else if (Projectile.rotation > MathHelper.ToRadians(-40))
                {
                    Owner.bodyFrame.Y = 2 * Owner.bodyFrame.Height;
                }
                else
                {
                    Owner.bodyFrame.Y = 1 * Owner.bodyFrame.Height;
                }
            }
            else
            {
                float rot = MathHelper.Pi - Projectile.rotation;
                if (rot > MathHelper.ToRadians(80))
                {
                    Owner.bodyFrame.Y = 4 * Owner.bodyFrame.Height;
                }
                else if (rot > MathHelper.ToRadians(20))
                {
                    Owner.bodyFrame.Y = 3 * Owner.bodyFrame.Height;
                }
                else if (rot > MathHelper.ToRadians(-40))
                {
                    Owner.bodyFrame.Y = 2 * Owner.bodyFrame.Height;
                }
                else
                {
                    Owner.bodyFrame.Y = 1 * Owner.bodyFrame.Height;
                }
            }
        }

        // Function facilitating the taking out of the sword
        private void PrepareStrike()
        {
            Progress = WINDUP * SWINGRANGE * (1f - Timer / prepTime); // Calculates rotation from initial angle
            Size = MathHelper.SmoothStep(0, 1, Timer / prepTime); // Make sword slowly increase in size as we prepare to strike until it reaches max

            if (Timer >= prepTime)
            {
                SoundEngine.PlaySound(SoundID.Item1); // Play sword sound here since playing it on spawn is too early
                CurrentStage = AttackStage.Execute; // If attack is over prep time, we go to next stage
            }
        }

        // Function facilitating the first half of the swing
        private void ExecuteStrike()
        {
            Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) * Timer / execTime);

            if (Timer >= execTime)
            {
                CurrentStage = AttackStage.Unwind;
            }
        }

        // Function facilitating the latter half of the swing where the sword disappears
        private void UnwindStrike()
        {
            Progress = MathHelper.SmoothStep(0, SWINGRANGE, (1f - UNWIND) + UNWIND * Timer / hideTime);
            Size = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime); // Make sword slowly decrease in size as we end the swing to make a smooth hiding animation

            if (Timer >= hideTime)
            {
                Projectile.Kill();
            }
        }
    }
    public class CursedFlametongueFlame : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Ancient Sand Stream");
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        float colorValue = 1;
        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.ignoreWater = false;
            Projectile.DamageType = ModContent.GetInstance<MageMeleeClass>();
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.extraUpdates = 3;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 24;
            Projectile.scale = 0.1f;
            Projectile.light = 0.8f;
        }

        public override bool? CanDamage()
        {
            if (Projectile.alpha < 100 && Projectile.ai[1] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // SpriteEffects helps to flip texture horizontally and vertically
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

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
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(Projectile.width, Projectile.height) / 2;
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2f);
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }

            Main.EntitySpriteDraw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.rotation = Main.rand.NextFloat(0, 6.2f); //speen start
            Projectile.frame = 2;
        }
        int dustoffset;
        int StateTimer = 0;
        public override void AI()
        {
            StateTimer++;
            Projectile.rotation += 0.05f * -Projectile.direction;
            if (dustoffset > 5)
            {
                if (Main.rand.Next(20) == 0) //dust spawn sqaure increases with hurtbox size
                {
                    int dust = Dust.NewDust(new Vector2(Projectile.position.X - (dustoffset / 2), Projectile.position.Y - (dustoffset / 2)), Projectile.width + dustoffset, Projectile.height + dustoffset, DustID.CursedTorch, Projectile.velocity.X * 1f, -5, 130, default, 0.8f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].fadeIn = 0.8f + (float)Main.rand.Next(5) * 0.1f;

                    //int dust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 55, Projectile.velocity.X, Projectile.velocity.Y, 130, default, 0.5f);
                }
            }
            if (Projectile.scale <= 1f)//increase size until specified amount
            {
                dustoffset += 1;//makes dust expand with projectile, also used for hitbox

                Projectile.scale += 0.016f;
            }
            else
            {
                Projectile.alpha += 2;
                Projectile.velocity *= 0.96f;
                //begin animation
                if (Projectile.frame < 4) //stop at frame 3
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= 30)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                }
            }
            if (Projectile.alpha > 200)//once faded enough or touches water kill projectile
            {
                Projectile.Kill();
            }
            Projectile.velocity *= 0.97f;
            colorValue *= 0.96f;
            Projectile.position += Main.player[Projectile.owner].velocity / 4;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox) //expands the hurt box, but hitbox size remains the same
        {
            if (Projectile.ai[0] == 0) // only on proj deals damage
            {
                hitbox.Width = dustoffset;
                hitbox.Height = dustoffset;
                hitbox.X -= dustoffset / 2 - (Projectile.width / 2);
                hitbox.Y -= dustoffset / 2 - (Projectile.height / 2);
            }
            base.ModifyDamageHitbox(ref hitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (Projectile.damage * 9) / 10;

            target.AddBuff(BuffID.CursedInferno, 420);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.PvP)
            {
                target.AddBuff(BuffID.CursedInferno, 180);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0;
            //Projectile.Kill();
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            Color color = Color.Lerp(Color.LimeGreen, Color.Yellow, colorValue);
            color.A = (Byte)Projectile.alpha;
            return color;
        }
    }
}