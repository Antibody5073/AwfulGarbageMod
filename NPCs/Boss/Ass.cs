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
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Summon;
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
    public class Ass : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            ShadowflameArrows,
            ConvergingFlames,
            ShadowflameApparition,
            ChaseDash,

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
            NPC.lifeMax = 40000; // The amount of health that this npc has
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
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.Ass")
            });
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            if (!Main.GameModeInfo.IsJourneyMode)
            {
                NPC.lifeMax = 50000;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 60000; // Increase by 5 if expert or master mode
                    if (Main.getGoodWorld || Main.zenithWorld)
                    {
                        NPC.lifeMax = 70000;
                    }
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
            notExpertRule.OnSuccess(ItemDropRule.FewFromOptions(1, 1, ModContent.ItemType<LavaLance>()));
            npcLoot.Add(ItemDropRule.ByCondition(new UnrealCondition(), ModContent.ItemType<CurseOfCandescence>(), 1, 1, 1));

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
                targetArea = Main.rand.NextVector2CircularEdge(300, 300);
                AI_State = (float)ActionState.ShadowflameArrows;
            }

            switch (AI_State)
            {
                case (float)ActionState.ShadowflameArrows:
                    ShadowflameArrows();
                    break;
                case (float)ActionState.ConvergingFlames:
                    ConvergingFireballs();
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


        private void ShadowflameArrows()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (DifficultyModes.Difficulty == 2)
            {
                if (AI_Timer % 210 < 60)
                {
                    if (AI_Timer % 210 == 30)
                    {
                        NPC.Center = player.Center + targetArea;
                        targetArea = Main.rand.NextVector2CircularEdge(300, 300);

                        spread = 1f;
                    }
                }
                else if (AI_Timer % 210 < 180)
                {
                    if (AI_Timer % 8 == 0)
                    {
                        for (int i = -2; i < 3; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * spread)) * 6.5f, ModContent.ProjectileType<AssProjArrow>(), 17, 0, Main.myPlayer, 90);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * spread)) * 4.5f, ModContent.ProjectileType<AssProjArrow>(), 17, 0, Main.myPlayer, 90);
                        }
                        spread += 1.75f;

                    }
                }
                int dust = Dust.NewDust(player.Center + targetArea, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
            else if (DifficultyModes.Difficulty == 1)
            {
                if (AI_Timer % 210 < 60)
                {
                    if (AI_Timer % 210 == 30)
                    {
                        NPC.Center = player.Center + targetArea;
                        targetArea = Main.rand.NextVector2CircularEdge(300, 300);

                        spread = 1f;
                    }
                }
                else if (AI_Timer % 210 < 180)
                {
                    if (AI_Timer % 8 == 0)
                    {
                        for (int i = -2; i < 3; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * spread)) * 6.5f, ModContent.ProjectileType<AssProjArrow>(), 17, 0, Main.myPlayer, 90);
                        }
                        spread += 1.75f;

                    }
                }
                int dust = Dust.NewDust(player.Center + targetArea, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
            else if (Main.expertMode)
            {
                if (AI_Timer % 150 < 35)
                {
                    if (AI_Timer % 150 == 30)
                    {
                        NPC.Center = player.Center + targetArea;
                        targetArea = Main.rand.NextVector2CircularEdge(300, 300);

                        spread = 1.5f;
                    }
                }
                else
                {
                    if (AI_Timer % 20 == 0)
                    {
                        for (int i = -3; i < 4; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * spread)) * 8f, ModContent.ProjectileType<AssProjArrow>(), 17, 0, Main.myPlayer, 90);
                        }
                    }
                }
                int dust = Dust.NewDust(player.Center + targetArea, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
            else
            {
                if (AI_Timer % 150 < 35)
                {
                    if (AI_Timer % 150 == 30)
                    {
                        NPC.Center = player.Center + targetArea;
                        targetArea = Main.rand.NextVector2CircularEdge(300, 300);

                        spread = 36f;
                    }
                }
                else
                {
                    if (AI_Timer % 20 == 0)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * spread)) * 8.5f, ModContent.ProjectileType<AssProjArrow>(), 17, 0, Main.myPlayer, 90);
                        }
                    }
                }
                int dust = Dust.NewDust(player.Center + targetArea, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].scale = 1.35f;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }

            if (AI_Timer == 1050)
            {

                AI_State = (float)ActionState.ConvergingFlames;
                AI_Timer = -30;
                atkUseCounter = 0;
            }
        }
        private void ConvergingFireballs()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer < 1)
            {
                NPC.alpha = (int)MathHelper.Lerp(0, 255, (30 + AI_Timer)/30);
            }
            else
            {
                if (DifficultyModes.Difficulty == 2)
                {
                    if (AI_Timer % 210 < 60)
                    {
                        if (AI_Timer % 210 == 30)
                        {
                            NPC.Center = player.Center + targetArea;
                            targetArea = Main.rand.NextVector2CircularEdge(300, 300);

                            spread = 1f;
                        }
                    }
                    else if (AI_Timer % 210 < 180)
                    {
                        if (AI_Timer % 8 == 0)
                        {
                            for (int i = -2; i < 3; i++)
                            {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * spread)) * 6.5f, ModContent.ProjectileType<AssProjArrow>(), 17, 0, Main.myPlayer, 90);
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * spread)) * 4.5f, ModContent.ProjectileType<AssProjArrow>(), 17, 0, Main.myPlayer, 90);
                            }
                            spread += 1.75f;

                        }
                    }
                    int dust = Dust.NewDust(player.Center + targetArea, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.35f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
                else if (DifficultyModes.Difficulty == 1)
                {
                    if (AI_Timer % 60 == 30)
                    {
                        targetArea = Main.rand.NextVector2CircularEdge(650, 650);

                        for (int i = 0; i < 5; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), Main.player[NPC.target].Center + targetArea.RotatedBy(i * MathHelper.TwoPi / 5), targetArea.RotatedBy(i * MathHelper.TwoPi / 5).SafeNormalize(Vector2.Zero) * -2f, ModContent.ProjectileType<AssProjFireball>(), 17, 0, Main.myPlayer, 650);
                        }
                        spread = 1f;

                    }
                }
                else if (Main.expertMode)
                {
                    if (AI_Timer % 150 < 35)
                    {
                        if (AI_Timer % 150 == 30)
                        {
                            NPC.Center = player.Center + targetArea;
                            targetArea = Main.rand.NextVector2CircularEdge(300, 300);

                            spread = 1.5f;
                        }
                    }
                    else
                    {
                        if (AI_Timer % 20 == 0)
                        {
                            for (int i = -3; i < 4; i++)
                            {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * spread)) * 8f, ModContent.ProjectileType<AssProjArrow>(), 17, 0, Main.myPlayer, 90);
                            }
                        }
                    }
                    int dust = Dust.NewDust(player.Center + targetArea, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.35f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
                else
                {
                    if (AI_Timer % 180 < 35)
                    {
                        if (AI_Timer % 180 == 30)
                        {
                            NPC.Center = player.Center + targetArea;
                            targetArea = Main.rand.NextVector2CircularEdge(300, 300);

                            spread = 36f;
                        }
                    }
                    else if (AI_Timer % 180 < 150)
                    {
                        if (AI_Timer % 20 == 0)
                        {
                            for (int i = -1; i < 2; i++)
                            {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center).RotatedBy(MathHelper.ToRadians(i * spread)) * 8.5f, ModContent.ProjectileType<AssProjArrow>(), 17, 0, Main.myPlayer, 90);
                            }
                        }
                    }
                    int dust = Dust.NewDust(player.Center + targetArea, NPC.width, NPC.height, DustID.Shadowflame, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.35f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }
    }
}