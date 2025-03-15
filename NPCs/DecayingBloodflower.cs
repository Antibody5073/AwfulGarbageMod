using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
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
    public class DecayingBloodflower : ModNPC
    {
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            PointTowardsPlayer,
            Passive,
            Teleport,
            Shoot,
            Teleport2,
            RottedForks,
            Teleport3,
            Summon,
            Teleport4,
            CrimsonRod
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
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 24; // The width of the npc's hitbox (in pixels)
            NPC.height = 36; // The height of the npc's hitbox (in pixels)
            NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
            NPC.damage = 0; // The amount of damage that this npc deals
            NPC.defense = 7; // The amount of defense that this npc has
            NPC.lifeMax = 500; // The amount of health that this npc has
            if (Main.expertMode)
            {
                NPC.lifeMax = 650;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 800; // Increase by 5 if expert or master mode
                }
            }
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.Item30; // The sound the NPC will make when it dies.
            NPC.value = 15000f; // How many copper coins the NPC will drop when killed.
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.chaseable = false;
            NPC.rarity = 5;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.DecayingBloodflower")
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, 1, 10, 13));
            npcLoot.Add(ItemDropRule.Common(ItemID.ViciousMushroom, 1, 2, 3));
            npcLoot.Add(ItemDropRule.FewFromOptions(1, 2, ModContent.ItemType<SigilOfEvil>(), ModContent.ItemType<BloomingEvil>(), ModContent.ItemType<EvilCharm>()));
        }
        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedEvilFlowerMiniboss, -1);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneOverworldHeight)
            {
                return 0.09f;
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
                case (float)ActionState.Teleport2:
                    Teleport2();
                    break;
                case (float)ActionState.RottedForks:
                    Ballohurt();
                    break;
                case (float)ActionState.Teleport3:
                    Teleport3();
                    break;
                case (float)ActionState.Summon:
                    Summon();
                    break;
                case (float)ActionState.Teleport4:
                    Teleport4();
                    break;
                case (float)ActionState.CrimsonRod:
                    Vilethorn();
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
                int dust = Dust.NewDust(teleportPos, 0, 0, DustID.IchorTorch, 0f, 0f, 0, default(Color), 1f);
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
                AI_Timer = 360;
                AI_State = (float)ActionState.Shoot;
            }
        }
        private void Teleport2()
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
                int dust = Dust.NewDust(teleportPos, 0, 0, DustID.IchorTorch, 0f, 0f, 0, default(Color), 1f);
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
                AI_Timer = 0;
                AI_State = (float)ActionState.RottedForks;
            }
        }
        private void Teleport3()
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
                int dust = Dust.NewDust(teleportPos, 0, 0, DustID.IchorTorch, 0f, 0f, 0, default(Color), 1f);
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
                AI_Timer = 360;
                AI_State = (float)ActionState.Summon;
            }
        }
        private void Teleport4()
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
                int dust = Dust.NewDust(teleportPos, 0, 0, DustID.IchorTorch, 0f, 0f, 0, default(Color), 1f);
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
                AI_Timer = 0;
                AI_State = (float)ActionState.CrimsonRod;
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
                    if (tile.HasTile && tile.TileType != TileID.CorruptThorns && tile.TileType != TileID.Cactus && tile.TileType != TileID.Trees && !TileID.Sets.Grass[tile.TileType])
                    {
                        return teleportPosition + new Vector2(0, -16);
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
            if (AI_Timer % 60 == 30)
            {
                Vector2 vel;
                for (int j = 0; j < 3; j++)
                {
                    vel = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.ToRadians(2.5f)) * 15f;
                    float distanceFromTarget = Vector2.Distance(Main.player[NPC.target].Center, NPC.Center);
                    vel.Y -= (distanceFromTarget / 7.5f) * 0.02f;
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ProjectileID.GoldenShowerHostile, 8, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = vel;
                    Main.projectile[proj].tileCollide = false;
                    Main.projectile[proj].extraUpdates -= 2;
                }
            }

            AI_Timer--;
            if (AI_Timer == 0)
            {
                NPC.alpha = 0;
                AI_Timer = 150;
                AI_State = (float)ActionState.Teleport2;
            }
        }
        private void Ballohurt()
        {
            if (AI_Timer == 30)
            {
                shotXspd = -17.5f;
            }
            if (AI_Timer > 30 && AI_Timer <= 60)
            {
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<HostileForkProj>(), 10, 0, Main.myPlayer, NPC.whoAmI);
                Main.projectile[proj].velocity = new Vector2(Main.rand.NextFloat(-16, 16), -16);
            }

            AI_Timer++;
            if (AI_Timer == 360)
            {
                NPC.alpha = 0;
                AI_Timer = 150;
                AI_State = (float)ActionState.Teleport3;
            }
        }
        private void Summon()
        {
            if (AI_Timer % 120 == 60)
            {
               int nPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.Crimera);
                Main.npc[nPC].velocity = new Vector2(0, Main.rand.NextFloat(-4, -6)).RotatedByRandom(MathHelper.ToRadians(15));
                Main.npc[nPC].lifeMax = (int)(Main.npc[nPC].lifeMax * 0.35f);
                Main.npc[nPC].life = Main.npc[nPC].lifeMax;
                Main.npc[nPC].damage = (int)(Main.npc[nPC].damage * 0.6f);
                Main.npc[nPC].knockBackResist = 0.75f;


            }

            AI_Timer--;
            if (AI_Timer == 0)
            {
                NPC.alpha = 0;
                AI_Timer = 150;
                AI_State = (float)ActionState.Teleport4;
            }
        }
        private void Vilethorn()
        {
            if (AI_Timer == 30)
            {
                Vector2 vel;
                Vector2 targetPosition = Main.player[NPC.target].Center - new Vector2(0, 240);
                vel = (Main.player[NPC.target].Center - NPC.Center - new Vector2(0, 240)).SafeNormalize(Vector2.Zero) * 6f;
                float distanceFromTarget = Vector2.Distance(Main.player[NPC.target].Center - new Vector2(0, 240), NPC.Center);

                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<CrimsonRodHostileProjectile>(), 8, 0, Main.myPlayer, targetPosition.X, targetPosition.Y, NPC.whoAmI);
                Main.projectile[proj].velocity = vel;
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].timeLeft = (int)(distanceFromTarget / 6);
                targetPosition = Main.player[NPC.target].Center - new Vector2(240, 240);
                vel = (Main.player[NPC.target].Center - NPC.Center - new Vector2(240, 240)).SafeNormalize(Vector2.Zero) * 6f;
                distanceFromTarget = Vector2.Distance(Main.player[NPC.target].Center - new Vector2(240, 240), NPC.Center);

                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<CrimsonRodHostileProjectile>(), 8, 0, Main.myPlayer, targetPosition.X, targetPosition.Y, NPC.whoAmI);
                Main.projectile[proj].velocity = vel;
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].timeLeft = (int)(distanceFromTarget / 6);
                targetPosition = Main.player[NPC.target].Center - new Vector2(-240, 240);
                vel = (Main.player[NPC.target].Center - NPC.Center - new Vector2(-240, 240)).SafeNormalize(Vector2.Zero) * 6f;
                distanceFromTarget = Vector2.Distance(Main.player[NPC.target].Center - new Vector2(-240, 240), NPC.Center);

                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<CrimsonRodHostileProjectile>(), 8, 0, Main.myPlayer, targetPosition.X, targetPosition.Y, NPC.whoAmI);
                Main.projectile[proj].velocity = vel;
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].timeLeft = (int)(distanceFromTarget / 6);
                targetPosition = Main.player[NPC.target].Center - new Vector2(480, 240);
                vel = (Main.player[NPC.target].Center - NPC.Center - new Vector2(480, 240)).SafeNormalize(Vector2.Zero) * 6f;
                distanceFromTarget = Vector2.Distance(Main.player[NPC.target].Center - new Vector2(480, 240), NPC.Center);

                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<CrimsonRodHostileProjectile>(), 8, 0, Main.myPlayer, targetPosition.X, targetPosition.Y, NPC.whoAmI);
                Main.projectile[proj].velocity = vel;
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].timeLeft = (int)(distanceFromTarget / 6);
                targetPosition = Main.player[NPC.target].Center - new Vector2(-480, 240);
                vel = (Main.player[NPC.target].Center - NPC.Center - new Vector2(-480, 240)).SafeNormalize(Vector2.Zero) * 6f;
                distanceFromTarget = Vector2.Distance(Main.player[NPC.target].Center - new Vector2(-480, 240), NPC.Center);

                proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<CrimsonRodHostileProjectile>(), 8, 0, Main.myPlayer, targetPosition.X, targetPosition.Y, NPC.whoAmI);
                Main.projectile[proj].velocity = vel;
                Main.projectile[proj].tileCollide = false;
                Main.projectile[proj].timeLeft = (int)(distanceFromTarget / 6);


            }


            AI_Timer++;
            if (AI_Timer == 240)
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