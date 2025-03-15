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
using ReLogic.Content;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace AwfulGarbageMod.NPCs.BossUnrealRework.DukeFishron
{
    // This ModNPC serves as an example of a completely custom AI.
    [AutoloadBossHead]
    public class DukeFishron : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            Rise,
            Fish,
            Walls,
            Pirates,
            Jellyfish,
            Tornado,
            Dash
        }
        private enum Frame
        {
            Idle1,
            Idle2,
            Idle3,
            Idle4,
            Idle5,
            Idle6,
            Dash1,
            Dash2
        }
        // These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        float AI_State;
        float Next_State;
        float AI_Timer;
        float bossPhase;
        public float atkX;
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
        int waveProj;
        Vector2 targetArea;
        Vector2 direction;
        Vector2 storedVel;
        Vector2 storedPos;
        Vector2 recoil;
        List<Vector2> playerPos = new List<Vector2> { };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.DukeFishron];
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Dazed] = true;


            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.width = 64; // The width of the npc's hitbox (in pixels)
            NPC.height = 64; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 70; // The amount of damage that this npc deals
            NPC.defense = 50; // The amount of defense that this npc has
            NPC.lifeMax = 60000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath1; // The sound the NPC will make when it dies.
            NPC.value = 45000; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.npcSlots = 100f;
            NPC.noTileCollide = true;

            if (!Main.dedServ)
            {
                Music = MusicID.DukeFishron;
            }
            NPC.BossBar = ModContent.GetInstance<TreeToadBossBar>();
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
            NPC.lifeMax = 60000;
            if (Main.masterMode)
            {
                NPC.lifeMax = 75000; // Increase by 5 if expert or master mode
                if (Main.getGoodWorld || Main.zenithWorld)
                {
                    NPC.lifeMax = 90000;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            foreach (IItemDropRule DukeFishronLoot in Main.ItemDropsDB.GetRulesForNPCID(NPCID.DukeFishron))
            {
                npcLoot.Add(DukeFishronLoot);
            }
            npcLoot.Add(ItemDropRule.Common(3318));
        }
        public override void OnKill()
        {
            NPC.downedFishron = true;
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
                player = Main.player[NPC.target];
                bossPhase = 1;
                AI_Timer = 0;
                AI_State = 0;



                if (player.Center.X > Main.rightWorld / 2)
                {
                    atkX = -1;
                }
                else
                {
                    atkX = 1;
                }
                NPC.Center = player.Center + new Vector2(atkX * -1200, 300);
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<DukeFishronProjWave>()] < 1)
            {
                waveProj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-atkX * 60, 0), ModContent.ProjectileType<DukeFishronProjWave>(), 30, 0, Main.myPlayer, NPC.whoAmI);
            }

            switch (AI_State)
            {
                case (float)ActionState.Rise:
                    Rise();
                    break;
                case (float)ActionState.Fish:
                    Fish();
                    break;
                case (float)ActionState.Walls:
                    Walls();
                    break;
                case (float)ActionState.Tornado:
                    Tornado();
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
                if (frame >= 6)
                {
                    frame = 0;
                }
            }

            NPC.frame.Y = frame * frameHeight;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawPos;
            Color color;
            Vector2 drawOrigin = NPC.frame.Size() / 2;

            SpriteEffects spriteEffects = SpriteEffects.None;

// Wave
            Projectile proj = Main.projectile[waveProj];
            // SpriteEffects helps to flip texture horizontally and vertically

            if (proj.velocity.X > 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            // Getting texture of projectile
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AwfulGarbageMod/NPCs/BossUnrealRework/DukeFishron/DukeFishronProjWave");

            int frameHeight = texture.Height / Main.projFrames[proj.type];
            int startY = frameHeight * proj.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;

            // Applying lighting and draw current frame
            drawColor = proj.GetAlpha(drawColor);
            Main.EntitySpriteDraw(texture,
                proj.Center - Main.screenPosition + new Vector2(0f, proj.gfxOffY),
                sourceRectangle, drawColor, proj.rotation, origin, proj.scale, spriteEffects, 0);
            //Boss

            spriteEffects = SpriteEffects.None;

            if (atkX < 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }


            if (true)
            {
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    drawPos = NPC.oldPos[k] - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY - 4); //.RotatedBy(NPC.rotation);
                    color = NPC.GetAlpha(drawColor) * (float)(((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);
                    Main.EntitySpriteDraw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }
            }
            drawPos = NPC.position - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY - 4); //.RotatedBy(NPC.rotation);
            color = NPC.GetAlpha(drawColor);
            Main.EntitySpriteDraw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);

            
            return false;
        }
        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return false;
            // You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
        }

        private void SwitchAttack(ActionState attack)
        {
            AI_State = (AI_State + Main.rand.Next(1, 6)) % 6;

            switch (AI_State)
            {
                case (float)ActionState.Fish:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    y0 = 0;
                    jumping = false;
                    break;
            }
        }
        private void Rise()
        {
            Player player = Main.player[NPC.target];
            AI_Timer += 1;
            if (AI_Timer == 120)
            {
                AI_State = (float)ActionState.Fish;
                AI_Timer = 0;
                atkUseCounter = 0;
            }
            NPC.direction = (int)atkX;

            NPC.position.X += atkX * 2;
            NPC.position.Y += (player.position.Y - NPC.position.Y - 300) / 8;
        }
        private void Fish()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 720)
            {
                AI_State = (float)ActionState.Walls;
                AI_Timer = -60;

            }
            if (AI_Timer >= 0)
            {
                if (AI_Timer % 40 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(atkX * Main.rand.NextFloat(-300, 300), -900), new Vector2(Main.rand.NextFloat(-2, 2), -7) + NPC.velocity * 1.25f, ModContent.ProjectileType<DukeFishronProjFish>(), 20, 0, Main.myPlayer, Main.rand.Next(0, 6), Main.rand.NextBool() ? 0.05f : -0.05f);
                }
                if (AI_Timer % 40 == 10)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(atkX * Main.rand.NextFloat(-900, -300), -900), new Vector2(Main.rand.NextFloat(-2, 2), -7) + NPC.velocity * 1.25f, ModContent.ProjectileType<DukeFishronProjFish>(), 20, 0, Main.myPlayer, Main.rand.Next(0, 6), Main.rand.NextBool() ? 0.05f : -0.05f);
                }
                if (AI_Timer % 40 == 20)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(atkX * Main.rand.NextFloat(-1500, -900), -900), new Vector2(Main.rand.NextFloat(-2, 2), -7) + NPC.velocity * 1.25f, ModContent.ProjectileType<DukeFishronProjFish>(), 20, 0, Main.myPlayer, Main.rand.Next(0, 6), Main.rand.NextBool() ? 0.05f : -0.05f);
                }
                if (AI_Timer % 40 == 30)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(atkX * Main.rand.NextFloat(-2100, -1500), -900), new Vector2(Main.rand.NextFloat(-2, 2), -7) + NPC.velocity * 1.25f, ModContent.ProjectileType<DukeFishronProjFish>(), 20, 0, Main.myPlayer, Main.rand.Next(0, 6), Main.rand.NextBool() ? 0.05f : -0.05f);
                }
            }
            NPC.direction = (int)atkX;

            NPC.velocity = new Vector2((player.Center.X - NPC.Center.X) / 250, 0);
            NPC.velocity.X += atkX * 2;
            NPC.velocity.Y = (player.position.Y - NPC.position.Y - 300) / 8;
        }
        private void Walls()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 660)
            {
                AI_State = (float)ActionState.Tornado;
                AI_Timer = -60;
            }
            if (AI_Timer == 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + new Vector2(atkX * 1200, -720) + new Vector2(0, i * 1440 / 8), new Vector2(0, 4), ModContent.ProjectileType<DukeFishronProjTyphoon>(), 25, 0, Main.myPlayer);
                }
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + new Vector2(atkX * 1400, -720) + new Vector2(0, i * 1440 / 8), new Vector2(0, -4), ModContent.ProjectileType<DukeFishronProjTyphoon>(), 25, 0, Main.myPlayer);
                }
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + new Vector2(atkX * 1600, -720) + new Vector2(0, i * 1440 / 8), new Vector2(0, 4), ModContent.ProjectileType<DukeFishronProjTyphoon>(), 25, 0, Main.myPlayer);
                }
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + new Vector2(atkX * 1800, -720) + new Vector2(0, i * 1440 / 8), new Vector2(0, -4), ModContent.ProjectileType<DukeFishronProjTyphoon>(), 25, 0, Main.myPlayer);
                }
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + new Vector2(atkX * 2000, -720) + new Vector2(0, i * 1440 / 8), new Vector2(0, 4), ModContent.ProjectileType<DukeFishronProjTyphoon>(), 25, 0, Main.myPlayer);
                }
                for (int i = 0; i < 8; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + new Vector2(atkX * 2200, -720) + new Vector2(0, i * 1440 / 8), new Vector2(0, -4), ModContent.ProjectileType<DukeFishronProjTyphoon>(), 25, 0, Main.myPlayer);
                }
            }
            NPC.direction = (int)atkX;

            NPC.velocity = new Vector2((player.Center.X - NPC.Center.X) / 400, 0);
            NPC.velocity.X += atkX * 3;
            NPC.velocity.Y = (player.position.Y - NPC.position.Y - 300) / 8;
        }
        private void Tornado()
        {
            Player player = Main.player[NPC.target];

            AI_Timer += 1;
            if (AI_Timer == 720)
            {
                AI_State = (float)ActionState.Fish;
                AI_Timer = -60;
            }
            if (AI_Timer >= 0)
            {
                if (AI_Timer % 360 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(48, 0), ModContent.ProjectileType<DukeFishronProjTornadoBoltSpawner>(), 25, 0, Main.myPlayer, NPC.whoAmI);
                }
                if (AI_Timer % 360 < 20)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(16, 0).RotatedBy(MathHelper.ToRadians(-24 + (AI_Timer % 360) * 6)), ModContent.ProjectileType<DukeFishronProjTyphoon2>(), 25, 0, Main.myPlayer, NPC.whoAmI);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(13, 0).RotatedBy(MathHelper.ToRadians(-27f + (AI_Timer % 360) * 6)), ModContent.ProjectileType<DukeFishronProjTyphoon2>(), 25, 0, Main.myPlayer, NPC.whoAmI);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(10, 0).RotatedBy(MathHelper.ToRadians(-30 + (AI_Timer % 360) * 6)), ModContent.ProjectileType<DukeFishronProjTyphoon2>(), 25, 0, Main.myPlayer, NPC.whoAmI);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(-33 + (AI_Timer % 360) * 6)), ModContent.ProjectileType<DukeFishronProjTyphoon2>(), 25, 0, Main.myPlayer, NPC.whoAmI);
                }
            }
            NPC.direction = (int)atkX;

            NPC.velocity = new Vector2((player.Center.X - NPC.Center.X) / 400, 0);
            NPC.velocity.X += atkX * 3;
            NPC.velocity.Y = (player.position.Y - NPC.position.Y - 300) / 8;
        }
    }
}