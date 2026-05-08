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
using AwfulGarbageMod.Items.Weapons.Magic.Hardmode;
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

    public class BlightdrasilCore : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            Spawn,
            CrimsonSpread,
            ChaseWall,
            IchorRain,
            RottedForks,
            CorruptionSpread,
            VilethornChase,
            CursedFireballs,
            FireballWalls

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
        float atkUseCounter;
        int frame;
        int frameCounter;
        float rand;
        bool rand2;
        bool didTheAttackAlready;
        bool atkFromLeft;

        Vector2 targetArea;
        Vector2 direction;
        Vector2 storedVel;
        Vector2 storedPos;
        Vector2 recoil;
        List<Vector2> playerPos = new List<Vector2> { };

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flutter Slime"); // Automatic from localization files
            Main.npcFrameCount[NPC.type] = 4; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;

            NPCID.Sets.TrailCacheLength[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 48; // The width of the npc's hitbox (in pixels)
            NPC.height = 48; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 66; // The amount of damage that this npc deals
            NPC.defense = 30; // The amount of defense that this npc has
            NPC.lifeMax = 35000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit3; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath66; // The sound the NPC will make when it dies.
            NPC.value = 0; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.npcSlots = 100f;
            NPC.noTileCollide = false;
            NPC.waterMovementSpeed = 0.5f;


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
            if (!Main.GameModeInfo.IsJourneyMode)
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
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.None;
        }

        public override void OnKill()
        {
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
                AI_State = (float)ActionState.Spawn;
            }

            switch (AI_State)
            {
                case (float)ActionState.Spawn:
                    Spawn();
                    break;
                case (float)ActionState.CrimsonSpread:
                    CrimsonSpread();
                    break;
                case (float)ActionState.ChaseWall:
                    ChaseWall();
                    break;
                case (float)ActionState.IchorRain:
                    IchorRain();
                    break;
                case (float)ActionState.RottedForks:
                    RottedForks();
                    break;
                case (float)ActionState.CorruptionSpread:
                    CorruptionSpread();
                    break;
                case (float)ActionState.VilethornChase:
                    VilethornChase();
                    break;
                case (float)ActionState.CursedFireballs:
                    CursedFireballs();
                    break;
                case (float)ActionState.FireballWalls:
                    FireballWalls();
                    break;
            }
        }
        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = (NPC.velocity.X + recoil.X) * 0.035f;

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
            Vector2 drawOrigin = NPC.frame.Size() / 2;
            Vector2 drawPos;
            Color color;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                drawPos = NPC.oldPos[k] - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY); //.RotatedBy(NPC.rotation);
                color = NPC.GetAlpha(drawColor) * (float)(((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
            }
            drawPos = NPC.position - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY); //.RotatedBy(NPC.rotation);
            color = NPC.GetAlpha(drawColor);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
        }

        private void Spawn()
        {
            Player player = Main.player[NPC.target];

            NPC.direction = Math.Sign(player.Center.X - NPC.Center.X);
            AI_Timer += 1;
            NPC.velocity *= 0.97f;
            if (AI_Timer >= 120)
            {
                AI_State = (float)ActionState.CrimsonSpread;

                AI_Timer = 0;
                atkUseCounter = 0;
                NPC.noTileCollide = false;
            }
        }
        private void CrimsonSpread()
        {
            Player player = Main.player[NPC.target];

            NPC.direction = Math.Sign(player.Center.X - NPC.Center.X);
            AI_Timer += 1;
            NPC.velocity *= 0.97f;
            for (int i = 0; i < 4; i++)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -15).RotatedBy(MathHelper.PiOver2 * i + AI_Timer / 30), ProjectileID.CrimsonSpray, 26, 0, 0, Main.player[NPC.target].whoAmI);
            }
            if (AI_Timer >= 60)
            {
                AI_State = (float)ActionState.ChaseWall;

                AI_Timer = 0;
                atkUseCounter = 0;
                NPC.noTileCollide = false;
            }
        }
        private void ChaseWall()
        {

            Player player = Main.player[NPC.target];

            Vector2 direction;

            float speed = 7.5f;

            float inertia = 64f;

            direction = player.Center - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            if (NPC.collideX) { NPC.velocity.X = NPC.oldVelocity.X * -0.95f; }
            if (NPC.collideY) { NPC.velocity.Y = NPC.oldVelocity.Y * -0.95f; }

            NPC.rotation = NPC.velocity.X * 0.1f;

            AI_Timer++;
            if (NPC.velocity.X > 0)
            {
                NPC.direction = 1;
            }
            else
            {
                NPC.direction = -1;
            }
            if (Main.expertMode)
            {
                if (AI_Timer % 180 == 150)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BlightdrasilHeart2>(), 26, 0, 0, NPC.whoAmI);
                }
            }
            else
            {
                if (AI_Timer % 240 == 150)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BlightdrasilHeart2>(), 26, 0, 0, NPC.whoAmI);
                }
            }
            if (AI_Timer >= 180 * 6 + 60)
            {
                AI_State = (float)ActionState.IchorRain;

                AI_Timer = -60;
                atkUseCounter = 3;
                NPC.noTileCollide = true;
            }
        }
        private void IchorRain()
        {
            Player player = Main.player[NPC.target];

            NPC boss = Main.npc[NPC.realLife];

            if (AI_Timer < 0)
            {
                float speed = Vector2.Distance(NPC.Center, boss.Center) / (1f - AI_Timer * 0.5f);
                float inertia = (1f - AI_Timer * 0.5f);
                Vector2 direction = boss.Center - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            }
            else
            {
                NPC.velocity = Vector2.Zero;

                if (AI_Timer % 2 == 0)
                {
                    if (AI_Timer > 210)
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
                if (AI_Timer > 210)
                {
                    atkUseCounter += 0.04f;
                    if (AI_Timer == 270)
                    {
                        rand = Main.rand.NextFloat(-96, 96);
                        atkFromLeft = Main.rand.NextBool();
                        if (atkFromLeft)
                        {
                            for (int i = -5; i < 5; i++)
                            {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X - (16 * 60), boss.Center.Y + rand + i * 96 * 2), new Vector2(64, 0), ModContent.ProjectileType<BlightdrasilWarning2>(), 0, 0, -1, 0, player.whoAmI);
                            }
                        }
                        else
                        {
                            for (int i = -5; i < 5; i++)
                            {
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X + (16 * 60), boss.Center.Y + rand + i * 96 * 2), new Vector2(-64, 0), ModContent.ProjectileType<BlightdrasilWarning2>(), 0, 0, -1, 0, player.whoAmI);
                            }
                        }
                    }
                    if (AI_Timer == 330)
                    {

                        if (atkFromLeft)
                        {
                            for (int i = -5; i < 5; i++)
                            {
                                if (Main.expertMode)
                                {
                                    Projectile porj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X - (16 * 60), boss.Center.Y + rand + i * 96 * 2), Vector2.Zero, ModContent.ProjectileType<BlightdrasilEaterSpawner>(), 0, 0, -1, 1, 5, 0);
                                    porj.timeLeft = 210 + 6 * 45;
                                }
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X - (16 * 60), boss.Center.Y + rand + i * 96 * 2), new Vector2(12, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X - (16 * 60), boss.Center.Y + rand + i * 96 * 2), new Vector2(15, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X - (16 * 60), boss.Center.Y + rand + i * 96 * 2), new Vector2(18, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                            }
                        }
                        else
                        {
                            for (int i = -5; i < 5; i++)
                            {
                                if (Main.expertMode)
                                {
                                    Projectile porj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X + (16 * 60), boss.Center.Y + rand + i * 96 * 2), Vector2.Zero, ModContent.ProjectileType<BlightdrasilEaterSpawner>(), 0, 0, -1, 1, -5, 0);
                                    porj.timeLeft = 210 + 6 * 45;
                                }
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X + (16 * 60), boss.Center.Y + rand + i * 96 * 2), new Vector2(-12, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X + (16 * 60), boss.Center.Y + rand + i * 96 * 2), new Vector2(-15, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X + (16 * 60), boss.Center.Y + rand + i * 96 * 2), new Vector2(-18, 0), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 1);
                            }
                        }
                    }
                }
            }
            AI_Timer++;
            if (AI_Timer >= 510)
            {
                AI_State = (float)ActionState.RottedForks;

                AI_Timer = 0;
                atkUseCounter = 0;
                NPC.noTileCollide = false;
            }
        }

        private void RottedForks()
        {

            Player player = Main.player[NPC.target];

            Vector2 direction;

            float speed = 7.5f;

            float inertia = 80f;

            direction = player.Center - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            if (NPC.collideX) { NPC.velocity.X = NPC.oldVelocity.X * -0.95f; }
            if (NPC.collideY) { NPC.velocity.Y = NPC.oldVelocity.Y * -0.95f; }

            NPC.rotation = NPC.velocity.X * 0.1f;

            AI_Timer++;
            if (NPC.velocity.X > 0)
            {
                NPC.direction = 1;
            }
            else
            {
                NPC.direction = -1;
            }

            if (AI_Timer % 45 == 30 && AI_Timer < 45 * 6 && AI_Timer > 45)
            {

                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_3")
                {
                    Volume = 0.4f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (player.Center - NPC.Center).SafeNormalize(Vector2.One) * -2, ModContent.ProjectileType<BlightdrasilRottedFork>(), 40, 0, -1, 0, 0);
            }
            if (AI_Timer >= 45 * 10)
            {
                AI_State = (float)ActionState.CorruptionSpread;

                AI_Timer = -0;
                atkUseCounter = 0;
                NPC.noTileCollide = false;
            }
        }

        private void CorruptionSpread()
        {
            Player player = Main.player[NPC.target];

            NPC.direction = Math.Sign(player.Center.X - NPC.Center.X);
            AI_Timer += 1;
            NPC.velocity *= 0.97f;
            for (int i = 0; i < 4; i++)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -15).RotatedBy(MathHelper.PiOver2 * i - AI_Timer / 30), ProjectileID.CorruptSpray, 26, 0, 0, Main.player[NPC.target].whoAmI);
            }
            if (AI_Timer >= 60)
            {
                AI_State = (float)ActionState.VilethornChase;

                AI_Timer = 0;
                atkUseCounter = 0;
                NPC.noTileCollide = false;
            }
        }

        private void VilethornChase()
        {

            Player player = Main.player[NPC.target];

            Vector2 direction;

            float speed = 10f;

            float inertia = 48f;

            direction = player.Center - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            if (NPC.collideX) { NPC.velocity.X = NPC.oldVelocity.X * -0.95f; }
            if (NPC.collideY) { NPC.velocity.Y = NPC.oldVelocity.Y * -0.95f; }

            NPC.rotation = NPC.velocity.X * 0.1f;

            AI_Timer++;
            if (NPC.velocity.X > 0)
            {
                NPC.direction = 1;
            }
            else
            {
                NPC.direction = -1;
            }

            if (AI_Timer % 180 == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 1).RotatedBy(MathHelper.ToRadians(i * 90)) * 28f, ModContent.ProjectileType<BlightdrasilVilethornBase>(), 30, 0, -1, 0, 0);
                }
            }
            if (AI_Timer % 180 == 90)
            {
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 1).RotatedBy(MathHelper.ToRadians(i * 90 + 45)) * 28f, ModContent.ProjectileType<BlightdrasilVilethornBase>(), 30, 0, -1, 0, 0);
                }
            }
            if (AI_Timer >= 180 * 4)
            {
                AI_State = (float)ActionState.CursedFireballs;

                AI_Timer = -60;
                atkUseCounter = 3;
                NPC.noTileCollide = true;
            }
        }
        private void CursedFireballs()
        {
            Player player = Main.player[NPC.target];

            NPC boss = Main.npc[NPC.realLife];

            if (AI_Timer < 0)
            {
                float speed = Vector2.Distance(NPC.Center, boss.Center) / (1f - AI_Timer * 0.5f);
                float inertia = (1f - AI_Timer * 0.5f);
                Vector2 direction = boss.Center - NPC.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            }
            else
            {
                NPC.velocity = Vector2.Zero;

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
                if (AI_Timer == 390)
                {
                    rand = Main.rand.NextFloat(-96, 96);
                    for (int i = -5; i < 5; i++)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X + rand + i * 96 * 2, boss.Center.Y + (16 * 60 + 64)), new Vector2(0, -64), ModContent.ProjectileType<BlightdrasilWarning2>(), 0, 0, -1, 1, player.whoAmI);
                    }
                }
                if (AI_Timer == 450)
                {

                    for (int i = -5; i < 5; i++)
                    {
                        if (Main.expertMode)
                        {
                            Projectile porj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X + rand + i * 96 * 2, boss.Center.Y + (16 * 60 + 64)), Vector2.Zero, ModContent.ProjectileType<BlightdrasilEaterSpawner>(), 0, 0, -1, 0, 0, -5);
                            porj.timeLeft = 150 + 4 * 120;
                        }
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X + rand + i * 96 * 2, boss.Center.Y + 16 * 60 + 64), new Vector2(0, -12), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 0);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X + rand + i * 96 * 2, boss.Center.Y + 16 * 60 + 64), new Vector2(0, -15), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 0);
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2(boss.Center.X + rand + i * 96 * 2, boss.Center.Y + 16 * 60 + 64), new Vector2(0, -18), ModContent.ProjectileType<BlightdrasilEater>(), 37, 0, -1, 0);
                    }
                }
            }
            AI_Timer++;
            if (AI_Timer >= 510)
            {
                AI_State = (float)ActionState.FireballWalls;

                AI_Timer = -120;
                atkUseCounter = 0;
                NPC.noTileCollide = false;
                atkFromLeft = Main.rand.NextBool();
            }
        }
        private void FireballWalls()
        {

            Player player = Main.player[NPC.target];
            NPC boss = Main.npc[NPC.realLife];

            Vector2 direction;

            float speed = 6;

            float inertia = 120f;

            if (AI_Timer == 0)
            {
                direction = player.Center - NPC.Center;

                direction.Normalize();
                direction *= -8;

                NPC.velocity = direction;
            }
            else if (AI_Timer > 0)
            {
                direction = player.Center - NPC.Center;
                direction.Normalize();
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
                if (AI_Timer > 60 && AI_Timer % 24 == 0)
                {
                    if (DifficultyModes.Difficulty > 0)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (player.Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(5 * i)) * 10f, ModContent.ProjectileType<BlightdrasilProjFireball>(), 28, 0, -1, Main.player[NPC.target].whoAmI);
                        }
                    }
                    else
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10f, ModContent.ProjectileType<BlightdrasilProjFireball>(), 28, 0, -1, Main.player[NPC.target].whoAmI);
                    }
                }
            }

            if (NPC.collideX) { NPC.velocity.X = NPC.oldVelocity.X * -0.95f; }
            if (NPC.collideY) { NPC.velocity.Y = NPC.oldVelocity.Y * -0.95f; }

            NPC.rotation = NPC.velocity.X * 0.1f;

            AI_Timer++;
            if (NPC.velocity.X > 0)
            {
                NPC.direction = 1;
            }
            else
            {
                NPC.direction = -1;
            }

            if (AI_Timer >= 120 * 5)
            {
                AI_State = (float)ActionState.CrimsonSpread;

                AI_Timer = -0;
                atkUseCounter = 0;
                NPC.noTileCollide = false;
            }
        }
    }
}