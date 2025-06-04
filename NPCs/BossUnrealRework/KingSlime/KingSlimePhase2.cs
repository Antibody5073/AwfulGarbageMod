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
using AwfulGarbageMod.Items.Consumables; using AwfulGarbageMod.Items.Consumables.BossSummon;
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
using ReLogic.Content;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.NPCs.BossUnrealRework.KingSlime
{
    // This ModNPC serves as an example of a completely custom AI.
    [AutoloadBossHead]
    public class KingSlimePhase2 : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            NormalJump,
            SlimeRain,
            Slam,
            HorizontalLunge,
            BigSlam,
            SpikeDash
        }
        // These are reference properties. One, for example, lets us write NPC.GetGlobalNPC<GlobalEnemyBossInfo>().attack as if it's NPC.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "NPC.GetGlobalNPC<GlobalEnemyBossInfo>().attack" in the AI code below would be "npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        float Next_State;
        public float AI_Timer;
        float bossPhase;
        float atkX;
        int atkUseCounter = 0;
        int atkDelay = 0;
        int frame;
        int frameCounter;
        bool jumping = false;
        bool lunging = false;
        int y0;
        bool rand;
        bool rand2;
        int tileHitboxIndex;
        float oldXvel;
        Vector2 targetArea;
        Vector2 direction;
        Vector2 storedVel;
        Vector2 storedPos;
        Vector2 recoil;
        List<Vector2> playerPos = new List<Vector2> { };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[50];
            NPCID.Sets.SpecificDebuffImmunity[Type][31] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][69] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][20] = true;


            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 98; // The width of the npc's hitbox (in pixels)
            NPC.height = 92; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 27; // The amount of damage that this npc deals
            NPC.defense = 12; // The amount of defense that this npc has
            NPC.lifeMax = 3400; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath1; // The sound the NPC will make when it dies.
            NPC.value = 100000; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.npcSlots = 100f;
            NPC.noTileCollide = true;

            if (!Main.dedServ)
            {
                Music = MusicID.Boss1;
            }
            NPC.BossBar = ModContent.GetInstance<TreeToadBossBar>();


            AIType = 50;
            AnimationType = 50;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange([
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.UnrealKingSlime")
            ]);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = 3400;
            if (Main.masterMode)
            {
                NPC.lifeMax = 4200; // Increase by 5 if expert or master mode
                if (Main.getGoodWorld || Main.zenithWorld)
                {
                    NPC.lifeMax = 5000;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            foreach (IItemDropRule KingSlimeLoot2 in Main.ItemDropsDB.GetRulesForNPCID(50))
            {
                npcLoot.Add(KingSlimeLoot2);
            }
            npcLoot.Add(ItemDropRule.Common(3318));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SlimyLocket>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SlimyKnives>(), 2, 1, 1));
        }
        public override void OnKill()
        {
            Main.StopSlimeRain(announce: false);
            NPC.downedSlimeKing = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[0] = NPC.NewNPC(NPC.GetBossSpawnSource(NPC.target), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<KingSlimeShadow>(), ai0: NPC.whoAmI);
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
                AI_Timer = 0;
                NPC.GetGlobalNPC<GlobalEnemyBossInfo>().attack = 0;
            }

            switch (NPC.GetGlobalNPC<GlobalEnemyBossInfo>().attack)
            {
                case (float)ActionState.NormalJump:
                    NormalJump();
                    break;
                case (float)ActionState.SlimeRain:
                    SlimeRain();
                    break;
                case (float)ActionState.Slam:
                    Slam();
                    break;
                case (float)ActionState.HorizontalLunge:
                    HorizontalLunge();
                    break;
                case (float)ActionState.BigSlam:
                    BigSlam();
                    break;
                case (float)ActionState.SpikeDash:
                    SpikeDash();
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

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(spriteBatch, screenPos, drawColor);
            Texture2D texture2 = ModContent.Request<Texture2D>("AwfulGarbageMod/NPCs/BossUnrealRework/KingSlime/Crown", (AssetRequestMode)2).Value;
            Vector2 drawOrigin2 = new Vector2(texture2.Width - 82, texture2.Height - 50);
            Rectangle frame2 = new Rectangle(0, 0, texture2.Width, texture2.Height);
            drawColor = Lighting.GetColor((int)base.NPC.Center.X / 16, (int)(base.NPC.Center.Y / 16f));
            Main.EntitySpriteDraw(texture2, base.NPC.Center - Main.screenPosition, frame2, drawColor, base.NPC.rotation, drawOrigin2, base.NPC.scale, SpriteEffects.None);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (true)
            {
                Vector2 drawOrigin = NPC.frame.Size() / 2;
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY - 4); //.RotatedBy(NPC.rotation);
                    Color color = NPC.GetAlpha(drawColor) * (float)(((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
                    spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos + new Vector2(0, -6), NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            return true;
        }
        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return false;
            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
        }

        private void SwitchAttack(ActionState attack)
        {
            NPC.GetGlobalNPC<GlobalEnemyBossInfo>().attack = (NPC.GetGlobalNPC<GlobalEnemyBossInfo>().attack + Main.rand.Next(1, 6)) % 6;
            KingSlimeShadow shadow = Main.npc[(int)NPC.ai[0]].ModNPC as KingSlimeShadow;
            shadow.ExternalSwitchAttack((NPC.GetGlobalNPC<GlobalEnemyBossInfo>().attack + Main.rand.Next(1, 6)) % 6);

            switch (NPC.GetGlobalNPC<GlobalEnemyBossInfo>().attack)
            {
                case (float)ActionState.NormalJump:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    jumping = false;
                    break;
                case (float)ActionState.SlimeRain:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    break;
                case (float)ActionState.Slam:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    break;
                case (float)ActionState.HorizontalLunge:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    lunging = false;
                    jumping = false;
                    break;
                case (float)ActionState.BigSlam:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    jumping = false;
                    break;
                case (float)ActionState.SpikeDash:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    lunging = false;
                    jumping = false;
                    break;
            }
        }
        private void NormalJump()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.SlimeRain);
            }

            if (jumping && NPC.velocity.Y == 0)
            {
                y0++;
                if (y0 == 2)
                {
                    atkDelay = 45;
                    jumping = false;
                }
            }

            if (atkDelay > 0)
            {
                atkDelay--;
                NPC.velocity.X *= 0.8f;
            }
            else
            {
                if (!jumping && AI_Timer <= 720)
                {
                    atkUseCounter++;
                    y0 = 0;

                    Vector2 jumpvel = Vector2.Zero;

                    float xspd = DifficultyModes.Difficulty == 2 ? 7.5f : 3;
                    jumpvel.X = player.Center.X - NPC.Center.X < 0 ? -xspd : xspd;
                    jumpvel.X += (player.Center.X - NPC.Center.X) / 100;

                    switch (atkUseCounter % 4)
                    {
                        case 0:
                            jumpvel.Y = -6;
                            break;
                        case 1:
                            jumpvel.Y = -7;
                            break;
                        case 2:
                            jumpvel.Y = -5;
                            break;
                        case 3:
                            jumpvel.Y = -14;
                            break;
                    }

                    NPC.velocity = jumpvel;

                    jumping = true;
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

            NPC.velocity.Y += 0.25f;
            if (NPC.velocity.Y > 15)
            {
                NPC.velocity.Y = 15;
            }
        }
        private void SlimeRain()
        {

            NPC.TargetClosest(false);
            Player player = Main.player[NPC.target];
            targetArea = player.Center + new Vector2(player.velocity.X * 20, -320);


            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction += (targetArea - NPC.Center) / 320;



            if (AI_Timer < 720)
            {
                if (Vector2.DistanceSquared(NPC.Center, targetArea) > 48 * 48)
                {
                    NPC.velocity += direction;
                    NPC.velocity *= 0.9f;
                }
                if (AI_Timer % 180 == 0)
                {
                    atkX = player.Center.X - 1500 + Main.rand.NextFloat(-60, 60);
                }
                if (AI_Timer % 180 == 90)
                {
                    atkX = player.Center.X + 1500 + Main.rand.NextFloat(-60, 60);
                }
                if (DifficultyModes.Difficulty == 2 || AI_Timer % 3 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<KingSlimeProjSlime>(), 12, 0, Main.myPlayer, atkX, player.Center.Y - 420);

                    if (AI_Timer % 180 < 90)
                    {
                        atkX += DifficultyModes.Difficulty == 2 ? 50 : 150;
                    }
                    else
                    {
                        atkX -= DifficultyModes.Difficulty == 2 ? 50 : 150;
                    }
                }
            }
            else
            {


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
                if (AI_Timer > 735)
                {
                    NPC.velocity.Y += 0.2f;
                }
                if (NPC.velocity.Y > 15)
                {
                    NPC.velocity.Y = 15;
                }
                NPC.velocity.X *= 0.92f;

            }

            AI_Timer += 1;

            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.Slam);
            }
        }
        private void Slam()
        {
            AI_Timer += 1;

            atkDelay++;
            if (AI_Timer < 720)
            {
                if (atkDelay <= 0)
                {
                }
                else if (atkDelay < 120)
                {
                    NPC.noTileCollide = true;
                    Player player = Main.player[NPC.target];


                    float inertia = 8f;
                    if (DifficultyModes.Difficulty == 2)
                    {
                        targetArea = player.Center + new Vector2(player.velocity.X * 50, -430);

                    }
                    else
                    {
                        targetArea = player.Center + new Vector2((player.velocity.X * 20), -430);
                    }
                    float speed = Vector2.Distance(NPC.Center, targetArea) / 18f;


                    direction = targetArea - NPC.Center;
                    direction.Normalize();
                    direction *= speed;

                    NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                    if (NPC.Center.Y > player.Center.Y - 120)
                    {
                        NPC.velocity.X /= 3;
                    }
                }
                else if (atkDelay == 120)
                {
                    NPC.globalEnemyBossInfo().finishedAtk = false;

                    NPC.velocity = new Vector2(0, 0.05f);
                    tileHitboxIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, (int)NPC.Center.Y), new Vector2(0, 5.25f), ModContent.ProjectileType<KingSlimeProjSlam>(), 0, 0, Main.myPlayer, NPC.target, NPC.whoAmI);
                }
                else if (atkDelay > 120)
                {
                    if (NPC.globalEnemyBossInfo().finishedAtk)
                    {
                        atkDelay = -15;
                        NPC.velocity = new Vector2(0, 0);

                        for (int i = 0; i < 2; i++)
                        {
                            int type = DifficultyModes.Difficulty == 2 ? Main.rand.NextFromList(NPCID.MotherSlime, NPCID.BlackSlime, NPCID.SpikedIceSlime, NPCID.SpikedJungleSlime) : NPCID.BlueSlime;
                            int nPC = NPC.NewNPC(NPC.GetBossSpawnSource(NPC.target), (int)NPC.Center.X, (int)NPC.Center.Y, type);

                            Main.npc[nPC].lifeMax *= 8;
                            Main.npc[nPC].life *= 8;

                            Main.npc[nPC].lifeMax /= 75;
                            Main.npc[nPC].life /= 75;

                            Main.npc[nPC].damage = (int)(Main.npc[nPC].damage * 2f);
                            Main.npc[nPC].velocity = Main.rand.NextVector2Circular(6, 3) + new Vector2(0, -8);
                        }
                        SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                    }
                }
            }
            else
            {
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
                NPC.velocity.Y += 0.2f;
                if (NPC.velocity.Y > 15)
                {
                    NPC.velocity.Y = 15;
                }
                NPC.velocity.X *= 0.92f;

            }


            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.HorizontalLunge);
            }
        }
        private void HorizontalLunge()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.BigSlam);
            }

            if (jumping && NPC.velocity.Y == 0)
            {
                y0++;
                if (y0 == 2)
                {
                    atkDelay = 90;
                    jumping = false;
                    if (AI_Timer <= 720)
                    {
                        lunging = true;

                        if (player.Center.X - NPC.Center.X < 0)
                        {
                            NPC.velocity.X = -27;
                        }
                        else
                        {
                            NPC.velocity.X = 27;
                        }
                        NPC.velocity.X += (player.Center.X - NPC.Center.X) / 75;
                    }
                }
            }
            for (int i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.t_Slime, 0f, 0f, 0, new Color(0, 0, 255, 0), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1.2f, 1.7f);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
            }
            if (atkDelay > 0)
            {
                atkDelay--;
                NPC.velocity.X *= 0.98f;
            }
            else
            {
                lunging = false;
                if (!jumping && AI_Timer <= 720)
                {
                    atkUseCounter++;
                    y0 = 0;

                    Vector2 jumpvel = Vector2.Zero;
                    if (player.Center.X - NPC.Center.X < 0)
                    {
                        jumpvel.X += (player.Center.X + 540 - NPC.Center.X) / 112;
                    }
                    else
                    {
                        jumpvel.X += (player.Center.X - 540 - NPC.Center.X) / 112;
                    }

                    jumpvel.Y = -14;

                    NPC.velocity = jumpvel;

                    jumping = true;
                }
            }

            if (NPC.velocity.Y < 0)
            {
                NPC.noTileCollide = true;
            }
            else
            {
                if (Main.player[NPC.target].Center.Y - 160 > NPC.Bottom.Y)
                {
                    NPC.noTileCollide = true;
                }
                else
                {
                    NPC.noTileCollide = false;
                }
            }

            NPC.velocity.Y += 0.25f;
            if (NPC.velocity.Y > 15)
            {
                NPC.velocity.Y = 15;
            }

        }
        private void BigSlam()
        {
            AI_Timer += 1;

            atkDelay++;
            if (AI_Timer < 720)
            {
                if (atkDelay <= 0)
                {
                }
                else if (AI_Timer < 240)
                {
                    NPC.noTileCollide = true;
                    Player player = Main.player[NPC.target];


                    float inertia = 8f;
                    if (DifficultyModes.Difficulty == 2)
                    {
                        targetArea = player.Center + new Vector2(player.velocity.X * 25, -450);

                    }
                    else
                    {
                        targetArea = player.Center + new Vector2((player.velocity.X * 10), -450);
                    }
                    float speed = Vector2.Distance(NPC.Center, targetArea) / 18f;

                    direction = targetArea - NPC.Center;
                    direction.Normalize();
                    direction *= speed;

                    NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                    if (NPC.Center.Y > player.Center.Y - 120)
                    {
                        NPC.velocity.X /= 3;
                    }

                    int dust = Dust.NewDust(NPC.BottomLeft + new Vector2(0, -16), 1, 1, DustID.t_Slime, 0f, 0f, 0, new Color(0, 0, 255, 0), 1f);
                    Main.dust[dust].scale = 1.25f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = false;
                    dust = Dust.NewDust(NPC.BottomRight + new Vector2(0, -16), 1, 1, DustID.t_Slime, 0f, 0f, 0, new Color(0, 0, 255, 0), 1f);
                    Main.dust[dust].scale = 1.25f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = false;
                }
                else if (AI_Timer == 240)
                {
                    NPC.globalEnemyBossInfo().finishedAtk = false;

                    NPC.velocity = new Vector2(0, 0.05f);
                    tileHitboxIndex = Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X, (int)NPC.Center.Y), new Vector2(0, 9), ModContent.ProjectileType<KingSlimeProjSlam>(), 0, 0, Main.myPlayer, NPC.target, NPC.whoAmI);
                }
                else if (AI_Timer > 240)
                {
                    if (NPC.globalEnemyBossInfo().finishedAtk)
                    {
                        NPC.velocity = new Vector2(0, 0);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(224, 0), ModContent.ProjectileType<KingSlimeProjGround>(), 0, 0, Main.myPlayer, 8);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(-224, 0), ModContent.ProjectileType<KingSlimeProjGround>(), 0, 0, Main.myPlayer, 8);
                        NPC.globalEnemyBossInfo().finishedAtk = false;

                        SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
                    }
                }
            }


            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.SpikeDash);
            }
        }
        private void SpikeDash()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.NormalJump);
            }

            if (jumping && NPC.velocity.Y == 0)
            {
                y0++;

                if (y0 == 1)
                {
                    if (AI_Timer <= 720)
                    {
                        lunging = true;
                        atkDelay = 45;
                        oldXvel = NPC.velocity.X;
                        if (player.Center.X - NPC.Center.X < 0)
                        {
                            NPC.velocity.X = -30;
                        }
                        else
                        {
                            NPC.velocity.X = 30;
                        }
                        NPC.velocity.Y = 1;

                    }
                }
                else if (y0 == 2)
                {
                    jumping = false;
                    if (AI_Timer <= 720)
                    {
                        lunging = false;
                    }
                }
            }
            if (atkDelay > 0)
            {
                atkDelay--;
                if (atkDelay % 2 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -2).RotatedByRandom(MathHelper.ToRadians(30)), DifficultyModes.Difficulty == 2 ? ProjectileID.IceSpike : ProjectileID.SpikedSlimeSpike, 9, 0, Main.myPlayer);
                }
                if (DifficultyModes.Difficulty == 2 ? atkDelay % 2 == 0 : atkDelay % 5 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4.5f).RotatedByRandom(MathHelper.ToRadians(45)), ProjectileID.SpikedSlimeSpike, 9, 0, Main.myPlayer);
                }
            }
            else
            {
                if (lunging)
                {
                    NPC.velocity.X = oldXvel / 3;
                }
                lunging = false;
                if (!jumping && AI_Timer <= 720)
                {
                    atkUseCounter++;
                    y0 = 0;

                    Vector2 jumpvel = Vector2.Zero;
                    if (player.Center.X - NPC.Center.X < 0)
                    {
                        jumpvel.X += (player.Center.X + 660 - NPC.Center.X) / 56;
                    }
                    else
                    {
                        jumpvel.X += (player.Center.X - 660 - NPC.Center.X) / 56;
                    }

                    jumpvel.Y = -14;

                    NPC.velocity = jumpvel;

                    jumping = true;
                }
                NPC.velocity.Y += 0.25f;
                if (NPC.velocity.Y > 15)
                {
                    NPC.velocity.Y = 15;
                }
            }

            if (NPC.velocity.Y < 0)
            {
                NPC.noTileCollide = true;
            }
            else
            {
                if (Main.player[NPC.target].Center.Y - 160 > NPC.Bottom.Y)
                {
                    NPC.noTileCollide = true;
                }
                else
                {
                    NPC.noTileCollide = false;
                }
            }
        }
    }
}