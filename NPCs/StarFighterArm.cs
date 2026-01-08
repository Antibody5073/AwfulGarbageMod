using AwfulGarbageMod;
using AwfulGarbageMod.BossBars;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Consumables; 
using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StramClasses.Classes.Acolyte.Projectiles;
using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.NPCs
{
    // This ModNPC serves as an example of a completely custom AI.
    public class StarFighterArm : ModNPC
    {
        public override string Texture => "AwfulGarbageMod/Items/Weapons/Ranged/SpineString";
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            ChasePlayer,
            Spin,
            DashTowardPlayer,
            CircleAroundPlayer,
            ChargeLaser,
            Laser,
        }

        // Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
        // These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
        private enum Frame
        {
            Asleep
        }

        // These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        float AI_State;
        float AI_Timer = 120;
        string texture = "AwfulGarbageMod/NPCs/StarFighterArm1";
        float rotate = MathHelper.ToRadians(180);

        Vector2 targetArea;
        Vector2 direction;
        Vector2 storedVel;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
            //NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;

        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CelestiteShard>(), 1, 2, 3));

        }
        public override void SetDefaults()
        {
            NPC.width = 36; // The width of the npc's hitbox (in pixels)
            NPC.height = 36; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 50; // The amount of damage that this npc deals
            NPC.defense = 10; // The amount of defense that this npc has
            NPC.lifeMax = 2000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit4; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath44; // The sound the NPC will make when it dies.
            NPC.value = 0f; // How many copper coins the NPC will drop when killed.
            NPC.noTileCollide = true;
            NPC.knockBackResist = 1.5f;
            NPC.noGravity = true;
            NPC.chaseable = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.StarFighterArm")
            });
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest();
            AI_Timer = 300;
        }
        public override void OnKill()
        {
            Main.NewText("[c/33cdff:<StarFighter> OW]");
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Main.expertMode)
            {
                NPC.lifeMax = 2500;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 3000; // Increase by 5 if expert or master mode
                }
            }
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {
            // The npc starts in the asleep state, waiting for a player to enter range
            StarFighterHead boss = (StarFighterHead)Main.npc[(int)NPC.ai[0]].ModNPC;
            if (!Main.npc[(int)NPC.ai[0]].active)
            {
                NPC.active = false;
                return;
            }
            if (AI_State != boss.AI_State)
            {
                AI_Timer = 0;
                AI_State = boss.AI_State;
            }
            
            switch (AI_State)
            {
                case (float)ActionState.ChasePlayer:
                    ChasePlayer();
                    break;
                case (float)ActionState.Spin:
                    Spin();
                    break;
                case (float)ActionState.DashTowardPlayer:
                    DashTowardPlayer();
                    break;
                case (float)ActionState.CircleAroundPlayer:
                    CircleAroundPlayer();
                    break;
                case (float)ActionState.ChargeLaser:
                    Laser();
                    break;
                case (float)ActionState.Laser:
                    Laser();
                    break;
            }
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = NPC.direction * -1;
            NPC.frame = new Rectangle(0, 0, ModContent.Request<Texture2D>(texture).Width(), ModContent.Request<Texture2D>(texture).Height());
            switch (texture)
            {
                case "AwfulGarbageMod/NPCs/StarFighterArm1":
                    rotate = MathHelper.ToRadians(90);
                    break;
                case "AwfulGarbageMod/Items/Weapons/Melee/StarFighterArm2":
                    rotate = MathHelper.ToRadians(90);
                    break;
                case "AwfulGarbageMod/Items/Weapons/Ranged/StarFighterArm3":
                    rotate = MathHelper.ToRadians(90);
                    break;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * NPC.Opacity;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;

            Vector2 drawOrigin = NPC.frame.Size() / 2;
            Vector2 drawPos = NPC.position - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY); //.RotatedBy(NPC.rotation);
            Color color = NPC.GetAlpha(drawColor);
            spriteBatch.Draw(ModContent.Request<Texture2D>(texture).Value, drawPos, NPC.frame, color, NPC.rotation + rotate, drawOrigin, NPC.scale, spriteEffects, 0f);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        private void ChasePlayer()
        {
            texture = "AwfulGarbageMod/NPCs/StarFighterArm1";

            NPC boss = Main.npc[(int)NPC.ai[0]];
            float speed = 8f;
            float inertia = 8f;
            Vector2 direction;
            Vector2 idlePosition = boss.Center;
            Vector2 pointTowardPos = boss.Center + boss.velocity * 150;

            if (NPC.ai[1] == 1)
            {
                idlePosition += boss.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.ToRadians(60)) * 75;

                NPC.rotation = (pointTowardPos - NPC.Center).ToRotation();


            }
            else
            {
                idlePosition += boss.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.ToRadians(-60)) * 75;

                NPC.rotation = (pointTowardPos - NPC.Center).ToRotation();
            }
            Vector2 vectorToIdlePosition = idlePosition - NPC.Center;

            speed = vectorToIdlePosition.Length() / 6f;
            if (speed > 36)
            {
                speed = 36;
            }
            inertia = 4f;

            direction = vectorToIdlePosition;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer++;
            if (AI_Timer % 24 == 0)
            {
                int damage = 17;
                if (Main.expertMode)
                {
                    damage = 16;
                    if (Main.masterMode)
                    {
                        damage = 15;
                    }
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (pointTowardPos - NPC.Center).SafeNormalize(Vector2.Zero) * 1f, ModContent.ProjectileType<StarFighterProjBlast2>(), damage, 0, Main.myPlayer, 0.05f);
                NPC.velocity = Vector2.Zero;


                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/SmallShot3")
                {
                    MaxInstances = 2,
                    Volume = 0.55f
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
            }
        }
        private void Spin()
        {
            NPC boss = Main.npc[(int)NPC.ai[0]];
            float speed = 8f;
            float inertia = 8f;
            Vector2 direction;
            Vector2 idlePosition = boss.Center;

            if (NPC.ai[1] == 1)
            {
                idlePosition += boss.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.ToRadians(60)) * 75;

                NPC.rotation += MathHelper.ToRadians(5);


            }
            else
            {
                idlePosition += boss.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.ToRadians(-60)) * 75;

                NPC.rotation -= MathHelper.ToRadians(5);
            }
            Vector2 vectorToIdlePosition = idlePosition - NPC.Center;

            speed = vectorToIdlePosition.Length() / 6f;
            if (speed > 36)
            {
                speed = 36;
            }
            inertia = 4f;

            direction = vectorToIdlePosition;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer++;
            if (AI_Timer % 4 == 0)
            {
                int damage = 17;
                if (Main.expertMode)
                {
                    damage = 16;
                    if (Main.masterMode)
                    {
                        damage = 15;
                    }
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(2, 0).RotatedBy(NPC.rotation), ModContent.ProjectileType<StarFighterProjBlast2>(), damage, 0, Main.myPlayer, 0.02f);
                NPC.velocity = Vector2.Zero;
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/SmallShot2")
                {
                    MaxInstances = 4,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
            }
        }
        private void DashTowardPlayer()
        {
            texture = "AwfulGarbageMod/NPCs/StarFighterArm3";

            NPC boss = Main.npc[(int)NPC.ai[0]];
            float speed = 8f;
            float inertia = 8f;
            Vector2 direction;
            Vector2 idlePosition = boss.Center;

            if (NPC.ai[1] == 1)
            {
                idlePosition += boss.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.ToRadians(90)) * 250;

                NPC.rotation = boss.velocity.ToRotation();


            }
            else
            {
                idlePosition += boss.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.ToRadians(-90)) * 250;

                NPC.rotation = boss.velocity.ToRotation();
            }
            Vector2 vectorToIdlePosition = idlePosition - NPC.Center;

            speed = vectorToIdlePosition.Length() / 6f;
            if (speed > 40)
            {
                speed = 40;
            }
            inertia = 3f;

            direction = vectorToIdlePosition;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer++;
            if (AI_Timer % 150 < 60 && AI_Timer % 10 == 1 && AI_Timer > 150)
            {
                int damage = 20;
                if (Main.expertMode)
                {
                    damage = 19;
                    if (Main.masterMode)
                    {
                        damage = 18;
                    }
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, boss.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2 * NPC.ai[1]) * 10, ModContent.ProjectileType<StarFighterProjStar>(), damage, 0, Main.myPlayer, boss.target, AI_Timer % 150);

                SoundEngine.PlaySound(SoundID.Item60, NPC.Center);
            }
        }
        private void CircleAroundPlayer()
        {
            texture = "AwfulGarbageMod/NPCs/StarFighterArm3";

            NPC boss = Main.npc[(int)NPC.ai[0]];
            float speed = 8f;
            float inertia = 8f;
            Vector2 direction;
            Vector2 idlePosition = boss.Center;

            if (NPC.ai[1] == 1)
            {

                if (AI_Timer % 90 == 0)
                {
                    Vector2 dir = NPC.Center - Main.player[boss.target].Center;
                    targetArea = Main.player[boss.target].Center + dir.RotatedByRandom(MathHelper.PiOver4 * 2.25f).SafeNormalize(Vector2.One) * 350;
                }
                if (AI_Timer % 90 == 45)
                {
                    Vector2 dir = Main.player[boss.target].Center - NPC.Center;
                    targetArea = Main.player[boss.target].Center + dir.SafeNormalize(Vector2.One) * 350 + Main.player[boss.target].velocity * 10;
                    SoundEngine.PlaySound(SoundID.Item60, NPC.Center);
                    NPC.velocity *= 0;

                }
            }
            else
            {
                if (AI_Timer % 90 == 45)
                {
                    Vector2 dir = NPC.Center - Main.player[boss.target].Center;
                    targetArea = Main.player[boss.target].Center + dir.RotatedByRandom(MathHelper.PiOver4 * 2.25f).SafeNormalize(Vector2.One) * 350;
                }
                if (AI_Timer % 90 == 0)
                {
                    if (AI_Timer == 0)
                    {
                        targetArea = NPC.Center;
                    }
                    else
                    {
                        Vector2 dir = Main.player[boss.target].Center - NPC.Center;
                        targetArea = Main.player[boss.target].Center + dir.SafeNormalize(Vector2.One) * 350 + Main.player[boss.target].velocity * 10;
                        SoundEngine.PlaySound(SoundID.Item60, NPC.Center);
                        NPC.velocity *= 0;

                    }
                }
            }
            Vector2 vectorToIdlePosition = targetArea - NPC.Center;

            speed = vectorToIdlePosition.Length() / 6f;
            if (speed > 36)
            {
                speed = 36;
            }
            inertia = 20f;

            direction = vectorToIdlePosition;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            NPC.rotation = NPC.velocity.ToRotation();

            AI_Timer++;
        }
        private void Laser()
        {
            texture = "AwfulGarbageMod/NPCs/StarFighterArm2";

            NPC boss = Main.npc[(int)NPC.ai[0]];
            float speed = 8f;
            float inertia = 8f;
            Vector2 direction;
            Vector2 idlePosition = boss.Center;
            Vector2 pointTowardPos = boss.Center + boss.velocity.SafeNormalize(Vector2.One) * 45;

            if (NPC.ai[1] == 1)
            {
                idlePosition += boss.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.ToRadians(60)) * 75;

                NPC.rotation = (pointTowardPos - NPC.Center).ToRotation();


            }
            else
            {
                idlePosition += boss.velocity.SafeNormalize(Vector2.One).RotatedBy(MathHelper.ToRadians(-60)) * 75;

                NPC.rotation = (pointTowardPos - NPC.Center).ToRotation();
            }
            Vector2 vectorToIdlePosition = idlePosition - NPC.Center;

            speed = vectorToIdlePosition.Length() / 6f;
            if (speed > 36)
            {
                speed = 36;
            }
            inertia = 4f;

            direction = vectorToIdlePosition;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer++;
            Dust dust = Dust.NewDustDirect(NPC.Center, 0, 0, DustID.Vortex, 0, 0, 0, default(Color), 1f);
            dust.scale = 2f;
            dust.velocity = (pointTowardPos - NPC.Center).SafeNormalize(Vector2.One) * 5 + NPC.velocity * 2;
            dust.noGravity = true;
        }
    }
}