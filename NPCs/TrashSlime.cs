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
using AwfulGarbageMod.Items.Consumables;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Items.Placeable.Furniture;

namespace AwfulGarbageMod.NPCs
{
    // This ModNPC serves as an example of a completely custom AI.
    public class TrashSlime : ModNPC
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
       

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;

        }

        public override void SetDefaults()
        {
            NPC.width = 36; // The width of the npc's hitbox (in pixels)
            NPC.height = 36; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = NPCAIStyleID.Slime; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 27; // The amount of damage that this npc deals
            NPC.defense = 0; // The amount of defense that this npc has
            NPC.lifeMax = 107; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.Item30; // The sound the NPC will make when it dies.
            NPC.value = 1f; // How many copper coins the NPC will drop when killed.
            NPC.noTileCollide = false;
            NPC.knockBackResist = 0.5f;
            NPC.chaseable = true;
            AIType = NPCID.GreenSlime;
            AnimationType = NPCID.GreenSlime;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.TrashSlime")
            });
        }
        
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Main.expertMode)
            {
                NPC.damage -= 13;
                if (Main.masterMode)
                {
                    NPC.damage -= 15;
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage /= 3;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Garbage>(), 1, 4, 15));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Trash>(), 100, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RecyclingBin>(), 60, 1, 1));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.townNPCs > 2f && !spawnInfo.Invasion && (Condition.DownedEyeOfCthulhu.IsMet() || Condition.DownedKingSlime.IsMet() || Condition.DownedEowOrBoc.IsMet() || DownedBossSystem.downedSeseKitsugai || DownedBossSystem.downedTreeToad) && !spawnInfo.Player.HasBuff<RecyclingBinBuff>())
            {
                return 0.3f;
            }
            return 0;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20)
            {
                NPC.frameCounter = 0;
            }
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

    }
}