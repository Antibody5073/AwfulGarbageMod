using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.CodeAnalysis;
using ReLogic.Peripherals.RGB;
using AwfulGarbageMod.Configs;

namespace AwfulGarbageMod.NPCs.Boss
{
    // This ModNPC serves as an example of a completely custom AI.
    [AutoloadBossHead]
    public class EyeOfTheStorm : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            PhaseTransition,

            RepositionCenter,
            RepositionToPlayer,
            RepositionLightning,

            SineWater1,
            AimedLightning1,
            Dash1,
            CloudAttack1,
            DashAgain1,

            WaterLightning2,
            Chase2,
            Dash2,
            ChaseAgain2,
            CloudAttack2,
            ChaseAgainAgain2,

            SineSnow3,
            Dash3,
            Lightning3,
            CloudAttack3,
            Chase3
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
        float wallX;
        float sineWaterDir;
        float lightningDir;
        int DashPhase;
        int sineWaterMod;
        int sineWaterMult;
        int counter;
        int frame;
        int frameCounter;
        Vector2 targetArea;
        Vector2 direction;
        Vector2 storedVel; 
        Vector2 storedPos;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flutter Slime"); // Automatic from localization files
            Main.npcFrameCount[NPC.type] = 5; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;

            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 94; // The width of the npc's hitbox (in pixels)
            NPC.height = 62; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 34; // The amount of damage that this npc deals
            NPC.defense = 13; // The amount of defense that this npc has
            NPC.lifeMax = 5500; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit3; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath52; // The sound the NPC will make when it dies.
            NPC.value = 150000f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.npcSlots = 100f;
            NPC.noTileCollide = true;

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/EotSTheme");
            }
            NPC.BossBar = ModContent.GetInstance<TreeToadBossBar>();

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
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.EyeOfTheStorm")
            });
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = 8000;
            if (Main.masterMode)
            {
                NPC.lifeMax = 9500; // Increase by 5 if expert or master mode
                if (Main.getGoodWorld || Main.zenithWorld)
                {
                    NPC.lifeMax = 12000;
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
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<EyeOfTheStormBag>()));


            // Trophies are spawned with 1/10 chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormRelic>()));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            ///npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<StormEssence>(), 1, 6, 9));

            notExpertRule.OnSuccess(ItemDropRule.FewFromOptions(1, 2, ModContent.ItemType<SkysBane>(), ModContent.ItemType<StormSpell>(), ModContent.ItemType<Thundercrack>(), ModContent.ItemType<LightningBurst>(), ModContent.ItemType<StormForecast>(), ModContent.ItemType<TheEyeOfTheStorm>()));

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
            Main.StartRain();
            Main.maxRaining = 1f;

            NPC.lifeMax = NPC.lifeMax * ModContent.GetInstance<Config>().BossHealthMultiplier / 100;
            NPC.life = NPC.lifeMax;
        }

        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedEyeOfTheStorm, -1);
            Main.StopRain();


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

            Player player = Main.player[NPC.target];

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if ((projectile.type == Mod.Find<ModProjectile>("EoTSWallCloud").Type || projectile.type == Mod.Find<ModProjectile>("EoTSWall").Type) && projectile.owner == player.whoAmI)
                {
                    projectile.timeLeft = 2;
                }
            }

            Main.rainTime = 3;
            Main.maxRaining = 1f;
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.78f);


            if (player.dead)
            {
                NPC.position.Y += 999;
                NPC.EncourageDespawn(0);
                return;
            }

            if (bossPhase == 0)
            {
                NPC.TargetClosest(true);
                wallX = Main.player[NPC.target].Center.X;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX + 600, Main.player[NPC.target].Center.Y), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWall").Type, 17, 0, Main.myPlayer);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX - 600, Main.player[NPC.target].Center.Y), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWall").Type, 17, 0, Main.myPlayer);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX + 600, Main.player[NPC.target].Center.Y + 1200), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWall").Type, 17, 0, Main.myPlayer);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX - 600, Main.player[NPC.target].Center.Y + 1200), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWall").Type, 17, 0, Main.myPlayer);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX + 600, Main.player[NPC.target].Center.Y - 1200), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWall").Type, 17, 0, Main.myPlayer);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX - 600, Main.player[NPC.target].Center.Y - 1200), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWall").Type, 17, 0, Main.myPlayer);
                bossPhase = 1;
                AI_Timer = 170;
                AI_State = (float)ActionState.PhaseTransition;
            }


            if (NPC.life < NPC.lifeMax * 3 / 4 && bossPhase == 1)
            {
                NPC.velocity = Vector2.Zero;
                bossPhase = 2;
                AI_Timer = 170;
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
                AI_State = (float)ActionState.PhaseTransition;
                if (Main.expertMode)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX + 550, Main.player[NPC.target].Center.Y - 360), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWallCloud").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX - 550, Main.player[NPC.target].Center.Y - 360), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWallCloud").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX + 500, Main.player[NPC.target].Center.Y - 360), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWallCloud").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX - 500, Main.player[NPC.target].Center.Y - 360), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWallCloud").Type, 17, 0, Main.myPlayer);
                }
            }

            if (NPC.life < NPC.lifeMax * 7 / 20 && bossPhase == 2)
            {
                bossPhase = 3;
                AI_Timer = 170;
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
                AI_State = (float)ActionState.PhaseTransition;
                if (Main.expertMode)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX + 450, Main.player[NPC.target].Center.Y - 360), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWallCloud").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX - 450, Main.player[NPC.target].Center.Y - 360), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWallCloud").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX + 400, Main.player[NPC.target].Center.Y - 360), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWallCloud").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(wallX - 400, Main.player[NPC.target].Center.Y - 360), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSWallCloud").Type, 17, 0, Main.myPlayer);
                }
            }



            if (Main.player[NPC.target].Center.X > wallX + 600)
            {
                Main.player[NPC.target].position.X = wallX + 600 - (Main.player[NPC.target].Center.X - Main.player[NPC.target].position.X);
            }
            if (Main.player[NPC.target].Center.X < wallX - 600)
            {
                Main.player[NPC.target].position.X = wallX - 600 - (Main.player[NPC.target].Center.X - Main.player[NPC.target].position.X);

            }

            switch (AI_State)
            {
                case (float)ActionState.PhaseTransition:
                    NPC.dontTakeDamage = true;
                    PhaseTransition();
                    break;
                case (float)ActionState.RepositionCenter:
                    NPC.dontTakeDamage = true;
                    RepositionCenter();
                    break;
                case (float)ActionState.RepositionToPlayer:
                    NPC.dontTakeDamage = true;
                    RepositionToPlayer();
                    break;
                case (float)ActionState.RepositionLightning:
                    NPC.dontTakeDamage = true;
                    RepositionLightning();
                    break;
                case (float)ActionState.SineWater1:
                    SineWater1();
                    break;
                case (float)ActionState.AimedLightning1:
                    AimedLightning1();
                    break;
                case (float)ActionState.Dash1:
                    Dash1();
                    break;
                case (float)ActionState.CloudAttack1:
                    CloudAttack1();
                    break;
                case (float)ActionState.DashAgain1:
                    DashAgain1();
                    break;
                case (float)ActionState.WaterLightning2:
                    WaterLightning2();
                    break;
                case (float)ActionState.Chase2:
                    Chase2();
                    break;
                case (float)ActionState.Dash2:
                    Dash2();
                    break;
                case (float)ActionState.ChaseAgain2:
                    ChaseAgain2();
                    break;
                case (float)ActionState.CloudAttack2:
                    CloudAttack2();
                    break;
                case (float)ActionState.ChaseAgainAgain2:
                    ChaseAgainAgain2();
                    break;
                case (float)ActionState.SineSnow3:
                    SineSnow3();
                    break;
                case (float)ActionState.Dash3:
                    Dash3();
                    break;
                case (float)ActionState.Lightning3:
                    Lightning3();
                    break;
                case (float)ActionState.CloudAttack3:
                    CloudAttack3();
                    break;
                case (float)ActionState.Chase3:
                    Chase3();
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
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos + new Vector2(0, -6), NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }
            }

           
            return true;
        }
        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
        }


        private void PhaseTransition()
        {
            AI_Timer -= 1;
            NPC.velocity = new Vector2(0, 0);

            if (AI_Timer > 84)
            {
                NPC.alpha += 3;
            }
            else
            {
                if (AI_Timer == 84)
                {
                    NPC.alpha = 255;

                    NPC.position = new Vector2(wallX - NPC.width / 2, Main.player[NPC.target].Center.Y - 400);
                }
                NPC.alpha -= 3;
            }

            if (AI_Timer == 0)
            {
                NPC.dontTakeDamage = false;

                if (bossPhase == 1)
                {
                    AI_State = (float)ActionState.SineWater1;
                    AI_Timer = 0;
                    sineWaterDir = 0;
                    targetArea = NPC.position;
                    if (Main.expertMode)
                    {
                        sineWaterMod = Main.rand.Next(0, 3);
                    }
                    else
                    {
                        sineWaterMod = Main.rand.Next(0, 4);
                    }
                    sineWaterMult = (int)((Main.rand.Next(0, 2) - 0.5f) * 2);
                }
                if (bossPhase == 2)
                {
                    AI_State = (float)ActionState.WaterLightning2;
                    AI_Timer = 0;
                    sineWaterDir = 0;
                    targetArea = NPC.position;
                    if (Main.expertMode)
                    {
                        sineWaterMod = Main.rand.Next(0, 5);
                    }
                    else
                    {
                        sineWaterMod = Main.rand.Next(0, 7);
                    }
                    sineWaterMult = (int)((Main.rand.Next(0, 2) - 0.5f) * 2);
                }
                if (bossPhase == 3)
                {
                    AI_State = (float)ActionState.SineSnow3;
                    AI_Timer = 0;
                    sineWaterDir = 0;
                    targetArea = NPC.position;
                    if (Main.expertMode)
                    {
                        sineWaterMod = Main.rand.Next(0, 6);
                    }
                    else 
                    {
                        sineWaterMod = Main.rand.Next(0, 8);
                    }
                    sineWaterMult = (int)((Main.rand.Next(0, 2) - 0.5f) * 2);
                }
            }
        }

        private void RepositionCenter()
        {
            AI_Timer -= 1;

            if (AI_Timer > 84)
            {
                NPC.alpha += 3;
            }
            else
            {
                if (AI_Timer == 84)
                {
                    NPC.alpha = 255;

                    NPC.position = new Vector2(wallX - NPC.width / 2, Main.player[NPC.target].Center.Y - 400);
                }
                NPC.alpha -= 3;
            }

            if (AI_Timer == 0)
            {
                NPC.dontTakeDamage = false;
                AI_State = Next_State;

                if (bossPhase == 1)
                {
                    if (Next_State == (float)ActionState.SineWater1)
                    {
                        AI_Timer = 0;
                        sineWaterDir = 0;
                        targetArea = NPC.position;
                        if (Main.expertMode)
                        {
                            sineWaterMod = Main.rand.Next(0, 3);
                        }
                        else
                        {
                            sineWaterMod = Main.rand.Next(0, 4);
                        }
                        sineWaterMult = (int)((Main.rand.Next(0, 2) - 0.5f) * 2);
                    }
                }
                if (bossPhase == 2)
                {
                    if (Next_State == (float)ActionState.WaterLightning2)
                    {
                        AI_Timer = 0;
                        sineWaterDir = 0;
                        targetArea = NPC.position;
                        if (Main.expertMode)
                        {
                            sineWaterMod = Main.rand.Next(0, 5);
                        }
                        else
                        {
                            sineWaterMod = Main.rand.Next(0, 7);
                        }
                        sineWaterMult = (int)((Main.rand.Next(0, 2) - 0.5f) * 2);
                    }
                }
                if (bossPhase == 3)
                {
                    if (Next_State == (float)ActionState.SineSnow3)
                    {
                        AI_Timer = 0;
                        sineWaterDir = 0;
                        targetArea = NPC.position;
                        if (Main.expertMode)
                        {
                            sineWaterMod = Main.rand.Next(0, 6);
                        }
                        else
                        {
                            sineWaterMod = Main.rand.Next(0, 8);
                        }
                        sineWaterMult = (int)((Main.rand.Next(0, 2) - 0.5f) * 2);
                    }
                }
            }
        }

        private void RepositionToPlayer()
        {
            AI_Timer -= 1;

            if (AI_Timer > 84)
            {
                NPC.alpha += 3;
            }
            else
            {
                if (AI_Timer == 84)
                {
                    NPC.alpha = 255;

                    NPC.position = new Vector2(Main.player[NPC.target].Center.X - NPC.width / 2, Main.player[NPC.target].Center.Y - 400);
                }
                NPC.alpha -= 3;
            }

            if (AI_Timer == 0)
            {
                NPC.dontTakeDamage = false;
                AI_State = Next_State;

                if (bossPhase == 1)
                {
                    if (Next_State == (float)ActionState.Dash1 || Next_State == (float)ActionState.DashAgain1)
                    {
                        AI_Timer = 375;
                    }
                    else if (Next_State == (float)ActionState.CloudAttack1)
                    {
                        AI_Timer = 700;
                    }
                }
                if (bossPhase == 2)
                {
                    if (Next_State == (float)ActionState.Chase2 || Next_State == (float)ActionState.ChaseAgain2 || Next_State == (float)ActionState.ChaseAgainAgain2)
                    {
                        AI_Timer = 480;
                    }
                    else if (Next_State == (float)ActionState.CloudAttack2)
                    {
                        AI_Timer = 750;
                    }
                }
                if (bossPhase == 3)
                {
                    if (Next_State == (float)ActionState.Dash3)
                    {
                        DashPhase = 1;
                        AI_Timer = 480;
                    }
                    else if (Next_State == (float)ActionState.CloudAttack3)
                    {
                        AI_Timer = 720;
                    }
                    if (Next_State == (float)ActionState.Chase3)
                    {
                        AI_Timer = 480;
                    }
                }
            }
        }

        private void RepositionLightning()
        {
            AI_Timer -= 1;

            if (AI_Timer > 84)
            {
                NPC.alpha += 3;
                float xv = Main.rand.NextFloat(-3, 3);
                float yv = Main.rand.NextFloat(-2, -5);
                int dust = Dust.NewDust(new Vector2(wallX - NPC.width / 2, Main.player[NPC.target].Center.Y), 1, 1, DustID.Electric, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1, 2);

            }
            else
            {
                if (AI_Timer == 84)
                {
                    NPC.alpha = 255;

                    NPC.position = new Vector2(wallX, Main.player[NPC.target].Center.Y) - new Vector2(NPC.width / 2, NPC.height / 2);
                }
                NPC.alpha -= 3;
            }

            if (AI_Timer == 0)
            {
                NPC.dontTakeDamage = false;
                AI_State = (float)ActionState.Lightning3;
                AI_Timer = 0;
            }
        }

        private void SineWater1()
        {

            NPC.position = targetArea + new Vector2((float)Math.Sin((double)MathHelper.ToRadians(sineWaterDir)) * 600, 0);
            sineWaterDir += 2.3341f * sineWaterMult;

            AI_Timer += 1;
            if (AI_Timer <= 450)
            {
                if (Main.expertMode)
                {
                    if (AI_Timer % 3 == sineWaterMod)
                    {
                        SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                    }
                }
                else
                {
                    if (AI_Timer % 4 == sineWaterMod)
                    {
                        SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                    }
                }
            }
            if (AI_Timer > 480)
            {
                AI_State = (float)ActionState.AimedLightning1;
                AI_Timer = 0;
            }
        }

        private void AimedLightning1()
        {

            NPC.position = targetArea + new Vector2((float)Math.Sin((double)MathHelper.ToRadians(sineWaterDir)) * 600, 0);
            sineWaterDir += 2.3341f * sineWaterMult;

            AI_Timer += 1;
            if (AI_Timer % 20 == 10)
            {
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(4, 4), Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center), Mod.Find<ModProjectile>("EoTSLightningTele").Type, 17, 0, Main.myPlayer);
            }
            if (AI_Timer > 240)
            {
                AI_State = (float)ActionState.RepositionToPlayer;
                Next_State = (float)ActionState.Dash1;
                AI_Timer = 170;
            }
        }

        private void Dash1()
        {
            if (AI_Timer % 75 == 0)
            {
                NPC.TargetClosest(true);
                targetArea = Main.player[NPC.target].Center;

                direction = targetArea - NPC.Center;
                if (direction != new Vector2(0, 0))
                {
                    direction.Normalize();
                }
                else
                {
                    direction = new Vector2(0, 1);
                }
                if (Main.expertMode)
                {
                    direction *= 24;
                }
                else 
                {
                    direction *= 21;
                }
                storedVel = direction;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, storedVel / 4, ModContent.ProjectileType<BossTelegraph>(), 0, 0, Main.myPlayer, DustID.AncientLight);

            }
            else if (AI_Timer % 75 > 50)
            {
                NPC.velocity = new Vector2(0, 0);
            }
            else if (AI_Timer % 75 == 50)
            {
                NPC.velocity = storedVel;
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
            }
            else
            {
                targetArea = Main.player[NPC.target].Center;
                float speed = 3f;
                float inertia = 32f;
                direction = targetArea - NPC.Center;
                if (direction != new Vector2(0, 0))
                {
                    direction.Normalize();
                }
                else
                {
                    direction = new Vector2(0, 1);
                }
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            }

            AI_Timer--;


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionToPlayer;
                Next_State = (float)ActionState.CloudAttack1;
                AI_Timer = 170;
            }
        }

        private void CloudAttack1()
        {

            targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;

            float inertia = 12f;


            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;


            AI_Timer--;

            if (AI_Timer % 65 == 45)
            {
                if (Main.expertMode)
                {
                    if (Main.masterMode)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + new Vector2(Main.rand.Next(250, 600), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSCloud1").Type, 17, 0, Main.myPlayer, 60);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + new Vector2(Main.rand.Next(250, 600), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSCloud1").Type, 17, 0, Main.myPlayer, 60);
                        }
                    }
                    
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + new Vector2(Main.rand.Next(250, 600), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSCloud1").Type, 17, 0, Main.myPlayer, 60);
                    }
                }
                

            }


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionToPlayer;
                Next_State = (float)ActionState.DashAgain1;
                AI_Timer = 170;
            }
        }

        private void DashAgain1()
        {
            if (AI_Timer % 75 == 0)
            {

                NPC.TargetClosest(true);
                targetArea = Main.player[NPC.target].Center;

                direction = targetArea - NPC.Center;
                if (direction != new Vector2(0, 0))
                {
                    direction.Normalize();
                }
                else
                {
                    direction = new Vector2(0, 1);
                }
                direction *= 24;

                storedVel = direction;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, storedVel / 4, ModContent.ProjectileType<BossTelegraph>(), 0, 0, Main.myPlayer, DustID.AncientLight);

            }
            else if (AI_Timer % 75 > 50)
            {
                NPC.velocity = new Vector2(0, 0);
            }
            else if (AI_Timer % 75 == 50)
            {
                NPC.velocity = storedVel;
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);

            }
            else
            {
                targetArea = Main.player[NPC.target].Center;
                float speed = 3f;
                float inertia = 32f;
                direction = targetArea - NPC.Center;
                if (direction != new Vector2(0, 0))
                {
                    direction.Normalize();
                }
                else
                {
                    direction = new Vector2(0, 1);
                }
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            }

            AI_Timer--;


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionCenter;
                Next_State = (float)ActionState.SineWater1;
                AI_Timer = 170;
            }
        }

        private void WaterLightning2()
        {

            NPC.position = targetArea + new Vector2((float)Math.Sin((double)MathHelper.ToRadians(sineWaterDir)) * 600, 0);
            sineWaterDir += 2.3341f * sineWaterMult;

            AI_Timer += 1;

            if (Main.expertMode)
            {
                if (AI_Timer % 5 == sineWaterMod)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                }
            }
            else
            {
                if (AI_Timer % 7 == sineWaterMod)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                }
            }
            if (AI_Timer % 36 == 18)
            {
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(4, 4), Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center), Mod.Find<ModProjectile>("EoTSLightningTele").Type, 17, 0, Main.myPlayer);
            }
            if (AI_Timer > 720)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionToPlayer;
                Next_State = (float)ActionState.Chase2;
                AI_Timer = 170;
            }
        }

        private void Chase2()
        {
            NPC.takenDamageMultiplier = 0.65f;
            targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
            float speed = Vector2.Distance(NPC.Center, targetArea) - 0.151251f;
            if (speed > 10)
            {
                speed = 10;
            }

            float inertia = 1f;


            direction = targetArea - NPC.Center;
            if (direction != new Vector2(0, 0))
            {
                direction.Normalize();
            }
            else
            {
                direction = new Vector2(0, 1);
            }
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;


            AI_Timer--;
            if (Main.expertMode) 
            {
                if (AI_Timer % 12 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                }
            }
            else
            {
                if (AI_Timer % 16 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                }
            }

            


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.Dash2;
                AI_Timer = 890;
            }
        }

        private void Dash2()
        {
            
            if (AI_Timer % 178 > 75)
            {
                NPC.dontTakeDamage = true;

                NPC.velocity = new Vector2(0, 0);

                if (AI_Timer % 178 == 177)
                {
                    storedPos = Main.player[NPC.target].Center - new Vector2(NPC.width / 2, NPC.height / 2) + Main.player[NPC.target].velocity * 51;
                    
                }
                float xv = Main.rand.NextFloat(-3, 3);
                float yv = Main.rand.NextFloat(-2, -5);
                int dust = Dust.NewDust(storedPos + new Vector2(NPC.width / 2, NPC.height / 2), 1, 1, DustID.Electric, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1, 2);
                Lighting.AddLight(storedPos, 0.6f, 0.75f, 0.8f);

                if (AI_Timer % 178 > 126)
                {
                    NPC.alpha += 5;
                }
                else
                {
                    if (AI_Timer % 178 == 126)
                    {
                        NPC.alpha = 255;
                        NPC.position = storedPos;
                    }
                    NPC.alpha -= 5;
                }
            }
            else
            {
                NPC.dontTakeDamage = false;

                if (AI_Timer % 178 == 75)
                {

                    NPC.TargetClosest(true);
                    targetArea = Main.player[NPC.target].Center;

                    direction = targetArea - NPC.Center;
                    if (direction != new Vector2(0, 0))
                    {
                        direction.Normalize();
                    }
                    else
                    {
                        direction = new Vector2(0, 1);
                    }
                    if (Main.expertMode)
                    {
                        direction *= 30;
                    }
                    else
                    {
                        direction *= 25;
                    }
                    storedVel = direction;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, storedVel / 4, ModContent.ProjectileType<BossTelegraph>(), 0, 0, Main.myPlayer, DustID.AncientLight);

                }
                else if (AI_Timer % 178 > 50)
                {
                    NPC.velocity = new Vector2(0, 0);
                }
                else if (AI_Timer % 178 == 50)
                {
                    NPC.velocity = storedVel;
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                }
                else
                {
                    targetArea = Main.player[NPC.target].Center;
                    float speed = 3f;
                    float inertia = 32f;
                    direction = targetArea - NPC.Center;
                    if (direction != new Vector2(0, 0))
                    {
                        direction.Normalize();
                    }
                    else
                    {
                        direction = new Vector2(0, 1);
                    }
                    direction *= speed;

                    NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            AI_Timer--;


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionToPlayer;
                Next_State = (float)ActionState.ChaseAgain2;
                AI_Timer = 170;
            }
        }

        private void ChaseAgain2()
        {
            NPC.takenDamageMultiplier = 0.65f;

            targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
            float speed = Vector2.Distance(NPC.Center, targetArea) - 0.151251f;
            if (speed > 10)
            {
                speed = 10;
            }

            float inertia = 1f;


            direction = targetArea - NPC.Center;
            if (direction != new Vector2(0, 0))
            {
                direction.Normalize();
            }
            else
            {
                direction = new Vector2(0, 1);
            }
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;


            AI_Timer--;

            if (Main.expertMode)
            {
                if (AI_Timer % 12 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                }
            }
            else
            {
                if (AI_Timer % 16 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                }
            }


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionToPlayer;
                Next_State = (float)ActionState.CloudAttack2;
                AI_Timer = 170;
            }
        }

        private void CloudAttack2()
        {

            AI_Timer--;

            targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;

            float inertia = 12f;


            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;



            if (Main.expertMode)
            {
                if (Main.masterMode)
                {
                    if (AI_Timer % 150 > 59)
                    {
                        if (AI_Timer % 12 == 0)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + new Vector2(Main.rand.Next(250, 500), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSCloud1").Type, 17, 0, Main.myPlayer, 45);
                        }
                    }
                }
                else
                {
                    if (AI_Timer % 150 > 71)
                    {
                        if (AI_Timer % 12 == 0)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + new Vector2(Main.rand.Next(250, 500), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSCloud1").Type, 17, 0, Main.myPlayer, 45);
                        }
                    }
                }
            }
            else
            {
                if (AI_Timer % 150 > 83)
                {
                    if (AI_Timer % 12 == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + new Vector2(Main.rand.Next(250, 500), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSCloud1").Type, 17, 0, Main.myPlayer, 45);
                    }
                }
            }
            


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionToPlayer;
                Next_State = (float)ActionState.ChaseAgainAgain2;
                AI_Timer = 170;
            }

        }

        private void ChaseAgainAgain2()
        {
            NPC.takenDamageMultiplier = 0.65f;

            targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
            float speed = Vector2.Distance(NPC.Center, targetArea) - 0.151251f;
            if (speed > 10)
            {
                speed = 10;
            }

            float inertia = 1f;


            direction = targetArea - NPC.Center;
            if (direction != new Vector2(0, 0))
            {
                direction.Normalize();
            }
            else
            {
                direction = new Vector2(0, 1);
            }
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;


            AI_Timer--;

            if (Main.expertMode)
            {
                if (AI_Timer % 12 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                }
            }
            else
            {
                if (AI_Timer % 16 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSWater1").Type, 17, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                }
            }


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionCenter;
                Next_State = (float)ActionState.WaterLightning2;
                AI_Timer = 170;
            }
        }
   
        private void SineSnow3()
        {

            NPC.position = targetArea + new Vector2((float)Math.Sin((double)MathHelper.ToRadians(sineWaterDir)) * 600, 0);
            sineWaterDir += 2.3341f * sineWaterMult;

            AI_Timer += 1;
            if (AI_Timer <= 450)
            {
                if (Main.expertMode)
                {
                    if (AI_Timer % 6 == sineWaterMod)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f).RotatedBy(MathHelper.ToRadians(30)), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f).RotatedBy(MathHelper.ToRadians(-30)), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                        SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    }
                }
                else
                {
                    if (AI_Timer % 8 == sineWaterMod)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f).RotatedBy(MathHelper.ToRadians(30)), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f).RotatedBy(MathHelper.ToRadians(-30)), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                        SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                    }
                }
            }
            if (AI_Timer > 480)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.Dash3;
                AI_Timer = 1424;
                DashPhase = 0;
            }
        }

        private void Dash3()
        {

            if (AI_Timer % 178 > 75)
            {
                NPC.dontTakeDamage = true;

                NPC.velocity = new Vector2(0, 0);

                if (AI_Timer % 178 == 177)
                {
                    if (DashPhase == 1)
                    {
                        DashPhase = 0;
                        storedPos = Main.player[NPC.target].Center - new Vector2(NPC.width / 2, NPC.height / 2) + Main.player[NPC.target].velocity * 51 + new Vector2(900 * (Main.rand.Next(0, 2) - 0.5f), -300);
                    }
                    else
                    {
                        DashPhase = 1;
                        storedPos = Main.player[NPC.target].Center - new Vector2(NPC.width / 2, NPC.height / 2) + Main.player[NPC.target].velocity * 51;
                    }
                }
                float xv = Main.rand.NextFloat(-3, 3);
                float yv = Main.rand.NextFloat(-2, -5);
                int dust = Dust.NewDust(storedPos + new Vector2(NPC.width / 2, NPC.height / 2), 1, 1, DustID.Electric, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1, 2);
                Lighting.AddLight(storedPos, 0.6f, 0.75f, 0.8f);

                if (AI_Timer % 178 > 126)
                {
                    NPC.alpha += 5;
                }
                else
                {
                    if (AI_Timer % 178 == 126)
                    {
                        NPC.alpha = 255;

                        NPC.position = storedPos;
                    }
                    NPC.alpha -= 5;
                }
            }
            else
            {
                NPC.dontTakeDamage = false;

                if (AI_Timer % 178 == 75)
                {

                    NPC.TargetClosest(true);
                    if (DashPhase == 1)
                    {
                        targetArea = Main.player[NPC.target].Center;
                    }
                    else
                    {
                        if (Main.player[NPC.target].Center.X < NPC.Center.X)
                        {
                            targetArea = Main.player[NPC.target].Center + new Vector2(-450, -300);
                        }
                        else
                        {
                            targetArea = Main.player[NPC.target].Center + new Vector2(450, -300);

                        }
                    }

                    direction = targetArea - NPC.Center;
                    if (direction != new Vector2(0, 0))
                    {
                        direction.Normalize();
                    }
                    else
                    {
                        direction = new Vector2(0, 1);
                    }
                    direction *= 30;

                    storedVel = direction;

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, storedVel / 4, ModContent.ProjectileType<BossTelegraph>(), 0, 0, Main.myPlayer, DustID.AncientLight);

                }
                else if (AI_Timer % 178 > 50)
                {
                    NPC.velocity = new Vector2(0, 0);
                }
                else if (AI_Timer % 178 == 50)
                {
                    NPC.velocity = storedVel;
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                }
                else
                {
                    if (DashPhase == 1)
                    {
                        targetArea = Main.player[NPC.target].Center;
                        float speed = 3f;
                        float inertia = 32f;
                        direction = targetArea - NPC.Center;
                        if (direction != new Vector2(0, 0))
                        {
                            direction.Normalize();
                        }
                        else
                        {
                            direction = new Vector2(0, 1);
                        }
                        direction *= speed;

                        NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                    }
                    else
                    {
                        counter++;
                        if (Main.expertMode)
                        {
                            if (counter % 2 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 0f), Mod.Find<ModProjectile>("EoTSWater2").Type, 17, 0, Main.myPlayer, 0.1f, 5.5f);
                            }
                        }
                        else
                        {
                            if (counter % 3 == 0)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 0f), Mod.Find<ModProjectile>("EoTSWater2").Type, 17, 0, Main.myPlayer, 0.1f, 5.5f);
                                SoundEngine.PlaySound(SoundID.Item21, NPC.Center);
                            }
                        }
                    }
                }
            }
            AI_Timer--;


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionLightning;
                AI_Timer = 170;
            }
        }

        private void Lightning3()
        {
            NPC.takenDamageMultiplier = 0.65f;


            if (AI_Timer % 150 == 0)
            {
                lightningDir = Main.rand.NextFloat(0, 360);
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(4, 4), Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center), Mod.Find<ModProjectile>("EoTSLightningTele").Type, 17, 0, Main.myPlayer);
                Main.projectile[proj].timeLeft = 60;

                if (Main.expertMode)
                {
                    if (Main.masterMode)
                    {
                        for (var j = 0; j < 14; j++)
                        {
                            int proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(4, 4), new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(lightningDir + j * 360 / 14)), Mod.Find<ModProjectile>("EoTSLightningTele").Type, 17, 0, Main.myPlayer);
                            Main.projectile[proj2].timeLeft = 60;
                        }
                    }
                    else
                    {
                        for (var j = 0; j < 12; j++)
                        {
                            int proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(4, 4), new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(lightningDir + j * 360 / 12)), Mod.Find<ModProjectile>("EoTSLightningTele").Type, 17, 0, Main.myPlayer);
                            Main.projectile[proj2].timeLeft = 60;
                        }
                    }
                }
                else
                {
                    for (var j = 0; j < 8; j++)
                    {
                        int proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(4, 4), new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(lightningDir + j * 360 / 8)), Mod.Find<ModProjectile>("EoTSLightningTele").Type, 17, 0, Main.myPlayer);
                        Main.projectile[proj2].timeLeft = 60;
                    }
                }
                
            }
            if (AI_Timer % 150 == 30)
            {
                lightningDir += 360 / 24;
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(4, 4), Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center), Mod.Find<ModProjectile>("EoTSLightningTele").Type, 17, 0, Main.myPlayer);
                Main.projectile[proj].timeLeft = 60;
                if (Main.expertMode)
                {
                    if (Main.masterMode)
                    {
                        for (var j = 0; j < 14; j++)
                        {
                            int proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(4, 4), new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(lightningDir + j * 360 / 14)), Mod.Find<ModProjectile>("EoTSLightningTele").Type, 17, 0, Main.myPlayer);
                            Main.projectile[proj2].timeLeft = 60;
                        }
                    }
                    else
                    {
                        for (var j = 0; j < 12; j++)
                        {
                            int proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(4, 4), new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(lightningDir + j * 360 / 12)), Mod.Find<ModProjectile>("EoTSLightningTele").Type, 17, 0, Main.myPlayer);
                            Main.projectile[proj2].timeLeft = 60;
                        }
                    }
                }
                else
                {
                    for (var j = 0; j < 8; j++)
                    {
                        int proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(4, 4), new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(lightningDir + j * 360 / 8)), Mod.Find<ModProjectile>("EoTSLightningTele").Type, 17, 0, Main.myPlayer);
                        Main.projectile[proj2].timeLeft = 60;
                    }
                }
            }

            AI_Timer++;
            if (AI_Timer == 750)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionToPlayer;
                Next_State = (float)ActionState.CloudAttack3;
                AI_Timer = 170;
            }
        }

        private void CloudAttack3()
        {
            targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
            float speed = Vector2.Distance(NPC.Center, targetArea) / 45f;

            float inertia = 12f;


            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;


            AI_Timer--;

            
            if (AI_Timer % 120 == 45)
            {
                if (Main.expertMode)
                {
                    if (Main.masterMode)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + new Vector2(Main.rand.Next(450, 650), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSCloud2").Type, 17, 0, Main.myPlayer, 60);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + new Vector2(Main.rand.Next(450, 650), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSCloud2").Type, 17, 0, Main.myPlayer, 60);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + new Vector2(Main.rand.Next(450, 650), 0).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(0, 360))), new Vector2(0, 0), Mod.Find<ModProjectile>("EoTSCloud2").Type, 17, 0, Main.myPlayer, 60);
                    }
                }
                

            }


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionToPlayer;
                Next_State = (float)ActionState.Chase3;
                AI_Timer = 170;
            }
        }

        private void Chase3()
        {
            NPC.takenDamageMultiplier = 0.65f;

            targetArea = Main.player[NPC.target].Center + new Vector2(0, -360);
            float speed = Vector2.Distance(NPC.Center, targetArea) - 0.151251f;
            if (speed > 10)
            {
                speed = 10;
            }

            float inertia = 1f;


            direction = targetArea - NPC.Center;
            if (direction != new Vector2(0, 0))
            {
                direction.Normalize();
            }
            else
            {
                direction = new Vector2(0, 1);
            }
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;


            AI_Timer--;
            if (Main.expertMode)
            {
                if (AI_Timer % 14 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f).RotatedBy(MathHelper.ToRadians(24)), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f).RotatedBy(MathHelper.ToRadians(-24)), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                }
            }
            else
            {
                if (AI_Timer % 20 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f).RotatedBy(MathHelper.ToRadians(24)), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(0, 5.5f).RotatedBy(MathHelper.ToRadians(-24)), Mod.Find<ModProjectile>("EoTSSnow").Type, 17, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                }
            }
            


            if (AI_Timer == 0)
            {
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.RepositionCenter;
                Next_State = (float)ActionState.SineSnow3;
                AI_Timer = 170;
            }
        }

    }
}