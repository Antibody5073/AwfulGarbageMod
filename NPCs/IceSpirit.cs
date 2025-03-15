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

namespace AwfulGarbageMod.NPCs
{
    // This ModNPC serves as an example of a completely custom AI.
    public class IceSpirit : ModNPC
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

            // Specify the debuffs it is immune to
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;

        }

        public override void SetDefaults()
        {
            NPC.width = 36; // The width of the npc's hitbox (in pixels)
            NPC.height = 36; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 0; // The amount of damage that this npc deals
            NPC.defense = 6; // The amount of defense that this npc has
            NPC.lifeMax = 220; // The amount of health that this npc has
            if (Main.expertMode)
            {
                NPC.lifeMax = 400;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 750; // Increase by 5 if expert or master mode
                }
            }
            NPC.HitSound = SoundID.NPCHit5; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.Item30; // The sound the NPC will make when it dies.
            NPC.value = 5000f; // How many copper coins the NPC will drop when killed.
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.chaseable = false;
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrostShard>(), 1, 2, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiritItem>(), 1, 4, 7));
            npcLoot.Add(ItemDropRule.OneFromOptions(3, ModContent.ItemType<SigilOfFrost>(), ModContent.ItemType<Items.Weapons.Magic.IceSpiritBeam>()));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneSnow && spawnInfo.Player.ZoneRockLayerHeight)
            {
                return 0.06f;
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
                case (float)ActionState.Chase:
                    Chase();
                    break;
                case (float)ActionState.Dash:
                    Dash();
                    break;
                case (float)ActionState.Fire:
                    Fire();
                    break;
                case (float)ActionState.Move:
                    Move();
                    break;
            }
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = NPC.direction * -1;

            // For the most part, our animation matches up with our states.
            switch (AI_State)
            {
                case (float)ActionState.Chase:
                    // npc.frame.Y is the goto way of changing animation frames. npc.frame starts from the top left corner in pixel coordinates, so keep that in mind.
                    NPC.frame.Y = (int)Frame.Asleep * frameHeight;
                    break;
            }
        }

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (AI_State == (float)ActionState.Passive)
            {
                NPC.TargetClosest(true);
                AI_State = (float)ActionState.Chase;
                NPC.chaseable = true;

                if (Main.expertMode)
                {
                    if (Main.masterMode)
                    {
                        NPC.damage = 40;
                    }
                    else
                    {
                        NPC.damage = 28;
                    }
                }
                else
                {
                    NPC.damage = 17;
                }
            }
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {


            if (AI_State == (float)ActionState.Passive)
            {
                NPC.TargetClosest(true);
                AI_State = (float)ActionState.Chase;
                NPC.chaseable = true;

                if (Main.expertMode)
                {
                    if (Main.masterMode)
                    {
                        NPC.damage = 40;
                    }
                    else
                    {
                        NPC.damage = 28;
                    }
                }
                else
                {
                    NPC.damage = 17;
                }
            }

        }


        private void Move()
        {

            NPC.knockBackResist = 0.8f;

            targetArea = Main.player[NPC.target].Center;
            float speed = Vector2.Distance(NPC.Center, targetArea) / 120f + 2f;
            float inertia = 20f;
            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer--;
            if (AI_Timer == 0)
            {
                AI_Timer = 120;
                AI_State = (float)ActionState.Chase;
            }
        }
        private void Chase()
        {
            NPC.knockBackResist = 0.5f;
            if (AI_Timer > 30)
            {
                NPC.TargetClosest(true);
                targetArea = Main.player[NPC.target].Center + new Vector2(0, -180);
                float speed = Vector2.Distance(NPC.Center, targetArea) / 20f;
                float inertia = 8f;
                direction = targetArea - NPC.Center;
                direction.Normalize();
                direction *= speed;

                NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;
            }
            else
            {
                NPC.velocity = new Vector2(0, 0);
            }
            if (Vector2.Distance(NPC.Center, targetArea) < 120f)
            {
                AI_Timer--;
            }

            if (AI_Timer == 0)
            {
                AI_Timer = 225;
                AI_State = (float)ActionState.Dash;

            }
        }
        private void Dash()
        {

            NPC.knockBackResist = 0f;

            if (AI_Timer % 75 == 0)
            {
                NPC.TargetClosest(true);
                targetArea = Main.player[NPC.target].Center;

                direction = targetArea - NPC.Center;
                direction.Normalize();
                direction *= 16;

                storedVel = direction;
                
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, storedVel, Mod.Find<ModProjectile>("IceSpiritTelegraph").Type, 0, 0, Main.myPlayer);

            }
            else if (AI_Timer % 75 > 60)
            {
                NPC.knockBackResist = 0.5f;
                NPC.velocity = new Vector2(0, 0);
            }
            else if (AI_Timer % 75 == 60)
            {
                NPC.velocity = storedVel;
                SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
            }
            else

            targetArea = Main.player[NPC.target].Center;
            float speed = 3f;
            float inertia = 32f;
            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;


            AI_Timer--;
           

            if (AI_Timer == 0)
            {
                AI_Timer = 48 * 8;
                AI_State = (float)ActionState.Fire;
            }
        }
        private void Fire()
        {

            NPC.knockBackResist = 0.5f;

            targetArea = Main.player[NPC.target].Center;
            float speed = 1.5f;
            float inertia = 32f;
            direction = targetArea - NPC.Center;
            direction.Normalize();
            direction *= speed;

            NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

            if (AI_Timer > 48)
            {
                if (AI_Timer % 48 == 23)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, storedVel / 2, Mod.Find<ModProjectile>("IceSpiritBeam").Type, 8, 0, Main.myPlayer);
                    SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                }
                if (AI_Timer % 48 == 0)
                {
                    NPC.TargetClosest(true);
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("IceSpiritTelegraph").Type, 8, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 16);
                    storedVel = Main.projectile[proj].velocity;
                }
            }
            AI_Timer--;

            if (AI_Timer == 0)
            {
                AI_Timer = 600;
                AI_State = (float)ActionState.Move;
            }
        }
        private void PointTowardsPlayer()
        {
            NPC.TargetClosest(true);

            NPC.knockBackResist = 0.8f;

            targetArea = Main.player[NPC.target].Center;
            direction = targetArea - NPC.Center;
            direction.Normalize();

            NPC.velocity = direction * 2;

            AI_State = (float)ActionState.Passive;

        }
        private void Passive()
        {
            NPC.knockBackResist = 0.8f;
        }
    }
}