using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;
using AwfulGarbageMod;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.BossBars;
using AwfulGarbageMod.Systems;
using System.Runtime.InteropServices;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Consumables;
using AwfulGarbageMod.Items.Weapons.Rogue;
using AwfulGarbageMod.Configs;

namespace AwfulGarbageMod.NPCs.Boss
{
    // This ModNPC serves as an example of a completely custom AI.
    [AutoloadBossHead]
    public class SeseKitsugai : ModNPC
	{
		// Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private enum ActionState
		{
            PhaseTransition, 

			FindTarget,
			PositionAbovePlayer1,
			ThrowBones,
			GravityBullets,

            CarcassCadaver,
            PositionAbovePlayer2,
            ThatOneNonspell,
            PositionAbovePlayer3,
            BoneWalls,

            FinalAtk
        }

		// Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
		// These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
		private enum Frame
		{
			Right2,
            Right1,
            Idle1,
            Left1,
            Left2,
            Idle2,
            Idle3,
            Idle4
		}

		// These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
		// Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
		// This is all to just make beautiful, manageable, and clean code.
		float AI_State;
		float AI_Timer;
		float BulletXvel;
        float bossPhase;
        float bulletDirection;
        float wallX1;
        float wallX2;
        int idleAnim;
        int animCounter;
		Vector2 targetArea;
		Vector2 direction;


        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Flutter Slime"); // Automatic from localization files
			Main.npcFrameCount[NPC.type] = 8; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Wet] = true;

        }

        public override void SetDefaults()
		{
			NPC.width = 40; // The width of the npc's hitbox (in pixels)
			NPC.height = 88; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 20; // The amount of damage that this npc deals
			NPC.defense = 8; // The amount of defense that this npc has
			NPC.lifeMax = 6000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit2; // The sound the NPC will make when being hit.
			NPC.DeathSound = SoundID.NPCDeath1; // The sound the NPC will make when it dies.
			NPC.value = 50000f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.boss = true;
			NPC.npcSlots = 100f;
			NPC.noTileCollide = true;


            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SeseTheme");
            }
            NPC.BossBar = ModContent.GetInstance<TreeToadBossBar>();

        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Graveyard,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.SeseKitsugai")
            });
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
			NPC.lifeMax = 8500;
            if (Main.masterMode)
            {
                NPC.lifeMax = 10000; // Increase by 5 if expert or master mode
                if (Main.getGoodWorld || Main.zenithWorld)
                {
                    NPC.lifeMax = 13000;
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {

            NPC.lifeMax = NPC.lifeMax * ModContent.GetInstance<Config>().BossHealthMultiplier / 100;
            NPC.life = NPC.lifeMax;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Tools.Fraxeture>(), 4));


            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SeseKitsugaiBag>()));

            // Trophies are spawned with 1/10 chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Boss.SeseKitsugaiTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Boss.SeseKitsugaiRelic>()));
            
            if (ModLoader.TryGetMod("Gensokyo", out Mod gensokyo))
            {
                if (gensokyo.TryFind("PointItem", out ModItem pointItem))
                {
                    npcLoot.Add(ItemDropRule.Common(pointItem.Type, 1, 15, 24));
                }
                if (gensokyo.TryFind("PowerItem", out ModItem powerItem))
                {
                    npcLoot.Add(ItemDropRule.Common(powerItem.Type, 1, 12, 18));
                }
            }
            
            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            ///npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
            {
                notExpertRule.OnSuccess(ItemDropRule.FewFromOptions(1, 1, ModContent.ItemType<Backbone>(), ModContent.ItemType<BoneSkewer>(), ModContent.ItemType<ToothpickTome>(), ModContent.ItemType<SpineString>(), ModContent.ItemType<PileOfActualBones>(), ModContent.ItemType<Skeletoss>()));

            }
            else
            {
                notExpertRule.OnSuccess(ItemDropRule.FewFromOptions(1, 1, ModContent.ItemType<Backbone>(), ModContent.ItemType<BoneSkewer>(), ModContent.ItemType<ToothpickTome>(), ModContent.ItemType<SpineString>(), ModContent.ItemType<PileOfActualBones>()));
            }


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


        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedSeseKitsugai, -1);

            // Since this hook is only ran in singleplayer and serverside, we would have to sync it manually.
            // Thankfully, vanilla sends the MessageID.WorldData packet if a BOSS was killed automatically, shortly after this hook is ran

            // If your NPC is not a boss and you need to sync the world (which includes ModSystem, check DownedBossSystem), use this code:
            /*
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			*/
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI() {
            // The npc starts in the asleep state, waiting for a player to enter range

            Player player = Main.player[NPC.target];

            if (player.dead)
            {
				NPC.position.Y += 999;
                NPC.EncourageDespawn(0);
				return;
            }

            if (bossPhase == 0)
            {
                NPC.TargetClosest(true);
                bossPhase = 1;
                AI_Timer = 120;
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
                AI_State = (float)ActionState.PhaseTransition;
            }

            if (NPC.life < NPC.lifeMax * 3 / 5 && bossPhase == 1)
            {
                bossPhase = 2;
                AI_Timer = 300;
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
                AI_State = (float)ActionState.PhaseTransition;
            }
            if (NPC.life < NPC.lifeMax * 1 / 5 && bossPhase == 2)
            {
                bossPhase = 3;
                AI_Timer = 300;
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
                AI_State = (float)ActionState.PhaseTransition;
            }

            switch (AI_State) {
                case (float)ActionState.PhaseTransition:
                    PhaseTransition();
                    break;
                case (float)ActionState.FindTarget:
                    FindTarget();
                    break;
                case (float)ActionState.PositionAbovePlayer1:
					PositionAbovePlayer1();
					break;
				case (float)ActionState.ThrowBones:
					ThrowBones();
					break;
				case (float)ActionState.GravityBullets:
					GravityBullets();
					break;
                case (float)ActionState.CarcassCadaver:
                    CarcassCadaver();
                    break;
                case (float)ActionState.PositionAbovePlayer2:
                    PositionAbovePlayer2();
                    break;
                case (float)ActionState.ThatOneNonspell:
                    ThatOneNonspell();
                    break;
                case (float)ActionState.PositionAbovePlayer3:
                    PositionAbovePlayer3();
                    break;
                case (float)ActionState.BoneWalls:
                    BoneWalls();
                    break;
                case (float)ActionState.FinalAtk:
                    FinalAtk();
                    break;
            }
		}

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight) {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = 0;

            if (NPC.velocity.X > 3)
            {
                NPC.frame.Y = (int)Frame.Right2 * frameHeight;
                idleAnim = 0;
                animCounter = -30;
            }
            else if (NPC.velocity.X < -3)
            {
                NPC.frame.Y = (int)Frame.Left2 * frameHeight;
                idleAnim = 0;
                animCounter = -30;
            }
            else if (NPC.velocity.X > 0.4f)
            {
                NPC.frame.Y = (int)Frame.Right1 * frameHeight;
                idleAnim = 0;
                animCounter = -30;
            }
            else if (NPC.velocity.X < -0.4f)
            {
                NPC.frame.Y = (int)Frame.Left1 * frameHeight;
                idleAnim = 0;
                animCounter = -30;
            }
            else
            {
                animCounter++;
                if (animCounter >= 15)
                {
                    animCounter = 0;
                    idleAnim++;
                }

                if (animCounter < 0)
                {
                    NPC.frame.Y = (int)Frame.Idle1 * frameHeight;
                }
                else
                {
                    switch (idleAnim % 4)
                    {
                        case 0:
                            NPC.frame.Y = (int)Frame.Idle2 * frameHeight;
                            break;
                        case 1:
                            NPC.frame.Y = (int)Frame.Idle3 * frameHeight;
                            break;
                        case 2:
                            NPC.frame.Y = (int)Frame.Idle4 * frameHeight;
                            break;
                        case 3:
                            NPC.frame.Y = (int)Frame.Idle3 * frameHeight;
                            break;
                    }
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"Sese_Gore_Bone").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"Sese_Gore_Bone").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"Sese_Gore_Bone").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"Sese_Gore_Head").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"Sese_Gore_Ribcage").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"Sese_Gore_Ribcage").Type);


            }
        }

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms() {
			return true;
			// You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
		}

		private void FindTarget()
		{
            NPC.TargetClosest(true);
            targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
            AI_State = (float)ActionState.PositionAbovePlayer1;
			AI_Timer = 75;
        }

        private void PhaseTransition()
        {

            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;

            float inertia = 12f;


            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer -= 1;
            if (AI_Timer == 0)
            {
                if (bossPhase == 1)
                {
                    AI_State = (float)ActionState.ThrowBones;
                    AI_Timer = 600;
                }
                if (bossPhase == 2)
                {
                    AI_State = (float)ActionState.CarcassCadaver;
                    AI_Timer = 420;
                }
                if (bossPhase == 3)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/haniwa_00")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound);
                    AI_State = (float)ActionState.FinalAtk;
                    AI_Timer = 0;
                    wallX1 = Main.player[NPC.target].Center.X + 600;
                    wallX2 = Main.player[NPC.target].Center.X - 600;
                }
            }

        }

        private void PositionAbovePlayer1() 
		{
            if (AI_Timer % 25 == 0)
            {
                float bulletSpd = 2f;
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                for (var i = 0; i < 9; i++)
                {
                    if (Main.expertMode)
                    {
                        for (var j = -1; j < 2; j++)
                        {
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(BulletXvel, -9), Mod.Find<ModProjectile>("SeseNonGravProj").Type, 17, 0, Main.myPlayer);
                            Main.projectile[proj].velocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * bulletSpd).RotatedBy(MathHelper.ToRadians(24f * j));

                        }
                    }
                    else
                    {
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(BulletXvel, -9), Mod.Find<ModProjectile>("SeseNonGravProj").Type, 17, 0, Main.myPlayer);
                        Main.projectile[proj].velocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * bulletSpd);
                    }
                    bulletSpd += 1f;
                }
            }

            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;
            
            float inertia = 12f;


            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

			AI_Timer -= 1;
			if (AI_Timer == 0)
			{
                AI_State = (float)ActionState.ThrowBones;
				AI_Timer = 600;
            }

        }

        private void ThrowBones()
        {
            float inertia = 12f;
            NPC.velocity = (NPC.velocity * (inertia - 1)) / inertia;

            int timer;
            if (Main.expertMode)
            {
                if (Main.masterMode)
                {
                    timer = 20;
                }
                else
                {
                    timer = 25;
                }
            }
            else
            {
                timer = 30;
            }

            if (AI_Timer % timer == 0)
			{
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-5.5f, 5.5f), -10), Mod.Find<ModProjectile>("SeseBoneProj").Type, 17, 0, Main.myPlayer, (Main.rand.Next(0, 2) - 0.5f) * 3.6f);
            }

            AI_Timer -= 1;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.GravityBullets;
                AI_Timer = 270;
				BulletXvel = 15f;
            }
        }

        private void GravityBullets() {
            if (AI_Timer > 150)
            {
                if (AI_Timer % 3 == 0)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_00")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(BulletXvel, -9), Mod.Find<ModProjectile>("SeseBulletProj").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(BulletXvel * -1, -9), Mod.Find<ModProjectile>("SeseBulletProj").Type, 17, 0, Main.myPlayer);

                    BulletXvel += 15f * -3f / 120f;
                }
            }
            AI_Timer -= 1;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.FindTarget;
            }
        }

        private void CarcassCadaver()
        {
            float inertia = 12f;
            NPC.velocity = (NPC.velocity * (inertia - 1)) / inertia;


            if (AI_Timer == 420)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("CarcassCadaverProj").Type, 17, 0, Main.myPlayer, 0.04f);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("CarcassCadaverProj").Type, 17, 0, Main.myPlayer, -0.04f);
            }

            AI_Timer -= 1;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.PositionAbovePlayer2;
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -540);
                AI_Timer = 180;
            }
        }

        private void PositionAbovePlayer2()
        {

            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;

            float inertia = 12f;


            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer -= 1;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.ThatOneNonspell;
                bulletDirection = 0;
                AI_Timer = 450;
            }

        }

        private void ThatOneNonspell()
        {
            float inertia = 12f;
            NPC.velocity = (NPC.velocity * (inertia - 1)) / inertia;

            int timer;
            if (Main.expertMode)
            {
                timer = 45;
            }
            else
            {
                timer = 60;
            }

            if (AI_Timer % timer == 0)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                for (var i = 0; i < 7; i++)
                {
                    for (var j = 0; j < 7; j++)
                    {
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0f, 232f).RotatedBy(MathHelper.ToRadians(60f * i - bulletDirection)), new Vector2(0, 0), ModContent.ProjectileType<SeseNonGravProj>(), 17, 0, Main.myPlayer);
                        Main.projectile[proj].velocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 4.5f).RotatedBy(MathHelper.ToRadians(60f * j + bulletDirection));

                    }
                }
                bulletDirection += 18;
            }

            AI_Timer -= 1;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.PositionAbovePlayer3;
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
                AI_Timer = 180;
            }
        }
        
        private void PositionAbovePlayer3()
        {

            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;

            float inertia = 12f;


            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer -= 1;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.BoneWalls;
                AI_Timer = 0;
            }

        }
        
        private void BoneWalls()
        {
            float inertia = 12f;
            NPC.velocity = (NPC.velocity * (inertia - 1)) / inertia;

            int timer;
            if (Main.expertMode)
            {
                if (Main.masterMode)
                {
                    timer = 120;
                }
                else
                {
                    timer = 140;
                }
            }
            else 
            {
                timer = 160;
            }

            if (AI_Timer < timer * 2)
            {
                if (AI_Timer % timer == 0)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound);
                    for (var j = -6; j < 7; j++)
                    {
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(j * 300, 0), new Vector2(0, -10), ModContent.ProjectileType<SeseBoneProj>(), 17, 0, Main.myPlayer, 1.8f);
                    }
                }
                if (AI_Timer % timer == timer / 2)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound);
                    for (var j = -5; j < 6; j++)
                    {
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(j * 300, 0), new Vector2(0, -10), ModContent.ProjectileType<SeseBoneProj>(), 17, 0, Main.myPlayer, -1.8f);
                    }
                }
            }
            AI_Timer ++;
            if (AI_Timer == timer * 3)
            {
                AI_State = (float)ActionState.CarcassCadaver;
                AI_Timer = 420;
            }
        }

        private void FinalAtk()
        {
            targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;

            float inertia = 12f;


            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX1, Main.player[NPC.target].Center.Y), new Vector2(0, 0), ModContent.ProjectileType<SeseWallProj>(), 17, 0, Main.myPlayer);
            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX2, Main.player[NPC.target].Center.Y), new Vector2(0, 0), ModContent.ProjectileType<SeseWallProj>(), 17, 0, Main.myPlayer);

            if (Main.player[NPC.target].Center.X > wallX1)
            {
                Main.player[NPC.target].position.X = wallX1 - (Main.player[NPC.target].Center.X - Main.player[NPC.target].position.X);
            }
            if (Main.player[NPC.target].Center.X < wallX2)
            {
                Main.player[NPC.target].position.X = wallX2 - (Main.player[NPC.target].Center.X - Main.player[NPC.target].position.X);

            }


            if (AI_Timer % 480 == 120 || AI_Timer % 480 == 360)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                for (var j = -15; j < 16; j++)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX1 - 600 + j * 15, NPC.Center.Y), new Vector2(0, -6), ModContent.ProjectileType<SeseBulletProj>(), 17, 0, Main.myPlayer);
                }
            }
            if (AI_Timer % 480 == 60 || AI_Timer % 480 == 420)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                for (var j = -12; j < 13; j++)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX1 - 1020 + j * 15, NPC.Center.Y), new Vector2(0, -6), ModContent.ProjectileType<SeseBulletProj>(), 17, 0, Main.myPlayer);
                }
            }
            if (AI_Timer % 480 == 180 || AI_Timer % 480 == 300)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                for (var j = -12; j < 13; j++)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX1 - 180 + j * 15, NPC.Center.Y), new Vector2(0, -6), ModContent.ProjectileType<SeseBulletProj>(), 17, 0, Main.myPlayer);
                }
            }
            if (Main.expertMode)
            {
                if (AI_Timer % 240 == 0 || AI_Timer % 240 == 15 || AI_Timer % 240 == 225)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound);
                    for (var j = -8; j < 9; j++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedBy(MathHelper.ToRadians(j * 22.5f)), ModContent.ProjectileType<SeseSpinProj>(), 17, 0, Main.myPlayer, 1.5f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedBy(MathHelper.ToRadians(j * 22.5f)), ModContent.ProjectileType<SeseSpinProj>(), 17, 0, Main.myPlayer, -1.5f);

                    }
                }
            }
            else
            {
                if (AI_Timer % 240 == 0)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound);
                    for (var j = -8; j < 9; j++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedBy(MathHelper.ToRadians(j * 22.5f)), ModContent.ProjectileType<SeseSpinProj>(), 17, 0, Main.myPlayer, 1.5f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedBy(MathHelper.ToRadians(j * 22.5f)), ModContent.ProjectileType<SeseSpinProj>(), 17, 0, Main.myPlayer, -1.5f);

                    }
                }
            }
            AI_Timer++;
        }
    }
}