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
using AwfulGarbageMod.Items.Consumables; using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.Items.Weapons.Rogue;
using rail;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Items;

namespace AwfulGarbageMod.NPCs.Boss
{
    // This ModNPC serves as an example of a completely custom AI.
    [AutoloadBossHead]
    public class FujiwaraNoIyozane : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            PhaseTransition,

            FindTarget,
            PositionAbovePlayer1,
            Midnon,
        }

        // Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
        // These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
        private enum Frame
        {
            Left2,
            Left1,
            Idle1,
            Idle2,
            Idle3,
            Right1,
            Right2
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
        int atkUseCounter;
        float spinSpd;
        float oldDir;
        float atkDir;
        float spinMagnitude;
        float spinDirection;
        int tileHitboxIndex;
        float targetDir;
        int randthing;
        float RotateMagnitude;
        float RotateDir;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flutter Slime"); // Automatic from localization files
            Main.npcFrameCount[NPC.type] = 7; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Wet] = true;

        }


        public override void SetDefaults()
        {
            NPC.width = 40; // The width of the npc's hitbox (in pixels)
            NPC.height = 88; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 40; // The amount of damage that this npc deals
            NPC.defense = 15; // The amount of defense that this npc has
            NPC.lifeMax = 21000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath1; // The sound the NPC will make when it dies.
            NPC.value = 300000f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.npcSlots = 100f;
            NPC.noTileCollide = true;


            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/TsugumiTheme");
            }
            NPC.BossBar = ModContent.GetInstance<TreeToadBossBar>();

        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            if (!Main.GameModeInfo.IsJourneyMode)
            {
                NPC.lifeMax = 27000;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 33000; // Increase by 5 if expert or master mode
                    if (Main.getGoodWorld || Main.zenithWorld)
                    {
                        NPC.lifeMax = 39000;
                    }
                }
            }
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else


            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TsugumiBag>()));

            // Trophies are spawned with 1/10 chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Boss.TsugumiTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Boss.TsugumiRelic>()));

            if (ModLoader.TryGetMod("Gensokyo", out Mod gensokyo))
            {
                if (gensokyo.TryFind("PointItem", out ModItem pointItem))
                {
                    npcLoot.Add(ItemDropRule.Common(pointItem.Type, 1, 18, 27));
                }
            }

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            ///npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

                notExpertRule.OnSuccess(ItemDropRule.FewFromOptionsWithNumerator(1, 3, 2, ModContent.ItemType<KitchenKnife>(), ModContent.ItemType<HorseSnapper>()));
                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SoulOfIghtImaHeadOut>(), 1, 10, 14));


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

        public override void OnSpawn(IEntitySource source)
        {

            NPC.lifeMax = NPC.lifeMax * ModContent.GetInstance<Config>().BossHealthMultiplier / 100;
            NPC.life = NPC.lifeMax;
        }
        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedTsugumi, -1);

            // Since this hook is only ran in singleplayer and serverside, we would have to sync it manually.
            // Thankfully, vanilla sends the MessageID.WorldData packet if a BOSS was killed automatically, shortly after this hook is ran

            // If your NPC is not a boss and you need to sync the world (which includes ModSystem, check DownedBossSystem), use this code:
            /*
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			*/
            NPC.globalEnemyBossInfo().killOrbitals = true;
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {
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
                NPC.globalEnemyBossInfo().killOrbitals = true;
                AI_State = (float)ActionState.PhaseTransition;
            }

            if (NPC.life < NPC.lifeMax * 7 / 10 && bossPhase == 1)
            {
                bossPhase = 2;
                AI_Timer = 240;
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
                AI_State = (float)ActionState.PhaseTransition;
            }
            if (NPC.life < NPC.lifeMax * 4 / 10 && bossPhase == 2)
            {
                bossPhase = 3;
                AI_Timer = 210;
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -420);
                NPC.takenDamageMultiplier = 1.2f;
                NPC.defense = 20;
                AI_State = (float)ActionState.PhaseTransition;
            }
            if (NPC.life < NPC.lifeMax * 1 / 10 && bossPhase == 3)
            {
                NPC.takenDamageMultiplier = 0.4f;
                NPC.defense = 24;
                bossPhase = 4;
                AI_Timer = 120;
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
                AI_State = (float)ActionState.PhaseTransition;
            }

            switch (AI_State)
            {
                case (float)ActionState.PhaseTransition:
                    PhaseTransition();
                    break;
                case (float)ActionState.FindTarget:
                    FindTarget();
                    break;
                case (float)ActionState.PositionAbovePlayer1:
                    PositionAbovePlayer1();
                    break;
                case (float)ActionState.Midnon:
                    Midnon();
                    break;
            }
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = 0;
            Player player = Main.player[NPC.target];

            /*if ((NPC.velocity.X == 0 || NPC.Center.Y < player.Center.Y - 240) && (AI_State == (float)ActionState.Slam || AI_State == (float)ActionState.Slam2 || AI_State == (float)ActionState.Slam3 || AI_State == (float)ActionState.Slam4))
            {
                if (NPC.velocity.Y > 1)
                {
                    NPC.frame.Y = (int)Frame.Idle1 * frameHeight;
                }
                else if (NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = (int)Frame.Idle1 * frameHeight;
                }
            }
            else*/
            
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
                    if (animCounter >= 45)
                    {
                        animCounter = 0;
                        idleAnim++;
                    }

                    if (animCounter < 0)
                    {
                        if (NPC.velocity.X > 0)
                        {
                            NPC.frame.Y = (int)Frame.Idle3 * frameHeight;
                        }
                        else
                        {
                            NPC.frame.Y = (int)Frame.Idle1 * frameHeight;
                        }
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
                                NPC.frame.Y = (int)Frame.Idle2 * frameHeight;
                                break;
                            case 3:
                                NPC.frame.Y = (int)Frame.Idle1 * frameHeight;
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
        public override bool? CanFallThroughPlatforms()
        {
            return true;

            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
        }

        private void FindTarget()
        {
            NPC.TargetClosest(true);
            targetArea = Main.player[NPC.target].Center + new Vector2(0, -420);
            AI_State = (float)ActionState.PositionAbovePlayer1;
            AI_Timer = 75;
        }
        private void PhaseTransition()
        {

            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;

            float inertia = 12f;

            NPC.globalEnemyBossInfo().killOrbitals = true;

            direction = targetArea - NPC.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer -= 1;
            if (AI_Timer == 0)
            {
                if (bossPhase == 1)
                {
                    AI_State = (float)ActionState.Midnon;
                    AI_Timer = 0;
                }
            }
        }
        private void PositionAbovePlayer1()
        {
            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;

            float inertia = 12f;


            direction = targetArea - NPC.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer -= 1;
            if (AI_Timer == 0)
            {
                AI_State = (float)ActionState.Midnon;
                AI_Timer = 0;
            }
        }
        private void Midnon()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            targetArea = player.Center + new Vector2(player.velocity.X * 45, -360);

            if (AI_Timer % 60 == 0)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                int damage = 32;
                if (Main.expertMode)
                {
                    damage = 31;
                    if (Main.masterMode)
                    {
                        damage = 30;
                    }
                }
                SoundEngine.PlaySound(impactSound);
                if (Main.expertMode)
                {
                    int rand = Main.rand.Next(0, 360);
                    for (var j = -8; j < 9; j++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(j * 16, -300), new Vector2(0, 5f), ModContent.ProjectileType<IyozaneProjWater>(), damage, 0, Main.myPlayer, 0, rand, 6);
                    }
                }
                else
                {
                    int rand = Main.rand.Next(0, 360);

                    for (var j = -4; j < 5; j++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(j * 16, -300), new Vector2(0, 5f), ModContent.ProjectileType<IyozaneProjWater>(), damage, 0, Main.myPlayer, 0, rand, 6);
                    }
                }
            }
            float speed = Vector2.Distance(NPC.Center, targetArea) / 15f;

            float inertia = 6f;


            direction = targetArea - NPC.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer += 1;
            if (AI_Timer == 300)
            {
                //AI_State = (float)ActionState.Slam;
                AI_Timer = -45;
                atkUseCounter = -1;
            }
        }
    }
}