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
    public class CosmicObserver : ModNPC
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
            NPC.damage = 0; // The amount of damage that NPC npc deals
            NPC.defense = 12; // The amount of defense that NPC npc has
            NPC.lifeMax = 125; // The amount of health that NPC npc has
            NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath4; // The sound the NPC will make when it dies.
            NPC.value = 100f; // How many copper coins the NPC will drop when killed.
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.chaseable = false;
            NPC.npcSlots = 0;
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
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.CosmicObserver")
            });
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * NPC.Opacity;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneSkyHeight && !spawnInfo.Invasion && !spawnInfo.PlayerInTown && (DownedBossSystem.downedTsugumi || NPC.downedBoss2))
            {
                return 0.7f;
            }
            return 0;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest();
            NPC.velocity = new Vector2(MathF.Sign(NPC.Center.X - Main.player[NPC.target].Center.X) * -3, 0);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                DownedBossSystem.numCosmicObserver++;
                if (DownedBossSystem.numCosmicObserver % 3 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -8), ModContent.ProjectileType<StarFighterProjSignal>(), 0, 0, Main.player[NPC.target].whoAmI, 1);
                }
                else
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -8), ModContent.ProjectileType<StarFighterProjSignal>(), 0, 0, Main.player[NPC.target].whoAmI, 0);
                }
                Main.NewText("The Cosmic Observer emits a strange signal...");
            }
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if ((Vector2.DistanceSquared(Main.player[projectile.owner].Center, NPC.Center) < 510 * 510 && !projectile.IsMinionOrSentryRelated))
            {
                return null;
            }
            return false;
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI()
        {
            if (NPC.Center.Y < Main.player[NPC.target].Center.Y - 500)
            {
                NPC.velocity.Y = 4;
            }
            if (NPC.Center.Y > Main.player[NPC.target].Center.Y + 500)
            {
                NPC.velocity.Y = -4;
            }
            NPC.velocity.Y *= 0.96f;
            NPC.noGravity = true;
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