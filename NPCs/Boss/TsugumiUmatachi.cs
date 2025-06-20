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
    public class TsugumiUmatachi : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            PhaseTransition,

            FindTarget,
            PositionAbovePlayer1,
            Midnon,
            Slam,
            Balls,
            BallAttack,
            RetractBalls,

            Horseshoes,
            PerpetualMotionMachine,
            Slam2,
            Balls2,
            BallAttack2,
            RetractBalls2,

            HorseshoeDashPrepare,
            HorseshoeDash,
            Slam3,
            HorseshoeCircle,

            Slam4,
            Balls3,
            FinalAtk
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
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/TsugumiTheme");
            }
            NPC.BossBar = ModContent.GetInstance<TreeToadBossBar>();

        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Boss.SeseKitsugaiTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Boss.SeseKitsugaiRelic>()));

            if (ModLoader.TryGetMod("Gensokyo", out Mod gensokyo))
            {
                if (gensokyo.TryFind("PointItem", out ModItem pointItem))
                {
                    npcLoot.Add(ItemDropRule.Common(pointItem.Type, 1, 18, 27));
                }
                if (gensokyo.TryFind("PowerItem", out ModItem powerItem))
                {
                    npcLoot.Add(ItemDropRule.Common(powerItem.Type, 1, 15, 22));
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
                case (float)ActionState.Slam:
                    Slam();
                    break;
                case (float)ActionState.Balls:
                    Balls();
                    break;
                case (float)ActionState.BallAttack:
                    BallAttack();
                    break;
                case (float)ActionState.RetractBalls:
                    RetractBalls();
                    break;
                case (float)ActionState.Horseshoes:
                    Horseshoes();
                    break;
                case (float)ActionState.PerpetualMotionMachine:
                    PerpetualMotionMachine();
                    break;
                case (float)ActionState.Slam2:
                    Slam2();
                    break;
                case (float)ActionState.Balls2:
                    Balls2();
                    break;
                case (float)ActionState.BallAttack2:
                    BallAttack2();
                    break;
                case (float)ActionState.RetractBalls2:
                    RetractBalls2();
                    break;
                case (float)ActionState.HorseshoeDashPrepare:
                    HorseshoeDashPrepare();
                    break;
                case (float)ActionState.HorseshoeDash:
                    HorseshoeDash();
                    break;
                case (float)ActionState.Slam3:
                    Slam3();
                    break;
                case (float)ActionState.HorseshoeCircle:
                    HorseshoeCircle();
                    break;
                case (float)ActionState.Slam4:
                    Slam4();
                    break;
                case (float)ActionState.Balls3:
                    Balls3();
                    break;
                case (float)ActionState.FinalAtk:
                    FinalAtk();
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

            if ((NPC.velocity.X == 0 || NPC.Center.Y < player.Center.Y - 240) && (AI_State == (float)ActionState.Slam || AI_State == (float)ActionState.Slam2 || AI_State == (float)ActionState.Slam3 || AI_State == (float)ActionState.Slam4))
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
            else
            {
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
            if ((AI_State == (float)ActionState.Slam || AI_State == (float)ActionState.Slam2) && NPC.HasValidTarget && Main.player[NPC.target].Top.Y - 30 < NPC.Bottom.Y)
            {
                // If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
                return false;
            }
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
                if (bossPhase == 2)
                {
                    AI_State = (float)ActionState.Horseshoes;
                    AI_Timer = 0;
                }
                if (bossPhase == 3)
                {
                    AI_State = (float)ActionState.HorseshoeDashPrepare;
                    AI_Timer = 0;
                    targetDir = (Main.rand.Next(0, 2) - 0.5f) * 2;
                    NPC.velocity = new Vector2(0, 0);
                }
                if (bossPhase == 4)
                {
                    AI_State = (float)ActionState.Slam4;
                    AI_Timer = -30;
                    NPC.velocity = new Vector2(0, 0);
                    atkUseCounter = -1;

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

            if (AI_Timer % 90 == 0)
            {
                NPC.TargetClosest(true);
                Player player = Main.player[NPC.target];
                targetArea = player.Center + new Vector2(Main.rand.NextFloat(-300, 300) + player.velocity.X * 150, -420);
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                int damage = 28;
                if (Main.expertMode)
                {
                    damage = 27;
                    if (Main.masterMode)
                    {
                        damage = 26;
                    }
                }
                SoundEngine.PlaySound(impactSound);
                for (var j = -8; j < 9; j++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -3f).RotatedBy(MathHelper.ToRadians(j * 22.5f)), ModContent.ProjectileType<TsugumiSpinProj>(), damage, 0, Main.myPlayer, 2.5f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -3f).RotatedBy(MathHelper.ToRadians(j * 22.5f)), ModContent.ProjectileType<TsugumiSpinProj>(), damage, 0, Main.myPlayer, -2.5f);
                }
                if (AI_Timer % 180 == 0)
                {
                    float bulletSpd = 1f;

                    damage = 30;
                    if (Main.expertMode)
                    {
                        damage = 29;
                        if (Main.masterMode)
                        {
                            damage = 28;
                        }
                    }
                    for (var i = 0; i < 9; i++)
                    {

                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(BulletXvel, -9), ModContent.ProjectileType<SeseNonGravProj>(), damage, 0, Main.myPlayer);
                        Main.projectile[proj].velocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * bulletSpd);

                        bulletSpd += 1f;
                    }
                }
            }
            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;

            float inertia = 12f;


            direction = targetArea - NPC.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer += 1;
            if (AI_Timer == 540)
            {
                AI_State = (float)ActionState.Slam;
                AI_Timer = -60;
                atkUseCounter = -1;
            }
        }
        private void Slam()
        {
            Player player = Main.player[NPC.target];

            if (AI_Timer < 0)
            {
                if (atkUseCounter >= 0)
                {
                    if (AI_Timer % 3 == 0)
                    {
                        SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                        {
                            Volume = 0.5f,
                        };
                        SoundEngine.PlaySound(impactSound);
                    }
                    int damage = 24;
                    if (Main.expertMode)
                    {
                        damage = 23;
                        if (Main.masterMode)
                        {
                            damage = 22;
                        }
                    }
                    if (Main.expertMode)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-6, -12)), ModContent.ProjectileType<TsugumiGravProj>(), damage, 0, Main.myPlayer);
                    }
                    else
                    {
                        if (AI_Timer % 2 == 0)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-4, 4), Main.rand.NextFloat(-6, -12)), ModContent.ProjectileType<TsugumiGravProj>(), damage, 0, Main.myPlayer);
                        }

                    }
                }
            }
            else if (AI_Timer == 0)
            {
                atkUseCounter++;
            }
            else if (AI_Timer < 75)
            {
                NPC.noTileCollide = true;

                float speed = Vector2.Distance(NPC.Center, targetArea) / 40f;

                float inertia = 8f;
                targetArea = player.Center + new Vector2(player.velocity.X * 90, -420);

                direction = targetArea - NPC.Center;
                direction.Normalize();
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;


            }
            else if (AI_Timer == 75)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                NPC.velocity = new Vector2(0, 0.5f);
            }
            else if (AI_Timer == 90)
            {
                NPC.globalEnemyBossInfo().finishedAtk = false;
                NPC.velocity = new Vector2(0, 0.05f);
                tileHitboxIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, (int)NPC.Center.Y), new Vector2(0, 5), ModContent.ProjectileType<TsugumiSlamProj>(), 0, 0, Main.myPlayer, NPC.target, NPC.whoAmI);

            }
            else if (AI_Timer > 90)
            {
                if (NPC.globalEnemyBossInfo().finishedAtk)
                {
                    AI_Timer = -30;
                    NPC.velocity = new Vector2(0, 0);

                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                }
            }
            AI_Timer += 1;


            if (atkUseCounter == 5)
            {
                NPC.noTileCollide = true;

                AI_State = (float)ActionState.Balls;
                AI_Timer = 0;

                if (Main.rand.NextBool())
                {
                    spinSpd = 1;
                }
                else
                {
                    spinSpd = -1;
                }

                NPC.globalEnemyBossInfo().OrbitDirection = Main.rand.NextFloat(0, 360);
                NPC.globalEnemyBossInfo().OrbitDistance = 0;
                NPC.globalEnemyBossInfo().killOrbitals = false;
                NPC.globalEnemyBossInfo().orbitalsDealDamage = false;
                int damage = 40;
                if (Main.expertMode)
                {
                    damage = 38;
                    if (Main.masterMode)
                    {
                        damage = 36;
                    }
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 0);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 45);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 90);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 135);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 180);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 225);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 270);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 315);


            }
        }
        private void Balls()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;
            NPC.globalEnemyBossInfo().OrbitDistance += (800 - NPC.globalEnemyBossInfo().OrbitDistance) / 45;

            if (AI_Timer == 150)
            {
                NPC.globalEnemyBossInfo().orbitalsDealDamage = true;
                AI_State = (float)ActionState.BallAttack;
                AI_Timer = 0;
            }
        }
        private void BallAttack()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;
            NPC.globalEnemyBossInfo().OrbitDistance += (800 - NPC.globalEnemyBossInfo().OrbitDistance) / 45;
            Player player = Main.player[NPC.target];

            if (AI_Timer < 300)
            {
                if (AI_Timer % 60 == 0)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                    {
                        Volume = 0.5f,
                    };
                    int damage = 28;
                    if (Main.expertMode)
                    {
                        damage = 27;
                        if (Main.masterMode)
                        {
                            damage = 26;
                        }
                    }
                    SoundEngine.PlaySound(impactSound);
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.type == ModContent.ProjectileType<TsugumiOrbitProj>() && projectile.ai[0] == NPC.whoAmI)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), projectile.Center, (player.Center - projectile.Center).SafeNormalize(Vector2.Zero) * 5, ModContent.ProjectileType<SeseNonGravProj>(), damage, 0, Main.myPlayer);
                        }
                    }
                }
            }

            if (Vector2.Distance(NPC.Center, player.Center) > NPC.globalEnemyBossInfo().OrbitDistance)
            {
                player.position += (NPC.Center - player.Center).SafeNormalize(Vector2.Zero) * (Vector2.Distance(NPC.Center, player.Center) - NPC.globalEnemyBossInfo().OrbitDistance);
            }

            if (AI_Timer == 480)
            {
                NPC.globalEnemyBossInfo().orbitalsDealDamage = true;
                AI_State = (float)ActionState.RetractBalls;
                AI_Timer = 0;
            }
        }
        private void RetractBalls()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;
            NPC.globalEnemyBossInfo().OrbitDistance -= 800 / 180;

            if (NPC.globalEnemyBossInfo().OrbitDistance <= 0)
            {
                NPC.globalEnemyBossInfo().killOrbitals = true;
                AI_State = (float)ActionState.FindTarget;
                AI_Timer = 0;
            }
        }
        private void Horseshoes()
        {
            Player player = Main.player[NPC.target];

            NPC.noTileCollide = true;

            float speed = Vector2.Distance(NPC.Center, targetArea) / 36f;

            float inertia = 5f;
            targetArea = player.Center + new Vector2(player.velocity.X * 100, -400);

            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            AI_Timer++;
            if (AI_Timer >= 0)
            {
                if (Main.expertMode)
                {
                    if (AI_Timer % 24 == 0)
                    {
                        float dir = Main.rand.NextFloat(135, 45);
                        for (int i = 0; i < 50; i++)
                        {
                            dir = Main.rand.NextFloat(135, 45);
                            if (Math.Abs(dir - oldDir) > 30f)
                            {
                                break;
                            }
                        }
                        oldDir = dir;
                        HorseshoePattern(MathHelper.ToRadians(oldDir), 0.15f, 6f);
                    }
                }
                else
                {
                    if (AI_Timer % 30 == 0)
                    {
                        float dir = Main.rand.NextFloat(135, 45);
                        for (int i = 0; i < 50; i++)
                        {
                            dir = Main.rand.NextFloat(135, 45);
                            if (Math.Abs(dir - oldDir) > 30f)
                            {
                                break;
                            }
                        }
                        oldDir = dir;
                        HorseshoePattern(MathHelper.ToRadians(oldDir), 0.15f, 6f);
                    }
                }
                if (AI_Timer == 720)
                {
                    AI_State = (float)ActionState.PerpetualMotionMachine;
                    atkDir = Main.rand.NextFloat(0, 360);
                    spinMagnitude = 0f;
                    spinDirection = Main.rand.NextFloat(0, 360);
                    spinSpd = 0;
                    AI_Timer = -60;
                    targetArea = NPC.position;
                    NPC.velocity = Vector2.Zero;
                }
            }
        }
        private void HorseshoePattern(float dir, float accel, float maxspd, int delay = 0)
        {
            SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
            {
                Volume = 0.5f,
            };
            int damage;
            SoundEngine.PlaySound(impactSound);
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(64, 0).RotatedBy(dir), new Vector2(0.01f, 0).RotatedBy(dir), ModContent.ProjectileType<TsugumiAccelProj>(), 17, 0, Main.myPlayer, accel, maxspd, delay);
            for (int i = 1; i < 15; i++)
            {
                damage = 30;
                if (Main.expertMode)
                {
                    damage = 28;
                    if (Main.masterMode)
                    {
                        damage = 26;
                    }
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(64, 0).RotatedBy(dir + MathHelper.ToRadians(6 * i)), new Vector2(0.01f, 0).RotatedBy(dir), ModContent.ProjectileType<TsugumiAccelProj>(), damage, 0, Main.myPlayer, accel, maxspd, delay);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(64, 0).RotatedBy(dir + MathHelper.ToRadians(6 * -i)), new Vector2(0.01f, 0).RotatedBy(dir), ModContent.ProjectileType<TsugumiAccelProj>(), damage, 0, Main.myPlayer, accel, maxspd, delay);
            }

            damage = 20;
            if (Main.expertMode)
            {
                damage = 19;
                if (Main.masterMode)
                {
                    damage = 18;
                }
            }
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2((1 * 8), 64).RotatedBy(dir), new Vector2(0.01f, 0).RotatedBy(dir), ModContent.ProjectileType<TsugumiAccelProjLong>(), damage, 0, Main.myPlayer, accel, maxspd, delay);
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2((1 * 8), -64).RotatedBy(dir), new Vector2(0.01f, 0).RotatedBy(dir), ModContent.ProjectileType<TsugumiAccelProjLong>(), damage, 0, Main.myPlayer, accel, maxspd, delay);
        }
        private void PerpetualMotionMachine()
        {
            AI_Timer++;
            Player player = Main.player[NPC.target];

            if (AI_Timer < 0)
            {
                float speed = Vector2.Distance(NPC.Center, player.Center + new Vector2(0, -450)) / 36f;

                float inertia = 8f;

                direction = player.Center + new Vector2(0, -450) - NPC.Center;
                direction.Normalize();
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            }
            else
            {
                if (AI_Timer == 0)
                {
                    targetArea = NPC.position;
                }
                if (AI_Timer % (Main.expertMode ? 6 : 8) == 0)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound);
                    for (int i = 0; i < 5; i++)
                    {
                        int damage = 26;
                        if (Main.expertMode)
                        {
                            damage = 25;
                            if (Main.masterMode)
                            {
                                damage = 24;
                            }
                        }
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(4, 0).RotatedBy(MathHelper.ToRadians(atkDir + i * 72)), ModContent.ProjectileType<TsugumiRangeProj>(), damage, 0, Main.myPlayer);
                    }
                    atkDir += 30;
                }
                if (AI_Timer < 240)
                {
                    spinSpd += 0.1f;
                    spinMagnitude += 0.25f;

                }
                spinDirection += spinSpd;

                NPC.position = targetArea + new Vector2(spinMagnitude, 0).RotatedBy(MathHelper.ToRadians(spinDirection));
                if (AI_Timer > 360)
                {
                    spinSpd -= 0.1f;
                    spinMagnitude -= 0.25f;
                }
                if (AI_Timer == 600)
                {
                    atkUseCounter = -1;
                    AI_State = (float)ActionState.Slam2;
                    AI_Timer = -90;
                }
            }
        }
        private void Slam2()
        {
            Player player = Main.player[NPC.target];

            if (AI_Timer < 0)
            {
            }
            else if (AI_Timer == 0)
            {
                atkUseCounter++;
            }
            else if (AI_Timer < 45)
            {
                NPC.noTileCollide = true;

                float speed = Vector2.Distance(NPC.Center, targetArea) / 16f;

                float inertia = 8f;
                targetArea = player.Center + new Vector2(player.velocity.X * 75, -420);

                direction = targetArea - NPC.Center;
                direction.Normalize();
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                if (NPC.Center.Y > player.Center.Y - 120)
                {
                    NPC.velocity.X /= 3;
                }

            }
            else if (AI_Timer == 45)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                NPC.velocity = new Vector2(0, 0.5f);
            }
            else if (AI_Timer == 60)
            {
                NPC.globalEnemyBossInfo().finishedAtk = false;

                NPC.velocity = new Vector2(0, 0.05f);
                tileHitboxIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, (int)NPC.Center.Y), new Vector2(0, 5), ModContent.ProjectileType<TsugumiSlamProj>(), 0, 0, Main.myPlayer, NPC.target, NPC.whoAmI);
            }
            else if (AI_Timer > 60)
            {
                if (NPC.globalEnemyBossInfo().finishedAtk)
                {
                    AI_Timer = (Main.expertMode ? -10 : -15);
                    NPC.velocity = new Vector2(0, 0);
                    int damage = 30;
                    if (Main.expertMode)
                    {
                        damage = 28;
                        if (Main.masterMode)
                        {
                            damage = 26;
                        }
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(12, 0), ModContent.ProjectileType<TsugumiGroundProj>(), damage, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-12, 0), ModContent.ProjectileType<TsugumiGroundProj>(), damage, 0, Main.myPlayer);

                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                }
            }

            AI_Timer += 1;


            if (atkUseCounter == 5)
            {
                NPC.noTileCollide = true;

                AI_State = (float)ActionState.Balls2;
                AI_Timer = 0;

                if (Main.rand.NextBool())
                {
                    spinSpd = 1;
                }
                else
                {
                    spinSpd = -1;
                }

                NPC.globalEnemyBossInfo().OrbitDirection = Main.rand.NextFloat(0, 360);
                NPC.globalEnemyBossInfo().OrbitDistance = 0;
                NPC.globalEnemyBossInfo().killOrbitals = false;
                NPC.globalEnemyBossInfo().orbitalsDealDamage = false;
                int damage = 40;
                if (Main.expertMode)
                {
                    damage = 38;
                    if (Main.masterMode)
                    {
                        damage = 36;
                    }
                }
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, (int)NPC.Center.Y), Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 360 / 8 * i);
                }
            }
        }
        private void Balls2()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;
            NPC.globalEnemyBossInfo().OrbitDistance += (600 - NPC.globalEnemyBossInfo().OrbitDistance) / 45;

            if (AI_Timer == 150)
            {
                NPC.globalEnemyBossInfo().orbitalsDealDamage = true;
                AI_State = (float)ActionState.BallAttack2;
                AI_Timer = 0;
            }
        }
        private void BallAttack2()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;
            NPC.globalEnemyBossInfo().OrbitDistance += (600 - NPC.globalEnemyBossInfo().OrbitDistance) / 45;
            Player player = Main.player[NPC.target];

            if (AI_Timer < 300)
            {
                if (AI_Timer % 45 == 0)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound);
                    float spinspd = Main.rand.NextBool() ? (Main.expertMode ? -0.5f : -0.75f) : (Main.expertMode ? 0.5f : 0.75f);
                    float dirrand = MathHelper.ToDegrees(Main.rand.NextFloatDirection());

                    int damage = 28;
                    if (Main.expertMode)
                    {
                        damage = 27;
                        if (Main.masterMode)
                        {
                            damage = 26;
                        }
                    }
                    for (int i = 0; i < (Main.expertMode?12:10); i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbit2Proj>(), damage, 0, Main.myPlayer, dirrand + 360 / (Main.expertMode ? 12 : 10) * i, spinspd);
                    }
                }
            }

            if (Vector2.Distance(NPC.Center, player.Center) > NPC.globalEnemyBossInfo().OrbitDistance)
            {
                player.position += (NPC.Center - player.Center).SafeNormalize(Vector2.Zero) * (Vector2.Distance(NPC.Center, player.Center) - NPC.globalEnemyBossInfo().OrbitDistance);
            }

            if (AI_Timer == 480)
            {
                NPC.globalEnemyBossInfo().orbitalsDealDamage = true;
                AI_State = (float)ActionState.RetractBalls2;
                AI_Timer = 0;
            }
        }
        private void RetractBalls2()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;
            NPC.globalEnemyBossInfo().OrbitDistance -= 600 / 180;

            if (NPC.globalEnemyBossInfo().OrbitDistance <= 0)
            {
                NPC.globalEnemyBossInfo().killOrbitals = true;
                AI_State = (float)ActionState.Horseshoes;
                AI_Timer = -120;
            }
        }
        private void HorseshoeDashPrepare()
        {
            Player player = Main.player[NPC.target];

            AI_Timer++;
            NPC.velocity += new Vector2(targetDir * 2, 0);
            if (NPC.velocity.X > 40)
            {
                NPC.velocity.X = 40;
            }
            if (NPC.velocity.X < -40)
            {
                NPC.velocity.X = -40;
            }
            if (NPC.Center.X > player.Center.X + 1100 || NPC.Center.X < player.Center.X - 1100)
            {
                AI_Timer = 0;
                AI_State = (float)ActionState.HorseshoeDash;
                randthing = Main.rand.Next(0, (Main.expertMode ? 6 : 8));
                atkUseCounter = 0;
            }
        }
        private void HorseshoeDash()
        {
            Player player = Main.player[NPC.target];

            NPC.velocity += new Vector2(targetDir * 2, 0);
            if (NPC.velocity.X > 40)
            {
                NPC.velocity.X = 40;
            }
            if (NPC.velocity.X < -40)
            {
                NPC.velocity.X = -40;
            }
            if (NPC.Center.X > player.Center.X + 1100)
            {
                NPC.position.X -= 2200;
                randthing = Main.rand.Next(0, (Main.expertMode ? 6 : 8));
                atkUseCounter += 1;
            }
            if (NPC.Center.X < player.Center.X - 1100)
            {
                NPC.position.X += 2200;
                randthing = Main.rand.Next(0, (Main.expertMode ? 6 : 8));
                atkUseCounter += 1;
            }
            if (player.Center.Y - 240 < NPC.Center.Y)
            {
                NPC.position.Y = player.Center.Y - 240 - NPC.height / 2;
            }
            if (player.Center.Y - 480 > NPC.Center.Y)
            {
                NPC.position.Y = player.Center.Y - 480 - NPC.height / 2;
            }

            if (AI_Timer % (Main.expertMode ? 6 : 8) == randthing)
            {
                if (atkUseCounter % 4 == 1)
                {
                    float horseshoedir = new Vector2(targetDir, 6).ToRotation();
                    HorseshoePattern(horseshoedir, 0.15f, 6f);
                }
                else
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound);

                    int damage = 27;
                    if (Main.expertMode)
                    {
                        damage = 26;
                        if (Main.masterMode)
                        {
                            damage = 25;
                        }
                    }
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiAccelProj>(), damage, 0, Main.myPlayer, 0.2f, 10f);
                    Main.projectile[proj].velocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.01f);

                }
            }

            if (AI_Timer >= 600)
            {
                AI_State = (float)ActionState.Slam3;
                NPC.velocity = new Vector2(0, 0);
                AI_Timer = -45;
                atkUseCounter = -1;
                BulletXvel = 5f;
            }

            AI_Timer++;
        }
        private void Slam3()
        {
            Player player = Main.player[NPC.target];
            if (Main.expertMode)
            {
                if (AI_Timer % (Main.expertMode ? 10 : 15) == 0)
                {
                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                    {
                        Volume = 0.5f,
                    };
                    SoundEngine.PlaySound(impactSound);

                    int damage = 24;
                    if (Main.expertMode)
                    {
                        damage = 23;
                        if (Main.masterMode)
                        {
                            damage = 22;
                        }
                    }
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-6, -12)), ModContent.ProjectileType<TsugumiGravProj>(), damage, 0, Main.myPlayer);
                }
            }
            if (AI_Timer < 0)
            {

            }
            else if (AI_Timer == 0)
            {
                atkUseCounter++;
            }
            else if (AI_Timer < 75)
            {
                NPC.noTileCollide = true;

                float speed = Vector2.Distance(NPC.Center, targetArea) / 40f;

                float inertia = 8f;
                targetArea = player.Center + new Vector2(player.velocity.X * 90, -420);

                direction = targetArea - NPC.Center;
                direction.Normalize();
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;


            }
            else if (AI_Timer == 75)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                NPC.velocity = new Vector2(0, 0.5f);
            }
            else if (AI_Timer == 90)
            {
                NPC.globalEnemyBossInfo().finishedAtk = false;
                NPC.velocity = new Vector2(0, 0.05f);
                tileHitboxIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, (int)NPC.Center.Y), new Vector2(0, 5), ModContent.ProjectileType<TsugumiSlamProj>(), 0, 0, Main.myPlayer, NPC.target, NPC.whoAmI);

            }
            else if (AI_Timer > 90)
            {
                if (AI_Timer < 102)
                {

                    if (AI_Timer % 3 == 0)
                    {
                        SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_00")
                        {
                            Volume = 0.5f,
                        };
                        SoundEngine.PlaySound(impactSound);
                    }

                    int damage = 30;
                    if (Main.expertMode)
                    {
                        damage = 28;
                        if (Main.masterMode)
                        {
                            damage = 26;
                        }
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(BulletXvel, -10 + BulletXvel), ModContent.ProjectileType<SeseBulletProj>(), damage, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(BulletXvel * -1, -10 + BulletXvel), ModContent.ProjectileType<SeseBulletProj>(), damage, 0, Main.myPlayer);

                    BulletXvel += -0.4f;
                }

                if (NPC.globalEnemyBossInfo().finishedAtk)
                {
                    AI_Timer = -20;
                    NPC.velocity = new Vector2(0, 0);
                    BulletXvel = 5f;
                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                }
            }
            AI_Timer += 1;
            if (atkUseCounter == 8)
            {
                NPC.noTileCollide = true;

                AI_State = (float)ActionState.HorseshoeCircle;
                AI_Timer = 0;

                RotateMagnitude = Vector2.Distance(Main.player[NPC.target].Center, NPC.Center);
                RotateDir = MathHelper.ToDegrees((NPC.Center - Main.player[NPC.target].Center).ToRotation());
            }
        }
        private void HorseshoeCircle()
        {
            Player player = Main.player[NPC.target];

            NPC.velocity = new Vector2(0, 0);

            NPC.noTileCollide = true;
            NPC.position = Main.player[NPC.target].Center + new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(RotateDir)) * RotateMagnitude - new Vector2(NPC.width / 2, NPC.height / 2);

            if (RotateMagnitude > 320)
            {
                RotateMagnitude -= 5;
            }
            else if (RotateMagnitude < 315)
            {
                RotateMagnitude += 5;
            }
            else
            {
                AI_Timer++;
                if (AI_Timer % 70 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item7, NPC.Center);
                    float horseshoedir = Main.rand.NextFloatDirection();
                    if (Main.masterMode)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            HorseshoePattern(horseshoedir + MathHelper.ToRadians(360 / 5 * i), 0.15f, 8f, 30);
                        }
                    }
                    else if (Main.expertMode)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            HorseshoePattern(horseshoedir + MathHelper.ToRadians(360 / 4 * i), 0.15f, 8f, 30);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            HorseshoePattern(horseshoedir + MathHelper.ToRadians(360 / 3 * i), 0.15f, 8f, 30);
                        }
                    }
                }
                if (AI_Timer > 600 && NPC.Center.Y < player.Center.Y)
                {
                    AI_Timer = 180;
                    targetArea = Main.player[NPC.target].Center + new Vector2(0, -420);
                    AI_State = (float)ActionState.PhaseTransition;
                }
            }
            RotateDir += 1.5f;
        }
        private void Slam4()
        {
            Player player = Main.player[NPC.target];

            if (AI_Timer < 0)
            {

            }
            else if (AI_Timer == 0)
            {
                atkUseCounter++;
            }
            else if (AI_Timer < 75)
            {
                NPC.noTileCollide = true;

                float speed = Vector2.Distance(NPC.Center, targetArea) / 40f;

                float inertia = 8f;
                targetArea = player.Center + new Vector2(player.velocity.X * 90, -420);

                direction = targetArea - NPC.Center;
                direction.Normalize();
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;


            }
            else if (AI_Timer == 75)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound);
                NPC.velocity = new Vector2(0, 0.5f);
            }
            else if (AI_Timer == 90)
            {
                NPC.globalEnemyBossInfo().finishedAtk = false;
                NPC.velocity = new Vector2(0, 0.05f);
                tileHitboxIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, (int)NPC.Center.Y), new Vector2(0, 5), ModContent.ProjectileType<TsugumiSlamProj>(), 0, 0, Main.myPlayer, NPC.target, NPC.whoAmI);

            }
            else if (AI_Timer > 90)
            {
                if (NPC.globalEnemyBossInfo().finishedAtk)
                {
                    AI_Timer = -30;
                    NPC.velocity = new Vector2(0, 0);

                    SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                }
            }
            AI_Timer += 1;


            if (atkUseCounter == 1)
            {
                NPC.noTileCollide = true;

                AI_State = (float)ActionState.Balls3;
                AI_Timer = 0;

                if (Main.rand.NextBool())
                {
                    spinSpd = 1;
                }
                else
                {
                    spinSpd = -1;
                }

                NPC.globalEnemyBossInfo().OrbitDirection = Main.rand.NextFloat(0, 360);
                NPC.globalEnemyBossInfo().OrbitDistance = 0;
                NPC.globalEnemyBossInfo().killOrbitals = false;
                NPC.globalEnemyBossInfo().orbitalsDealDamage = false;

                int damage = 40;
                if (Main.expertMode)
                {
                    damage = 38;
                    if (Main.masterMode)
                    {
                        damage = 36;
                    }
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 0);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 45);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 90);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 135);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 180);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 225);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 270);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbitProj>(), damage, 0, Main.myPlayer, NPC.whoAmI, 315);
            }
        }
        private void Balls3()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;
            NPC.globalEnemyBossInfo().OrbitDistance += (800 - NPC.globalEnemyBossInfo().OrbitDistance) / 45;

            if (AI_Timer == 150)
            {
                NPC.globalEnemyBossInfo().orbitalsDealDamage = true;
                AI_State = (float)ActionState.FinalAtk;
                AI_Timer = 0;
            }
        }
        private void FinalAtk()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;
            NPC.globalEnemyBossInfo().OrbitDistance += (800 - NPC.globalEnemyBossInfo().OrbitDistance) / 45;
            Player player = Main.player[NPC.target];

            int damage = 28;
            if (Main.expertMode)
            {
                damage = 27;
                if (Main.masterMode)
                {
                    damage = 26;
                }
            }
            if (AI_Timer % 240 == 0)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                float dirrand = MathHelper.ToDegrees(Main.rand.NextFloatDirection());
                SoundEngine.PlaySound(impactSound);
                for (int i = 0; i < 10; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbit3Proj>(), damage, 0, Main.myPlayer, dirrand + 360 / 10 * i, 1, 900);
                }
            }
            if (AI_Timer % 240 == 120)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/e_shot_01")
                {
                    Volume = 0.5f,
                };
                float dirrand = MathHelper.ToDegrees(Main.rand.NextFloatDirection());
                SoundEngine.PlaySound(impactSound);
                for (int i = 0; i < 10; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TsugumiOrbit3Proj>(), damage, 0, Main.myPlayer, dirrand + 360 / 10 * i, -1, 900);
                }
            }

            if (Vector2.Distance(NPC.Center, player.Center) > NPC.globalEnemyBossInfo().OrbitDistance)
            {
                player.position += (NPC.Center - player.Center).SafeNormalize(Vector2.Zero) * (Vector2.Distance(NPC.Center, player.Center) - NPC.globalEnemyBossInfo().OrbitDistance);
            }
        }
    }
}