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
    public class Mosquito : ModNPC
    {
        private enum ActionState
        {
            ChasePlayer,
            PositionBesidePlayer,
            Dash
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3; // make sure to set NPC for your modnpcs.
        }

        Vector2 oldVel;

        int frameCounter = 0;
        int frame = 0;
        int tick = 0;
        float AI_State;
        float AI_Timer = 150;

        public override void SetDefaults()
        {
            NPC.width = 24; // The width of the npc's hitbox (in pixels)
            NPC.height = 24; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // NPC npc has a completely unique AI, so we set NPC to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 13; // The amount of damage that NPC npc deals
            NPC.defense = 8; // The amount of defense that NPC npc has
            NPC.lifeMax = 35; // The amount of health that NPC npc has
            NPC.HitSound = SoundID.NPCHit33; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath16; // The sound the NPC will make when it dies.
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.Mosquito")
            });
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneJungle && spawnInfo.Player.ZoneOverworldHeight)
            {
                return 0.15f;
            }
            return 0;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Proboscis>(), 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwampyGrip>(), 50));
            npcLoot.Add(ItemDropRule.Common(ItemID.JungleGrassSeeds, 10));
            npcLoot.Add(ItemDropRule.Common(ItemID.Stinger, 5));
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest();
        }
        
        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {
            switch (AI_State)
            {
                case (float)ActionState.ChasePlayer:
                    ChasePlayer();
                    break;
                case (float)ActionState.PositionBesidePlayer:
                    ChasePlayer();
                    break;
                case (float)ActionState.Dash:
                    ChasePlayer();
                    break;
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
                if (frame >= 3)
                {
                    frame = 0;
                }
            }
            NPC.frame.Y = frame * frameHeight;

        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            NPC.HealEffect(20);
            NPC.life += 20;
            if (NPC.life > NPC.lifeMax)
            {
                NPC.life = NPC.lifeMax;
            }
            NPC.netUpdate = true;
        }
        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        private void ChasePlayer()
        {
            Player player = Main.player[NPC.target];

            Vector2 direction;

            float speed = 5;

            float inertia = 64f;

            direction = player.Center - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            if (NPC.collideX) { NPC.velocity.X = NPC.oldVelocity.X * -0.95f; }
            if (NPC.collideY) { NPC.velocity.Y = NPC.oldVelocity.Y * -0.95f; }

            NPC.rotation = NPC.velocity.X * 0.1f;

            if (NPC.velocity.X > 0)
            {
                NPC.direction = 1;
            }
            else
            {
                NPC.direction = -1;
            }
        }
    }
}