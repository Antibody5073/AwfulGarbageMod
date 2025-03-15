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
using System.ComponentModel.Design;

namespace AwfulGarbageMod.NPCs.Boss
{
    // This ModNPC serves as an example of a completely custom AI.
    [AutoloadBossHead]
    public class SpiritReaper : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            FloatTowardsPlayer,
            ScytheHover,
            ScytheThrow,
            ScytheChase,
            ScytheHover2,
            ScytheThrow2,
            SpiritSummon,

            Midphase1,

            ScytheHover3,
            ScytheThrow3,
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
        Vector2 projVel;
        int atkUseCounter;
        int frame;
        int frameCounter;
        int spinDirection;
        bool holdingScythe;
        float scytheDirection;
        float scytheVisualDirection;
        float scytheSmoothness;
        bool altAtk;
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
            NPC.width = 96; // The width of the npc's hitbox (in pixels)
            NPC.height = 96; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 80; // The amount of damage that this npc deals
            NPC.defense = 40; // The amount of defense that this npc has
            NPC.lifeMax = 600000; // The amount of health that this npc has
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.Candescence")
            });
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = 800000;
            if (Main.masterMode)
            {
                NPC.lifeMax = 1000000; // Increase by 5 if expert or master mode
                if (Main.getGoodWorld || Main.zenithWorld)
                {
                    NPC.lifeMax = 1500000;
                }
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (ProjectileID.Sets.CultistIsResistantTo[projectile.type])
            {
                modifiers.FinalDamage *= 0.7f;
            }
            base.ModifyHitByProjectile(projectile, ref modifiers);
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

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CandesciteOre>(), 1, 69, 95));

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
            potionType = ItemID.SuperHealingPotion;
        }
        public override void OnSpawn(IEntitySource source)
        {

            NPC.lifeMax = NPC.lifeMax * ModContent.GetInstance<Config>().BossHealthMultiplier / 100;
            NPC.life = NPC.lifeMax;
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {

            npcHitbox.Width = 64;
            npcHitbox.Height = 64;
            return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
        }
        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedEyeOfTheStorm, -1);
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

            Lighting.AddLight(NPC.Center, Color.GhostWhite.ToVector3() * 6.8f);


            if (player.dead)
            {
                NPC.position.Y += 999;
                NPC.EncourageDespawn(0);
                return;
            }

            if (NPC.CountNPCS(NPCID.DungeonSpirit) < 1)
            {
                NPC.dontTakeDamage = false;
            }
            else
            {
                NPC.dontTakeDamage = true;
            }

            if (bossPhase == 0)
            {
                NPC.TargetClosest(true);
                bossPhase = 1;
                AI_Timer = 0;
                AI_State = (float)ActionState.FloatTowardsPlayer;
                holdingScythe = true;
                scytheDirection = 0;
                scytheSmoothness = 1;
                scytheVisualDirection = 0;
            }

            if (NPC.life < NPC.lifeMax * 2 / 3 && bossPhase == 1)
            {
                NPC.TargetClosest(true);
                NPC.globalEnemyBossInfo().killOrbitals = true;

                bossPhase = 2;
                AI_Timer = 0;
                AI_State = (float)ActionState.ScytheHover3;

                holdingScythe = true;
                scytheDirection = 0;
                scytheSmoothness = 1;
                scytheVisualDirection = 0;

                altAtk = Main.rand.NextBool();
            }

            switch (AI_State)
            {
                case (float)ActionState.FloatTowardsPlayer:
                    FloatTowardsPlayer();
                    break;
                case (float)ActionState.ScytheHover:
                    ScytheHover();
                    break;
                case (float)ActionState.ScytheThrow:
                    ScytheThrow();
                    break;
                case (float)ActionState.ScytheChase:
                    ScytheChase();
                    break;
                case (float)ActionState.ScytheHover2:
                    ScytheHover();
                    break;
                case (float)ActionState.ScytheThrow2:
                    ScytheThrow();
                    break;
                case (float)ActionState.SpiritSummon:
                    SpiritSummon();
                    break;
                case (float)ActionState.ScytheHover3:
                    ScytheHover3();
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
            scytheVisualDirection += AGUtils.TurnTowards(180, Vector2.One.RotatedBy(scytheDirection), scytheVisualDirection, Vector2.Zero, scytheSmoothness);

            Texture2D texture = (Texture2D)TextureAssets.Item[ItemID.DeathSickle];
            if (holdingScythe)
            {
                Rectangle rectangle25 = new Rectangle(0, 0, texture.Width, texture.Height);
                Vector2 origin33 = rectangle25.Size() / 2f;
                Color alpha13 = NPC.GetAlpha(drawColor);
                Main.EntitySpriteDraw(texture, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY) + Vector2.One.RotatedBy(scytheVisualDirection - MathHelper.PiOver4) * 25 + Vector2.One.RotatedBy(scytheVisualDirection - 3 * MathHelper.PiOver4) * 65, rectangle25, alpha13, scytheVisualDirection - MathHelper.PiOver2, origin33, 1.5f, SpriteEffects.None);
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
            AI_Timer += 1;
            if (AI_Timer == 60)
            {
                NPC.dontTakeDamage = false;
                AI_State = (float)ActionState.ScytheHover;
                AI_Timer = 0;
                atkUseCounter = 0;
                if (Main.rand.NextBool())
                {
                    spinDirection = 1;
                }
                else
                {
                    spinDirection = -1;
                }
                Vector2 direction2 = player.Center - NPC.Center;
                targetArea = player.Center + direction2.SafeNormalize(Vector2.Zero) * -320;
            }
        }
        private void ScytheHover()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = targetArea - NPC.Center;
            NPC.velocity = direction * 0.07f;
            NPC.velocity += player.velocity * 3/4;

            AI_Timer += 1;
            if (AI_Timer > 0)
            {
                Vector2 direction2 = player.Center - NPC.Center;
                targetArea = player.Center + direction2.SafeNormalize(Vector2.Zero) * -320 + direction2.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(100 * spinDirection)) * 144f;

                Vector2 scythedir = player.Center - NPC.Center;
                scytheDirection = scythedir.ToRotation();
                if (AI_Timer % 60 < 40)
                {

                    scytheDirection = scythedir.ToRotation() - MathHelper.ToRadians(30);
                    scytheSmoothness = 3;
                }
                else
                {
                    scytheDirection = scythedir.ToRotation() + MathHelper.ToRadians(90);
                    scytheSmoothness = 8;
                }

                if (AI_Timer % 60 == 40)
                {
                    if (atkUseCounter == 12)
                    {
                        if (AI_State == (float)ActionState.ScytheHover)
                        {
                            AI_State = (float)ActionState.ScytheThrow;
                        }
                        if (AI_State == (float)ActionState.ScytheHover2)
                        {
                            AI_State = (float)ActionState.ScytheThrow2;
                        }

                        AI_Timer = 0;
                        atkUseCounter = 0;
                    }
                    else
                    {
                        atkUseCounter++;
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjSlash>(), 40, 0, NPC.target);
                        Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 12f;
                        if (Main.expertMode)
                        {
                            proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjSlash>(), 40, 0, NPC.target);
                            Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(36)) * 10.5f;
                            proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjSlash>(), 40, 0, NPC.target);
                            Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-36)) * 10.5f;
                        }
                        if (Main.rand.NextBool(3))
                        {
                            spinDirection *= -1;
                        }
                    }
                }
            }
        }
        private void ScytheThrow()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = targetArea - NPC.Center;

            NPC.velocity = direction * 0.07f;
            NPC.velocity += player.velocity * 3/4;
            AI_Timer += 1;
            if (AI_Timer > 0)
            {
                Vector2 direction2 = player.Center - NPC.Center;
                targetArea = player.Center + direction2.SafeNormalize(Vector2.Zero) * -320 + direction2.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(100 * spinDirection)) * 144f;

                Vector2 scythedir = player.Center - NPC.Center;
                scytheDirection = scythedir.ToRotation();
                if (AI_Timer < 60)
                {

                    scytheDirection = scythedir.ToRotation() - MathHelper.ToRadians(30);
                    scytheSmoothness = 6;
                    holdingScythe = true;
                }
                else
                {
                    holdingScythe = false;
                    if (AI_Timer > 120)
                    {
                        if (player.ownedProjectileCounts[ModContent.ProjectileType<SpiritReaperProjScythe>()] < 1)
                        {
                            AI_Timer = 0;

                            scytheVisualDirection = scythedir.ToRotation() + MathHelper.ToRadians(45);
                            scytheSmoothness = 6;
                            holdingScythe = true;
                        }
                    }

                }

                if (AI_Timer == 60)
                {
                    if (atkUseCounter == 3)
                    {
                        if (AI_State == (float)ActionState.ScytheThrow)
                        {
                            AI_State = (float)ActionState.ScytheChase;
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjScytheChase>(), 40, 0, NPC.target, NPC.whoAmI);
                        }
                        if (AI_State == (float)ActionState.ScytheThrow2)
                        {
                            AI_State = (float)ActionState.SpiritSummon;
                        }
                        AI_Timer = 0;
                        atkUseCounter = 0;

                    }
                    else
                    {
                        atkUseCounter++;

                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjScythe>(), 40, 0, NPC.target, NPC.whoAmI);
                        Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 24f;
                    }
                }
            }
        }
        private void ScytheChase()
        {
            Player player = Main.player[NPC.target];
            AI_Timer += 1;
            NPC.velocity = new Vector2(0, 0);


            if (AI_Timer % 125 < 25)
            {
                NPC.alpha += 10;
            }
            else if (AI_Timer % 125 < 55)
            {
                if (AI_Timer % 125 == 25)
                {
                    NPC.alpha = 255;
                    if (Main.rand.NextBool())
                    {
                        storedVel = player.velocity.SafeNormalize(Vector2.One);
                    }
                    else
                    {
                        storedVel = player.velocity.SafeNormalize(Vector2.One).RotatedByRandom(MathHelper.Pi);
                    }
                }

                float xv = Main.rand.NextFloat(-3, 3);
                float yv = Main.rand.NextFloat(-2, -5);
                int dust = Dust.NewDust(NPC.Center, 1, 1, DustID.DungeonSpirit, xv, yv, 0, default(Color), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1, 2);
                NPC.velocity = Vector2.Zero;
                NPC.Center = player.Center + storedVel * 360;
                NPC.alpha -= 10;
            }
            else
            {
                NPC.alpha = 0;
                NPC.velocity = storedVel * -18;
            }
            if (AI_Timer == 1100)
            {
                NPC.dontTakeDamage = false;
                AI_State = (float)ActionState.ScytheHover2;
                AI_Timer = 0;
                atkUseCounter = 0;
                if (Main.rand.NextBool())
                {
                    spinDirection = 1;
                }
                else
                {
                    spinDirection = -1;
                }
                Vector2 direction2 = player.Center - NPC.Center;
                targetArea = player.Center + direction2.SafeNormalize(Vector2.Zero) * -320;
                holdingScythe = true;
            }
        }
        private void SpiritSummon()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = targetArea - NPC.Center;

            NPC.velocity = direction * 0.07f;
            NPC.velocity += player.velocity * 3/4;

            AI_Timer += 1;
            if (AI_Timer > 0)
            {
                Vector2 direction2 = player.Center - NPC.Center;
                targetArea = player.Center + direction2.SafeNormalize(Vector2.Zero) * -320 + direction2.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(100 * spinDirection)) * 144f;

                Vector2 scythedir = player.Center - NPC.Center;
                scytheDirection = scythedir.ToRotation() - MathHelper.ToRadians(30);
                scytheSmoothness = 3;

                if (AI_Timer % 60 == 0)
                {
                    if (atkUseCounter == 10)
                    {
                        AI_State = (float)ActionState.ScytheHover;
                        AI_Timer = 0;
                        atkUseCounter = 0;
                        if (Main.rand.NextBool())
                        {
                            spinDirection = 1;
                        }
                        else
                        {
                            spinDirection = -1;
                        }
                        direction2 = player.Center - NPC.Center;
                        targetArea = player.Center + direction2.SafeNormalize(Vector2.Zero) * -320;
                        holdingScythe = true;
                    }
                    else
                    {
                        atkUseCounter++;
                        int npc = NPC.NewNPC(NPC.GetBossSpawnSource(NPC.target), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.DungeonSpirit);
                        Main.npc[npc].lifeMax *= 5;
                        Main.npc[npc].life = Main.npc[npc].lifeMax;
                    }
                }
            }
        }

        private void ScytheHover3()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction = targetArea - NPC.Center;

            NPC.velocity = direction * 0.07f;
            NPC.velocity += player.velocity * 3/4;

            AI_Timer += 1;
            if (AI_Timer > 0)
            {

                Vector2 direction2 = player.Center - NPC.Center;
                targetArea = player.Center + direction2.SafeNormalize(Vector2.Zero) * -320 + direction2.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(100 * spinDirection)) * 144f;

                Vector2 scythedir = player.Center - NPC.Center;

                if (altAtk)
                {
                    if (atkUseCounter == 4 || atkUseCounter == 9)
                    {
                        scytheDirection = scythedir.ToRotation() - MathHelper.ToRadians(30);
                        scytheSmoothness = 3;


                        if (AI_Timer == 30)
                        {
                            for (int i = 0; i < 60; i++)
                            {
                                int dust = Dust.NewDust(NPC.Center, 1, 1, DustID.DungeonSpirit, 0, 0, 0, default(Color), 1f);
                                Main.dust[dust].scale = 1.75f;
                                Main.dust[dust].velocity = new Vector2(12, 0).RotatedBy(MathHelper.TwoPi * i / 60);
                                Main.dust[dust].noGravity = true;
                            }
                        }

                        if (AI_Timer >= 120 && AI_Timer < 180 && AI_Timer % 12 == 0)
                        {
                            if (Main.rand.NextBool(2))
                            {
                                spinDirection *= -1;
                            }

                            for (int i = 0; i < 60; i++)
                            {
                                int dust = Dust.NewDust(NPC.Center, 1, 1, DustID.DungeonSpirit, 0, 0, 0, default(Color), 1f);
                                Main.dust[dust].scale = 1.75f;
                                Main.dust[dust].velocity = new Vector2(12, 0).RotatedBy(MathHelper.TwoPi * i / 60);
                                Main.dust[dust].noGravity = true;
                            }
                            NPC.Center = player.Center + new Vector2(380, 0).RotatedByRandom(MathHelper.TwoPi);

                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjSlash>(), 40, 0, NPC.target);
                            Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(24)) * 12f;
                            proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjSlash>(), 40, 0, NPC.target);
                            Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-24)) * 12f;
                        }
                        if (AI_Timer == 210)
                        {
                            atkUseCounter++;

                            AI_Timer = 0;
                        }

                    }
                    else
                    {
                        scytheDirection = scythedir.ToRotation();
                        if (AI_Timer < 40)
                        {

                            scytheDirection = scythedir.ToRotation() - MathHelper.ToRadians(30);
                            scytheSmoothness = 3;
                        }
                        else
                        {
                            scytheDirection = scythedir.ToRotation() + MathHelper.ToRadians(90);
                            scytheSmoothness = 8;
                        }

                        if (AI_Timer == 40)
                        {
                            if (atkUseCounter == 10)
                            {
                                if (AI_State == (float)ActionState.ScytheHover3)
                                {
                                    AI_State = (float)ActionState.ScytheHover3;
                                }
                                altAtk = Main.rand.NextBool();
                                AI_Timer = 0;
                                atkUseCounter = 0;
                            }
                            else
                            {
                                atkUseCounter++;
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjSlash>(), 40, 0, NPC.target);
                                Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 12f;
                                if (Main.expertMode)
                                {
                                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjSlash>(), 40, 0, NPC.target);
                                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(36)) * 10f;
                                    proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjSlash>(), 40, 0, NPC.target);
                                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-36)) * 10f;
                                }
                                if (Main.rand.NextBool(3))
                                {
                                    spinDirection *= -1;
                                }
                            }
                        }
                        if (AI_Timer == 60)
                        {
                            AI_Timer = 0;
                        }
                    }
                }
                else
                {
                    scytheDirection = scythedir.ToRotation();
                    if (AI_Timer % 75 < 45)
                    {

                        scytheDirection = scythedir.ToRotation() - MathHelper.ToRadians(30);
                        scytheSmoothness = 3;
                    }
                    else
                    {
                        scytheDirection = scythedir.ToRotation() + MathHelper.ToRadians(90);
                        scytheSmoothness = 8;
                    }

                    if (AI_Timer % 75 == 45)
                    {
                        if (atkUseCounter == 10)
                        {
                            if (AI_State == (float)ActionState.ScytheHover3)
                            {
                                AI_State = (float)ActionState.ScytheHover3;
                            }
                            altAtk = Main.rand.NextBool();
                            AI_Timer = 0;
                            atkUseCounter = 0;
                        }
                        else
                        {
                            atkUseCounter++;
                            for (var i = 0; Main.expertMode ? i < 7 : i < 5; i++)
                            {
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<Projectiles.SpiritReaperProjSlashSmall>(), 35, 0, NPC.target);
                                Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10f + Main.rand.NextVector2Circular(6f, 6f);
                            }
                            if (Main.rand.NextBool(3))
                            {
                                spinDirection *= -1;
                            }
                        }
                    }
                }
            }
        }

    }
}