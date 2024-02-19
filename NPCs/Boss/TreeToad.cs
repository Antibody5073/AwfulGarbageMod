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
using AwfulGarbageMod.Items.Weapons;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Consumables;
using AwfulGarbageMod.Items.Vanity;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.Collections.Generic;
using AwfulGarbageMod.Items.Weapons.Rogue;
using AwfulGarbageMod.Configs;

namespace AwfulGarbageMod.NPCs.Boss
{
    // This ModNPC serves as an example of a completely custom AI.
    [AutoloadBossHead]
    public class TreeToad : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            Wait,
            Jump,
            Fall,

            HighJump,
            HighFall,
            RapidJump,
            RapidFall,
            Wait2,
            HoverDash,
            Dash,
            Wait3,
            HighJump2,
            HighFall2,
            FlyUp,
            ScreenDash,
            Circle,
            HoverDash2,
            Dash2,
            Wait4,
        }

        // Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
        // These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
        private enum Frame
        {
            Idle,
            Jump1,
            Jump2,
            Fall1,
            Fall2,
            WingsIdle1,
            WingsIdle2,
            WingsIdle3,
            WingsIdle4,
            WingIdle,
            WingJump1,
            WingJump2,
            WingFall1,
            WingFall2,
            Rest1,
            Rest2,
            Rest3,
            Rest4,
            Rest5,
            Rest6,
            WingRest1,
            WingRest2,
            WingRest3,
            WingRest4,
            WingRest5,
            WingRest6,

        }

        // These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        float AI_State;
        float AI_Timer;
        float JumpCount;
        float frameCounter;
        float frameNumber;
        float JumpsUsed;
        float GravSpeed;
        float randLeaf;
        float targetDir;
        float dashNum;
        float RotateDir;
        float RotateMagnitude;
        float orbDir;
        bool drawTrail = false;
        int highJumpNum;
        Vector2 DashVel;
        Vector2 targetArea;
        Vector2 toPlayer;
        Vector2 toPlayer2;
        float phase = 1;
        float y0;
        bool treeToadRest = false;



        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flutter Slime"); // Automatic from localization files
            Main.npcFrameCount[NPC.type] = 26; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Wet] = true;
            
            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 74; // The width of the npc's hitbox (in pixels)
            NPC.height = 52; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 20; // The amount of damage that this npc deals
            NPC.defense = 4; // The amount of defense that this npc has
            NPC.takenDamageMultiplier = 0.8f;
            NPC.lifeMax = 2500; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath1; // The sound the NPC will make when it dies.
            NPC.value = 50000f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.npcSlots = 100f;


            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/TreeToadTheme");
            }
            NPC.BossBar = ModContent.GetInstance<SeseKitsugaiBossBar>();

        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.TreeToad")
            });
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = 3000;
            if (Main.masterMode)
            {
                NPC.lifeMax = 3400; // Increase by 5 if expert or master mode
                if (Main.getGoodWorld || Main.zenithWorld)
                {
                    NPC.lifeMax = 4500;
                }
            }
        }


        public override void OnSpawn(IEntitySource source)
        {

            NPC.lifeMax *= (ModContent.GetInstance<Config>().BossHealthMultiplier / 100);
            NPC.life = NPC.lifeMax;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TreeToadBag>()));

            // Trophies are spawned with 1/10 chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Boss.TreeToadTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Boss.TreeToadRelic>()));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            ///npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
            {
                notExpertRule.OnSuccess(ItemDropRule.FewFromOptions(1, 1, ItemID.FrogLeg, ModContent.ItemType<LeafBlade>(), ModContent.ItemType<ToadEyes>(), ModContent.ItemType<SylvanTome>(), ModContent.ItemType<ToadsTongue>(), ModContent.ItemType<LeafScepter>(), ModContent.ItemType<Phloem>(), ModContent.ItemType<TreeChopper>()));
            }
            else
            {
                notExpertRule.OnSuccess(ItemDropRule.FewFromOptions(1, 1, ItemID.FrogLeg, ModContent.ItemType<LeafBlade>(), ModContent.ItemType<ToadEyes>(), ModContent.ItemType<SylvanTome>(), ModContent.ItemType<ToadsTongue>(), ModContent.ItemType<LeafScepter>(), ModContent.ItemType<Phloem>()));
            }
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.ArmorPolish, 10));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SuwaHat>(), 15));



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

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(BuffID.BrokenArmor, 1800);
            }
        }

        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedTreeToad, -1);

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
        public override void AI()
        {
            // The npc starts in the asleep state, waiting for a player to enter range
            DrawOffsetY = 20;
            Player player = Main.player[NPC.target];

            if (player.dead)
            {
                NPC.position.Y += 999;
                NPC.EncourageDespawn(0);
                return;
            }

            switch (AI_State)
            {
                case (float)ActionState.Jump:
                    NPC.TargetClosest(true);
                    Jump();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;

                    break;
                case (float)ActionState.Fall:
                    NPC.damage = 26;
                    if (Main.expertMode)
                    {
                        NPC.damage = 34;
                        if (Main.masterMode)
                        {
                            NPC.damage = 42;
                        }
                    }
                    Fall();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    break;
                case (float)ActionState.Wait:
                    Wait();
                    NPC.damage = 0;
                    NPC.takenDamageMultiplier = 1.33f;
                    NPC.defense = 0;
                    break;
                case (float)ActionState.HighJump:
                    NPC.TargetClosest(true);
                    HighJump();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    break;
                case (float)ActionState.HighFall:
                    HighFall();
                    NPC.damage = 26;
                    if (Main.expertMode)
                    {
                        NPC.damage = 34;
                        if (Main.masterMode)
                        {
                            NPC.damage = 42;
                        }
                    }
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    break;
                case (float)ActionState.RapidJump:
                    NPC.TargetClosest(true);
                    RapidJump();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    break;
                case (float)ActionState.RapidFall:
                    RapidFall();
                    NPC.damage = 26;
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    if (Main.expertMode)
                    {
                        NPC.damage = 34;
                        if (Main.masterMode)
                        {
                            NPC.damage = 42;
                        }
                    }
                    break;
                case (float)ActionState.Wait2:
                    Wait2();
                    NPC.damage = 0;
                    NPC.takenDamageMultiplier = 1.33f;
                    NPC.defense = 0;
                    break;
                case (float)ActionState.HoverDash:
                    NPC.damage = 0;
                    HoverDash();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    break;
                case (float)ActionState.Dash:
                    NPC.damage = 26;
                    if (Main.expertMode)
                    {
                        NPC.damage = 34;
                        if (Main.masterMode)
                        {
                            NPC.damage = 42;
                        }
                    }
                    Dash();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    break;
                case (float)ActionState.Wait3:
                    NPC.damage = 0;
                    Wait3();
                    NPC.takenDamageMultiplier = 1.33f;
                    NPC.defense = 0;
                    break;
                case (float)ActionState.HoverDash2:
                    NPC.damage = 0;
                    HoverDash2();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    break;
                case (float)ActionState.Dash2:
                    NPC.damage = 26;
                    if (Main.expertMode)
                    {
                        NPC.damage = 34;
                        if (Main.masterMode)
                        {
                            NPC.damage = 42;
                        }
                    }
                    Dash2();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    break;
                case (float)ActionState.Wait4:
                    NPC.damage = 0;
                    Wait4();
                    NPC.takenDamageMultiplier = 1.33f;
                    NPC.defense = 0;
                    break;
                case (float)ActionState.HighJump2:
                    NPC.TargetClosest(true);
                    HighJump2();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    break;
                case (float)ActionState.HighFall2:
                    HighFall2();
                    NPC.damage = 26;
                    if (Main.expertMode)
                    {
                        NPC.damage = 34;
                        if (Main.masterMode)
                        {
                            NPC.damage = 42;
                        }
                    }
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    break;
                case (float)ActionState.Circle:
                    Circle();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    NPC.damage = 0;
                    break;
                case (float)ActionState.FlyUp:
                    FlyUp();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    NPC.damage = 0;
                    break;
                case (float)ActionState.ScreenDash:
                    ScreenDash();
                    NPC.takenDamageMultiplier = 0.8f;
                    NPC.defense = 4;
                    NPC.damage = 0;
                    break;
            }
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = NPC.direction;


            // For the most part, our animation matches up with our states.
            switch (AI_State)
            {
                case (float)ActionState.Wait:
                    // npc.frame.Y is the goto way of changing animation frames. npc.frame starts from the top left corner in pixel coordinates, so keep that in mind.
                    NPC.frame.Y = (int)Frame.Idle * frameHeight;
                    if (treeToadRest)
                    {
                        frameCounter++;
                        if (frameCounter % 5 == 0)
                        {
                            frameNumber++;
                            if (frameNumber == 6)
                            {
                                frameNumber = 0;
                            }
                        }
                        switch (frameNumber)
                        {
                            case 0:
                                NPC.frame.Y = (int)Frame.Rest1 * frameHeight;
                                break;
                            case 1:
                                NPC.frame.Y = (int)Frame.Rest2 * frameHeight;
                                break;
                            case 2:
                                NPC.frame.Y = (int)Frame.Rest3 * frameHeight;
                                break;
                            case 3:
                                NPC.frame.Y = (int)Frame.Rest4 * frameHeight;
                                break;
                            case 4:
                                NPC.frame.Y = (int)Frame.Rest5 * frameHeight;
                                break;
                            case 5:
                                NPC.frame.Y = (int)Frame.Rest6 * frameHeight;
                                break;
                        }
                    }
                    break;
                case (float)ActionState.Jump:
                    NPC.frame.Y = (int)Frame.Jump1 * frameHeight;
                    break;
                case (float)ActionState.Fall:
                    if (AI_Timer < 55)
                    {
                        NPC.frame.Y = (int)Frame.Jump2 * frameHeight;
                    }
                    else if (AI_Timer < 65)
                    {
                        NPC.frame.Y = (int)Frame.Fall1 * frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = (int)Frame.Fall2 * frameHeight;
                    }
                    break;
                case (float)ActionState.HighJump:
                    NPC.frame.Y = (int)Frame.WingJump1 * frameHeight;
                    break;
                case (float)ActionState.HighFall:
                    if (NPC.velocity.Y > 0)
                    {
                        NPC.frame.Y = (int)Frame.WingFall2 * frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = (int)Frame.WingJump2 * frameHeight;
                    }
                    break;
                case (float)ActionState.RapidJump:
                    NPC.frame.Y = (int)Frame.WingJump1 * frameHeight;
                    break;
                case (float)ActionState.RapidFall:
                    if (AI_Timer < 40)
                    {
                        NPC.frame.Y = (int)Frame.WingJump2 * frameHeight;
                    }
                    else if (AI_Timer < 50)
                    {
                        NPC.frame.Y = (int)Frame.WingFall1 * frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = (int)Frame.WingFall2 * frameHeight;
                    }
                    break;
                case (float)ActionState.Wait2:
                    NPC.frame.Y = (int)Frame.WingIdle * frameHeight;
                    break;
                case (float)ActionState.HighJump2:
                    NPC.frame.Y = (int)Frame.WingJump1 * frameHeight;
                    break;
                case (float)ActionState.HighFall2:
                    if (NPC.velocity.Y > 0)
                    {
                        NPC.frame.Y = (int)Frame.WingFall2 * frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = (int)Frame.Jump2 * frameHeight;
                    }
                    frameNumber = 0;
                    break;
                case (float)ActionState.Circle:
                    frameCounter++;
                    if (frameCounter % 3 == 0)
                    {
                        frameNumber++;
                        if (frameNumber == 4)
                        {
                            frameNumber = 0;
                        }
                    }
                    switch (frameNumber)
                    {
                        case 0:
                            NPC.frame.Y = (int)Frame.WingsIdle1 * frameHeight;
                            break;
                        case 1:
                            NPC.frame.Y = (int)Frame.WingsIdle2 * frameHeight;
                            break;
                        case 2:
                            NPC.frame.Y = (int)Frame.WingsIdle3 * frameHeight;
                            break;
                        case 3:
                            NPC.frame.Y = (int)Frame.WingsIdle4 * frameHeight;
                            break;
                    }
                    break;
                case (float)ActionState.FlyUp:
                    if (NPC.velocity.Y > -0.1f)
                    {
                        NPC.frame.Y = (int)Frame.WingIdle * frameHeight;
                        frameNumber = 0;
                    }
                    else
                    {
                        frameCounter++;
                        if (frameCounter % 3 == 0)
                        {
                            frameNumber++;
                            if (frameNumber == 4)
                            {
                                frameNumber = 0;
                            }
                        }
                        switch (frameNumber)
                        {
                            case 0:
                                NPC.frame.Y = (int)Frame.WingsIdle1 * frameHeight;
                                break;
                            case 1:
                                NPC.frame.Y = (int)Frame.WingsIdle2 * frameHeight;
                                break;
                            case 2:
                                NPC.frame.Y = (int)Frame.WingsIdle3 * frameHeight;
                                break;
                            case 3:
                                NPC.frame.Y = (int)Frame.WingsIdle4 * frameHeight;
                                break;
                        }
                        break;
                    }
                    break;
                case (float)ActionState.ScreenDash:
                    frameCounter++;
                    if (frameCounter % 2 == 0)
                    {
                        frameNumber++;
                        if (frameNumber == 4)
                        {
                            frameNumber = 0;
                        }
                    }
                    switch (frameNumber)
                    {
                        case 0:
                            NPC.frame.Y = (int)Frame.WingsIdle1 * frameHeight;
                            break;
                        case 1:
                            NPC.frame.Y = (int)Frame.WingsIdle2 * frameHeight;
                            break;
                        case 2:
                            NPC.frame.Y = (int)Frame.WingsIdle3 * frameHeight;
                            break;
                        case 3:
                            NPC.frame.Y = (int)Frame.WingsIdle4 * frameHeight;
                            break;
                    }
                    break;
                case (float)ActionState.HoverDash:
                    frameCounter++;
                    if (frameCounter % 3 == 0)
                    {
                        frameNumber++;
                        if (frameNumber == 4)
                        {
                            frameNumber = 0;
                        }
                    }
                    switch (frameNumber)
                    {
                        case 0:
                            NPC.frame.Y = (int)Frame.WingsIdle1 * frameHeight;
                            break;
                        case 1:
                            NPC.frame.Y = (int)Frame.WingsIdle2 * frameHeight;
                            break;
                        case 2:
                            NPC.frame.Y = (int)Frame.WingsIdle3 * frameHeight;
                            break;
                        case 3:
                            NPC.frame.Y = (int)Frame.WingsIdle4 * frameHeight;
                            break;
                    }
                    break;
                case (float)ActionState.Dash:
                    NPC.frame.Y = (int)Frame.WingsIdle3 * frameHeight;
                    break;
                case (float)ActionState.HoverDash2:
                    frameCounter++;
                    if (frameCounter % 3 == 0)
                    {
                        frameNumber++;
                        if (frameNumber == 4)
                        {
                            frameNumber = 0;
                        }
                    }
                    switch (frameNumber)
                    {
                        case 0:
                            NPC.frame.Y = (int)Frame.WingsIdle1 * frameHeight;
                            break;
                        case 1:
                            NPC.frame.Y = (int)Frame.WingsIdle2 * frameHeight;
                            break;
                        case 2:
                            NPC.frame.Y = (int)Frame.WingsIdle3 * frameHeight;
                            break;
                        case 3:
                            NPC.frame.Y = (int)Frame.WingsIdle4 * frameHeight;
                            break;
                    }
                    break;
                case (float)ActionState.Dash2:
                    NPC.frame.Y = (int)Frame.WingsIdle3 * frameHeight;
                    break;
                case (float)ActionState.Wait3:
                    if (NPC.velocity.Y == 0)
                    {
                        NPC.frame.Y = (int)Frame.WingIdle * frameHeight;
                        if (treeToadRest)
                        {
                            frameCounter++;
                            if (frameCounter % 5 == 0)
                            {
                                frameNumber++;
                                if (frameNumber == 6)
                                {
                                    frameNumber = 0;
                                }
                            }
                            switch (frameNumber)
                            {
                                case 0:
                                    NPC.frame.Y = (int)Frame.WingRest1 * frameHeight;
                                    break;
                                case 1:
                                    NPC.frame.Y = (int)Frame.WingRest2 * frameHeight;
                                    break;
                                case 2:
                                    NPC.frame.Y = (int)Frame.WingRest3 * frameHeight;
                                    break;
                                case 3:
                                    NPC.frame.Y = (int)Frame.WingRest4 * frameHeight;
                                    break;
                                case 4:
                                    NPC.frame.Y = (int)Frame.WingRest5 * frameHeight;
                                    break;
                                case 5:
                                    NPC.frame.Y = (int)Frame.WingRest6 * frameHeight;
                                    break;
                            }
                        }
                    }
                    break;
                case (float)ActionState.Wait4:
                    if (NPC.velocity.Y == 0)
                    {
                        NPC.frame.Y = (int)Frame.WingIdle * frameHeight;
                        if (treeToadRest)
                        {
                            frameCounter++;
                            if (frameCounter % 5 == 0)
                            {
                                frameNumber++;
                                if (frameNumber == 6)
                                {
                                    frameNumber = 0;
                                }
                            }
                            switch (frameNumber)
                            {
                                case 0:
                                    NPC.frame.Y = (int)Frame.WingRest1 * frameHeight;
                                    break;
                                case 1:
                                    NPC.frame.Y = (int)Frame.WingRest2 * frameHeight;
                                    break;
                                case 2:
                                    NPC.frame.Y = (int)Frame.WingRest3 * frameHeight;
                                    break;
                                case 3:
                                    NPC.frame.Y = (int)Frame.WingRest4 * frameHeight;
                                    break;
                                case 4:
                                    NPC.frame.Y = (int)Frame.WingRest5 * frameHeight;
                                    break;
                                case 5:
                                    NPC.frame.Y = (int)Frame.WingRest6 * frameHeight;
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            if (true)
            {
                Vector2 drawOrigin = NPC.frame.Size() / 2;
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY); //.RotatedBy(NPC.rotation);
                    Color color = NPC.GetAlpha(drawColor) * (float)(((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos + new Vector2(0, -11), NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }
            }
            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"TreeToad_Gore_Head").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"TreeToad_Gore_Leg").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"TreeToad_Gore_Leg").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"TreeToad_Gore_Hand").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"TreeToad_Gore_Hand").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"TreeToad_Gore_Wing").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, Mod.Find<ModGore>($"TreeToad_Gore_Wing").Type);

            }
        }

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {

            if (AI_State == (float)ActionState.HoverDash || AI_State == (float)ActionState.Dash)
            {
                return true;
            }

            if ((AI_State == (float)ActionState.Fall || AI_State == (float)ActionState.Wait) && NPC.HasValidTarget && Main.player[NPC.target].Center.Y > NPC.Bottom.Y)
            {
                // If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
                return true;
            }
            return false;
            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
        }

        private void fallStop()
        {
            SoundEngine.PlaySound(SoundID.Item10, NPC.Center);
            for (var i = 0; i < 8; i++)
            {
                float xv = Main.rand.NextFloat(-3, 3);
                float yv = Main.rand.NextFloat(-2, -5);
                int dust = Dust.NewDust(NPC.Bottom, 1, 1, DustID.Grass, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1, 2);
            }

            if (AI_State == (float)ActionState.Fall)
            {
                treeToadRest = false;
                if (JumpsUsed == 9)
                {
                    AI_Timer = 15;
                }
                else if (JumpsUsed == 0)
                {
                    treeToadRest = true;
                    frameCounter = 0;
                    AI_Timer = -450;
                }
                else
                {
                    AI_Timer = 0;
                }
                AI_State = (float)ActionState.Wait;
            }
            else
            {
                if (JumpCount == 5)
                {
                    AI_Timer = 0;
                    AI_State = (float)ActionState.Wait2;
                }
                else
                {
                    AI_State = (float)ActionState.RapidJump;
                }
            }
            NPC.velocity.X = 0;
            NPC.noTileCollide = false;

            if (JumpsUsed == 9 || JumpsUsed == 10)
            {
                SoundEngine.PlaySound(SoundID.Item7, NPC.Center);

                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f;
                if (Main.expertMode)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        int proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                        Main.projectile[proj2].velocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f).RotatedBy(MathHelper.ToRadians(1.2f * i));
                        int proj3 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                        Main.projectile[proj3].velocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f).RotatedBy(MathHelper.ToRadians(-1.2f * i));

                    }
                }
            }
            else
            {
                if (JumpCount % 3 == 0)
                {
                    int proj;
                    for (int i = 0; i < 4; i++)
                    {
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadOrbTele").Type, 0, 0, Main.myPlayer);
                        Main.projectile[proj].velocity = new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(90) * i + orbDir);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadOrb").Type, 0, 0, Main.myPlayer);
                        Main.projectile[proj].velocity = new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(90) * i + orbDir);
                    }
                }
            }
        }

        private void Jump()
        {
            // We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
            y0 = 0;
            NPC.noTileCollide = true;

            SoundEngine.PlaySound(SoundID.DoubleJump, NPC.Center);


            NPC.takenDamageMultiplier = 1;
            JumpsUsed++;
            JumpCount++;
            float Xdifference = Main.player[NPC.target].Center.X - NPC.Center.X;
            float JumpSpeed;
            if (Main.player[NPC.target].Bottom.Y < NPC.Bottom.Y)
            {
                //JumpSpeed = -8f;
                //GravSpeed = -8f / 60;
                JumpSpeed = (Main.player[NPC.target].Bottom.Y - NPC.Bottom.Y) / 30 - 8f;
                GravSpeed = ((Main.player[NPC.target].Bottom.Y - NPC.Bottom.Y) / 30 - 8f) / 60;
            }
            else
            {
                JumpSpeed = -8f;
                GravSpeed = -8f / 60;
            }
            if (JumpsUsed == 9)
            {
                if (Main.player[NPC.target].Center.X > NPC.Center.X)
                {
                    NPC.velocity = new Vector2((Xdifference - 450) / 120 + Main.player[NPC.target].velocity.X, JumpSpeed);
                }
                else
                {
                    NPC.velocity = new Vector2((Xdifference + 450) / 120 + Main.player[NPC.target].velocity.X, JumpSpeed);
                }
                AI_Timer = 0;
                AI_State = (float)ActionState.Fall;
            }
            else if (JumpsUsed == 10)
            {
                JumpSpeed += -4f;
                GravSpeed += -4f / 60;
                JumpCount = 0;
                if (Main.player[NPC.target].Center.X > NPC.Center.X)
                {
                    NPC.velocity = new Vector2(Xdifference / 60 + Main.player[NPC.target].velocity.X, JumpSpeed);
                }
                else
                {
                    NPC.velocity = new Vector2(Xdifference / 60 + Main.player[NPC.target].velocity.X, JumpSpeed);
                }
                JumpsUsed = 0;
                AI_Timer = 0;
                randLeaf = Main.rand.Next(0, 12);
                AI_State = (float)ActionState.Fall;
            }
            else
            {
                if (JumpCount % 3 == 0)
                {
                    NPC.velocity = new Vector2(Xdifference / 120 + Main.player[NPC.target].velocity.X, JumpSpeed);
                    orbDir = Main.rand.NextFloatDirection();
                }
                else
                {
                    NPC.velocity = new Vector2(Xdifference / 120, JumpSpeed);
                }
                AI_Timer = 0;
                AI_State = (float)ActionState.Fall;
            }

            if (NPC.velocity.X < 0)
            {
                NPC.direction = (int)MathHelper.ToRadians(90);
            }
            else
            {
                NPC.direction = (int)MathHelper.ToRadians(-90);
            }

            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadTelegraph").Type, 0, 0, Main.myPlayer, GravSpeed);
        }

        private void Wait()
        {
            AI_Timer++;
            if (AI_Timer > 20)
            {
                if (NPC.life < NPC.lifeMax * 3 / 5)
                {
                    phase = 2;
                    AI_State = (float)ActionState.HighJump;

                }
                else
                {
                    AI_State = (float)ActionState.Jump;
                    AI_Timer = 0;
                }
            }
        }

        private void Fall()
        {
            AI_Timer++;

            if (NPC.velocity.Y == 0)
            {
                y0++;
                if (y0 == 2)
                {
                    fallStop();
                }
            }

            NPC.velocity.Y -= GravSpeed;

            if (JumpsUsed == 0 && AI_Timer % 12 == randLeaf)
            {
                SoundEngine.PlaySound(SoundID.Item7, NPC.Center);
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("TreeToadLeafTele").Type, 17, 0, Main.myPlayer);
                Main.projectile[proj].timeLeft = 120 - (int)AI_Timer;
            }

            if (AI_Timer == 60)
            {
                SoundEngine.PlaySound(SoundID.Item7, NPC.Center);
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f;
            }

            if (Main.expertMode)
            {
                if (AI_Timer == 55 || AI_Timer == 65)
                {
                    SoundEngine.PlaySound(SoundID.Item7, NPC.Center);
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f;
                }
            }

            if (NPC.velocity.Y < 0)
            {
                NPC.noTileCollide = true;
            }
            else
            {
                if (Main.player[NPC.target].Center.Y > NPC.Bottom.Y)
                {
                    NPC.noTileCollide = true;
                }
                else
                {
                    NPC.noTileCollide = false;
                }

            }

            if (AI_Timer > 120)
            {
                fallStop();
            }
        }

        private void HighJump()
        {
            NPC.velocity.Y = -24f;
            NPC.noTileCollide = true;
            AI_Timer = 0;
            AI_State = (float)ActionState.HighFall;
            if (NPC.Center.X < Main.player[NPC.target].Center.X)
            {
                NPC.direction = (int)MathHelper.ToRadians(-90);
            }
            else
            {
                NPC.direction = (int)MathHelper.ToRadians(90);
            }
        }

        private void HighFall()
        {
            AI_Timer++;
            //Main.NewText(AI_Timer);

            if (AI_Timer < 60)
            {
                NPC.velocity.Y += 0.3f;
            }
            if (AI_Timer == 60)
            {

                NPC.velocity = new Vector2(0, 0);
                NPC.position.X = Main.player[NPC.target].Center.X - (NPC.Center.X - NPC.position.X);
                NPC.position.Y = Main.player[NPC.target].Top.Y - 540f;
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 12), Mod.Find<ModProjectile>("TreeToadTelegraph").Type, 0, 0, Main.myPlayer, 0);
            }
            if (AI_Timer > 80)
            {
                if (Main.player[NPC.target].Center.Y > NPC.Bottom.Y)
                {
                    NPC.noTileCollide = true;
                }
                else
                {
                    NPC.noTileCollide = false;
                }
                if (NPC.velocity.Y == 0)
                {
                    int proj;

                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadOrbTele").Type, 0, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = new Vector2(-10, 0);

                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadOrb").Type, 0, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = new Vector2(-10, 0);

                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadOrbTele").Type, 0, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = new Vector2(10, 0);

                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadOrb").Type, 0, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = new Vector2(10, 0);


                    AI_Timer = 0;
                    AI_State = (float)ActionState.RapidJump;
                    JumpCount = 0;
                    if (Main.expertMode)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            for (var j = 0; j < 12; j++)
                            {
                                float xv = Main.rand.NextFloat(-4, 4);
                                float yv = Main.rand.NextFloat(-4, -8);
                                int dust = Dust.NewDust(NPC.Bottom, 1, 1, DustID.Grass, xv, yv, 0, default(Color), 1f);
                                Main.dust[dust].scale = Main.rand.NextFloat(1, 2);
                            }
                            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2((Main.rand.NextFloat() - 0.5f) * 10, -8), Mod.Find<ModProjectile>("TreeToadLeafGrav").Type, 0, 0, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2((Main.rand.NextFloat() - 0.5f) * 10 + Main.player[NPC.target].velocity.X, -8), Mod.Find<ModProjectile>("TreeToadLeafGrav").Type, 0, 0, Main.myPlayer);
                        }
                    }
                }
            }
            if (AI_Timer > 75)
            {
                NPC.velocity.Y = 12f;
            }
        }

        private void RapidJump()
        {
            JumpCount++;
            JumpsUsed = 9;
            SoundEngine.PlaySound(SoundID.DoubleJump, NPC.Center);
            float Xdifference = Main.player[NPC.target].Center.X - NPC.Center.X;
            float JumpSpeed;
            if (Main.player[NPC.target].Bottom.Y < NPC.Bottom.Y)
            {
                //JumpSpeed = -8f;
                //GravSpeed = -8f / 60;
                JumpSpeed = (Main.player[NPC.target].Bottom.Y - NPC.Bottom.Y) / 40 - 12f;
                GravSpeed = ((Main.player[NPC.target].Bottom.Y - NPC.Bottom.Y) / 40 - 12f) / 45;
            }
            else
            {
                JumpSpeed = -12f;
                GravSpeed = -12f / 45;
            }

            NPC.velocity = new Vector2(Xdifference / 90 + Main.player[NPC.target].velocity.X, JumpSpeed);
            if (NPC.velocity.X < 0)
            {
                NPC.direction = (int)MathHelper.ToRadians(90);
            }
            else
            {
                NPC.direction = (int)MathHelper.ToRadians(-90);
            }
            AI_Timer = 0;
            y0 = 0;
            AI_State = (float)ActionState.RapidFall;

            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadTelegraph").Type, 0, 0, Main.myPlayer, GravSpeed);
        }

        private void RapidFall()
        {
            AI_Timer++;

            if (NPC.velocity.Y == 0)
            {
                y0++;
                if (y0 == 2)
                {
                    fallStop();
                }
            }

            NPC.velocity.Y -= GravSpeed;

            if (AI_Timer == 45)
            {
                SoundEngine.PlaySound(SoundID.Item7, NPC.Center);

                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f;
            }

            if (Main.expertMode)
            {
                if (AI_Timer == 40 || AI_Timer == 50)
                {
                    SoundEngine.PlaySound(SoundID.Item7, NPC.Center);

                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f;
                }
            }

            if (NPC.velocity.Y < 0)
            {
                NPC.noTileCollide = true;
            }
            else
            {
                if (Main.player[NPC.target].Center.Y > NPC.Bottom.Y)
                {
                    NPC.noTileCollide = true;
                }
                else
                {
                    NPC.noTileCollide = false;
                }

            }

            if (AI_Timer > 120)
            {
                fallStop();
            }
        }

        private void Wait2()
        {
            AI_Timer++;
            if (AI_Timer > 60)
            {
                dashNum = 0;
                if (NPC.Center.X > Main.player[NPC.target].Center.X)
                {
                    targetDir = 1;
                }
                else
                {
                    targetDir = -1;
                }
                AI_State = (float)ActionState.HoverDash;
                AI_Timer = -60;
            }
        }

        private void HoverDash()
        {
            NPC.noTileCollide = true;
            AI_Timer++;
            float speed = Vector2.Distance(NPC.Center, Main.player[NPC.target].Center + new Vector2(120 * targetDir, -80)) / (16f - AI_Timer * 0.5f);
            float inertia = (16f - AI_Timer * 0.5f);
            Vector2 direction = Main.player[NPC.target].Center + new Vector2(180 * targetDir, -120) - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            if (NPC.Center.X < Main.player[NPC.target].Center.X)
            {
                NPC.direction = (int)MathHelper.ToRadians(-90);
            }
            else
            {
                NPC.direction = (int)MathHelper.ToRadians(90);
            }

            if (AI_Timer < 26)
            {
                NPC.rotation = NPC.velocity.X * 0.03f;
            }

            if (AI_Timer == 30)
            {

                NPC.position = Main.player[NPC.target].Center + new Vector2(180 * targetDir, -120) - (NPC.Center - NPC.position);
                AI_Timer = 0;

                if (dashNum == 3)
                {
                    direction = (Main.player[NPC.target].Center - NPC.Center);
                    direction.Normalize();
                    NPC.velocity = direction * -10;
                    treeToadRest = true;
                    frameCounter = 0;
                    AI_State = (float)ActionState.Wait3;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);

                    direction = (Main.player[NPC.target].Center - NPC.Center);
                    direction.Normalize();
                    NPC.velocity = direction * 16;
                    AI_State = (float)ActionState.Dash;
                    dashNum++;
                }
            }

        }

        private void Dash()
        {
            drawTrail = true;
            AI_Timer++;

            NPC.velocity *= 0.97f;
            NPC.rotation = NPC.velocity.X * 0.03f;

            if (AI_Timer == 30)
            {
                drawTrail = false;
                AI_Timer = 0;
                AI_State = (float)ActionState.HoverDash;
                targetDir *= -1;
            }
        }

        private void Wait3()
        {
            NPC.noTileCollide = false;
            AI_Timer++;
            NPC.velocity.X *= 0.8f;
            NPC.velocity.Y += 0.2f;
            NPC.rotation = NPC.velocity.X * 0.03f;

            if (AI_Timer > 360)
            {
                dashNum = 0;
                if (NPC.Center.X > Main.player[NPC.target].Center.X)
                {
                    targetDir = 1;
                }
                else
                {
                    targetDir = -1;
                }
                highJumpNum = 0;
                AI_State = (float)ActionState.HighJump2;
                AI_Timer = 0;
            }
        }

        private void HighJump2()
        {
            NPC.velocity.Y = -24f;
            NPC.noTileCollide = true;
            AI_Timer = 0;
            AI_State = (float)ActionState.HighFall2;
            if (NPC.Center.X < Main.player[NPC.target].Center.X)
            {
                NPC.direction = (int)MathHelper.ToRadians(-90);
            }
            else
            {
                NPC.direction = (int)MathHelper.ToRadians(90);
            }
        }

        private void HighFall2()
        {
            AI_Timer++;
            //Main.NewText(AI_Timer);

            if (AI_Timer < 60)
            {
                NPC.velocity.Y += 0.3f;
            }
            if (AI_Timer == 60)
            {

                NPC.velocity = new Vector2(0, 0);
                NPC.position.X = (Main.player[NPC.target].Center.X - (NPC.Center.X - NPC.position.X)) + Main.player[NPC.target].velocity.X * (540 / 12) + Main.rand.Next(-90, 90);
                NPC.position.Y = Main.player[NPC.target].Top.Y - 540f;
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 12), Mod.Find<ModProjectile>("TreeToadTelegraph").Type, 0, 0, Main.myPlayer, 0);
            }
            if (AI_Timer > 80)
            {
                if (Main.player[NPC.target].Center.Y > NPC.Bottom.Y)
                {
                    NPC.noTileCollide = true;
                }
                else
                {
                    NPC.noTileCollide = false;
                }
                if (NPC.velocity.Y == 0)
                {
                    int proj;

                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadOrbTele").Type, 0, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = new Vector2(-10, 0);

                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadOrb").Type, 0, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = new Vector2(-10, 0);

                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadOrbTele").Type, 0, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = new Vector2(10, 0);

                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadOrb").Type, 0, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = new Vector2(10, 0);


                    AI_Timer = 0;
                    if (highJumpNum == 5)
                    {

                        AI_State = (float)ActionState.FlyUp;
                    }
                    else
                    {
                        AI_State = (float)ActionState.HighJump2;
                        highJumpNum++;
                    }
                    JumpCount = 0;
                    if (Main.expertMode)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            for (var j = 0; j < 12; j++)
                            {
                                float xv = Main.rand.NextFloat(-4, 4);
                                float yv = Main.rand.NextFloat(-4, -8);
                                int dust = Dust.NewDust(NPC.Bottom, 1, 1, DustID.Grass, xv, yv, 0, default(Color), 1f);
                                Main.dust[dust].scale = Main.rand.NextFloat(1, 2);
                            }
                            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2((Main.rand.NextFloat() - 0.5f) * 10, -8), Mod.Find<ModProjectile>("TreeToadLeafGrav").Type, 0, 0, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2((Main.rand.NextFloat() - 0.5f) * 10 + Main.player[NPC.target].velocity.X, -8), Mod.Find<ModProjectile>("TreeToadLeafGrav").Type, 0, 0, Main.myPlayer);
                        }
                    }
                }
            }
            if (AI_Timer > 75)
            {
                NPC.velocity.Y = 12f;
            }
        }

        private void FlyUp()
        {
            NPC.noTileCollide = true;
            AI_Timer++;
            if (AI_Timer > 60)
            {
                NPC.velocity.Y -= 0.3f;
            }
            if (AI_Timer == 180)
            {
                AI_Timer = 0;
                targetDir = (Main.rand.Next(0, 2) - 0.5f) * 2;
                dashNum = 0;
                AI_State = (float)ActionState.ScreenDash;
                randLeaf = Main.rand.Next(0, 2);

            }

        }

        private void ScreenDash()
        {
            NPC.noTileCollide = true;
            drawTrail = true;
            if (AI_Timer == 0)
            {
                NPC.position.X = (Main.player[NPC.target].Center.X - (NPC.Center.X - NPC.position.X)) - Main.rand.Next(1000, 1100) * targetDir;
                NPC.position.Y = Main.player[NPC.target].Top.Y - 300f;
                NPC.velocity = new Vector2(40 * targetDir, Main.rand.NextFloat(-1, 1) * 2.5f);
                if (NPC.Center.X < Main.player[NPC.target].Center.X)
                {
                    NPC.direction = (int)MathHelper.ToRadians(-90);
                }
                else
                {
                    NPC.direction = (int)MathHelper.ToRadians(90);
                }
            }
            else
            {
                int freq;
                if (Main.expertMode)
                {
                    freq = 2;
                }
                else
                {
                    freq = 3;
                }

                    if (AI_Timer % freq == randLeaf)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(targetDir, -6), Mod.Find<ModProjectile>("TreeToadLeafGrav").Type, 0, 0, Main.myPlayer);
                }
                if (AI_Timer == 60)
                {
                    if (dashNum == 8)
                    {
                        AI_Timer = 0;
                        AI_State = (float)ActionState.Circle;
                        RotateMagnitude = Vector2.Distance(Main.player[NPC.target].Center, NPC.Center);
                        RotateDir = MathHelper.ToDegrees((NPC.Center - Main.player[NPC.target].Center).ToRotation());
                    }
                    else
                    {
                        AI_Timer = -1;
                        dashNum++;
                        targetDir *= -1;
                        randLeaf = Main.rand.Next(0, 2);
                    }
                }
            }
            AI_Timer++;

        }

        private void Circle()
        {
            NPC.velocity = new Vector2(0, 0);

            NPC.noTileCollide = true;
            NPC.position = Main.player[NPC.target].Center + new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(RotateDir)) * RotateMagnitude - new Vector2(NPC.width / 2, NPC.height / 2);

            if (RotateMagnitude > 350)
            {
                RotateMagnitude -= 5;
            }
            else
            {
                AI_Timer++;
                if (AI_Timer % 60 == 0 || AI_Timer % 60 == 5 || AI_Timer % 60 == 10)
                {
                    SoundEngine.PlaySound(SoundID.Item7, NPC.Center);
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f;

                }
                if (AI_Timer > 600)
                {
                    if(Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) < 5f && NPC.Center.Y < Main.player[NPC.target].Center.Y)
                    {
                        AI_State = (float)ActionState.HoverDash2;
                        NPC.velocity = new Vector2(0, 0);
                        dashNum = 0;
                        if (NPC.Center.X > Main.player[NPC.target].Center.X)
                        {
                            targetDir = 1;
                        }
                        else
                        {
                            targetDir = -1;
                        }
                        AI_Timer = -60;
                        drawTrail = false;
                    }
                }
            }
            RotateDir += 1.5f;
        }

        private void HoverDash2()
        {
            NPC.noTileCollide = true;
            AI_Timer++;
            float speed = Vector2.Distance(NPC.Center, Main.player[NPC.target].Center + new Vector2(120 * targetDir, -80)) / (16f - AI_Timer * 0.5f);
            float inertia = (16f - AI_Timer * 0.5f);
            Vector2 direction = Main.player[NPC.target].Center + new Vector2(180 * targetDir, -120) - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            if (NPC.Center.X < Main.player[NPC.target].Center.X)
            {
                NPC.direction = (int)MathHelper.ToRadians(-90);
            }
            else
            {
                NPC.direction = (int)MathHelper.ToRadians(90);
            }

            if (AI_Timer < 26)
            {
                NPC.rotation = NPC.velocity.X * 0.03f;
            }

            if (AI_Timer == 30)
            {

                NPC.position = Main.player[NPC.target].Center + new Vector2(180 * targetDir, -120) - (NPC.Center - NPC.position);
                AI_Timer = 0;

                if (dashNum == 3)
                {
                    direction = (Main.player[NPC.target].Center - NPC.Center);
                    direction.Normalize();
                    NPC.velocity = direction * -10; 
                    treeToadRest = true;
                    frameCounter = 0;
                    AI_State = (float)ActionState.Wait4;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);

                    direction = (Main.player[NPC.target].Center - NPC.Center);
                    direction.Normalize();
                    NPC.velocity = direction * 16;
                    AI_State = (float)ActionState.Dash2;
                    dashNum++;
                }
            }

        }

        private void Dash2()
        {
            AI_Timer++;
            drawTrail = true;

            NPC.velocity *= 0.97f;
            NPC.rotation = NPC.velocity.X * 0.03f;

            if (AI_Timer == 30)
            {
                drawTrail = false;
                AI_Timer = 0;
                AI_State = (float)ActionState.HoverDash2;
                targetDir *= -1;
            }
        }

        private void Wait4()
        {
            NPC.noTileCollide = false;
            AI_Timer++;
            NPC.velocity.X *= 0.8f;
            NPC.velocity.Y += 0.2f;
            NPC.rotation = NPC.velocity.X * 0.03f;

            if (AI_Timer > 360)
            {
                dashNum = 0;
                if (NPC.Center.X > Main.player[NPC.target].Center.X)
                {
                    targetDir = 1;
                }
                else
                {
                    targetDir = -1;
                }
                highJumpNum = 0;
                AI_State = (float)ActionState.HighJump;
                AI_Timer = 0;
            }
        }
    }
}