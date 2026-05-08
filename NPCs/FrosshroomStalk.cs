using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.NPCs
{
    // This ModNPC serves as an example of a completely custom AI.
    public class FrosshroomStalk : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {

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
        int frameCounter = 0;
        int frame = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(AI_State);
            writer.Write(AI_Timer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            AI_State = reader.ReadSingle();
            AI_Timer = reader.ReadSingle();
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1; // make sure to set this for your modnpcs.

            // Specify the debuffs it is immune to
        }

        public override void SetDefaults()
        {
            NPC.width = 24; // The width of the npc's hitbox (in pixels)
            NPC.height = 36; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 0; // The amount of damage that this npc deals
            NPC.defense = 0; // The amount of defense that this npc has
            NPC.lifeMax = 600; // The amount of health that this npc has
            if (Main.expertMode)
            {
                NPC.lifeMax = 900;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 1200; // Increase by 5 if expert or master mode
                }
            }
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.Item30; // The sound the NPC will make when it dies.
            NPC.value = 5000f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.chaseable = false;

        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.FrosshroomStalk")
            });
        }

        public override void OnKill()
        {
            for (int i = 0; i < 3; i++)
            {
                NPC nPC = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.IceSlime, ai3: 1);
                nPC.velocity = new Vector2(0, Main.rand.NextFloat(-2, -7)).RotatedByRandom(MathHelper.ToRadians(45));
                nPC.netUpdate = true;
                if (DifficultyModes.Difficulty == 2)
                {
                    nPC.lifeMax *= 10;
                    nPC.life = nPC.lifeMax;
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Frosshroom>(), 1, 5, 9));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrigidSeed>(), 10));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneSnow)
            {
                if (NPC.AnyNPCs(NPC.type))
                {
                    return 0.06f;
                }
                return 0.6f;
            }
            return 0;
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {

        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = 1;

        }

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            player.AddBuff(ModContent.BuffType<SporedBuff>(), 60 * 60);
            player.AddBuff(BuffID.Chilled, 10 * 60);
            if (DifficultyModes.Difficulty == 2 && Main.rand.NextBool(10))
            {
                player.AddBuff(BuffID.Frozen, 60);
            }
        }
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.axe == 0)
            {
                modifiers.SetMaxDamage(1);
            }
            else
            {
                modifiers.FlatBonusDamage += item.axe * 5;
                modifiers.SetMaxDamage(item.axe * 5);
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetMaxDamage(1);
        }
    }
}