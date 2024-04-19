using AwfulGarbageMod.BossBars;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Consumables;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.NPCs;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AwfulGarbageMod.NPCs.Boss
{
    // These three class showcase usage of the WormHead, WormBody and WormTail classes from Worm.cs
    [AutoloadBossHead]
    internal class FrigidiusHead : WormHead
	{
		public override int BodyType => ModContent.NPCType<FrigidiusBody>();

		public override int TailType => ModContent.NPCType<FrigidiusTail>();

		public override void SetStaticDefaults() {

        }

        private enum ActionState
        {
            CirclePlayer1,
            GoOffScreen,
            DashTowardsPlayer,
            ChasePlayer,
            BurrowDown,
            DashUp,

            CirclePlayer2,
            CirclePlayer3, 
            GoOffScreen2,
            DashTowardsPlayer2,
            ChasePlayer2,
            GoOffScreen3,
            DashUp2,

            PhaseTransition1,
            PhaseTransition2,
            Midphase,
            PhaseTransition3,

            Despawn
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<FrigidiusBag>()));

            // Trophies are spawned with 1/10 chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Boss.FrigidiusTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Boss.FrigidiusRelic>()));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            ///npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());


            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<FrigidiumBar>(), 1, 2, 3));


            // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
            // Boss masks are spawned with 1/7 chance
            ///notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MinionBossMask>(), 7));

            // This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
            // We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
            // which requires these parameters to be defined
            ///int itemType = ModContent.ItemType<ExampleItem>();
            ///var parameters = new DropOneByOne.Parameters()
            ///{
            ///    ChanceNumerator = 1,
            ///    ChanceDenominator = 1,
            ///    MinimumStackPerChunkBase = 1,
            ///    MaximumStackPerChunkBase = 1,
            ///    MinimumItemDropsCount = 12,
            ///    MaximumItemDropsCount = 15,
            ///};

            ///notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

            // Finally add the leading rule
            npcLoot.Add(notExpertRule);
        }

        public override void SetDefaults() {
			// Head is 10 defense, body 20, tail 30.
			NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.width = 32;
            NPC.height = 32;
			NPC.aiStyle = -1;
            NPC.damage = 60; // The amount of damage that this npc deals
            NPC.defense = 5; // The amount of defense that this npc has
            NPC.lifeMax = 54000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit2; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath1; // The sound the NPC will make when it dies.
            NPC.value = 50000f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.npcSlots = 100f;

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/FrigidiusTheme");
            }
            NPC.BossBar = ModContent.GetInstance<TreeToadBossBar>();
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = 62000;
            if (Main.masterMode)
            {
                NPC.lifeMax = 70000; // Increase by 5 if expert or master mode
                if (Main.getGoodWorld || Main.zenithWorld)
                {
                    NPC.lifeMax = 99999;
                }
            }
        }

        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedFrigidius, -1);

            // Since this hook is only ran in singleplayer and serverside, we would have to sync it manually.
            // Thankfully, vanilla sends the MessageID.WorldData packet if a BOSS was killed automatically, shortly after this hook is ran

            // If your NPC is not a boss and you need to sync the world (which includes ModSystem, check DownedBossSystem), use this code:
            /*
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			*/
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
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
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.FrigidiusHead")
            });
        }


        public override void Init() {
			// Set the segment variance
			// If you want the segment length to be constant, set these two properties to the same value
			MinSegmentLength = 30;
			MaxSegmentLength = 30;

			CommonWormInit(this);
		}

		// This method is invoked from FrigidiusHead, FrigidiusBody and FrigidiusTail
		internal static void CommonWormInit(WormFrigidius worm) {
			// These two properties handle the movement of the worm
			worm.MoveSpeed = 5.5f;
			worm.Acceleration = 0.045f;
		}

        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest();
            targetPos = Main.player[NPC.target].Center;

            NPC.lifeMax *= (ModContent.GetInstance<Config>().BossHealthMultiplier / 100);
            NPC.life = NPC.lifeMax;
        }

        float AI_State;
        float Next_State;
        float AI_Timer;
        float bossPhase = 1;
        float RotateDir;
        float RotateMagnitude = 1000;
        float tempx;
        float temp;
        Vector2 targetPos;
        Vector2 storedVel = Vector2.Zero;

        public override void AI()
        {
            DrawOffsetY = 16;

            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                AI_State = (float)ActionState.Despawn;
                NPC.EncourageDespawn(0);
                return;
            }

            if (NPC.CountNPCS(ModContent.NPCType<FrigidiusTail>()) < 1)
            {
                NPC.ai[0] = 0;
            }


            if (NPC.life < NPC.lifeMax * 3 / 5 && bossPhase == 1)
            {
                bossPhase = 2;
                AI_Timer = 150;
                AI_State = (float)ActionState.PhaseTransition1;
            }

            switch (AI_State)
            {
                case (float)ActionState.PhaseTransition1:
                    PhaseTransition1();
                    break;
                case (float)ActionState.PhaseTransition2:
                    PhaseTransition2();
                    break;
                case (float)ActionState.Midphase:
                    Midphase();
                    break;
                case (float)ActionState.PhaseTransition3:
                    PhaseTransition3();
                    break;
                case (float)ActionState.CirclePlayer1:
                    CirclePlayer1();
                    break;
                case (float)ActionState.CirclePlayer2:
                    CirclePlayer2();
                    break;
                case (float)ActionState.CirclePlayer3:
                    CirclePlayer3();
                    break;
                case (float)ActionState.GoOffScreen:
                    GoOffScreen();
                    break;
                case (float)ActionState.GoOffScreen2:
                    GoOffScreen2();
                    break;
                case (float)ActionState.DashTowardsPlayer:
                    DashTowardsPlayer();
                    break;
                case (float)ActionState.DashTowardsPlayer2:
                    DashTowardsPlayer2();
                    break;
                case (float)ActionState.ChasePlayer:
                    ChasePlayer();
                    break;
                case (float)ActionState.ChasePlayer2:
                    ChasePlayer2();
                    break;
                case (float)ActionState.BurrowDown:
                    BurrowDown();
                    break;
                case (float)ActionState.GoOffScreen3:
                    GoOffScreen3();
                    break;
                case (float)ActionState.DashUp:
                    DashUp();
                    break;
                case (float)ActionState.DashUp2:
                    DashUp2();
                    break;
                case (float)ActionState.Despawn:
                    Despawn();
                    break;
            }
        }

        private void PhaseTransition1()
        {
            NPC.damage = 0;
            NPC.dontTakeDamage = true;
            Player player = Main.player[NPC.target];

            NPC.velocity += new Vector2(0, 0.11f);
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;


            NPC.alpha += 2;
            if (NPC.alpha > 150)
            {
                NPC.alpha = 90;
            }

            AI_Timer--;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.PhaseTransition2;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
                targetPos = Main.player[NPC.target].Center;
                RotateMagnitude = 1000;
                RotateDir = MathHelper.ToDegrees((NPC.Center - Main.player[NPC.target].Center).ToRotation());

            }
        }
        private void PhaseTransition2()
        {
            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 120;
                }
            }
            else
            {
                NPC.damage = 60;
            }
                Player player = Main.player[NPC.target];

            NPC.velocity = new Vector2(0, 0);

            NPC.noTileCollide = true;
            float speed = Vector2.Distance(player.Center, targetPos) / 20000;


            Vector2 direction = player.Center - targetPos;
            direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            targetPos += direction;

            NPC.position = targetPos + new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(RotateDir)) * RotateMagnitude - new Vector2(NPC.width / 2, NPC.height / 2);

            Vector2 temp = targetPos - NPC.Center;

            AI_Timer++;


            NPC.rotation = temp.ToRotation() - MathHelper.ToRadians(0);

            if (RotateMagnitude > 400)
            {
                RotateMagnitude -= 5;
            }
            else
            {
                AI_State = (float)ActionState.Midphase;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
            }
            NPC.alpha += 2;
            if (NPC.alpha > 150)
            {
                NPC.alpha = 90;
            }

            RotateDir += 1.5f;


            if (AI_Timer >= 30 & AI_Timer < 110)
            {
                if (AI_Timer % 10 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<IceChunk>());
                    }
                }
            }
        }
        private void Midphase()
        {
            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 120;
                }
            }
            else
            {
                NPC.damage = 60;
            }
            Player player = Main.player[NPC.target];

            NPC.velocity = new Vector2(0, 0);

            NPC.noTileCollide = true;
            float speed = Vector2.Distance(player.Center, targetPos) / 20000;


            Vector2 direction = player.Center - targetPos;
            direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            targetPos += direction;

            NPC.position = targetPos + new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(RotateDir)) * RotateMagnitude - new Vector2(NPC.width / 2, NPC.height / 2);

            Vector2 temp = targetPos - NPC.Center;



            NPC.rotation = temp.ToRotation() - MathHelper.ToRadians(0);

            
            NPC.alpha += 2;
            if (NPC.alpha > 150)
            {
                NPC.alpha = 90;
            }

            if (NPC.CountNPCS(ModContent.NPCType<IceChunk>()) < 1)
            {
                AI_State = (float)ActionState.PhaseTransition3;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
            }

            AI_Timer++;
            RotateDir += 2f;

        }
        private void PhaseTransition3()
        {
            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 120;
                }
            }
            else
            {
                NPC.damage = 60;
            }
            Player player = Main.player[NPC.target];

            NPC.velocity = new Vector2(0, 0);

            NPC.noTileCollide = true;
            float speed = Vector2.Distance(player.Center, targetPos) / 20000;


            Vector2 direction = player.Center - targetPos;
            direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            targetPos += direction;

            NPC.position = targetPos + new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(RotateDir)) * RotateMagnitude - new Vector2(NPC.width / 2, NPC.height / 2);

            Vector2 temp = targetPos - NPC.Center;

            AI_Timer++;


            NPC.rotation = temp.ToRotation() - MathHelper.ToRadians(0);

            if (RotateMagnitude > 300)
            {
                RotateMagnitude -= 0.5f;
            }
            else
            {
                AI_State = (float)ActionState.CirclePlayer2;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
                NPC.dontTakeDamage = false;
            }
            NPC.alpha -= 2;
            if (NPC.alpha < 0)
            {
                NPC.alpha = 0;
            }

            RotateDir += 1.5f;
        }

        private void CirclePlayer1()
        {
            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 120;
                }
            }
            else
            {
                NPC.damage = 60;
            }
            Player player = Main.player[NPC.target];

            NPC.velocity = new Vector2(0, 0);

            NPC.noTileCollide = true;
            float speed = Vector2.Distance(player.Center, targetPos) / 20000;

            
            Vector2 direction = player.Center - targetPos;
            direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            targetPos += direction;
            
            NPC.position = targetPos + new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(RotateDir)) * RotateMagnitude - new Vector2(NPC.width / 2, NPC.height / 2);

            Vector2 temp = targetPos - NPC.Center;



            NPC.rotation = temp.ToRotation() - MathHelper.ToRadians(0);

            if (RotateMagnitude > 300)
            {
                RotateMagnitude -= 5;
            }
            else if (RotateMagnitude < 295)
            {
                RotateMagnitude += 5;
            }
            else
            {
                if (AI_Timer % 90 == 30)
                {
                    SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceBolt").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 7f;
                    if (Main.expertMode)
                    {
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceBolt").Type, 17, 0, Main.myPlayer);
                        Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(24)) * 7f;
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceBolt").Type, 17, 0, Main.myPlayer);
                        Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-24)) * 7f;
                    } 
                }
                else if (AI_Timer % 90 < 30)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1f;
                    Main.dust[dust].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10f;
                    Main.dust[dust].noGravity = true;
                }
                if (AI_Timer % 90 == 0)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_0")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound, NPC.Center);
                }
                AI_Timer++;

                if (AI_Timer == 900)
                {
                    AI_State = (float)ActionState.GoOffScreen;
                    AI_Timer = 180;
                }
            }
            RotateDir += 1.5f;
        }
        private void CirclePlayer2()
        {
            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 120;
                }
            }
            else
            {
                NPC.damage = 60;
            }
            Player player = Main.player[NPC.target];

            NPC.velocity = new Vector2(0, 0);

            NPC.noTileCollide = true;
            float speed = Vector2.Distance(player.Center, targetPos) / 20000;


            Vector2 direction = player.Center - targetPos;
            direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            targetPos += direction;

            NPC.position = targetPos + new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(RotateDir)) * RotateMagnitude - new Vector2(NPC.width / 2, NPC.height / 2);

            Vector2 temp = targetPos - NPC.Center;



            NPC.rotation = temp.ToRotation() - MathHelper.ToRadians(0);

            if (RotateMagnitude > 300)
            {
                RotateMagnitude -= 5;
            }
            else if (RotateMagnitude < 295)
            {
                RotateMagnitude += 5;
            }
            else
            {
                if (AI_Timer < 30)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1f;
                    Main.dust[dust].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10f;
                    Main.dust[dust].noGravity = true;

                }
                else if (AI_Timer % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceBolt").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero) * 7f;
                }
                if (AI_Timer == 0)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_0")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound, NPC.Center);
                }
                AI_Timer++;
                if (AI_Timer == 500)
                {
                    AI_State = (float)ActionState.CirclePlayer3;
                    AI_Timer = 0;
                }
            }
            RotateDir += 1.5f;
        }
        private void CirclePlayer3()
        {
            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 120;
                }
            }
            else
            {
                NPC.damage = 60;
            }
            Player player = Main.player[NPC.target];

            NPC.velocity = new Vector2(0, 0);

            NPC.noTileCollide = true;
            float speed = Vector2.Distance(player.Center, targetPos) / 20000;


            Vector2 direction = player.Center - targetPos;
            direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            targetPos += direction;

            NPC.position = targetPos + new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(RotateDir)) * RotateMagnitude - new Vector2(NPC.width / 2, NPC.height / 2);

            Vector2 temp = targetPos - NPC.Center;



            NPC.rotation = temp.ToRotation() - MathHelper.ToRadians(0);

            if (RotateMagnitude > 420)
            {
                RotateMagnitude -= 5;
            }
            else if (RotateMagnitude < 415)
            {
                RotateMagnitude += 5;
            }
            else
            {
                if (AI_Timer % 150 < 8)
                {
                    int dust;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.type == ModContent.NPCType<FrigidiusBody>())
                        {
                            dust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Frost, 0f, 0f, 0, default(Color), 1f);
                            Main.dust[dust].scale = 1f;
                            Main.dust[dust].velocity = new Vector2(1, 0).RotatedBy(npc.rotation * 10f);
                            Main.dust[dust].noGravity = true;
                        }
                    }

                }
                else if (AI_Timer % 150 == 60)
                {
                    SoundEngine.PlaySound(SoundID.Item28, NPC.Center);
                    int proj;
                    float projspd = 4.5f;
                    if (Main.expertMode)
                    {
                        projspd = 7f;
                    }
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.type == ModContent.NPCType<FrigidiusBody>())
                        {
                            proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.velocity, Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer);
                            Main.projectile[proj].velocity = new Vector2(1, 0).RotatedBy(npc.rotation) * projspd;
                        }
                    }
                }
                AI_Timer++;
                if (AI_Timer == 600)
                {
                    AI_State = (float)ActionState.GoOffScreen2;
                    AI_Timer = 180;
                }
            }
            RotateDir += 2f;
        }
        private void GoOffScreen()
        {
            if (Main.expertMode)
            {
                NPC.damage = 60;

                if (Main.masterMode)
                {
                    NPC.damage = 80;
                }
            }
            else
            {
                NPC.damage = 40;
            }
            Player player = Main.player[NPC.target];


            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity -= direction * 0.0002f;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;


            AI_Timer--;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.DashTowardsPlayer;
                AI_Timer = 0;
                NPC.velocity = new Vector2 (0, 0);
            }
        }
        private void GoOffScreen2()
        {
            if (Main.expertMode)
            {
                NPC.damage = 60;

                if (Main.masterMode)
                {
                    NPC.damage = 80;
                }
            }
            else
            {
                NPC.damage = 40;
            }
            Player player = Main.player[NPC.target];


            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity -= direction * 0.0002f;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;


            AI_Timer--;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.DashTowardsPlayer2;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
            }
        }
        private void GoOffScreen3()
        {
            if (Main.expertMode)
            {
                NPC.damage = 60;

                if (Main.masterMode)
                {
                    NPC.damage = 80;
                }
            }
            else
            {
                NPC.damage = 40;
            }
            Player player = Main.player[NPC.target];


            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity -= direction * 0.0002f;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;


            AI_Timer--;
            if (AI_Timer == 0)
            {

                AI_State = (float)ActionState.CirclePlayer2;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
                targetPos = Main.player[NPC.target].Center;
                RotateMagnitude = 1000;
                RotateDir = MathHelper.ToDegrees((NPC.Center - Main.player[NPC.target].Center).ToRotation());
            }
        }

        private void DashTowardsPlayer()
        {
            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 120;
                }
            }
            else
            {
                NPC.damage = 60;
            }
            Player player = Main.player[NPC.target];

            if (AI_Timer % 150 == 0)
            {
                Vector2 oldPos = NPC.position;
                NPC.position = player.Center + new Vector2(1200, 0).RotatedByRandom(MathHelper.ToRadians(360)) + new Vector2(NPC.width/2, NPC.height/2);
                NPC.velocity = Vector2.Zero;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.type == ModContent.NPCType<FrigidiusBody>() || npc.type == ModContent.NPCType<FrigidiusTail>())
                    {
                        npc.position += oldPos - NPC.position;
                    }
                    

                }
                
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("IceSpiritTelegraph").Type, 0, 0, Main.myPlayer);
                Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20f;
                storedVel = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 40f;
            }

            if (AI_Timer % 150 == 60)
            {
                SoundEngine.PlaySound(SoundID.Roar);

                NPC.velocity = storedVel;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (AI_Timer % 150 == 90)
            {
                if (Main.expertMode)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 12f;
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * -12f;
                    if (Main.masterMode)
                    {
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer);
                        Main.projectile[proj].velocity = (NPC.velocity).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(90)) * 12f;
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer);
                        Main.projectile[proj].velocity = (NPC.velocity).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-90)) * 12f;
                    }
                }
                else
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (NPC.velocity).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(90)) * 12f;
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (NPC.velocity).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-90)) * 12f;
                }
            }

            if (AI_Timer % 150 == 140)
            {
                NPC.velocity = Vector2.Zero;
            }



            AI_Timer++;
            if (AI_Timer == 750)
            {
                AI_State = (float)ActionState.ChasePlayer;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
            }
        }
        private void DashTowardsPlayer2()
        {
            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 120;
                }
            }
            else
            {
                NPC.damage = 60;
            }
            Player player = Main.player[NPC.target];

            if (AI_Timer % 150 == 0)
            {
                Vector2 oldPos = NPC.position;
                NPC.position = player.Center + new Vector2(Main.rand.NextFloat(1140, 1260), 0).RotatedByRandom(MathHelper.ToRadians(360)) + new Vector2(NPC.width / 2, NPC.height / 2);
                NPC.velocity = Vector2.Zero;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.type == ModContent.NPCType<FrigidiusBody>() || npc.type == ModContent.NPCType<FrigidiusTail>())
                    {
                        npc.position += oldPos - NPC.position;
                    }


                }

                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("IceSpiritTelegraph").Type, 0, 0, Main.myPlayer);
                Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20f;
                storedVel = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * (40f + Main.expertMode.ToInt() * 15);
            }

            if (AI_Timer % 150 == 60)
            {
                SoundEngine.PlaySound(SoundID.Roar);

                NPC.velocity = storedVel;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (AI_Timer % 150 >= 60 && AI_Timer % 150 <= 140)
            {
                int tempmod = 3;
                if (Main.expertMode)
                {
                    tempmod = 2;
                }
                if (AI_Timer % tempmod == 0)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer, Main.expertMode.ToInt() * 60);
                    Main.projectile[proj].velocity = (NPC.velocity).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(90)) * (5f + Main.expertMode.ToInt() * 13);
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer, Main.expertMode.ToInt() * 60);
                    Main.projectile[proj].velocity = (NPC.velocity).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-90)) * (5f + Main.expertMode.ToInt() * 13);
                }
            }

            if (AI_Timer % 150 == 145)
            {
                NPC.velocity = Vector2.Zero;
            }



            AI_Timer++;
            if (AI_Timer == 750)
            {
                //AI_State = (float)ActionState.ChasePlayer2;
                //AI_Timer = 0;
                //NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.DashUp2;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
                targetPos = Main.player[NPC.target].Center;
                RotateMagnitude = 1000;
                RotateDir = MathHelper.ToDegrees((NPC.Center - Main.player[NPC.target].Center).ToRotation());
            }
        }
        private void ChasePlayer()
        {
            if (Main.expertMode)
            {
                NPC.damage = 60;

                if (Main.masterMode)
                {
                    NPC.damage = 80;
                }
            }
            else
            {
                NPC.damage = 40;
            }
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity += direction * 0.002f + direction.SafeNormalize(Vector2.Zero) * 0.1f;

            NPC.velocity *= 0.9f;

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            int tempmod;
            if (Main.expertMode)
            {
                tempmod = 18;
                if (Main.masterMode)
                {
                    tempmod = 15;
                }
            }
            else
            {
                tempmod = 24;
            }
            if (AI_Timer % tempmod == 0)
            {
                Vector2 position = player.Center + new Vector2(1000, 0).RotatedByRandom(MathHelper.ToRadians(360));
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), position, NPC.velocity, Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer);
                Main.projectile[proj].velocity = (Main.player[NPC.target].Center - position).SafeNormalize(Vector2.Zero) * 8f;
            }


            AI_Timer++;
            if (AI_Timer == 1200)
            {
                AI_State = (float)ActionState.BurrowDown;
                AI_Timer = 150;
                NPC.velocity = new Vector2(0, 0);
            }
        }
        private void ChasePlayer2()
        {
            if (Main.expertMode)
            {
                NPC.damage = 60;

                if (Main.masterMode)
                {
                    NPC.damage = 80;
                }
            }
            else
            {
                NPC.damage = 40;
            }
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;

            if (AI_Timer % 480 < 210)
            {
                NPC.velocity = direction * 0.01f + direction.SafeNormalize(Vector2.Zero) * 2f;
            }
            else if (AI_Timer % 480 < 270)
            {
                if (AI_Timer % 480 == 210)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound, NPC.Center);
                }
                NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * 0.01f;
                RotateDir = NPC.velocity.ToRotation();
                if (AI_Timer % 3 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.SafeNormalize(Vector2.Zero) * 12, Mod.Find<ModProjectile>("IceShardFrigidiusTelegraph").Type, 0, 0, Main.myPlayer, AGUtils.TurnTowards(((AI_Timer % 480) - 210) * 5f/60, player.Center, RotateDir, NPC.Center));
                }
            }
            else if (AI_Timer % 480 < 360)
            {
                if (AI_Timer % 480 == 270)
                {
                    SoundEngine.PlaySound(SoundID.Roar);
                }
                RotateDir += AGUtils.TurnTowards(5, player.Center, RotateDir, NPC.Center);
                NPC.velocity = new Vector2(24, 0).RotatedBy(RotateDir);
            }
            else if (AI_Timer % 480 < 390)
            {
                NPC.velocity *= 0.9f;
            }
            else if (AI_Timer % 480 == 390)
            {
                SoundEngine.PlaySound(SoundID.Roar);

                NPC.velocity = direction.SafeNormalize(Vector2.Zero) * 18f;
                if (Main.expertMode)
                {
                    SoundEngine.PlaySound(SoundID.Item28, NPC.Center);

                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceBolt").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 5.5f;
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceBolt").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(24)) * 5.5f;
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("IceBolt").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-24)) * 5.5f;
                }
            }
            else
            {
                NPC.velocity *= 0.99f;
            }


            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            AI_Timer++;
            if (AI_Timer == 1920)
            {
                AI_State = (float)ActionState.GoOffScreen3;
                AI_Timer = 180;
            }
        }

        private void BurrowDown()
        {
            NPC.damage = 0;
            Player player = Main.player[NPC.target];

            NPC.velocity += new Vector2(0, 0.13f);
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;


            AI_Timer--;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.DashUp;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
            }
        }

        private void DashUp()
        {
            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 120;
                }
            }
            else
            {
                NPC.damage = 60;
            }
            Player player = Main.player[NPC.target];

            if (AI_Timer % 180 == 0)
            {
                tempx = 3.8f;
                Vector2 oldPos = NPC.position;
                NPC.position = player.Center + new Vector2(Main.rand.NextFloat(-30, 30), 1000) + new Vector2(NPC.width / 2, NPC.height / 2);
                NPC.velocity = Vector2.Zero;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.type == ModContent.NPCType<FrigidiusBody>() || npc.type == ModContent.NPCType<FrigidiusTail>())
                    {
                        npc.position += oldPos - NPC.position;
                    }


                }

                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("IceSpiritTelegraph").Type, 0, 0, Main.myPlayer);
                Main.projectile[proj].velocity = new Vector2(0, -20f);
                storedVel = new Vector2(0, -35f);
            }

            if (AI_Timer % 180 == 60)
            {
                SoundEngine.PlaySound(SoundID.Roar);

                NPC.velocity = storedVel;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (AI_Timer % 180 >= 90 && AI_Timer % 180 <= 140)
            {
                if (AI_Timer % 3 == 0)
                {
                    if (NPC.Center.Y < player.Center.Y)
                    {
                        int proj;
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(tempx, -8), Mod.Find<ModProjectile>("IceBolt").Type, 0, 0, Main.myPlayer, 1);
                        Main.projectile[proj].timeLeft = 600;
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-tempx, -8), Mod.Find<ModProjectile>("IceBolt").Type, 0, 0, Main.myPlayer, 1);
                        Main.projectile[proj].timeLeft = 600;
                    }
                    tempx -= 0.21f;
                }
            }

            if (AI_Timer % 180 == 140)
            {
                NPC.velocity = Vector2.Zero;
            }



            AI_Timer++;
            if (AI_Timer == 180)
            {
                AI_State = (float)ActionState.CirclePlayer1;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
                targetPos = Main.player[NPC.target].Center;

                RotateMagnitude = 1000;
                RotateDir = MathHelper.ToDegrees((NPC.Center - Main.player[NPC.target].Center).ToRotation());
            }
        }
        private void DashUp2()
        {
            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 120;
                }
            }
            else
            {
                NPC.damage = 60;
            }
            Player player = Main.player[NPC.target];

            if (AI_Timer % 420 == 0)
            {
                tempx = 3.8f;
                Vector2 oldPos = NPC.position;
                NPC.position = player.Center + new Vector2(Main.rand.NextFloat(-30, 30), 1000) + new Vector2(NPC.width / 2, NPC.height / 2);
                NPC.velocity = Vector2.Zero;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.type == ModContent.NPCType<FrigidiusBody>() || npc.type == ModContent.NPCType<FrigidiusTail>())
                    {
                        npc.position += oldPos - NPC.position;
                    }


                }

                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("IceSpiritTelegraph").Type, 0, 0, Main.myPlayer);
                Main.projectile[proj].velocity = new Vector2(0, -20f);
                storedVel = new Vector2(0, -35f);
            }

            if (AI_Timer % 420 == 60)
            {
                SoundEngine.PlaySound(SoundID.Roar);

                NPC.velocity = storedVel;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (AI_Timer % 420 == 90)
            {
                int proj;
                SoundEngine.PlaySound(SoundID.Item28);

                for (var i = 0; i < 16; i++)
                {
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, Main.rand.NextFloat(-8, -11)).RotatedByRandom(MathHelper.ToRadians(36)), Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer, ai1: 0.15f, ai2: 9f);
                }
                for (var i = 0; i < 25; i++)
                {
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, Main.rand.NextFloat(-11, -14)).RotatedByRandom(MathHelper.ToRadians(36)), Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer, ai1: 0.15f, ai2: 9f);
                }
                for (var i = 0; i < 36; i++)
                {
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, Main.rand.NextFloat(-14, -17)).RotatedByRandom(MathHelper.ToRadians(36)), Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer, ai1: 0.15f, ai2: 9f);
                }
                for (var i = 0; i < 49; i++)
                {
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, Main.rand.NextFloat(-17, -20)).RotatedByRandom(MathHelper.ToRadians(36)), Mod.Find<ModProjectile>("IceShardFrigidius").Type, 17, 0, Main.myPlayer, ai1: 0.15f, ai2: 9f);
                }
            }

            if (AI_Timer % 420 == 140)
            {
                NPC.velocity = Vector2.Zero;
            }



            AI_Timer++;
            if (AI_Timer == 420)
            {
                AI_State = (float)ActionState.ChasePlayer2;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
            }
        }

        private void Despawn()
        {
            if (Main.expertMode)
            {
                NPC.damage = 60;

                if (Main.masterMode)
                {
                    NPC.damage = 80;
                }
            }
            else
            {
                NPC.damage = 40;
            }
            Player player = Main.player[NPC.target];


            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity -= direction * 0.0002f;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

        }
    }

	internal class FrigidiusBody : WormBody
	{
		public override void SetStaticDefaults() {
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;

        }

        public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.damage = 25; // The amount of damage that this npc deals
            NPC.defense = 15; // The amount of defense that this npc has
        }

        public override void AI()
        {
            DrawOffsetY = 16;
            if (NPC.HasBuff(BuffID.OnFire))
            {
                NPC.DelBuff(NPC.FindBuffIndex(BuffID.OnFire));
            }
            if (NPC.HasBuff(BuffID.Frostburn))
            {
                NPC.DelBuff(NPC.FindBuffIndex(BuffID.Frostburn));
            }
            base.AI();
        }

        public override void Init() {
			FrigidiusHead.CommonWormInit(this);
		}
	}

	internal class FrigidiusTail : WormTail
	{
		public override void SetStaticDefaults() {
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
		}

		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.damage = 25; // The amount of damage that this npc deals
            NPC.defense = 15; // The amount of defense that this npc has
        }

        public override void AI()
        {
            DrawOffsetY = 16;
            base.AI();
        }
        public override void Init() {
			FrigidiusHead.CommonWormInit(this);
		}
	}
}
