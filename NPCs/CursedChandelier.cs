using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.NPCs
{
    // This ModNPC serves as an example of a completely custom AI.
    public class CursedChandelier : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            PointTowardsPlayer,
            Passive,
            HomingWaterSides,
            FloorSlam,
            WaterStream,
            HomingWaterSides2,
            Flamethrower,
            WaterStorm
        }

        // Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
        // These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
        private enum Frame
        {
            Asleep
        }

        // These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        float AI_State;
        float AI_Timer = 150;
        float shotXspd;
        float homingSpd;
        Vector2 projVel;
        Vector2 targetArea;
        Vector2 direction;
        Vector2 teleportPos;
        Vector2 chainPos;

        int frameCounter = 0;
        readonly int[] frameLen = { 6, 6, 6, 6, 6 };
        int frame = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(AI_State);
            writer.Write(AI_Timer);
            writer.Write(targetArea.X);
            writer.Write(targetArea.Y);
            writer.Write(direction.X);
            writer.Write(direction.Y);
            writer.Write(teleportPos.X);
            writer.Write(teleportPos.Y);
            writer.Write(chainPos.X);
            writer.Write(chainPos.Y);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            AI_State = reader.ReadSingle();
            AI_Timer = reader.ReadSingle();
            targetArea = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            direction = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            teleportPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            chainPos = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
        }

        public override void SetDefaults()
        {
            NPC.width = 24; // The width of the npc's hitbox (in pixels)
            NPC.height = 36; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 50; // The amount of damage that this npc deals
            NPC.defense = 10; // The amount of defense that this npc has
            NPC.lifeMax = 7000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit4; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCHit55; // The sound the NPC will make when it dies.
            NPC.value = 15000f; // How many copper coins the NPC will drop when killed.
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.chaseable = false;
            NPC.rarity = 3;

        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (!Main.GameModeInfo.IsJourneyMode)
            {
                NPC.lifeMax = 9000;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 11000; // Increase by 5 if expert or master mode
                    if (Main.getGoodWorld || Main.zenithWorld)
                    {
                        NPC.lifeMax = 13000;
                    }
                }
            }
        }
        public override void OnSpawn(IEntitySource source)
        {

            NPC.lifeMax = NPC.lifeMax * ModContent.GetInstance<Config>().BossHealthMultiplier / 100;
            NPC.life = NPC.lifeMax;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.CursedChandelier")
            });
        }

        public override void OnKill()
        {
            
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.FewFromOptions(2, 1, ItemID.Nazar, ModContent.ItemType<CursedCandleStaff>(), ModContent.ItemType<CursedChandelierStaff>(), ModContent.ItemType<BloomingMarrow>()));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {

            if (spawnInfo.Player.GetModPlayer<GlobalPlayer>().NoMiniboss)
            {
                return 0;
            }
            if (spawnInfo.Player.ZoneDungeon && (spawnInfo.SpawnTileType == TileID.Spikes || spawnInfo.SpawnTileType == TileID.BlueDungeonBrick || spawnInfo.SpawnTileType == TileID.GreenDungeonBrick || spawnInfo.SpawnTileType == TileID.PinkDungeonBrick))
            {
                return 0.05f;
            }
            return 0;
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {

            // The npc starts in the asleep state, waiting for a player to enter range
            switch (AI_State)
            {
                case (float)ActionState.PointTowardsPlayer:
                    PointTowardsPlayer();
                    break;
                case (float)ActionState.Passive:
                    Passive();
                    break;
                case (float)ActionState.HomingWaterSides:
                    HomingWaterSides();
                    break;
                case (float)ActionState.FloorSlam:
                    FloorSlam();
                    break;
                case (float)ActionState.WaterStream:
                    WaterStream();
                    break;
                case (float)ActionState.HomingWaterSides2:
                    HomingWaterSides2();
                    break;
                case (float)ActionState.Flamethrower:
                    Flamethrower();
                    break;
                case (float)ActionState.WaterStorm:
                    WaterStorm();
                    break;
            }
            Lighting.AddLight(NPC.Center, Color.DeepSkyBlue.ToVector3());
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = 1;

            if (++frameCounter >= frameLen[frame])
            {
                frameCounter = 0;
                frame++;
                if (frame >= 5)
                {
                    frame = 0;
                }
            }
            NPC.frame.Y = frame * frameHeight;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (AI_State > 1)
            {
                Vector2 playerArmPosition = chainPos;

                Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("AwfulGarbageMod/NPCs/CursedChandelierChain");
                Rectangle? chainSourceRectangle = null;
                // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
                float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
                Vector2 chainDrawPosition = NPC.Top;
                Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
                }
                float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

                // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
                while (chainLengthRemainingToDraw > 0f)
                {
                    // This code gets the lighting at the current tile coordinates
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                    // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                    // This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

                    var chainTextureToDraw = chainTexture;
                    if (chainCount >= 4)
                    {
                        // Use normal chainTexture and lighting, no changes
                    }

                    // Here, we draw the chain texture at the coordinates
                    Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                    chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }
            return true;
        }
        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            player.AddBuff(ModContent.BuffType<MinibossBuff>(), 60 * 10);
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            Main.player[projectile.owner].AddBuff(ModContent.BuffType<MinibossBuff>(), 60 * 10);
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (AI_State != (float)ActionState.Passive || (Vector2.DistanceSquared(Main.player[projectile.owner].Center, NPC.Center) < 320 * 320 && !projectile.IsMinionOrSentryRelated))
            {
                return null;
            }
            return false;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (AI_State == (float)ActionState.Passive)
            {
                NPC.TargetClosest(true);
                AI_Timer = 0;
                chainPos = NPC.Top;
                AI_State = (float)ActionState.HomingWaterSides;
                NPC.chaseable = true;
            }

            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
            }
        }
        private void HomingWater()
        {
            if (AI_Timer == 0)
            {
                Player player = Main.player[NPC.target];

                Vector2 direction = player.Center - NPC.Center;
                projVel = new Vector2(0, 1);
                homingSpd = 0f;
            }
            if (AI_Timer > 60 && AI_Timer < 300)
            {
                if (AI_Timer % 5 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                    int damage = 17;
                    if (Main.expertMode)
                    {
                        damage = 16;
                        if (Main.masterMode)
                        {
                            damage = 16;
                        }
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(MathHelper.PiOver2) * 8, ModContent.ProjectileType<CursedChandelierProjHoming>(), damage, 0, Main.myPlayer, homingSpd, 180);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(-MathHelper.PiOver2) * 8, ModContent.ProjectileType<CursedChandelierProjHoming>(), damage, 0, Main.myPlayer, homingSpd, 180);
                    if (DifficultyModes.Difficulty == 0)
                    {
                        homingSpd += 0.07f;
                    }
                    else if(DifficultyModes.Difficulty == 1)
                    {
                        homingSpd += 0.08f;
                    }
                    else if (DifficultyModes.Difficulty == 2)
                    {
                        homingSpd += 0.09f;
                    }
                }
            }

            if (DifficultyModes.Difficulty > 0 && AI_Timer < 210)
            {
                int damage = 17;
                if (Main.expertMode)
                {
                    damage = 16;
                    if (Main.masterMode)
                    {
                        damage = 16;
                    }
                }
                if (AI_Timer % 15 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), -3), ModContent.ProjectileType<CursedChandelierProjRise>(), damage, 0, Main.myPlayer, 0.02f);
                }
            }
        }
        private void HomingWaterSides()
        {
            HomingWater();
            if (AI_Timer == 360)
            {
                AI_State = (float)ActionState.FloorSlam;
                AI_Timer = 0;
            }
            AI_Timer++;
        }

        private void FloorSlam()
        {
            NPC.TargetClosest(true);
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0f;
            if (AI_Timer < 15)
            {
                NPC.velocity = new Vector2(0, 0.05f);
            }
            else
            {
                NPC.velocity.Y += 0.2f;
                NPC.noTileCollide = false;
            }

            if (NPC.collideY || AI_Timer > 300)
            {
                float rand = Main.rand.NextFloat(MathHelper.TwoPi);

                int damage = 24;
                if (Main.expertMode)
                {
                    damage = 23;
                    if (Main.masterMode)
                    {
                        damage = 22;
                    }
                }
                if (DifficultyModes.Difficulty == 0)
                {
                    rand = Main.rand.NextFloat(-0.15f, 0.15f);
                    for (int i = -10; i < 10; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(i * 0.3f + rand, 4), ModContent.ProjectileType<CursedChandelierProjRise>(), damage, 0, Main.myPlayer, -0.02f);
                    }
                }
                else if (DifficultyModes.Difficulty == 1)
                {
                    for (int i = 0; i < 60; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(2.5f * MathF.Cos(i * MathHelper.TwoPi / 60 + rand), 0.5f * MathF.Sin(i * MathHelper.TwoPi / 60 + rand)) + new Vector2(0, 4), ModContent.ProjectileType<CursedChandelierProjRise>(), damage, 0, Main.myPlayer, -0.02f);
                    }
                }
                else
                {
                    for (int i = 0; i < 75; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2Circular(2.5f, 0.5f) + new Vector2(0, 4), ModContent.ProjectileType<CursedChandelierProjRise>(), damage, 0, Main.myPlayer, -0.02f);
                    }
                }
                AI_State = (float)ActionState.WaterStream;
                AI_Timer = -240;
            }

            AI_Timer++;
        }
        private void WaterStream()
        {
            NPC.TargetClosest(true);
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0f;
            if (AI_Timer > 0)
            {
                int damage = 15;
                if (Main.expertMode)
                {
                    damage = 14;
                    if (Main.masterMode)
                    {
                        damage = 14;
                    }
                }
                if (AI_Timer % (DifficultyModes.Difficulty == 2 ? 24 : 30) == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 5f, ModContent.ProjectileType<CursedChandelierProjWater>(), damage, 0);
                    if (DifficultyModes.Difficulty > 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(30)) * 5f, ModContent.ProjectileType<CursedChandelierProjWater>(), damage, 0);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-30)) * 5f, ModContent.ProjectileType<CursedChandelierProjWater>(), damage, 0);
                    }
                }
            }

            if (AI_Timer > 180)
            {
                NPC.velocity.Y -= 0.05f;
            }
            if (AI_Timer == 450)
            {
                AI_State = (float)ActionState.HomingWaterSides2;
                AI_Timer = -90;
            }

            AI_Timer++;
        }

        private void PointTowardsPlayer()
        {
            NPC.TargetClosest(true);
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0f;

            targetArea = Main.player[NPC.target].Center;
            direction = targetArea - NPC.Center;
            direction.Normalize();

            NPC.velocity = Vector2.Zero;

            AI_State = (float)ActionState.Passive;

        }
        private void HomingWaterSides2()
        {
            HomingWater();
            if (AI_Timer == 360)
            {
                AI_State = (float)ActionState.Flamethrower;
                AI_Timer = 0;
            }
            AI_Timer++;
        }

        private void Flamethrower()
        {
            if (AI_Timer == 0)
            {
                homingSpd = 0f;
            }
            if (AI_Timer > 60 && AI_Timer < 210)
            {
                if (AI_Timer % 2 == 0)
                {
                    Player player = Main.player[NPC.target];

                    Vector2 direction = player.Center - NPC.Center;
                    projVel = direction.SafeNormalize(Vector2.Zero);

                    SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVel * homingSpd, ProjectileID.Flames, 22, 0, Main.myPlayer, 1);
                    proj.hostile = true;
                    proj.friendly = false;

                    homingSpd += 0.25f;
                }
            }
            if (AI_Timer > 209 && AI_Timer < 360)
            {
                if (AI_Timer % 2 == 0)
                {
                    projVel = projVel.RotatedBy(1);

                    SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVel * homingSpd, ProjectileID.Flames, 22, 0, Main.myPlayer, 1);
                    proj.hostile = true;
                    proj.friendly = false;

                }
                if (AI_Timer % 45 == 0)
                {
                    NPC nPC = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.CursedSkull);
                    nPC.lifeMax *= 3;
                    nPC.life = nPC.lifeMax;

                }
            }
            if (AI_Timer == 420)
            {
                AI_State = (float)ActionState.WaterStorm;
                AI_Timer = 0;
            }
            AI_Timer++;
        }
        private void WaterStorm()
        {
            if (AI_Timer == 0)
            {
                homingSpd = 0f;
                projVel = Main.rand.NextVector2CircularEdge(1, 1);

            }
            if (AI_Timer < 480)
            {
                if (AI_Timer % 15 == 0)
                {
                    int damage = 17;
                    if (Main.expertMode)
                    {
                        damage = 16;
                        if (Main.masterMode)
                        {
                            damage = 15;
                        }
                    }
                    if (DifficultyModes.Difficulty == 0)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(i * MathHelper.TwoPi / 4) * 3.75f, ModContent.ProjectileType<CursedChandelierProjWater>(), damage, 0, Main.myPlayer, homingSpd);
                        }
                        projVel = projVel.RotatedBy(2);
                    }
                    else if (DifficultyModes.Difficulty == 1)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(i * MathHelper.TwoPi / 5) * 3.75f, ModContent.ProjectileType<CursedChandelierProjWater>(), damage, 0, Main.myPlayer, homingSpd);
                        }
                        projVel = projVel.RotatedBy(2);
                    }
                    else
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVel.RotatedBy(i * MathHelper.TwoPi / 7) * 3.75f, ModContent.ProjectileType<CursedChandelierProjWater>(), damage, 0, Main.myPlayer, homingSpd);
                        }
                        projVel = projVel.RotatedBy(2);
                    }
                }
            }
            if (AI_Timer < 300)
            {
                Player player = Main.player[NPC.target];

                if (player.Center.Y > NPC.Center.Y)
                {
                    NPC.velocity.Y += 0.1f;
                }
                else
                {
                    NPC.velocity.Y -= 0.1f;
                }
                NPC.velocity.Y *= 0.97f;
            }
            else
            {
                NPC.velocity.Y -= 0.05f;
            }
            if (AI_Timer == 540)
            {
                homingSpd = 0f;

                AI_State = (float)ActionState.HomingWaterSides;
                AI_Timer = 0;
                projVel = new Vector2(0, 1);
            }
            AI_Timer++;
        }
        private void Passive()
        {
            NPC.knockBackResist = 0f;
            NPC.velocity += new Vector2(0, -0.3f);
        }
    }
}