using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.NPCs
{
    // This ModNPC serves as an example of a completely custom AI.
    public class SentientSapling : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            PointTowardsPlayer,
            Passive,
            Teleport,
            Shoot
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
        Vector2 targetArea;
        Vector2 direction;
        Vector2 storedVel;
        Vector2 teleportPos;

        int frameCounter = 0;
        readonly int[] frameLen = { 10, 1, 10, 1, 1 };
        int frame = 0;

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
            NPC.damage = 0; // The amount of damage that this npc deals
            NPC.defense = 5; // The amount of defense that this npc has
            NPC.lifeMax = 150; // The amount of health that this npc has
            if (Main.expertMode)
            {
                NPC.lifeMax = 275;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 350; // Increase by 5 if expert or master mode
                }
            }
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.Item30; // The sound the NPC will make when it dies.
            NPC.value = 10000f; // How many copper coins the NPC will drop when killed.
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.chaseable = false;
            NPC.rarity = 2;

        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.SentientSapling")
            });
        }


        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Acorn, 1, 3, 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EnchantedLeaf>(), 1, 3, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.Magic.ForestBeam>(), 4, 1, 1));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneForest && !spawnInfo.Invasion && !spawnInfo.PlayerInTown)
            {
                return 0.07f;
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
                case (float)ActionState.Teleport:
                    Teleport();
                    break;
                case (float)ActionState.Shoot:
                    Shoot();
                    break;
            }
        }

        // Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
        // We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
        public override void FindFrame(int frameHeight)
        {
            // This makes the sprite flip horizontally in conjunction with the npc.direction.
            NPC.spriteDirection = 1;
            if (++frameCounter >= 4 * frameLen[frame])
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

        // Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (AI_State != (float)ActionState.Passive || Vector2.DistanceSquared(Main.LocalPlayer.Center, NPC.Center) < 320 * 320)
            {
                return null;
            }
            return false;
        }
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (AI_State == (float)ActionState.Passive)
            {
                NPC.TargetClosest(true);
                AI_Timer = 150;

                AI_State = (float)ActionState.Teleport;
                NPC.chaseable = true;

               
            }
        }
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {


            if (AI_State == (float)ActionState.Passive)
            {
                NPC.TargetClosest(true);
                AI_Timer = 150;

                AI_State = (float)ActionState.Teleport;
                NPC.chaseable = true;

            }

        }

        private void Teleport()
        {
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            if (AI_Timer > 90)
            {
                NPC.position.Y += 0.6f;
                NPC.alpha += 4;
                NPC.dontTakeDamage = false;
            }
            else if (AI_Timer == 90)
            {
                NPC.alpha = 255;
                teleportPos = GetTeleportPos();
                teleportPos.Y = (float)Math.Floor(teleportPos.Y / 16) * 16;
                NPC.dontTakeDamage = true;

            }
            else if (AI_Timer > 61)
            {
                int dust = Dust.NewDust(teleportPos, 0, 0, DustID.CursedTorch, 0f, 0f, 0, default(Color), 1f);
                Main.dust[dust].velocity.Y = Main.rand.NextFloat(-3, -7);
                Main.dust[dust].noGravity = false;
            }
            else if (AI_Timer == 61)
            {
                NPC.position = teleportPos - new Vector2(NPC.width / 2, NPC.height) + new Vector2(0, 0.6f * 60);
                NPC.alpha = 60 * 4;
            }
            else
            {
                NPC.alpha -= 4;
                NPC.position.Y -= 0.6f;
                NPC.dontTakeDamage = false;

            }
            AI_Timer--;
            if (AI_Timer == 0)
            {
                NPC.alpha = 0;
                AI_Timer = (int)(80 + ((float)NPC.life * 240 / NPC.lifeMax));
                AI_State = (float)ActionState.Shoot;
            }
        }
        private Vector2 GetTeleportPos()
        {
            NPC.noTileCollide = false;

            for (int i = 0; i < 1000; i++)
            {
                Vector2 teleportPosition = Main.player[NPC.target].Center + new Vector2(Main.rand.NextFloat(-400, 400), -320);
                Tile tile = Framing.GetTileSafely(teleportPosition);
                for (int j = 0; j < 64; j++)
                {
                    if (tile.HasTile && AGUtils.IsNotAmbientObject(tile.TileType))
                    {
                        return teleportPosition;
                    }
                    else
                    {
                        teleportPosition.Y += 16;
                        tile = Framing.GetTileSafely(teleportPosition);
                    }
                }
            }
            return Main.player[NPC.target].Center;
        }

        private void Shoot()
        {
            if (AI_Timer % 80 == 40)
            {
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<ForestBeam>(), 6, 0, Main.myPlayer);
                Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10f;
            }

            AI_Timer--;
            if (AI_Timer == 0)
            {
                NPC.alpha = 0;
                AI_Timer = 150;
                AI_State = (float)ActionState.Teleport;
            }
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
        private void Passive()
        {
            NPC.knockBackResist = 0f;
        }
    }
}