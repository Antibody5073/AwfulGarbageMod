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
using Terraria.UI;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.NPCs.Boss
{
    // This ModNPC serves as an example of a completely custom AI.
    [AutoloadBossHead]
    public class Candescence : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            FloatTowardsPlayer,
            FireBlast,
            FireGrid,
            Flamethrower,
            FireGridDestroy,
            CircleDash,
            OrbSpinPrepare,
            OrbSpin,
            OrbSpinRetract,
            HomingFireball,
            AlignDash,

            FloatTowardsPlayer2,
            FireBlast2,
            FireGrid2,
            DancingSparks,
            FireGridDestroy2,
            Flamethrower2,
            OrbSpinPrepare2,
            OrbSpin2,
            OrbSpinRetract2,
            FireballSpread,
            HomingFireball2,
            AlignDash2
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
        float spinSpd;
        Vector2 projVel;
        float homingSpd;
        int sineWaterMod;
        float atkdir;
        float atkdir2;
        float atklen;
        float atklen2;
        int atkUseCounter;
        int frame;
        int frameCounter;
        bool rand;
        bool rand2;
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
            Main.npcFrameCount[NPC.type] = 5; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;

            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 64; // The width of the npc's hitbox (in pixels)
            NPC.height = 64; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 66; // The amount of damage that this npc deals
            NPC.defense = 59; // The amount of defense that this npc has
            NPC.lifeMax = 35000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit3; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath52; // The sound the NPC will make when it dies.
            NPC.value = 500000; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.npcSlots = 100f;
            NPC.noTileCollide = true;

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/CandescenceTheme");
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
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.Candescence")
            });
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = 40000;
            if (Main.masterMode)
            {
                NPC.lifeMax = 45000; // Increase by 5 if expert or master mode
                if (Main.getGoodWorld || Main.zenithWorld)
                {
                    NPC.lifeMax = 50000;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CandescenceBag>()));


            // Trophies are spawned with 1/10 chance
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormTrophy>(), 10));

            // ItemDropRule.MasterModeCommonDrop for the relic
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.Boss.EyeOfTheStormRelic>()));

            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            ///npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CandesciteBar>(), 1, 2, 3));
            notExpertRule.OnSuccess(ItemDropRule.FewFromOptions(1, 3, ModContent.ItemType<LavaLance>()));

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

            NPC.lifeMax = NPC.lifeMax * ModContent.GetInstance<Config>().BossHealthMultiplier / 100;
            NPC.life = NPC.lifeMax;
        }

        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedFireMoth, -1);
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
        public override void AI()
        {
            // The npc starts in the asleep state, waiting for a player to enter range

            Player player = Main.player[NPC.target];

            Lighting.AddLight(NPC.Center, Color.Orange.ToVector3() * 2.5f);


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
                AI_State = (float)ActionState.FloatTowardsPlayer;
            }
            if (NPC.life < NPC.lifeMax * 3 / 5 && bossPhase == 1)
            {
                NPC.TargetClosest(true);
                NPC.globalEnemyBossInfo().killOrbitals = true;

                bossPhase = 2;
                AI_Timer = 0;
                AI_State = (float)ActionState.FloatTowardsPlayer2;
            }

            switch (AI_State)
            {
                case (float)ActionState.FloatTowardsPlayer:
                    FloatTowardsPlayer();
                    break;
                case (float)ActionState.FireBlast:
                    FireBlast();
                    break;
                case (float)ActionState.FireGrid:
                    FireGrid();
                    break;
                case (float)ActionState.Flamethrower:
                    Flamethrower();
                    break;
                case (float)ActionState.FireGridDestroy:
                    FireGridDestroy();
                    break;
                case (float)ActionState.CircleDash:
                    CircleDash();
                    break;
                case (float)ActionState.OrbSpinPrepare:
                    OrbSpinPrepare();
                    break;
                case (float)ActionState.OrbSpin:
                    OrbSpin();
                    break;
                case (float)ActionState.OrbSpinRetract:
                    OrbSpinRetract();
                    break;
                case (float)ActionState.HomingFireball:
                    HomingFireball();
                    break;
                case (float)ActionState.AlignDash:
                    AlignDash();
                    break;
                case (float)ActionState.FloatTowardsPlayer2:
                    FloatTowardsPlayer2();
                    break;
                case (float)ActionState.FireBlast2:
                    FireBlast2();
                    break;
                case (float)ActionState.FireGrid2:
                    FireGrid2();
                    break;
                case (float)ActionState.DancingSparks:
                    DancingSparks();
                    break;
                case (float)ActionState.FireGridDestroy2:
                    FireGridDestroy2();
                    break;
                case (float)ActionState.Flamethrower2:
                    Flamethrower2();
                    break;
                case (float)ActionState.OrbSpinPrepare2:
                    OrbSpinPrepare2();
                    break;
                case (float)ActionState.OrbSpin2:
                    OrbSpin2();
                    break;
                case (float)ActionState.OrbSpinRetract2:
                    OrbSpinRetract2();
                    break;
                case (float)ActionState.FireballSpread:
                    FireballSpread();
                    break;
                case (float)ActionState.HomingFireball2:
                    HomingFireball2();
                    break;
                case (float)ActionState.AlignDash2:
                    AlignDash2();
                    break;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.aiStyle == 19 && projectile.CountsAsClass(DamageClass.Melee))
            {
                modifiers.FinalDamage *= 1.3f;
            }
            base.ModifyHitByProjectile(projectile, ref modifiers);
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.CountsAsClass(DamageClass.Melee))
            {
                modifiers.FinalDamage *= 1.3f;
            }
            base.ModifyHitByItem(player, item, ref modifiers);
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


        private void FloatTowardsPlayer()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            NPC.velocity = direction * 0.015f + direction.SafeNormalize(Vector2.Zero) * -5f;
            NPC.dontTakeDamage = true;
            AI_Timer += 1;
            if (AI_Timer == 180)
            {
                NPC.dontTakeDamage = false;
                AI_State = (float)ActionState.FireBlast;
                AI_Timer = 0;
                atkUseCounter = 0;
            }
        }
        private void FireBlast()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.015f + direction.SafeNormalize(Vector2.Zero) * 1f;

            NPC.position += recoil;
            recoil *= 0.98f;

            AI_Timer += 1;
            if (AI_Timer > 0)
            {
                if (Main.expertMode)
                {
                    if (AI_Timer % 60 == 0)
                    {
                        atkUseCounter++;
                        if (atkUseCounter == 7)
                        {
                            if (DifficultyModes.Difficulty > 0)
                            {
                                AI_State = (float)ActionState.FireGrid;
                                AI_Timer = -75;
                                NPC.GetGlobalNPC<GlobalEnemyBossInfo>().killOrbitals = false;

                                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/charge_3")
                                {
                                    Volume = 0.3f,
                                };
                                SoundEngine.PlaySound(impactSound, NPC.Center);
                            }
                            else
                            {
                                AI_State = (float)ActionState.Flamethrower;
                                AI_Timer = 0;
                            }
                            atkUseCounter = 0;
                            playerPos.Clear();
                        }
                        else
                        {
                            recoil = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * -8;
                            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                            int projType = DifficultyModes.Difficulty > 0 ? ModContent.ProjectileType<CandescenceProjFireBlastUnreal>() : ModContent.ProjectileType<CandescenceProjFireBlast>();
                            float projSpd = DifficultyModes.Difficulty > 0 ? 1.5f : 8;

                            if (DifficultyModes.Difficulty == 2)
                            {
                                projSpd = 3f;
                            }
                            if (Main.rand.NextBool())
                            {
                                for (var i = -2; i < 3; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * 18)) * projSpd, projType, 17, 0, Main.myPlayer, 90);
                                }
                            }
                            else
                            {
                                for (var i = -2; i < 2; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * 18 + 9)) * projSpd, projType, 17, 0, Main.myPlayer, 90);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (AI_Timer % 90 == 0)
                    {
                        atkUseCounter++;
                        if (atkUseCounter == 6)
                        {
                            AI_State = (float)ActionState.Flamethrower;
                            AI_Timer = 0;
                            atkUseCounter = 0;
                            playerPos.Clear();
                        }
                        else
                        {
                            recoil = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * -12;
                            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                            if (Main.rand.NextBool())
                            {
                                for (var i = -2; i < 3; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * 18)) * 8, ModContent.ProjectileType<CandescenceProjFireBlast>(), 17, 0, Main.myPlayer);
                                }
                            }
                            else
                            {
                                for (var i = -2; i < 2; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * 18 + 9)) * 8, ModContent.ProjectileType<CandescenceProjFireBlast>(), 17, 0, Main.myPlayer);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void FireGrid()
        {
            Player player = Main.player[NPC.target];

            NPC.velocity = Vector2.Zero;

            NPC.position += recoil;
            recoil *= 0.95f;
            if (AI_Timer < 0)
            {
                int dust = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0, 0, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = new Vector2(6, 0);
                Main.dust[dust].noGravity = true; 
                dust = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0, 0, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = new Vector2(-6, 0);
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0, 0, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = new Vector2(0, 6);
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0, 0, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = new Vector2(0, -6);
                Main.dust[dust].noGravity = true;
            }
            if (AI_Timer == 0)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_3")
                {
                    Volume = 0.3f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);

                float projSpd = DifficultyModes.Difficulty == 2 ? 3 : 4;
                for (var i = 0; i < 24; i++)
                {
                    for (var j = 0; j < 4; j++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * projSpd, 0).RotatedBy(j * MathHelper.PiOver2), ModContent.ProjectileType<CandescenceProjTwinFire>(), 17, 0, Main.myPlayer, NPC.whoAmI, 0, (j + 1) * MathHelper.PiOver2);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * projSpd, 0).RotatedBy(j * MathHelper.PiOver2), ModContent.ProjectileType<CandescenceProjTwinFire>(), 17, 0, Main.myPlayer, NPC.whoAmI, 180, (j - 1) * MathHelper.PiOver2);
                    }
                }
            }
            if (AI_Timer == 60)
            {
                AI_State = (float)ActionState.Flamethrower;
                AI_Timer = 0;
            }
            AI_Timer += 1;
        }
        private void Flamethrower()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.02f + direction.SafeNormalize(Vector2.Zero) * -3.5f;

            AI_Timer += 1;
            playerPos.Add(player.Center);
            if (playerPos.Count > 24)
            {
                playerPos.RemoveAt(0);
            }
            if (AI_Timer % 6 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
            }
            if (AI_Timer % 4 == 0)
            {
                if (AI_Timer > 120)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(playerPos[0] - NPC.Center) * 16, ProjectileID.Flames, 25, 0, Main.myPlayer);
                    Main.projectile[proj].hostile = true;
                    Main.projectile[proj].friendly = false;
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(playerPos[0] - NPC.Center) * 8, ProjectileID.Flames, 25, 0, Main.myPlayer);
                    Main.projectile[proj].hostile = true;
                    Main.projectile[proj].friendly = false;
                    if (Main.expertMode)
                    {
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(playerPos[0] - NPC.Center) * -16, ProjectileID.Flames, 25, 0, Main.myPlayer);
                        Main.projectile[proj].hostile = true;
                        Main.projectile[proj].friendly = false;
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(playerPos[0] - NPC.Center) * -8, ProjectileID.Flames, 25, 0, Main.myPlayer);
                        Main.projectile[proj].hostile = true;
                        Main.projectile[proj].friendly = false;
                    }
                }
                else
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(playerPos[0] - NPC.Center) * 15 * (AI_Timer / 120), ProjectileID.Flames, 5, 0, Main.myPlayer);
                    Main.projectile[proj].hostile = true;
                    Main.projectile[proj].friendly = false;
                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(playerPos[0] - NPC.Center) * 7.5f * (AI_Timer / 120), ProjectileID.Flames, 5, 0, Main.myPlayer);
                    Main.projectile[proj].hostile = true;
                    Main.projectile[proj].friendly = false;
                    if (Main.expertMode)
                    {
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(playerPos[0] - NPC.Center) * -15 * (AI_Timer / 120), ProjectileID.Flames, 5, 0, Main.myPlayer);
                        Main.projectile[proj].hostile = true;
                        Main.projectile[proj].friendly = false;
                        proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(playerPos[0] - NPC.Center) * -7.5f * (AI_Timer / 120), ProjectileID.Flames, 5, 0, Main.myPlayer);
                        Main.projectile[proj].hostile = true;
                        Main.projectile[proj].friendly = false;
                    }
                }
            }
            if (AI_Timer == 420)
            {
                if (DifficultyModes.Difficulty > 0)
                {
                    AI_State = (float)ActionState.FireGridDestroy;
                    AI_Timer = -60;
                }
                else
                {
                    AI_State = (float)ActionState.CircleDash;
                    AI_Timer = 0;
                }
                atkUseCounter = 0;
            }
        }
        private void FireGridDestroy()
        {
            Player player = Main.player[NPC.target];

            NPC.velocity = Vector2.Zero;
            if (AI_Timer == 0)
            {
                NPC.GetGlobalNPC<GlobalEnemyBossInfo>().killOrbitals = true;

                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/hone_shot")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
            }
            if (AI_Timer == 30)
            {
                AI_State = (float)ActionState.CircleDash;
                AI_Timer = 0;
            }
            AI_Timer += 1;
        }
        private void CircleDash()
        {
            if (AI_Timer < 60)
            {
                Player player = Main.player[NPC.target];

                Vector2 direction = player.Center - NPC.Center;
                direction.SafeNormalize(Vector2.Zero);
                NPC.velocity = direction * 0.03f + direction.SafeNormalize(Vector2.Zero) * -6f;
            }
            else if (AI_Timer == 60)
            {
                NPC.TargetClosest(true);
                targetArea = Main.player[NPC.target].Center;
                direction = targetArea - NPC.Center;

                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                {
                    Volume = 0.4f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
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
                    direction *= 27;
                }
                storedVel = direction;
                NPC.velocity = new Vector2(0, 0);

            }
            else if (AI_Timer == 90)
            {
                atkUseCounter++;
                NPC.velocity = storedVel;
                if (Main.expertMode)
                {
                    if (DifficultyModes.Difficulty > 0)
                    {
                        for (var i = 0; i < 7; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedByRandom(MathHelper.TwoPi) * 1f, ModContent.ProjectileType<CandescenceProjFireBlastUnreal>(), 17, 0, Main.myPlayer, 75);
                        }
                    }
                    else
                    {
                        for (var i = 0; i < 7; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedByRandom(MathHelper.TwoPi) * 1, ModContent.ProjectileType<CandescenceProjFireball>(), 17, 0, Main.myPlayer, 0, 0, 0.25f);
                        }
                    }
                }
                projVel = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedByRandom(MathHelper.TwoPi) * 1;

                if (DifficultyModes.Difficulty > 0)
                {
                    if (DifficultyModes.Difficulty == 2)
                    {
                        for (var i = 0; i < 16; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(i * MathHelper.TwoPi / 16) * 1f, ModContent.ProjectileType<CandescenceProjFireBlastUnreal>(), 17, 0, Main.myPlayer, 75);
                        }
                    }
                    else
                    {
                        for (var i = 0; i < 12; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(i * MathHelper.TwoPi / 12) * 1f, ModContent.ProjectileType<CandescenceProjFireBlastUnreal>(), 17, 0, Main.myPlayer, 75);
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < 10; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(i * MathHelper.TwoPi / 10), ModContent.ProjectileType<CandescenceProjFireball>(), 17, 0, Main.myPlayer, 0, 0, 0.25f);
                    }
                }
                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
            }
            else if (AI_Timer > 90 && AI_Timer < 120)
            {
                NPC.velocity *= 0.96f;
                if (AI_Timer % 5 == 0)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(NPC.velocity) * 10, ProjectileID.Flames, 25, 0, Main.myPlayer);
                    Main.projectile[proj].hostile = true;
                    Main.projectile[proj].friendly = false;
                }
            }
            else if (AI_Timer == 120)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(NPC.velocity) * 18, ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.ToRadians(12)) * 18, ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.ToRadians(-12)) * 18, ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.ToRadians(24)) * 18, ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.ToRadians(-24)) * 18, ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.ToRadians(36)) * 18, ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(NPC.velocity).RotatedBy(MathHelper.ToRadians(-36)) * 18, ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
                if (atkUseCounter == 4)
                {

                    targetArea = Main.player[NPC.target].Center;
                    direction = targetArea - NPC.Center;

                    NPC.globalEnemyBossInfo().OrbitDirection = MathHelper.ToDegrees(direction.ToRotation());
                    NPC.globalEnemyBossInfo().OrbitDistance = 0;
                    NPC.globalEnemyBossInfo().killOrbitals = false;
                    NPC.globalEnemyBossInfo().orbitalsDealDamage = true;

                    if (Main.rand.NextBool())
                    {
                        spinSpd = 1.5f;
                    }
                    else
                    {
                        spinSpd = -1.5f;
                    }
                    for (var i = 1; i <= 20; i += 1)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CandescenceProjLavaOrb>(), 17, 0, Main.myPlayer, NPC.whoAmI, 0, (float)i / 20);
                    }
                    NPC.velocity = new Vector2(0, 0);
                    AI_State = (float)ActionState.OrbSpinPrepare;
                    AI_Timer = 0;
                    atkUseCounter = 0;

                }
                AI_Timer = 0;
            }
            AI_Timer++;
        }
        private void OrbSpinPrepare()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDistance += 2f;
            NPC.globalEnemyBossInfo().OrbitDistance *= 1.02f;

            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.015f + direction.SafeNormalize(Vector2.Zero) * -3f;
            if (AI_Timer == 150)
            {
                AI_State = (float)ActionState.OrbSpin;
                AI_Timer = 0;
            }
        }
        private void OrbSpin()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;

            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.01f + direction.SafeNormalize(Vector2.Zero) * 1.5f;

            if (AI_Timer == 360)
            {
                AI_State = (float)ActionState.OrbSpinRetract;
                AI_Timer = 0;
            }
            if (Main.expertMode && AI_Timer % 6 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 8), ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -8), ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
            }
            if (DifficultyModes.Difficulty == 1)
            {
                if (AI_Timer % 45 == 0)
                {

                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                    {
                        Volume = 0.4f,
                    };
                    SoundEngine.PlaySound(impactSound, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, player.Center - NPC.Center, ModContent.ProjectileType<CandescenceProjWarning>(), 22, 0, Main.myPlayer, NPC.whoAmI, 0);
                }
            }
            if (DifficultyModes.Difficulty == 2)
            {
                if (AI_Timer % 15 == 0)
                {

                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                    {
                        Volume = 0.4f,
                    };
                    SoundEngine.PlaySound(impactSound, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, player.Center - NPC.Center + Main.rand.NextVector2Circular(128, 128), ModContent.ProjectileType<CandescenceProjWarning>(), 22, 0, Main.myPlayer, NPC.whoAmI, 0);
                }
            }
        }
        private void OrbSpinRetract()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDistance *= 0.9f;
            NPC.globalEnemyBossInfo().OrbitDistance -= 1;

            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.015f + direction.SafeNormalize(Vector2.Zero) * 1.5f;
            if (NPC.globalEnemyBossInfo().OrbitDistance <= 0)
            {
                NPC.globalEnemyBossInfo().killOrbitals = true;
                AI_State = (float)ActionState.HomingFireball;
                AI_Timer = 0;
                atkUseCounter = 0;
            }
        }
        private void HomingFireball()
        {
            NPC.velocity = Vector2.Zero;
            if (AI_Timer == 0)
            {
                Player player = Main.player[NPC.target];

                Vector2 direction = player.Center - NPC.Center;
                projVel = direction.SafeNormalize(Vector2.Zero);
                homingSpd = 0.15f;
                if (DifficultyModes.Difficulty > 0)
                {
                    projVel = projVel.RotatedBy(MathHelper.PiOver2);
                }
            }
            if (AI_Timer > 60 && AI_Timer < 300)
            {
                if (AI_Timer % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(MathHelper.PiOver2) * 12, ModContent.ProjectileType<CandescenceProjHomingFireball>(), 15, 0, Main.myPlayer, homingSpd);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(-MathHelper.PiOver2) * 12, ModContent.ProjectileType<CandescenceProjHomingFireball>(), 15, 0, Main.myPlayer, homingSpd);
                    if (DifficultyModes.Difficulty == 2)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel * 12, ModContent.ProjectileType<CandescenceProjHomingFireball>(), 15, 0, Main.myPlayer, homingSpd);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(MathHelper.Pi) * 12, ModContent.ProjectileType<CandescenceProjHomingFireball>(), 15, 0, Main.myPlayer, homingSpd);
                    }
                    homingSpd += 0.075f;
                    if (DifficultyModes.Difficulty == 2)
                    {
                        homingSpd += 0.01f;
                    }
                }
            }
            if (AI_Timer == 360)
            {
                AI_State = (float)ActionState.AlignDash;
                AI_Timer = 0;
                atkUseCounter = 0;
                atkdir = MathHelper.PiOver2;
            }
            AI_Timer++;
        }
        private void AlignDash()
        {
            Player player = Main.player[NPC.target];
            if (AI_Timer == 0)
            {
                atkdir += Main.rand.Next(1, 4) * MathHelper.PiOver2;
            }
            if (AI_Timer < 60)
            {
                float speed = Vector2.Distance(NPC.Center, player.Center + new Vector2(240, 0).RotatedBy(atkdir)) / (31f - AI_Timer * 0.5f);
                float inertia = (31f - AI_Timer * 0.5f);
                Vector2 direction = Main.player[NPC.target].Center + new Vector2(240, 0).RotatedBy(atkdir) - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
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
            }
            if (AI_Timer == 60)
            {
                NPC.GetGlobalNPC<GlobalEnemyBossInfo>().killOrbitals = false;

                if (DifficultyModes.Difficulty > 0)
                {
                    float projSpd = DifficultyModes.Difficulty == 2 ? 5.5f : 10;
                    for (var i = -23; i < 24; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * projSpd, 0).RotatedBy(atkdir + MathHelper.PiOver2), ModContent.ProjectileType<CandescenceProjTwinFire>(), 17, 0, Main.myPlayer, NPC.whoAmI, 0, atkdir);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * projSpd, 0).RotatedBy(atkdir + MathHelper.PiOver2), ModContent.ProjectileType<CandescenceProjTwinFire>(), 17, 0, Main.myPlayer, NPC.whoAmI, 180, atkdir + MathHelper.Pi);
                    }
                }
                NPC.Center = player.Center + new Vector2(240, 0).RotatedBy(atkdir);
                NPC.velocity = Vector2.Zero;
                storedVel = new Vector2(-22, 0).RotatedBy(atkdir);

                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                {
                    Volume = 0.4f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
            }
            if (AI_Timer == 70)
            {
                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);
                NPC.GetGlobalNPC<GlobalEnemyBossInfo>().killOrbitals = true;

                NPC.velocity = storedVel;
            }
            if (AI_Timer == 100)
            {
                AI_Timer = -1;
                atkUseCounter++;
                if (atkUseCounter == 6)
                {
                    recoil = NPC.velocity;
                    AI_State = (float)ActionState.FireBlast;
                    AI_Timer = -90;
                    atkUseCounter = 0;
                }
            }
            AI_Timer++;
        }

        private void FloatTowardsPlayer2()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            NPC.velocity = direction * 0.015f + direction.SafeNormalize(Vector2.Zero) * -5f;
            NPC.dontTakeDamage = true;
            AI_Timer += 1;
            if (AI_Timer == 180)
            {
                NPC.dontTakeDamage = false;
                AI_State = (float)ActionState.FireBlast2;
                AI_Timer = 0;
                atkUseCounter = 0;
            }
        }
        private void FireBlast2()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.0175f + direction.SafeNormalize(Vector2.Zero) * -1f;

            NPC.position += recoil;
            recoil *= 0.98f;

            AI_Timer += 1;
            if (AI_Timer > 58)
            {
                if (Main.expertMode)
                {
                    int timeBetweenAttack = 90;
                    if (AI_Timer % timeBetweenAttack == 0)
                    {
                        atkUseCounter++;
                        if (atkUseCounter == 10)
                        {
                            if (DifficultyModes.Difficulty > 0)
                            {
                                AI_State = (float)ActionState.FireGrid2;
                                AI_Timer = -75;
                                NPC.GetGlobalNPC<GlobalEnemyBossInfo>().killOrbitals = false;

                                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/charge_3")
                                {
                                    Volume = 0.3f,
                                };
                                SoundEngine.PlaySound(impactSound, NPC.Center);
                            }
                            else
                            {
                                AI_State = (float)ActionState.DancingSparks;
                                AI_Timer = 0;
                            }

                            atkUseCounter = 0;
                            playerPos.Clear();
                        }
                        else
                        {
                            rand = Main.rand.NextBool();
                            rand2 = Main.rand.NextBool(2, 5);
                            if (didTheAttackAlready)
                            {
                                rand2 = false;
                            }
                            if (rand2)
                            {
                                didTheAttackAlready = true;
                            }
                            else
                            {
                                didTheAttackAlready = false;
                            }
                        }
                    }
                    if (AI_State == (float)ActionState.FireBlast2)
                    {
                        if (rand2)
                        {
                            if (AI_Timer % timeBetweenAttack < 20)
                            {
                                recoil = Vector2.Zero;
                                NPC.velocity = Vector2.Zero;
                                if (AI_Timer % 2 == 0)
                                {
                                    if (rand)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians((AI_Timer % timeBetweenAttack) * 2.35f - 34f)) * 8, ModContent.ProjectileType<CandescenceProjFireBlast>(), 17, 0, Main.myPlayer);
                                    }
                                    else
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians((AI_Timer % timeBetweenAttack) * -2.35f + 34f)) * 8, ModContent.ProjectileType<CandescenceProjFireBlast>(), 17, 0, Main.myPlayer);
                                    }
                                }
                            }
                            if (AI_Timer % timeBetweenAttack == 20)
                            {
                                recoil = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * -8f;

                            }
                        }
                        else
                        {
                            if (AI_Timer % timeBetweenAttack == 0 || AI_Timer % timeBetweenAttack == 8 || AI_Timer % timeBetweenAttack == 16)
                            {
                                if (AI_Timer % timeBetweenAttack == 0)
                                {
                                    storedVel = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);
                                }
                                recoil = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * -6f;
                                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                                if (rand)
                                {
                                    for (var i = -2; i < 3; i++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, storedVel.RotatedBy(MathHelper.ToRadians(i * 20)) * 8, ModContent.ProjectileType<CandescenceProjFireBlast>(), 17, 0, Main.myPlayer);
                                    }
                                }
                                else
                                {
                                    for (var i = -3; i < 3; i++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, storedVel.RotatedBy(MathHelper.ToRadians(i * 20 + 10)) * 8, ModContent.ProjectileType<CandescenceProjFireBlast>(), 17, 0, Main.myPlayer);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (AI_Timer % 60 == 0)
                    {
                        atkUseCounter++;
                        if (atkUseCounter == 7)
                        {
                            AI_State = (float)ActionState.DancingSparks;
                            AI_Timer = 0;
                            atkUseCounter = 0;
                            playerPos.Clear();
                        }
                        else
                        {
                            recoil = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * -6;
                            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
                            if (Main.rand.NextBool())
                            {
                                for (var i = -2; i < 3; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * 18)) * 8, ModContent.ProjectileType<CandescenceProjFireBlast>(), 17, 0, Main.myPlayer);
                                }
                            }
                            else
                            {
                                for (var i = -2; i < 2; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * 18 + 9)) * 8, ModContent.ProjectileType<CandescenceProjFireBlast>(), 17, 0, Main.myPlayer);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void FireGrid2()
        {
            Player player = Main.player[NPC.target];

            NPC.velocity = Vector2.Zero;

            NPC.position += recoil;
            recoil *= 0.95f;
            if (AI_Timer < 0)
            {
                int dust = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0, 0, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = new Vector2(6, 0);
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0, 0, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = new Vector2(-6, 0);
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0, 0, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = new Vector2(0, 6);
                Main.dust[dust].noGravity = true;
                dust = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0, 0, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = new Vector2(0, -6);
                Main.dust[dust].noGravity = true;
            }
            if (AI_Timer == 0)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_3")
                {
                    Volume = 0.3f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);

                float projSpd = DifficultyModes.Difficulty == 2 ? 3 : 4;
                for (var i = 0; i < 24; i++)
                {
                    for (var j = 0; j < 4; j++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * projSpd, 0).RotatedBy(j * MathHelper.PiOver2), ModContent.ProjectileType<CandescenceProjTwinFire>(), 17, 0, Main.myPlayer, NPC.whoAmI, 0, (j + 1) * MathHelper.PiOver2);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * projSpd, 0).RotatedBy(j * MathHelper.PiOver2), ModContent.ProjectileType<CandescenceProjTwinFire>(), 17, 0, Main.myPlayer, NPC.whoAmI, 180, (j - 1) * MathHelper.PiOver2);
                    }
                }
            }
            if (AI_Timer == 60)
            {
                AI_State = (float)ActionState.DancingSparks;
                AI_Timer = 0;
            }
            AI_Timer += 1;
        }
        private void DancingSparks()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.02f + direction.SafeNormalize(Vector2.Zero) * -3.5f;

            AI_Timer += 1;
            playerPos.Add(player.Center);
            if (playerPos.Count > 24)
            {
                playerPos.RemoveAt(0);
            }

            if (AI_Timer == 1)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/charge_3")
                {
                    Volume = 0.3f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
            }
            if (AI_Timer == 120 || AI_Timer == 160)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/burst_02")
                {
                    Volume = 0.19f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
            }
            if (AI_Timer > 120 && AI_Timer < 420)
            {
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(10, 10), Vector2.Normalize(playerPos[0] - NPC.Center).RotatedByRandom(MathHelper.ToRadians(5)) * 6, ModContent.ProjectileType<DancingSparksProj>(), 24, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
            }
            if (AI_Timer < 120)
            {
                int dust = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0, 0, 0, default(Color), 1f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = Vector2.Normalize(playerPos[0] - NPC.Center).RotatedByRandom(MathHelper.ToRadians(15)) * 15;
                Main.dust[dust].noGravity = true;
            }

            if (AI_Timer == 480)
            {
                if (DifficultyModes.Difficulty > 0)
                {
                    AI_State = (float)ActionState.FireGridDestroy2;
                    AI_Timer = -60;
                }
                else
                {
                    AI_State = (float)ActionState.Flamethrower2;
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atklen = 0;
                    atklen2 = 0;
                }
            }
        }
        private void FireGridDestroy2()
        {
            Player player = Main.player[NPC.target];

            NPC.velocity = Vector2.Zero;
            if (AI_Timer == 0)
            {
                NPC.GetGlobalNPC<GlobalEnemyBossInfo>().killOrbitals = true;

                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/hone_shot")
                {
                    Volume = 0.5f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
            }
            if (AI_Timer == 30)
            {
                AI_State = (float)ActionState.Flamethrower2;
                AI_Timer = 0;
                atkUseCounter = 0;
                atklen = 0;
                atklen2 = 0;
            }
            AI_Timer += 1;
        }
        private void Flamethrower2()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.04f + direction.SafeNormalize(Vector2.Zero) * -8f;

            AI_Timer += 1;
            if (AI_Timer % 6 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);
            }
            if (AI_Timer % 360 == 0)
            {
                atkdir = Main.rand.NextFloat(0, 360);
                atklen = 0;
            }
            if (AI_Timer % 360 < 180)
            {
                atklen2 -= 0.4f;
                atklen += 0.1f;
                if (atklen > 16)
                {
                    atklen = 16;
                }
            }
            if (AI_Timer % 360 == 180)
            {
                atklen2 = 0;
                atkdir2 = Main.rand.NextFloat(0, 360);
            }
            if (AI_Timer % 360 >= 180)
            {
                atklen2 += 0.1f;
                atklen -= 0.4f;
                if (atklen2 > 16)
                {
                    atklen2 = 16;
                }
            }

            atkdir += 0.95f;
            atkdir2 -= 0.95f;

            if (AI_Timer % 4 == 0)
            {
                for (var i = 0; i < 3; i++)
                {
                    if (atklen > 0)
                    {
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(atklen, 0).RotatedBy(MathHelper.ToRadians(atkdir + (i * 120))), ProjectileID.Flames, 25, 0, Main.myPlayer);
                        Main.projectile[proj].hostile = true;
                        Main.projectile[proj].friendly = false;
                    }
                    if (atklen2 > 0)
                    {
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(atklen2, 0).RotatedBy(MathHelper.ToRadians(atkdir2 + (i * 120))), ProjectileID.Flames, 25, 0, Main.myPlayer);
                        Main.projectile[proj].hostile = true;
                        Main.projectile[proj].friendly = false;
                    }
                }

            }
            if (DifficultyModes.Difficulty == 1)
            {
                if (AI_Timer % 60 == 0)
                {

                    SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                    {
                        Volume = 0.4f,
                    };
                    SoundEngine.PlaySound(impactSound, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, player.Center - NPC.Center, ModContent.ProjectileType<CandescenceProjWarning>(), 22, 0, Main.myPlayer, NPC.whoAmI, 0);
                }
            }
            if (AI_Timer == 540)
            {
                targetArea = Main.player[NPC.target].Center;
                direction = targetArea - NPC.Center;

                atkdir = MathHelper.ToDegrees(direction.ToRotation());
                NPC.globalEnemyBossInfo().OrbitDirection = 0;
                NPC.globalEnemyBossInfo().OrbitDistance = 0;
                NPC.globalEnemyBossInfo().killOrbitals = false;
                NPC.globalEnemyBossInfo().orbitalsDealDamage = true;

                if (Main.rand.NextBool())
                {
                    spinSpd = 1f;
                }
                else
                {
                    spinSpd = -1f;
                }
                for (var i = 1; i <= 20; i += 1)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CandescenceProjLavaOrb>(), 17, 0, Main.myPlayer, NPC.whoAmI, atkdir, (float)i / 20);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CandescenceProjLavaOrb>(), 17, 0, Main.myPlayer, NPC.whoAmI, atkdir + 180, (float)i / 20);

                }
                for (var i = 1; i <= 20; i += 1)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CandescenceProjLavaOrb2>(), 17, 0, Main.myPlayer, NPC.whoAmI, atkdir, (float)i / 20 - 1f / 40);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CandescenceProjLavaOrb2>(), 17, 0, Main.myPlayer, NPC.whoAmI, atkdir + 180, (float)i / 20 - 1f / 40);
                }
                NPC.velocity = new Vector2(0, 0);
                AI_State = (float)ActionState.OrbSpinPrepare2;
                AI_Timer = 0;
                atkUseCounter = 0;
            }
        }
        private void OrbSpinPrepare2()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDistance += 2f;
            NPC.globalEnemyBossInfo().OrbitDistance *= 1.02f;

            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.015f + direction.SafeNormalize(Vector2.Zero) * -3f;
            if (AI_Timer == 150)
            {
                AI_State = (float)ActionState.OrbSpin2;
                AI_Timer = 0;
            }
        }
        private void OrbSpin2()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDirection += spinSpd;

            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.01f + direction.SafeNormalize(Vector2.Zero) * 1f;

            if (AI_Timer == 360)
            {
                AI_State = (float)ActionState.OrbSpinRetract2;
                AI_Timer = 0;
            }
            if (Main.expertMode && AI_Timer % 5 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 8), ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -8), ProjectileID.Flames, 22, 0, Main.myPlayer);
                Main.projectile[proj].hostile = true;
                Main.projectile[proj].friendly = false;
            }
        }
        private void OrbSpinRetract2()
        {
            AI_Timer++;
            NPC.globalEnemyBossInfo().OrbitDistance *= 0.9f;
            NPC.globalEnemyBossInfo().OrbitDistance -= 1;

            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity = direction * 0.015f + direction.SafeNormalize(Vector2.Zero) * 1.5f;
            if (NPC.globalEnemyBossInfo().OrbitDistance <= 0)
            {
                NPC.globalEnemyBossInfo().killOrbitals = true;
                AI_State = (float)ActionState.FireballSpread;
                AI_Timer = 0;
                atkUseCounter = 0;
                NPC.velocity = Vector2.Zero;
            }
        }
        private void FireballSpread()
        {
            AI_Timer++;

            if (AI_Timer > 60)
            {
                SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                if (AI_Timer % 30 == 0)
                {

                    if (AI_Timer == 90)
                    {
                        targetArea = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center);
                    }
                    for (var i = -9; i < 9; i++)
                    {
                        Vector2 vel = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * 7 + 3.5f));
                        if ((targetArea - vel).LengthSquared() > 0.002f)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel * 2.5f, ModContent.ProjectileType<CandescenceProjFireballSlow>(), 17, 0, Main.myPlayer);
                        }
                    }
                    if (Main.expertMode)
                    {
                        for (var i = 0; i < 15; i++)
                        {
                            Vector2 vel = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedByRandom(MathHelper.ToRadians(75));
                            if ((targetArea - vel).LengthSquared() > 0.002f)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel * 2.5f, ModContent.ProjectileType<CandescenceProjFireballSlow>(), 17, 0, Main.myPlayer);
                            }
                        }
                    }
                    targetArea = targetArea.RotatedByRandom(MathHelper.ToRadians(15));
                }
            }
            if (AI_Timer == 180)
            {
                AI_State = (float)ActionState.HomingFireball2;
                AI_Timer = 0;
            }
        }
        private void HomingFireball2()
        {
            NPC.velocity = Vector2.Zero;
            if (AI_Timer == 0)
            {
                Player player = Main.player[NPC.target];

                Vector2 direction = player.Center - NPC.Center;
                projVel = direction.SafeNormalize(Vector2.Zero);
                homingSpd = 27f;
            }
            if (AI_Timer > 30 && AI_Timer < 480)
            {
                if (AI_Timer % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(MathHelper.PiOver2) * homingSpd, ModContent.ProjectileType<CandescenceProjHomingFireball>(), 15, 0, Main.myPlayer, 2);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(-MathHelper.PiOver2) * homingSpd, ModContent.ProjectileType<CandescenceProjHomingFireball>(), 15, 0, Main.myPlayer, 2);
                    if (AI_Timer > 180)
                    {
                        homingSpd -= 0.42f;
                    }
                }
            }
            if (AI_Timer == 600)
            {
                AI_State = (float)ActionState.AlignDash2;
                AI_Timer = 0;
                atkUseCounter = 0;
                atkdir = MathHelper.PiOver2;
            }
            AI_Timer++;
        }
        private void AlignDash2()
        {
            Player player = Main.player[NPC.target];
            if (AI_Timer == 0)
            {
                atkdir += Main.rand.Next(1, 4) * MathHelper.PiOver2;
            }
            if (AI_Timer == 1)
            {
                atkdir2 = Main.rand.NextBool() ? -90 : 90;

            }
            if (AI_Timer < 30)
            {
                float speed = Vector2.Distance(NPC.Center, player.Center + new Vector2(240, 0).RotatedBy(atkdir)) / (16f - AI_Timer * 0.5f);
                float inertia = (16f - AI_Timer * 0.5f);
                Vector2 direction = Main.player[NPC.target].Center + new Vector2(240, 0).RotatedBy(atkdir) - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
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
            }
            if (AI_Timer == 20)
            {
                storedVel = new Vector2(-25, 0).RotatedBy(atkdir);
            }
            if (AI_Timer == 30)
            {
                NPC.GetGlobalNPC<GlobalEnemyBossInfo>().killOrbitals = false;

                if (DifficultyModes.Difficulty > 0)
                {
                    float projSpd = DifficultyModes.Difficulty == 2 ? 2f : 4f;
                    for (var i = -23; i < 24; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * projSpd, 0).RotatedBy(atkdir + MathHelper.PiOver2), ModContent.ProjectileType<CandescenceProjTwinFire>(), 17, 0, Main.myPlayer, NPC.whoAmI, 0, atkdir);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * projSpd, 0).RotatedBy(atkdir + MathHelper.PiOver2), ModContent.ProjectileType<CandescenceProjTwinFire>(), 17, 0, Main.myPlayer, NPC.whoAmI, 180, atkdir + MathHelper.Pi);
                    }
                }
                NPC.Center = player.Center + new Vector2(240, 0).RotatedBy(atkdir);
                NPC.velocity = Vector2.Zero;
                storedVel = new Vector2(-20, 0).RotatedBy(atkdir);

                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/warning")
                {
                    Volume = 0.4f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
            }
            if (AI_Timer > 20 && AI_Timer < 100)
            {
                if (AI_Timer % 3 == 0)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, storedVel.RotatedBy(MathHelper.ToRadians(atkdir2)) * 0.6f, ProjectileID.Flames, 25, 0, Main.myPlayer);
                    Main.projectile[proj].hostile = true;
                    Main.projectile[proj].friendly = false;
                    Main.projectile[proj].tileCollide = false;
                }
            }
            if (AI_Timer == 70)
            {
                NPC.GetGlobalNPC<GlobalEnemyBossInfo>().killOrbitals = true;

                SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center);

                NPC.velocity = storedVel;
            }
            if (AI_Timer == 100)
            {
                AI_Timer = -1;
                atkUseCounter++;
                if (atkUseCounter == 9)
                {
                    recoil = NPC.velocity;
                    AI_State = (float)ActionState.FireBlast2;
                    AI_Timer = -90;
                    atkUseCounter = 0;
                }
            }
            AI_Timer++;
        }

    }
}