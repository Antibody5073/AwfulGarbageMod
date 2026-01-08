using AwfulGarbageMod;
using AwfulGarbageMod.BossBars;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Consumables; 
using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.NPCs
{
    // NPC ModNPC serves as an example of a completely custom AI.
    public class ForestBat : ModNPC
    {

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4; // make sure to set NPC for your modnpcs.
        }

        Vector2 oldVel;

        int frameCounter = 0;
        int frame = 0;
        int tick = 0;

        public override void SetDefaults()
        {
            NPC.width = 24; // The width of the npc's hitbox (in pixels)
            NPC.height = 24; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // NPC npc has a completely unique AI, so we set NPC to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 12; // The amount of damage that NPC npc deals
            NPC.defense = 4; // The amount of defense that NPC npc has
            NPC.lifeMax = 25; // The amount of health that NPC npc has
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath4; // The sound the NPC will make when it dies.
            NPC.value = 100f; // How many copper coins the NPC will drop when killed.
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0.75f;
            NPC.noGravity = true;
            NPC.chaseable = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of NPC town NPC listed in the bestiary.
				// With Town NPCs, you usually set NPC to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.ForestBat")
            });
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneForest && !spawnInfo.Invasion && !spawnInfo.PlayerInTown && DownedBossSystem.downedTreeToad && !spawnInfo.Player.ZoneSkyHeight)
            {
                return 0.17f;
            }
            return 0;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EnchantedLeaf>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.GrassSeeds, 5, 2, 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotRemixSeed(), ItemID.ChainKnife, 100));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.RemixSeed(), ItemID.KOCannon, 100));
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest();
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                for (int i = 0; i < 9; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(3, 3) + new Vector2(0, -6), GoreID.TreeLeaf_VanityTreeYellowWillow);
                }
            }
        }

        public override bool PreAI()
        {
            tick++;
            {
                if (tick == 30)
                {
                    NPC.dontTakeDamage = false;
                }
            }
            if (NPC.life != NPC.lifeMax && DifficultyModes.Difficulty == 1)
            {
                NPC.ai[1] += 1f;

                if (NPC.ai[1] == 360)
                {
                    oldVel = NPC.velocity;
                    NPC.velocity = Vector2.Zero;
                    for (int i = 0; i < 15; i++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenFairy, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].scale = Main.rand.NextFloat(1f, 1.5f);
                        Main.dust[dust].velocity = Main.rand.NextVector2Circular(8, 8);
                        Main.dust[dust].noGravity = true;
                    }
                }
                if (NPC.ai[1] == 390)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 12f, ModContent.ProjectileType<Projectiles.ForestBeam>(), 6, 0);
                    }
                }
                if (NPC.ai[1] == 420)
                {
                    NPC.ai[1] = 0;
                    NPC.velocity = oldVel;
                }

                if (NPC.ai[1] >= 360)
                {
                    return false;
                }
            }
            if ((NPC.life != NPC.lifeMax || NPC.ai[3] == 1) && DifficultyModes.Difficulty == 2)
            {
                NPC.ai[1] += 1f;

                if (NPC.ai[1] == 90)
                {
                    oldVel = NPC.velocity;
                    NPC.velocity = Vector2.Zero;
                    for (int i = 0; i < 15; i++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenFairy, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].scale = Main.rand.NextFloat(1f, 1.5f);
                        Main.dust[dust].velocity = Main.rand.NextVector2Circular(8, 8);
                        Main.dust[dust].noGravity = true;
                    }
                }
                if (NPC.ai[1] == 105)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 12f + Main.rand.NextVector2Circular(1f, 1f), ModContent.ProjectileType<Projectiles.ForestBeam>(), 6, 0);
                        }
                        if (NPC.ai[3] == 1)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(20)) * 12f, ModContent.ProjectileType<Projectiles.ForestBeam>(), 6, 0);
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-20)) * 12f, ModContent.ProjectileType<Projectiles.ForestBeam>(), 6, 0);
                        }
                    }
                }
                if (NPC.ai[1] == 105)
                {
                    NPC.ai[1] = 0;
                    NPC.velocity = oldVel;
                }

                if (NPC.ai[1] >= 90)
                {
                    return false;
                }
            }
            return true;
        }
        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {
            NPC.noGravity = true;
            if (NPC.collideX)
            {
                NPC.velocity.X = NPC.oldVelocity.X * -0.5f;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                {
                    NPC.velocity.X = 2f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                {
                    NPC.velocity.X = -2f;
                }
            }
            if (NPC.collideY)
            {
                NPC.velocity.Y = NPC.oldVelocity.Y * -0.5f;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1f)
                {
                    NPC.velocity.Y = 1f;
                }
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1f)
                {
                    NPC.velocity.Y = -1f;
                }
            }

            NPC.TargetClosest();


            if (NPC.wet)
            {
                if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y *= 0.95f;
                }
                NPC.velocity.Y -= 0.5f;
                if (NPC.velocity.Y < -4f)
                {
                    NPC.velocity.Y = -4f;
                }
                NPC.TargetClosest();
            }

            if (NPC.direction == -1 && NPC.velocity.X > -3f)
            {
                NPC.velocity.X -= 0.1f;
                if (NPC.velocity.X > 3f)
                {
                    NPC.velocity.X -= 0.1f;
                }
                else if (NPC.velocity.X > 0f)
                {
                    NPC.velocity.X += 0.05f;
                }
                if (NPC.velocity.X < -3f)
                {
                    NPC.velocity.X = -3f;
                }
            }
            else if (NPC.direction == 1 && NPC.velocity.X < 3f)
            {
                NPC.velocity.X += 0.1f;
                if (NPC.velocity.X < -3f)
                {
                    NPC.velocity.X += 0.1f;
                }
                else if (NPC.velocity.X < 0f)
                {
                    NPC.velocity.X -= 0.05f;
                }
                if (NPC.velocity.X > 3f)
                {
                    NPC.velocity.X = 3f;
                }
            }
            if (NPC.directionY == -1 && (double)NPC.velocity.Y > -1.25)
            {
                NPC.velocity.Y -= 0.04f;
                if ((double)NPC.velocity.Y > 1.25)
                {
                    NPC.velocity.Y -= 0.05f;
                }
                else if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y += 0.03f;
                }
                if ((double)NPC.velocity.Y < -1.25)
                {
                    NPC.velocity.Y = -1.25f;
                }
            }
            else if (NPC.directionY == 1 && (double)NPC.velocity.Y < 1.25)
            {
                NPC.velocity.Y += 0.04f;
                if ((double)NPC.velocity.Y < -1.25)
                {
                    NPC.velocity.Y += 0.05f;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.velocity.Y -= 0.03f;
                }
                if ((double)NPC.velocity.Y > 1.25)
                {
                    NPC.velocity.Y = 1.25f;
                }
            }
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // NPC makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = NPC.direction;

            if (++frameCounter >= 6)
            {
                frameCounter = 0;
                frame++;
                if (frame >= 4)
                {
                    frame = 0;
                }
            }
            NPC.frame.Y = frame * frameHeight;

        }

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

    }
}