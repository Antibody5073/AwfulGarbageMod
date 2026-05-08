using AwfulGarbageMod;
using AwfulGarbageMod.BossBars;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.ItemDropRules;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories.Hardmode;
using AwfulGarbageMod.Items.Consumables;
using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Melee.Hardmode;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Ranged.Hardmode;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Items.Weapons.Summon.Hardmode;
using AwfulGarbageMod.NPCs.BossUnrealRework.EyeOfCthulhu;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Peripherals.RGB;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.NPCs.Boss
{
    // This ModNPC serves as an example of a completely custom AI.
    [AutoloadBossHead]
    public class Blightdrasil : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            Spawn,
            
            SideLeaves,
            WorldFeeders,
            LeavesTowardPlayer,
            Vilethorn,
            IchorRain,
            Donuts,
            RottedForks,

            LeafCircle,
            WorldFeeders2,
            ShadowOrbGrav,
            EaterOfSouls,
            RottedForks2,
            ClingerSpin,
            Donuts2,
            Crimeras,
            CursedFireballBounce,
            IchorRain2,

            PhaseTransition
        }

        // Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
        // These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
        private enum Frame
        {
            Spin1,
            Spin2,
            Spin3,
            Spin4,
            Spin5
        }

        // These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        float AI_State;
        float Next_State;
        float AI_Timer;
        float bossPhase;
        float spread;
        float atklen2;
        float atkUseCounter;
        int frame;
        int frameCounter;
        float rand; 
        float spinSpd;
        bool atkFromLeft;
        bool didTheAttackAlready;
        Vector2 targetArea;
        Vector2 direction;
        Vector2 storedVel;
        Vector2 storedPos;
        Vector2 recoil;
        List<Vector2> playerPos = new List<Vector2> { };

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flutter Slime"); // Automatic from localization files
            Main.npcFrameCount[NPC.type] = 1; // make sure to set this for your modnpcs.
                                              // Specify the debuffs it is immune to

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 216; // The width of the npc's hitbox (in pixels)
            NPC.height = 216; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 0; // The amount of damage that this npc deals
            NPC.defense = 50; // The amount of defense that this npc has
            NPC.lifeMax = 50000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit3; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath52; // The sound the NPC will make when it dies.
            NPC.value = 500000; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.npcSlots = 100f;
            NPC.noTileCollide = true;
            NPC.hide = false;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/EvilTreeTheme");
            }
            NPC.BossBar = ModContent.GetInstance<TreeToadBossBar>();

        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.Blightdrasil")
            });
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            if (!Main.GameModeInfo.IsJourneyMode)
            {
                NPC.lifeMax = 700000;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 90000; // Increase by 5 if expert or master mode
                    if (Main.getGoodWorld || Main.zenithWorld)
                    {
                        NPC.lifeMax = 110000;
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BlightdrasilBag>()));


            // Trophies are spawned with 1/10 chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormRelic>()));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            ///npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<HeartOfBlight>(), 1));
            notExpertRule.OnSuccess(ItemDropRule.FewFromOptions(1, 3, ModContent.ItemType<Toxichorp>(), ModContent.ItemType<CursedFlametongue>(), ModContent.ItemType<BlightedBranch>(), ModContent.ItemType<RottenChainfork>()));
            npcLoot.Add(ItemDropRule.ByCondition(new UnrealCondition(), ModContent.ItemType<AntediluvianArtifact>(), 1, 1, 1));

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
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (!Main.GameModeInfo.IsJourneyMode)
            {
                NPC.lifeMax = 70000;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 90000; // Increase by 5 if expert or master mode
                    if (Main.getGoodWorld || Main.zenithWorld)
                    {
                        NPC.lifeMax = 110000;
                    }
                }
            }
            NPC.lifeMax = NPC.lifeMax * ModContent.GetInstance<Config>().BossHealthMultiplier / 100;
            NPC.life = NPC.lifeMax;
            NPC.alpha = 255;
        }

        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night

           

            NPC.SetEventFlagCleared(ref DownedBossSystem.downedEvilTree, -1);
            NPC.globalEnemyBossInfo().killOrbitals = true;


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
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (ProjectileID.Sets.MinionShot[projectile.type] || projectile.sentry || projectile.minion)
            {
                modifiers.SourceDamage *= 0.7f;
            }
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }
        public override void AI()
        {
            // The npc starts in the asleep state, waiting for a player to enter range

            Player player = Main.player[NPC.target];

            Lighting.AddLight(NPC.Center, Color.Orange.ToVector3() * 2.5f);

            NPC.DiscourageDespawn(10);
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
                AI_Timer = 0;
                targetArea = Main.rand.NextVector2CircularEdge(300, 300);
                AI_State = (float)ActionState.Spawn;
            }
            if (NPC.life < NPC.lifeMax * 7 / 10 && bossPhase == 1)
            {
                NPC.TargetClosest(true);
                NPC.globalEnemyBossInfo().killOrbitals = true;

                bossPhase = 2;
                AI_Timer = -60;
                SoundEngine.PlaySound(SoundID.Item107);

                for (int i = 0; i < 60; i++) {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CursedTorch);
                    dust.velocity = Main.rand.NextVector2Circular(8, 8);
                    dust.scale = Main.rand.NextFloat(0.5f, 2f);

                    dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Ichor);
                    dust.velocity = Main.rand.NextVector2Circular(8, 8);
                    dust.scale = Main.rand.NextFloat(0.5f, 2f);
                }

                AI_State = (float)ActionState.ShadowOrbGrav;
            }
            if (NPC.life < NPC.lifeMax * 3 / 10 && bossPhase == 2)
            {
                NPC.TargetClosest(true);
                NPC.globalEnemyBossInfo().killOrbitals = true;

                bossPhase = 3;
                AI_Timer = -60;
                SoundEngine.PlaySound(SoundID.Item107);

                for (int i = 0; i < 60; i++) {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.CursedTorch);
                    dust.velocity = Main.rand.NextVector2Circular(8, 8);
                    dust.scale = Main.rand.NextFloat(0.5f, 2f);

                    dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Ichor);
                    dust.velocity = Main.rand.NextVector2Circular(8, 8);
                    dust.scale = Main.rand.NextFloat(0.5f, 2f);
                }
                AI_State = (float)ActionState.PhaseTransition;
                NPC.dontTakeDamage = true;
                NPC.hide = true;


                int type1 = ModContent.ProjectileType<BlightdrasilEaterSpawner>();
                int type2 = ModContent.ProjectileType<BlightdrasilRottedFork>();
                foreach (var item in Main.projectile)
                {
                    if (item.type == type1 || item.type == type2)
                    {
                        item.active = false;
                    }
                }
            }

            switch (AI_State)
            {
                case (float)ActionState.Spawn:
                    Spawn();
                    break;
                case (float)ActionState.SideLeaves:
                    SideLeaves();
                    break;
                case (float)ActionState.WorldFeeders:
                    WorldFeeders();
                    break;
                case (float)ActionState.LeavesTowardPlayer:
                    LeavesTowardPlayer();
                    break;
                case (float)ActionState.Vilethorn:
                    Vilethorn();
                    break;
                case (float)ActionState.IchorRain:
                    IchorRain();
                    break;
                case (float)ActionState.Donuts:
                    Donuts();
                    break;
                case (float)ActionState.RottedForks:
                    RottedForks();
                    break;
                case (float)ActionState.LeafCircle:
                    LeafCircle();
                    break;
                case (float)ActionState.WorldFeeders2:
                    WorldFeeders2();
                    break;
                case (float)ActionState.ShadowOrbGrav:
                    ShadowOrbGrav();
                    break;
                case (float)ActionState.Donuts2:
                    Donuts2();
                    break;
                case (float)ActionState.CursedFireballBounce:
                    CursedFireballBounce();
                    break;
                case (float)ActionState.IchorRain2:
                    IchorRain2();
                    break;
                case (float)ActionState.RottedForks2:
                    RottedForks2();
                    break;
                case (float)ActionState.ClingerSpin:
                    ClingerSpin();
                    break;
                case (float)ActionState.EaterOfSouls:
                    EaterOfSouls();
                    break;
                case (float)ActionState.Crimeras:
                    Crimeras();
                    break;
                case (float)ActionState.PhaseTransition:
                    PhaseTransition();
                    break;
            }
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = NPC.direction;
            frameCounter++;
            if (frameCounter > 5)
            {
                frameCounter = 0;
                frame++;
                if (frame >= Main.npcFrameCount[NPC.type])
                {
                    frame = 0;
                }
            }

            NPC.frame.Y = frame * frameHeight;
        }

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
        }

        private void Spawn()
        {
            AI_Timer += 1;
            NPC.position.Y -= 1;
            NPC.alpha = (int)MathHelper.Lerp(255, 0, AI_Timer / 60);
            if (AI_Timer == 60)
            {
                AI_State = (float)ActionState.LeavesTowardPlayer;

                AI_Timer = 0;
                atkUseCounter = 0;
                NPC.noTileCollide = false;
            }
        }

        private void SideLeaves()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer > 0)
            {
                if (AI_Timer % 20 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1);

                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(15, 0), ModContent.ProjectileType<BlightdrasilProjLeaf>(), 26, 0, 0, Main.player[NPC.target].whoAmI);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-15, 0), ModContent.ProjectileType<BlightdrasilProjLeaf>(), 26, 0, 0, Main.player[NPC.target].whoAmI);
                    if (Main.expertMode)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -15), ModContent.ProjectileType<BlightdrasilProjLeaf>(), 26, 0, 0, Main.player[NPC.target].whoAmI);
                    }
                }
            }
            if (AI_Timer == 450)
            {

                AI_State = (float)ActionState.WorldFeeders;
                AI_Timer = -60;
                atkUseCounter = 0;
            }
        }
        private void WorldFeeders()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer > 0)
            {
                if (AI_Timer % 60 == 0)
                {
                    NPC npC = NPC.NewNPCDirect(NPC.GetSource_ReleaseEntity(), NPC.Bottom, 98);
                    npC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 14f;
                    npC.lifeMax = 900;
                    npC.life = npC.lifeMax;
                }
            }

            if (AI_Timer == 210)
            {

                AI_State = (float)ActionState.LeavesTowardPlayer;
                AI_Timer = -150;
                atkUseCounter = 0;
            }
        }
        private void LeavesTowardPlayer()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;

            if (AI_Timer == -60)
            {
                if (Main.expertMode)
                {
                    for (int i = -6; i < 7; i++)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 28).RotatedBy(MathHelper.ToRadians(i * 15)), ModContent.ProjectileType<BlightdrasilVilethornBase>(), 30, 0, -1, 0, 0);
                    }
                }
            }
            if (AI_Timer > 0)
            {
                if (AI_Timer % 8 == 0)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10f, ModContent.ProjectileType<BlightdrasilProjLeaf>(), 26, 0, -1, Main.player[NPC.target].whoAmI);
                    SoundEngine.PlaySound(SoundID.Item1);
                }
            }
            if (AI_Timer == 300)
            {

                AI_State = (float)ActionState.Vilethorn;
                AI_Timer = -90;
                atkUseCounter = 0;
            }
        }
        private void Vilethorn()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 0)
            {
                for (int i = -1; i < 2; i++)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, (Main.player[NPC.target].Center - NPC.Bottom).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(i * 6)) * 28f, ModContent.ProjectileType<BlightdrasilVilethornBase>(), 30, 0, -1, 0, 0);
                }
            }
            if (AI_Timer == 60)
            {
                for (int i = -3; i < 4; i++)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, (Main.player[NPC.target].Center - NPC.Bottom).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(i * 13.5f)) * 28f, ModContent.ProjectileType<BlightdrasilVilethornBase>(), 30, 0, -1, 0, 0);
                }
            }
            if (AI_Timer == 120)
            {
                for (int i = -4; i < 5; i++)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, (Main.player[NPC.target].Center - NPC.Bottom).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(i * 5)) * 28f, ModContent.ProjectileType<BlightdrasilVilethornBase>(), 30, 0, -1, 0, 0);
                }
            }
            if (AI_Timer == 180)
            {
                for (int i = -5; i < 6; i++)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, (Main.player[NPC.target].Center - NPC.Bottom).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(i * 24)) * 28f, ModContent.ProjectileType<BlightdrasilVilethornBase>(), 30, 0, -1, 0, 0);
                }
                for (int i = 0; i < 3; i++)
                {
                    NPC npC = NPC.NewNPCDirect(NPC.GetSource_ReleaseEntity(), NPC.Bottom, NPCID.Corruptor);
                    npC.velocity = Main.rand.NextVector2Circular(7, 7);
                    npC.lifeMax = 400;
                    npC.life = npC.lifeMax;
                }
            }
            if (AI_Timer > 210)
            {
                if (Main.expertMode)
                {
                    if (AI_Timer % 20 == 0)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -15).RotatedBy(MathHelper.ToRadians(7.5f * i)), ModContent.ProjectileType<BlightdrasilProjLeaf>(), 24, 0, 0, Main.player[NPC.target].whoAmI);
                        }
                    }
                }
            }
            if (AI_Timer == 300)
            {

                AI_State = (float)ActionState.IchorRain;
                AI_Timer = -150;
                atkUseCounter = 3;
            }
        }
        private void IchorRain()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer % 2 == 0)
            {
                if (AI_Timer > 90)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-atkUseCounter / 2, atkUseCounter / 2), -atkUseCounter), ModContent.ProjectileType<BlightdrasilIchor>(), 30, 0, -1, 0, 0);
                }
                else
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-atkUseCounter / 2, atkUseCounter / 2), -atkUseCounter), ModContent.ProjectileType<BlightdrasilIchor>(), 30, 0, -1, 0, 0);
                }
            }
            if (AI_Timer % 15 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item13);
            }
            if (AI_Timer > 90)
            {
                atkUseCounter += 0.04f;
                if (AI_Timer % 60 == 0)
                {

                    NPC npC = NPC.NewNPCDirect(NPC.GetSource_ReleaseEntity(), NPC.Bottom, NPCID.FloatyGross);
                    npC.velocity = Main.rand.NextVector2Circular(3, 2) + new Vector2(0, -4);
                    npC.lifeMax = 600;
                    npC.life = npC.lifeMax;
                    npC.knockBackResist = 0;
                }
            }
            if (AI_Timer == 300)
            {
                AI_State = (float)ActionState.Donuts;
                AI_Timer = -120;
                atkUseCounter = 0;
            }
        }
        private void Donuts()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 0)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath13);

                if (player.Center.X < NPC.Center.X)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-8, 0), ModContent.ProjectileType<BlightdrasilHeart>(), 32, 0, -1, 0, 0);
                }
                else
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(8, 0), ModContent.ProjectileType<BlightdrasilHeart>(), 32, 0, -1, 0, 0);
                }
            }
            if (AI_Timer == 195)
            {
                AI_State = (float)ActionState.RottedForks;
                AI_Timer = -90;
                atkUseCounter = 0;
            }
        }
        private void RottedForks()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 0)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_3")
                {
                    Volume = 0.4f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -7.5f), ModContent.ProjectileType<BlightdrasilRottedFork>(), 40, 0, -1, 0, 0);
            }
            if (AI_Timer == 90)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_3")
                {
                    Volume = 0.4f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(7.5f, 0), ModContent.ProjectileType<BlightdrasilRottedFork>(), 40, 0, -1, 0, 0);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-7.5f, 0), ModContent.ProjectileType<BlightdrasilRottedFork>(), 40, 0, -1, 0, 0);
            }
            if (AI_Timer == 180)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_3")
                {
                    Volume = 0.4f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 7.5f), ModContent.ProjectileType<BlightdrasilRottedFork>(), 40, 0, -1, 0, 0);
            }


            if (AI_Timer == 240)
            {
                AI_State = (float)ActionState.SideLeaves;
                AI_Timer = -120;
                atkUseCounter = 0;
            }
        }

        private void LeafCircle()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer > 0)
            {
                if (AI_Timer % 90 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1);

                    for (int i = -15; i < 16; i++)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(i * 12f)) * 15f, ModContent.ProjectileType<BlightdrasilProjLeaf>(), 26, 0, -1, 0, 0);
                    }
                }
            }
            if (AI_Timer == 450)
            {

                AI_State = (float)ActionState.WorldFeeders2;
                AI_Timer = -150;
                atkUseCounter = -2;
            }
        }
        private void WorldFeeders2()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer > 0)
            {
                if (AI_Timer % 20 == 0)
                {
                    NPC npC = NPC.NewNPCDirect(NPC.GetSource_ReleaseEntity(), NPC.Bottom, 98);
                    npC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(atkUseCounter * 30)) * 14f;
                    npC.lifeMax = 900;
                    npC.life = npC.lifeMax;
                    atkUseCounter++;
                }
            }

            if (AI_Timer == 120)
            {

                AI_State = (float)ActionState.ShadowOrbGrav;
                AI_Timer = -120;
                atkUseCounter = 0;
            }
        }
        private void ShadowOrbGrav()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 0)
            {
                rand = Main.rand.NextFloat(0, 0.5f);
                atkUseCounter = -2;
            }
            if (AI_Timer > 0 && AI_Timer < 90)
            {
                if (atkUseCounter <= 2)
                {
                    if (AI_Timer % 15 == 0)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(rand + atkUseCounter, -8), ModContent.ProjectileType<BlightdrasilOrb>(), 40, 0, -1, 0.05f, 0);
                        atkUseCounter++;
                    }
                }
            }
            if (AI_Timer == 100)
            {
                atkUseCounter = 2;
                rand -= 0.5f;
            }
            if (AI_Timer > 120 && AI_Timer < 210)
            {

                if (atkUseCounter >= -2)
                {
                    if (AI_Timer % 15 == 0)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(rand + atkUseCounter, -8), ModContent.ProjectileType<BlightdrasilOrb>(), 40, 0, -1, 0.05f, 0);
                        atkUseCounter--;
                    }
                }
            }

            if (AI_Timer == 300)
            {
                for (int i = 0; i < 3; i++)
                {
                    NPC npC = NPC.NewNPCDirect(NPC.GetSource_ReleaseEntity(), NPC.Center, NPCID.IchorSticker);
                    npC.velocity = Main.rand.NextVector2Circular(4, 4);
                    npC.lifeMax = 300;
                    npC.life = npC.lifeMax;
                }
            }

            if (AI_Timer == 450)
            {

                AI_State = (float)ActionState.EaterOfSouls;
                AI_Timer = -30;
                atkUseCounter = 0;
            }
        }
        private void EaterOfSouls()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 0)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(player.Center.X, NPC.Center.Y - (64 + 16 * 60)), new Vector2(0, 64), ModContent.ProjectileType<BlightdrasilWarning>(), 0, 0, -1, 1, player.whoAmI);
                if (DifficultyModes.Difficulty > 0)
                {
                    atkFromLeft = Main.rand.NextBool();
                    if (atkFromLeft)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (16 * 60), player.Center.Y), new Vector2(64, 0), ModContent.ProjectileType<BlightdrasilWarning>(), 0, 0, -1, 0, player.whoAmI);
                    }
                    else
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X + (16 * 60), player.Center.Y), new Vector2(-64, 0), ModContent.ProjectileType<BlightdrasilWarning>(), 0, 0, -1, 0, player.whoAmI);
                    }
                }
            }
            if (AI_Timer == 60)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/hone_shot")
                {
                    Volume = 0.5f,
                };
                if (Main.expertMode)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(player.Center.X, NPC.Center.Y - (64 + 16 * 60)), Vector2.Zero, ModContent.ProjectileType<BlightdrasilEaterSpawner>(), 0, 0, -1, 0, 0, 5);
                }
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(player.Center.X, NPC.Center.Y - (64 + 16 * 60)), new Vector2(0, 12), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 0);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(player.Center.X, NPC.Center.Y - (64 + 16 * 60)), new Vector2(0, 15), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 0);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(player.Center.X, NPC.Center.Y - (64 + 16 * 60)), new Vector2(0, 18), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 0);
                if (DifficultyModes.Difficulty > 0)
                {
                    if (atkFromLeft)
                    {
                        if (Main.expertMode)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (16 * 60), player.Center.Y), Vector2.Zero, ModContent.ProjectileType<BlightdrasilEaterSpawner>(), 0, 0, -1, 1, 5, 0);
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (16 * 60), player.Center.Y), new Vector2(12, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (16 * 60), player.Center.Y), new Vector2(15, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (16 * 60), player.Center.Y), new Vector2(18, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                    }
                    else
                    {
                        if (Main.expertMode)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X + (16 * 60), player.Center.Y), Vector2.Zero, ModContent.ProjectileType<BlightdrasilEaterSpawner>(), 0, 0, -1, 1, -5, 0);
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X + (16 * 60), player.Center.Y), new Vector2(-12, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X + (16 * 60), player.Center.Y), new Vector2(-15, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X + (16 * 60), player.Center.Y), new Vector2(-18, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                    }
                }
            }

            if (AI_Timer == 75)
            {
                AI_State = (float)ActionState.RottedForks2;
                AI_Timer = -150;
                atkUseCounter = 0;
            }
        }
        private void RottedForks2()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 0)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_3")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 vel = Main.rand.NextVector2CircularEdge(5.5f * i + 2.75f, 5.5f * i + 2.75f);
                    for (int j = 0; j < 3; j++)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -64), vel.RotatedBy(MathHelper.TwoPi * j / 3), ModContent.ProjectileType<BlightdrasilRottedFork>(), 40, 0, -1, 600 + 100 * i, 0);
                    }
                }
            }

            if (AI_Timer == 90)
            {
                AI_State = (float)ActionState.ClingerSpin;
                AI_Timer = 0;
                atkUseCounter = 0;

                if (Main.rand.NextBool())
                {
                    spinSpd = 1f;
                }
                else
                {
                    spinSpd = -1f;
                }


                NPC.globalEnemyBossInfo().OrbitDirection = Main.rand.NextFloat(0, 360);
                NPC.globalEnemyBossInfo().OrbitDistance = 0;
                NPC.globalEnemyBossInfo().killOrbitals = false;
                NPC.globalEnemyBossInfo().orbitalsDealDamage = true;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BlightdrasilProjClinger>(), 45, 0, Main.myPlayer, NPC.whoAmI, 0);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BlightdrasilProjClinger>(), 45, 0, Main.myPlayer, NPC.whoAmI, 120);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BlightdrasilProjClinger>(), 45, 0, Main.myPlayer, NPC.whoAmI, 240);
            }
        }
        private void ClingerSpin()
        {
            Player player = Main.player[NPC.target];

            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;
            NPC.globalEnemyBossInfo().OrbitDistance += (800 - NPC.globalEnemyBossInfo().OrbitDistance) / 45;

            AI_Timer += 1;

            if (AI_Timer == 420)
            {
                AI_State = (float)ActionState.Donuts2;
                AI_Timer = -540;
                atkUseCounter = 0;
            }
        }
        private void Donuts2()
        {
            Player player = Main.player[NPC.target];
            NPC.globalEnemyBossInfo().OrbitDistance += (-NPC.globalEnemyBossInfo().OrbitDistance) / 45;

            if (NPC.globalEnemyBossInfo().OrbitDistance <= 5)
            {
                NPC.globalEnemyBossInfo().killOrbitals = true;
            }
            AI_Timer += 1;
            if (AI_Timer == 0)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath13);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(8, 0), ModContent.ProjectileType<BlightdrasilHeart>(), 32, 0, -1, 0, 0);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-8, 0), ModContent.ProjectileType<BlightdrasilHeart>(), 32, 0, -1, 0, 0);
            }
            if (AI_Timer == 205)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath13);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -6), ModContent.ProjectileType<BlightdrasilHeart>(), 32, 0, -1, 0, 0);
            }
            if (AI_Timer == 300)
            {
                AI_State = (float)ActionState.Crimeras;
                AI_Timer = -225;
                atkUseCounter = 0;
            }
        }
        private void Crimeras()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 0)
            {
                atkFromLeft = Main.rand.NextBool();
                if (atkFromLeft)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (16 * 60), player.Center.Y), new Vector2(64, 0), ModContent.ProjectileType<BlightdrasilWarning>(), 0, 0, -1, 0, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X + (16 * 60), player.Center.Y), new Vector2(-64, 0), ModContent.ProjectileType<BlightdrasilWarning>(), 0, 0, -1, 0, player.whoAmI);
                }
                if (DifficultyModes.Difficulty > 0)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(player.Center.X, NPC.Center.Y - (64 + 16 * 60)), new Vector2(0, 64), ModContent.ProjectileType<BlightdrasilWarning>(), 0, 0, -1, 1, player.whoAmI);
                }
            }
            if (AI_Timer == 60)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/hone_shot")
                {
                    Volume = 0.5f,
                };
                if (atkFromLeft)
                {
                    if (Main.expertMode)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (16 * 60), player.Center.Y), Vector2.Zero, ModContent.ProjectileType<BlightdrasilEaterSpawner>(), 0, 0, -1, 1, 5, 0);
                    }
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (16 * 60), player.Center.Y), new Vector2(12, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (16 * 60), player.Center.Y), new Vector2(15, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X - (16 * 60), player.Center.Y), new Vector2(18, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                }
                else
                {
                    if (Main.expertMode)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X + (16 * 60), player.Center.Y), Vector2.Zero, ModContent.ProjectileType<BlightdrasilEaterSpawner>(), 0, 0, -1, 1, -5, 0);
                    }
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X + (16 * 60), player.Center.Y), new Vector2(-12, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X + (16 * 60), player.Center.Y), new Vector2(-15, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X + (16 * 60), player.Center.Y), new Vector2(-18, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                }
                if (DifficultyModes.Difficulty > 0)
                {
                    if (Main.expertMode)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(player.Center.X, NPC.Center.Y - (64 + 16 * 60)), Vector2.Zero, ModContent.ProjectileType<BlightdrasilEaterSpawner>(), 0, 0, -1, 0, 0, 5);
                    }
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(player.Center.X, NPC.Center.Y - (64 + 16 * 60)), new Vector2(0, 12), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 0);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(player.Center.X, NPC.Center.Y - (64 + 16 * 60)), new Vector2(0, 15), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 0);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(player.Center.X, NPC.Center.Y - (64 + 16 * 60)), new Vector2(0, 18), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 0);
                }
                AI_State = (float)ActionState.CursedFireballBounce;
                AI_Timer = -15;
                atkUseCounter = 0;
            }
        }
        private void CursedFireballBounce()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 0)
            {
                rand = Main.rand.NextFloat(0, 0.5f);
                atkUseCounter = -2;
            }
            if (AI_Timer > 0 && AI_Timer < 450)
            {
                if (AI_Timer % 3 == 0)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(7, 0).RotatedByRandom(MathHelper.PiOver4 * 0.1f).RotatedBy(MathF.Sin(AI_Timer * 0.02f) * 1f), ModContent.ProjectileType<BlightdrasilProjFireball>(), 28, 0, -1);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-7, 0).RotatedByRandom(MathHelper.PiOver4 * 0.1f).RotatedBy(MathF.Sin(AI_Timer * 0.02f) * 1f), ModContent.ProjectileType<BlightdrasilProjFireball>(), 28, 0, -1);
                }
            }
            if (AI_Timer == -60 || AI_Timer == 480)
            {
                if (Main.expertMode)
                {
                    for (int i = -6; i < 7; i++)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 28).RotatedBy(MathHelper.ToRadians(i * 15)), ModContent.ProjectileType<BlightdrasilVilethornBase>(), 30, 0, -1, 0, 0);
                    }
                }
            }

            if (AI_Timer == 510)
            {

                AI_State = (float)ActionState.IchorRain2;
                AI_Timer = -180;
                atkUseCounter = 3;
            }
        }

        private void IchorRain2()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer % 2 == 0)
            {
                if (AI_Timer > 90)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-atkUseCounter / 2, atkUseCounter / 2), -atkUseCounter), ModContent.ProjectileType<BlightdrasilIchor>(), 30, 0, -1, 0, 0);
                }
                else
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-atkUseCounter / 2, atkUseCounter / 2), -atkUseCounter), ModContent.ProjectileType<BlightdrasilIchor>(), 30, 0, -1, 0, 0);
                }
            }
            if (AI_Timer % 15 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item13);
            }
            if (AI_Timer > 90)
            {
                atkUseCounter += 0.04f;
                if (AI_Timer % 60 == 0)
                {

                    NPC npC = NPC.NewNPCDirect(NPC.GetSource_ReleaseEntity(), NPC.Bottom, NPCID.FloatyGross);
                    npC.velocity = Main.rand.NextVector2Circular(3, 2) + new Vector2(0, -4);
                    npC.lifeMax = 600;
                    npC.life = npC.lifeMax;
                    npC.knockBackResist = 0;
                }
            }
            if (AI_Timer == 300)
            {
                AI_State = (float)ActionState.LeafCircle;
                AI_Timer = -60;
                atkUseCounter = 0;
            }
        }
        private void PhaseTransition()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer > 0)
            {
                if (AI_Timer < 150)
                {
                    if (AI_Timer % 30 == 0)
                    {
                        ActiveSound sound = SoundEngine.FindActiveSound(SoundID.Zombie53);
                        if (sound != null)
                        {
                            sound.Stop();
                        }
                        SoundEngine.PlaySound(SoundID.Zombie53);
                    }
                }
            }
            if (AI_Timer == 150)
            {
                SoundEngine.PlaySound(SoundID.Zombie93);

                NPC npc = NPC.NewNPCDirect(NPC.GetBossSpawnSource(NPC.target), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BlightdrasilCore>(), NPC.whoAmI, NPC.whoAmI);
                npc.realLife = NPC.whoAmI;
                npc.velocity = new Vector2(0, -12);
                npc.lifeMax = NPC.lifeMax;
                npc.life = NPC.life;
                NPC.Opacity = 0.5f;
            }
        }
    }
}