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

namespace AwfulGarbageMod.NPCs.Boss
{
	// This ModNPC serves as an example of a completely custom AI.
	public class TreeToad : ModNPC
	{
		// Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
		private enum ActionState
		{
			Wait,
			Jump,
			Fall
		}

		// Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
		// These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
		private enum Frame
		{
			Idle,
			Jump1,
			Jump2,
			Fall1,
			Fall2
		}

		// These are reference properties. One, for example, lets us write AI_State as if it's NPC.ai[0], essentially giving the index zero our own name.
		// Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "npc.ai[0]", which is quite hard to read.
		// This is all to just make beautiful, manageable, and clean code.
		float AI_State;
		float AI_Timer;
        float JumpCount;
		float JumpsUsed;
        float GravSpeed;
		float randLeaf;



        public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Flutter Slime"); // Automatic from localization files
			Main.npcFrameCount[NPC.type] = 6; // make sure to set this for your modnpcs.

			// Specify the debuffs it is immune to
			NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData {
				SpecificallyImmuneTo = new int[] {
					BuffID.Wet // This NPC will be immune to the Poisoned debuff.
				}
			});
		}

		public override void SetDefaults()
		{
			NPC.width = 88; // The width of the npc's hitbox (in pixels)
			NPC.height = 64; // The height of the npc's hitbox (in pixels)
			NPC.aiStyle = -1; // This npc has a completely unique AI, so we set this to -1. The default aiStyle 0 will face the player, which might conflict with custom AI code.
			NPC.damage = 20; // The amount of damage that this npc deals
			NPC.defense = 0; // The amount of defense that this npc has
			NPC.lifeMax = 2200; // The amount of health that this npc has
			NPC.HitSound = SoundID.NPCHit1; // The sound the NPC will make when being hit.
			NPC.DeathSound = SoundID.NPCDeath1; // The sound the NPC will make when it dies.
			NPC.value = 50000f; // How many copper coins the NPC will drop when killed.
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.boss = true;
			NPC.npcSlots = 100f;


			Music = MusicID.Boss3;

            NPC.BossBar = ModContent.GetInstance<TreeToadBossBar>();

        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
			NPC.lifeMax = 2700;
            if (Main.masterMode)
            {
                NPC.lifeMax = 3400; // Increase by 5 if expert or master mode
            }
        }

        public override void OnKill()
        {
            // This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedTreeToad, -1);

            // Since this hook is only ran in singleplayer and serverside, we would have to sync it manually.
            // Thankfully, vanilla sends the MessageID.WorldData packet if a BOSS was killed automatically, shortly after this hook is ran

            // If your NPC is not a boss and you need to sync the world (which includes ModSystem, check DownedBossSystem), use this code:
            /*
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			*/
        }

        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
        public override void AI() {
			// The npc starts in the asleep state, waiting for a player to enter range
			switch (AI_State) {
				case (float)ActionState.Jump:
					Jump();
					break;
				case (float)ActionState.Fall:
					NPC.damage = 16;
					if (Main.expertMode)
					{
						NPC.damage = 25;
						if (Main.masterMode) 
						{
							NPC.damage = 36;
						}
					}
					Fall();
					break;
				case (float)ActionState.Wait:
					Wait();
					NPC.damage = 0;
					break;
			}
		}

		// Here in FindFrame, we want to set the animation frame our npc will use depending on what it is doing.
		// We set npc.frame.Y to x * frameHeight where x is the xth frame in our spritesheet, counting from 0. For convenience, we have defined a enum above.
		public override void FindFrame(int frameHeight) {
			// This makes the sprite flip horizontally in conjunction with the npc.direction.
			NPC.spriteDirection = NPC.direction;

			frameHeight = 106;
			// For the most part, our animation matches up with our states.
			switch (AI_State) {
				case (float)ActionState.Wait:
					// npc.frame.Y is the goto way of changing animation frames. npc.frame starts from the top left corner in pixel coordinates, so keep that in mind.
					NPC.frame.Y = (int)Frame.Idle * frameHeight;
					break;
				case (float)ActionState.Jump:
					NPC.frame.Y = (int)Frame.Jump1 * frameHeight;
					break;
				case (float)ActionState.Fall:
					if (AI_Timer < 55)
					{
                        NPC.frame.Y = (int)Frame.Jump2 * frameHeight;
                    }
					else if (AI_Timer < 65)
					{
                        NPC.frame.Y = (int)Frame.Fall1 * frameHeight;
                    }
					else 
					{
                        NPC.frame.Y = (int)Frame.Fall2 * frameHeight;
                    }
					break;
			}
		}

		// Here, because we use custom AI (aiStyle not set to a suitable vanilla value), we should manually decide when Flutter Slime can fall through platforms
		public override bool? CanFallThroughPlatforms() {
			if (AI_State == (float)ActionState.Fall && NPC.HasValidTarget && Main.player[NPC.target].Center.Y > NPC.Bottom.Y) {
				// If Flutter Slime is currently falling, we want it to keep falling through platforms as long as it's above the player
				return true;
			}
			return false;
			// You could also return null here to apply vanilla behavior (which is the same as false for custom AI)
		}

		
		private void Jump() 
		{
			// We apply an initial velocity the first tick we are in the Jump frame. Remember that -Y is up.
			NPC.takenDamageMultiplier = 1;
			JumpsUsed++;
			JumpCount++;
            float Xdifference = Main.player[NPC.target].Center.X - NPC.Center.X;
			float JumpSpeed;
            if (Main.player[NPC.target].Bottom.Y < NPC.Bottom.Y)
			{
                //JumpSpeed = -8f;
                //GravSpeed = -8f / 60;
                JumpSpeed = (Main.player[NPC.target].Bottom.Y - NPC.Bottom.Y)/30 - 8f;
                GravSpeed = ((Main.player[NPC.target].Bottom.Y - NPC.Bottom.Y)/30 - 8f) / 60;
            }
			else
			{
                JumpSpeed = -8f;
                GravSpeed = -8f / 60;
            }
			if (JumpsUsed == 9)
			{
				if (Main.player[NPC.target].Center.X > NPC.Center.X)
				{
					NPC.velocity = new Vector2((Xdifference - 450) / 120 + Main.player[NPC.target].velocity.X, JumpSpeed);
                }
				else
				{
					NPC.velocity = new Vector2((Xdifference + 450) / 120 + Main.player[NPC.target].velocity.X, JumpSpeed);
                }
                AI_Timer = 0;
                AI_State = (float)ActionState.Fall;
            }
            else if (JumpsUsed == 10)
            {
                JumpSpeed += -4f;
                GravSpeed += -4f / 60;
                JumpCount = 0;
                if (Main.player[NPC.target].Center.X > NPC.Center.X)
				{
                    NPC.velocity = new Vector2(Xdifference / 60 + Main.player[NPC.target].velocity.X, JumpSpeed);
				}
				else
				{
                    NPC.velocity = new Vector2(Xdifference / 60 + Main.player[NPC.target].velocity.X, JumpSpeed);
		        }
				JumpsUsed = 0;
                AI_Timer = 0;
				randLeaf = Main.rand.Next(0, 12);
                AI_State = (float)ActionState.Fall;
            }
            else
			{
				if (JumpCount % 3 == 0)
				{
					NPC.velocity = new Vector2(Xdifference / 120 + Main.player[NPC.target].velocity.X, JumpSpeed);
				}
				else
				{
					NPC.velocity = new Vector2(Xdifference / 120, JumpSpeed);
				}
				AI_Timer = 0;
				AI_State = (float)ActionState.Fall;
			}

            if (NPC.velocity.X < 0)
            {
                NPC.direction = (int)MathHelper.ToRadians(90);
            }
            else
            {
                NPC.direction = (int)MathHelper.ToRadians(-90);
            }

            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadTelegraph").Type, 0, 0, Main.myPlayer, GravSpeed);
        }

        private void Wait()
        {
            AI_Timer++;
			if (AI_Timer > 20)
			{
				AI_State = (float)ActionState.Jump;
				AI_Timer = 0;
			}
        }
			
        private void Fall() {
			AI_Timer++;

            NPC.velocity.Y -= GravSpeed;

            if (JumpsUsed == 0 && AI_Timer % 12 == randLeaf)
            {
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 0), Mod.Find<ModProjectile>("TreeToadLeafTele").Type, 17, 0, Main.myPlayer);
				Main.projectile[proj].timeLeft = 120 - (int)AI_Timer;
            }

            if (AI_Timer == 60)
            {
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f;
            }

            if (Main.expertMode)
            {
				if (AI_Timer == 55 || AI_Timer == 65)
				{
					int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
					Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f;
				}
			}

            if (NPC.velocity.Y < 0)
			{
				NPC.noTileCollide = true;
			}
			else
			{
				if (Main.player[NPC.target].Center.Y > NPC.Bottom.Y)
				{
					NPC.noTileCollide = true;
				}
				else
				{
                    NPC.noTileCollide = false;
                }

			}
			
			if (AI_Timer > 120) {
                if (JumpsUsed == 9)
                {
					AI_Timer = 15;
                }
				else if (JumpsUsed == 0)
				{
					AI_Timer = -450;
					NPC.takenDamageMultiplier = 1.25f;
				}
				else
				{
                    AI_Timer = 0;
                }
                AI_State = (float)ActionState.Wait;
				NPC.velocity.X = 0;
                NPC.noTileCollide = false;

                if (JumpCount % 3 == 0 || JumpsUsed == 9 || JumpsUsed == 10)
                {
                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                    Main.projectile[proj].velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f;
					if (Main.expertMode)
					{
                        for (var i = 0; i < 3; i++)
                        {
                            int proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                            Main.projectile[proj2].velocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f).RotatedBy(MathHelper.ToRadians(1.2f * i));
                            int proj3 = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModProjectile>("TreeToadLeaf").Type, 17, 0, Main.myPlayer);
                            Main.projectile[proj3].velocity = ((Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8f).RotatedBy(MathHelper.ToRadians(-1.2f * i));

                        }
                    }
                    
                }
            }
        }
	}
}