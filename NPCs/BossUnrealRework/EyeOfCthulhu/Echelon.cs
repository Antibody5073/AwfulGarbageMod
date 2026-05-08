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

namespace AwfulGarbageMod.NPCs.BossUnrealRework.EyeOfCthulhu
{
    // This ModNPC serves as an example of a completely custom AI.
    public class Echelon : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            None,
            VerticalLaser,
            FormationLaser,
            Dash,
            TeardropRain,
            PhaseTransition,
            Flamethrower,
            LaserWall,
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
        float rotate = MathHelper.ToRadians(180);

        int frameCounter = 0;
        int frame = 0;

        Vector2 targetArea;
        Vector2 direction;
        Vector2 storedVel;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
            //NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;

        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CelestiteShard>(), 1, 2, 3));

        }
        public override void SetDefaults()
        {
            NPC.width = 30; // The width of the npc's hitbox (in pixels)
            NPC.height = 30; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 35; // The amount of damage that this npc deals
            NPC.defense = 16; // The amount of defense that this npc has
            NPC.lifeMax = 2000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit19; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath12; // The sound the NPC will make when it dies.
            NPC.value = 0f; // How many copper coins the NPC will drop when killed.
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.chaseable = true;
            NPC.takenDamageMultiplier = 0.8f;
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
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.Echelon")
            });
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest();
            AI_Timer = 300;
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
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {
            // The npc starts in the asleep state, waiting for a player to enter range
            EyeOfCthulhu boss = Main.npc[(int)NPC.ai[0]].GetGlobalNPC<EyeOfCthulhu>();
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
                case (float)ActionState.VerticalLaser:
                    VerticalLaser();
                    break;
                case (float)ActionState.FormationLaser:
                    FormationLaser();
                    break;
                case (float)ActionState.Dash:
                    Dash();
                    break;
                case (float)ActionState.TeardropRain:
                    TeardropRain();
                    break;
                case (float)ActionState.PhaseTransition:
                    PhaseTransition();
                    break;
                case (float)ActionState.Flamethrower:
                    Flamethrower();
                    break;
                case (float)ActionState.LaserWall:
                    LaserWall();
                    break;
            }
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = NPC.direction * -1;

            if (++frameCounter >= 12)
            {
                frameCounter = 0;
                frame++;
                if (frame >= 2)
                {
                    frame = 0;
                }
            }
            NPC.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;

            Vector2 drawOrigin = NPC.frame.Size() / 2;
            Vector2 drawPos = NPC.position - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY); //.RotatedBy(NPC.rotation);
            Color color = NPC.GetAlpha(drawColor);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPos, NPC.frame, color, NPC.rotation + rotate, drawOrigin, NPC.scale, spriteEffects, 0f);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        private void VerticalLaser()
        {
            EyeOfCthulhu boss = Main.npc[(int)NPC.ai[0]].GetGlobalNPC<EyeOfCthulhu>();
            
            int index = (int)NPC.ai[1] * 16 - 1;
            if (index > boss.oldPos.Count - 1)
            {
                index = boss.oldPos.Count - 1;
            }

            if (boss.AI_Timer < 0)
            {
                float speed = Vector2.Distance(NPC.Center, boss.oldPos[index]) / (1f - boss.AI_Timer * 0.5f);
                float inertia = (1f - boss.AI_Timer * 0.5f);
                Vector2 direction = boss.oldPos[index] - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            }
            else
            {
                NPC.velocity = Vector2.Zero;
                NPC.Center = boss.oldPos[index];
            }


            float turn = AGUtils.TurnTowards(5, NPC.Center + new Vector2(0, 5), NPC.rotation, NPC.Center);
            NPC.rotation += turn;
        }

        private void FormationLaser()
        {
            EyeOfCthulhu boss = Main.npc[(int)NPC.ai[0]].GetGlobalNPC<EyeOfCthulhu>();
            NPC bossnpc = Main.npc[(int)NPC.ai[0]];
            Vector2 targetPos = bossnpc.Center;
            switch (NPC.ai[1])
            {
                default:
                    break;
                case 1:
                    targetPos += new Vector2(-75, 20);
                    break;
                case 2:
                    targetPos += new Vector2(75, 20);
                    break;
                case 3:
                    targetPos += new Vector2(-125, -20);
                    break;
                case 4:
                    targetPos += new Vector2(125, -20);
                    break;

            }
            if (boss.AI_Timer < 0)
            {
                float speed = Vector2.Distance(NPC.Center, targetPos) / (1f - boss.AI_Timer * 0.5f);
                float inertia = (1f - boss.AI_Timer * 0.5f);
                Vector2 direction = targetPos - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;    
            }
            else
            {
                NPC.velocity = Vector2.Zero;
                NPC.Center = targetPos;
            }
                float turn = AGUtils.TurnTowards(5, NPC.Center + new Vector2(0, 5), NPC.rotation, NPC.Center);
            NPC.rotation += turn;
        }

        private void Dash()
        {
            EyeOfCthulhu boss = Main.npc[(int)NPC.ai[0]].GetGlobalNPC<EyeOfCthulhu>();
            NPC bossnpc = Main.npc[(int)NPC.ai[0]];
            Vector2 targetPos = bossnpc.Center;
            switch (NPC.ai[1])
            {
                default:
                    break;
                case 1:
                    targetPos += new Vector2(-180, 0);
                    break;
                case 2:
                    targetPos += new Vector2(180, 0);
                    break;
                case 3:
                    targetPos += new Vector2(-360, 0);
                    break;
                case 4:
                    targetPos += new Vector2(360, 0);
                    break;

            }
            if (boss.AI_Timer < 0)
            {
                float speed = Vector2.Distance(NPC.Center, targetPos) / (1f - boss.AI_Timer * 0.5f);
                float inertia = (1f - boss.AI_Timer * 0.5f);
                Vector2 direction = targetPos - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;    
            }
            else
            {
                NPC.velocity = Vector2.Zero;
                NPC.Center = targetPos;
            }
            float turn = AGUtils.TurnTowards(5, NPC.Center + new Vector2(0, 5).RotatedBy(bossnpc.rotation), NPC.rotation, NPC.Center);
            NPC.rotation += turn;
        }

        private void TeardropRain()
        {
            EyeOfCthulhu boss = Main.npc[(int)NPC.ai[0]].GetGlobalNPC<EyeOfCthulhu>();
            NPC bossnpc = Main.npc[(int)NPC.ai[0]];
            Vector2 targetPos = bossnpc.Center;
            switch (NPC.ai[1])
            {
                default:
                    break;
                case 1:
                    targetPos += new Vector2(-400, 0);
                    break;
                case 2:
                    targetPos += new Vector2(800, 0);
                    break;
                case 3:
                    targetPos += new Vector2(-800, 0);
                    break;
                case 4:
                    targetPos += new Vector2(400, 0);
                    break;

            }
            if (boss.AI_Timer < 0)
            {
                float speed = Vector2.Distance(NPC.Center, targetPos) / (1f - boss.AI_Timer * 0.5f);
                float inertia = (1f - boss.AI_Timer * 0.5f);
                Vector2 direction = targetPos - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;    
            }
            else
            {
                NPC.velocity.Y = 0;
                switch (NPC.ai[1])
                {
                    default:
                        break;
                    case 1:
                        NPC.velocity.X = 3f;
                        break;
                    case 2:
                        NPC.velocity.X = -3f;
                        break;
                    case 3:
                        NPC.velocity.X = 3f;
                        break;
                    case 4:
                        NPC.velocity.X = -3f;
                        break;
                }
                if (boss.AI_Timer % 15 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item171, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -5.5f), ModContent.ProjectileType<EyeOfCthulhuProjTeardrop>(), 13, 0, NPC.target, 0.075f);
                }
            }
                float turn = AGUtils.TurnTowards(5, NPC.Center + new Vector2(0, -5), NPC.rotation, NPC.Center);
            NPC.rotation += turn;
        }
        private void PhaseTransition()
        {
            EyeOfCthulhu boss = Main.npc[(int)NPC.ai[0]].GetGlobalNPC<EyeOfCthulhu>();
            NPC bossnpc = Main.npc[(int)NPC.ai[0]];
            Vector2 targetPos = bossnpc.Center;
            if (boss.AI_Timer < 0)
            {
                float speed = Vector2.Distance(NPC.Center, targetPos) / (1f - boss.AI_Timer * 0.5f);
                float inertia = (1f - boss.AI_Timer * 0.5f);
                Vector2 direction = targetPos - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;    
            }
            else
            {

                NPC.velocity = Vector2.Zero;
                NPC.Center = targetPos;
            }
                float turn = AGUtils.TurnTowards(5, NPC.Center + new Vector2(0, -5), NPC.rotation, NPC.Center);
            NPC.rotation += turn;
        }
        private void Flamethrower()
        {
            EyeOfCthulhu boss = Main.npc[(int)NPC.ai[0]].GetGlobalNPC<EyeOfCthulhu>();
            NPC bossnpc = Main.npc[(int)NPC.ai[0]];
            Vector2 targetPos = bossnpc.Center;
            Player target = Main.player[bossnpc.target];
            switch (NPC.ai[1])
            {
                default:
                    break;
                case 1:
                    targetPos += new Vector2(0, -60);
                    break;
                case 2:
                    targetPos += new Vector2(0, -80);
                    break;
                case 3:
                    targetPos += new Vector2(0, -100);
                    break;
                case 4:
                    targetPos += new Vector2(0, -120);
                    break;

            }
            if (boss.AI_Timer < 0)
            {
                float speed = Vector2.Distance(NPC.Center, targetPos) / (1f - boss.AI_Timer * 0.5f);
                float inertia = (1f - boss.AI_Timer * 0.5f);
                Vector2 direction = targetPos - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            }
            else
            {
                switch (NPC.ai[1])
                {
                    default:
                        break;
                    case 1:
                        if (boss.AI_Timer < 30)
                        {
                            NPC.velocity = Vector2.Zero;
                            NPC.Center = targetPos;
                            float turn = AGUtils.TurnTowards(5, NPC.Center + new Vector2(0, 5).RotatedBy(bossnpc.rotation), NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        else if (boss.AI_Timer <= 120)
                        {
                            if (boss.AI_Timer == 30)
                            {
                                NPC.velocity = new Vector2(boss.atkDir * -4, -8);
                            }
                            targetPos = target.Center + new Vector2(-250, 0) * boss.atkDir;
                            float speed = Vector2.Distance(NPC.Center, targetPos) / (1f - (boss.AI_Timer - 120) * 0.5f);
                            float inertia = (1f - (boss.AI_Timer - 120) * 0.5f);
                            Vector2 direction = targetPos - NPC.Center;
                            direction = direction.SafeNormalize(Vector2.Zero);
                            direction *= speed;

                            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                            float turn = AGUtils.TurnTowards(12, NPC.Center + NPC.velocity, NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        else
                        {
                            if (boss.AI_Timer == 121)
                            {
                                NPC.velocity = new Vector2(2, 0) * boss.atkDir;
                            }
                            NPC.velocity += new Vector2(0.25f, 0) * boss.atkDir;
                            float turn = AGUtils.TurnTowards(90, NPC.Center + NPC.velocity, NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        break;
                    case 2:
                        if (boss.AI_Timer < 90)
                        {
                            NPC.velocity = Vector2.Zero;
                            NPC.Center = targetPos;
                            float turn = AGUtils.TurnTowards(5, NPC.Center + new Vector2(0, 5).RotatedBy(bossnpc.rotation), NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        else if (boss.AI_Timer <= 180)
                        {
                            if (boss.AI_Timer == 90)
                            {
                                NPC.velocity = new Vector2(boss.atkDir * -4, -8);
                            }
                            targetPos = target.Center + new Vector2(-250, 0) * boss.atkDir;
                            float speed = Vector2.Distance(NPC.Center, targetPos) / (1f - (boss.AI_Timer - 180) * 0.5f);
                            float inertia = (1f - (boss.AI_Timer - 180) * 0.5f);
                            Vector2 direction = targetPos - NPC.Center;
                            direction = direction.SafeNormalize(Vector2.Zero);
                            direction *= speed;

                            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                            float turn = AGUtils.TurnTowards(12, NPC.Center + NPC.velocity, NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        else
                        {

                            if (boss.AI_Timer == 181)
                            {
                                NPC.velocity = new Vector2(2, 0) * boss.atkDir;
                            }
                            NPC.velocity += new Vector2(0.25f, 0) * boss.atkDir; float turn = AGUtils.TurnTowards(90, NPC.Center + NPC.velocity, NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        break;
                    case 3:
                        if (boss.AI_Timer < 150)
                        {
                            NPC.velocity = Vector2.Zero;
                            NPC.Center = targetPos;
                            float turn = AGUtils.TurnTowards(5, NPC.Center + new Vector2(0, 5).RotatedBy(bossnpc.rotation), NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        else if (boss.AI_Timer <= 240)
                        {
                            if (boss.AI_Timer == 150)
                            {
                                NPC.velocity = new Vector2(boss.atkDir * -4, -8);
                            }
                            targetPos = target.Center + new Vector2(-250, 0) * boss.atkDir;
                            float speed = Vector2.Distance(NPC.Center, targetPos) / (1f - (boss.AI_Timer - 240) * 0.5f);
                            float inertia = (1f - (boss.AI_Timer - 240) * 0.5f);
                            Vector2 direction = targetPos - NPC.Center;
                            direction = direction.SafeNormalize(Vector2.Zero);
                            direction *= speed;

                            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                            float turn = AGUtils.TurnTowards(12, NPC.Center + NPC.velocity, NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        else
                        {

                            if (boss.AI_Timer == 241)
                            {
                                NPC.velocity = new Vector2(2, 0) * boss.atkDir;
                            }
                            NPC.velocity += new Vector2(0.25f, 0) * boss.atkDir; float turn = AGUtils.TurnTowards(90, NPC.Center + NPC.velocity, NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        break;
                    case 4:
                        if (boss.AI_Timer < 210)
                        {
                            NPC.velocity = Vector2.Zero;
                            NPC.Center = targetPos;
                            float turn = AGUtils.TurnTowards(5, NPC.Center + new Vector2(0, 5).RotatedBy(bossnpc.rotation), NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        else if (boss.AI_Timer <= 300)
                        {
                            if (boss.AI_Timer == 210)
                            {
                                NPC.velocity = new Vector2(boss.atkDir * -4, -8);
                            }
                            targetPos = target.Center + new Vector2(-250, 0) * boss.atkDir;
                            float speed = Vector2.Distance(NPC.Center, targetPos) / (1f - (boss.AI_Timer - 300) * 0.5f);
                            float inertia = (1f - (boss.AI_Timer - 300) * 0.5f);
                            Vector2 direction = targetPos - NPC.Center;
                            direction = direction.SafeNormalize(Vector2.Zero);
                            direction *= speed;

                            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                            float turn = AGUtils.TurnTowards(12, NPC.Center + NPC.velocity, NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        else
                        {

                            if (boss.AI_Timer == 301)
                            {
                                NPC.velocity = new Vector2(2, 0) * boss.atkDir;
                            }
                            NPC.velocity += new Vector2(0.25f, 0) * boss.atkDir; float turn = AGUtils.TurnTowards(90, NPC.Center + NPC.velocity, NPC.rotation, NPC.Center);
                            NPC.rotation += turn;
                        }
                        break;
                }
                if (NPC.Center.X > target.Center.X + 1500)
                {
                    NPC.Center = new Vector2(target.Center.X + 1500, NPC.Center.Y);
                }
                if (NPC.Center.X < target.Center.X - 1500)
                {
                    NPC.Center = new Vector2(target.Center.X - 1500, NPC.Center.Y);
                }
            }
        }
        private void LaserWall()
        {
            EyeOfCthulhu boss = Main.npc[(int)NPC.ai[0]].GetGlobalNPC<EyeOfCthulhu>();
            NPC bossnpc = Main.npc[(int)NPC.ai[0]];
            Vector2 targetPos = bossnpc.Center;
            switch (NPC.ai[1])
            {
                default:
                    break;
                case 1:
                    targetPos += new Vector2(-120, 0);
                    break;
                case 2:
                    targetPos += new Vector2(120, 0);
                    break;
                case 3:
                    targetPos += new Vector2(-240, 0);
                    break;
                case 4:
                    targetPos += new Vector2(240, 0);
                    break;

            }
            if (boss.AI_Timer < 0)
            {
                float speed = Vector2.Distance(NPC.Center, targetPos) / (1f - boss.AI_Timer * 0.5f);
                float inertia = (1f - boss.AI_Timer * 0.5f);
                Vector2 direction = targetPos - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                if (boss.AI_Timer < -45)
                {
                    NPC.velocity = new Vector2(0, -15);
                }
            }
            else
            {
                NPC.velocity = Vector2.Zero;
                NPC.Center = targetPos;
            }
            float turn = AGUtils.TurnTowards(5, NPC.Center + new Vector2(0, 5).RotatedBy(bossnpc.rotation), NPC.rotation, NPC.Center);
            NPC.rotation += turn;
        }
    }
}