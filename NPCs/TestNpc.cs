using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
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
using AwfulGarbageMod.Items;

namespace AwfulGarbageMod.NPCs
{
    // This ModNPC serves as an example of a completely custom AI.
    public class TestNpc : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            PointTowardsPlayer,
            Passive,
            Chase,
            Dash,
            Fire,
            Move
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
        float AI_Timer = 120;
        Vector2 targetArea;
        Vector2 direction;
        Vector2 storedVel;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1; // make sure to set this for your modnpcs.

        }

        public override void SetDefaults()
        {
            NPC.width = 36; // The width of the npc's hitbox (in pixels)
            NPC.height = 36; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = 0; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 0; // The amount of damage that this npc deals
            NPC.defense = 0; // The amount of defense that this npc has
            NPC.lifeMax = 99999999; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit5; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.Item30; // The sound the NPC will make when it dies.
            NPC.value = 5000f; // How many copper coins the NPC will drop when killed.
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0f;
            NPC.chaseable = true;
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrostShard>(), 1, 2, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiritItem>(), 1, 4, 6));

        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return 0;
        }

        public override bool? CanFallThroughPlatforms()
        {
            return false;
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {
            NPC.life = 999999999;
        }
    }
}