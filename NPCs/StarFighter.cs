using AwfulGarbageMod.BossBars;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Consumables; 
using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.NPCs;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace AwfulGarbageMod.NPCs
{
    // These three class showcase usage of the WormHead, WormBody and WormTail classes from Worm.cs
    public class StarFighterHead : StarFighterWormHead
	{
		public override int BodyType => ModContent.NPCType<StarFighterBody>();

		public override int TailType => ModContent.NPCType<StarFighterTail>();

		public override void SetStaticDefaults() {

        }

        private enum ActionState
        {
            ChasePlayer,
            Spin,
            DashTowardPlayer,
            CircleAroundPlayer,
            ChargeLaser,
            Laser,

            Despawn
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CelestiteShard>(), 1, 2, 3));

        }

        public override void SetDefaults() {
			// Head is 10 defense, body 20, tail 30.
			NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.width = 32;
            NPC.height = 32;
			NPC.aiStyle = -1;
            NPC.damage = 45; // The amount of damage that this npc deals
            NPC.defense = 5; // The amount of defense that this npc has
            NPC.lifeMax = 5000; // The amount of health that this npc has
            NPC.HitSound = SoundID.NPCHit42; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath37; // The sound the NPC will make when it dies.
            NPC.value = 25000f; // How many copper coins the NPC will drop when killed.
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.npcSlots = 10; 
            
            NPC.boss = true;

            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/oops");
            }
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            if (!Main.GameModeInfo.IsJourneyMode)
            {
                NPC.lifeMax = 6250;
                if (Main.masterMode)
                {
                    NPC.lifeMax = 7500; // Increase by 5 if expert or master mode
                    if (Main.getGoodWorld || Main.zenithWorld)
                    {
                        NPC.lifeMax = 8750;
                    }
                }
            }
        }

        public override void OnKill()
        {

            NPC.SetEventFlagCleared(ref DownedBossSystem.downedStarFighter, -1);

            // Since this hook is only ran in singleplayer and serverside, we would have to sync it manually.
            // Thankfully, vanilla sends the MessageID.WorldData packet if a BOSS was killed automatically, shortly after this hook is ran

            // If your NPC is not a boss and you need to sync the world (which includes ModSystem, check DownedBossSystem), use this code:
            /*
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
			*/
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.AwfulGarbageMod.Bestiary.StarFighterHead")
            });
        }


        public override void Init() {
			// Set the segment variance
			// If you want the segment length to be constant, set these two properties to the same value
			MinSegmentLength = 13;
			MaxSegmentLength = 13;

			CommonWormInit(this);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * NPC.Opacity;
        }

        // This method is invoked from StarFighterHead, StarFighterBody and StarFighterTail
        internal static void CommonWormInit(WormStarFighter worm) {
			// These two properties handle the movement of the worm
			worm.MoveSpeed = 5.5f;
			worm.Acceleration = 0.045f;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;

            Vector2 drawOrigin = NPC.frame.Size() / 2;
            Vector2 drawPos = NPC.position - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY); //.RotatedBy(NPC.rotation);
            Color color = NPC.GetAlpha(drawColor);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.TargetClosest();
            hand1 = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<StarFighterArm>(), NPC.whoAmI, NPC.whoAmI, -1);
            hand2 = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<StarFighterArm>(), NPC.whoAmI, NPC.whoAmI, 1);
            bossPhase = 1;
            targetPos = Main.player[NPC.target].Center;
            AI_State = (float)ActionState.ChasePlayer;
            NPC.lifeMax = NPC.lifeMax * ModContent.GetInstance<Config>().BossHealthMultiplier / 100;
            NPC.life = NPC.lifeMax;
            Main.NewText("[c/33cdff:<StarFighter> THREAT DETECTED... INITIATING EXTERMINATION PROTOCOL]");
        }

        public float AI_State;
        float Next_State;
        float AI_Timer;
        float bossPhase = 1;
        float RotateDir;
        float RotateMagnitude = 1000;
        float tempx;
        float temp;
        Vector2 targetPos;
        Vector2 storedVel = Vector2.Zero;
        int hand1;
        int hand2;

        public override void AI()
        {

            Player player = Main.player[NPC.target];

            if (!NPC.HasValidTarget)
            {
                AI_State = (float)ActionState.Despawn;
                NPC.EncourageDespawn(0);
                return;
            }
            if (bossPhase == 1 && !Main.npc[hand1].active && !Main.npc[hand2].active) 
            {
                Main.NewText("[c/33cdff:<StarFighter> UNIT DISARMED...]");
                bossPhase = 2;
            }

            if (NPC.CountNPCS(ModContent.NPCType<StarFighterTail>()) < 1)
            {
                NPC.ai[0] = 0;
            }

            switch (AI_State)
            {
                case (float)ActionState.ChasePlayer:
                    ChasePlayer();
                    break;
                case (float)ActionState.Spin:
                    Spin();
                    break;
                case (float)ActionState.DashTowardPlayer:
                    DashTowardPlayer();
                    break;
                case (float)ActionState.CircleAroundPlayer:
                    CircleAroundPlayer();
                    break;
                case (float)ActionState.ChargeLaser:
                    ChargeLaser();
                    break;
                case (float)ActionState.Laser:
                    Laser();
                    break;
                case (float)ActionState.Despawn:
                    Despawn();
                    break;
            }
        }

        private void ChasePlayer()
        {

            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 130;
                }
            }
            else
            {
                NPC.damage = 50;
            }
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity += direction * 0.0015f + direction.SafeNormalize(Vector2.Zero) * 0.1f;

            NPC.velocity *= 0.9f;

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            int tempmod;
            if (Main.expertMode)
            {
                tempmod = 12;
                if (Main.masterMode)
                {
                    tempmod = 10;
                }
            }
            else
            {
                tempmod = 15;
            }
            if (AI_Timer % tempmod == 0)
            {
                int damage = 17;
                if (Main.expertMode)
                {
                    damage = 16;
                    if (Main.masterMode)
                    {
                        damage = 15;
                    }
                }
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/SmallShot2");
                SoundEngine.PlaySound(impactSound, NPC.Center);
                if (bossPhase == 2)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathF.Pow(MathF.Sin(AI_Timer / 50f), 2) * MathHelper.PiOver2) * 2f, ModContent.ProjectileType<StarFighterProjBlast>(), damage, 0, Main.myPlayer, 0.04f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathF.Pow(MathF.Sin(AI_Timer / 50f), 2) * -MathHelper.PiOver2) * 2f, ModContent.ProjectileType<StarFighterProjBlast>(), damage, 0, Main.myPlayer, 0.04f);
                }
                else
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathF.Sin(AI_Timer / 35f) * MathHelper.ToRadians(60)) * 4f, ModContent.ProjectileType<StarFighterProjBlast>(), damage, 0, Main.myPlayer, 0.03f);
                }
            }


            AI_Timer++;
            if (AI_Timer == 1200)
            {
                AI_State = (float)ActionState.Spin;
                AI_Timer = 0;
            }
        }
        private void Spin()
        {

            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 130;
                }
            }
            else
            {
                NPC.damage = 50;
            }
            Player player = Main.player[NPC.target];

            if (NPC.velocity.LengthSquared() < 10 * 10)
            {
                NPC.velocity += NPC.velocity.SafeNormalize(Vector2.One) * 0.1f;
            }

            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(8));

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            if (AI_Timer % 3 == 0)
            {
                int damage = 22;
                if (Main.expertMode)
                {
                    damage = 21;
                    if (Main.masterMode)
                    {
                        damage = 20;
                    }
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(120, 120), Vector2.Zero, ModContent.ProjectileType<StarFighterProjExplosion>(), damage, 0, Main.myPlayer);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(120, 120), Vector2.Zero, ModContent.ProjectileType<StarFighterProjExplosion>(), damage, 0, Main.myPlayer);
            }
            if (bossPhase == 2)
            {
                if (AI_Timer % 16 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item9, NPC.Center);

                    int damage = 22;
                    if (Main.expertMode)
                    {
                        damage = 21;
                        if (Main.masterMode)
                        {
                            damage = 20;
                        }
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity / 2, ModContent.ProjectileType<StarFighterProjStar>(), damage, 0, Main.myPlayer, player.whoAmI);
                }
            }
            else
            {
                if (AI_Timer % 45 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item9, NPC.Center);

                    int damage = 22;
                    if (Main.expertMode)
                    {
                        damage = 21;
                        if (Main.masterMode)
                        {
                            damage = 20;
                        }
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity / 2, ModContent.ProjectileType<StarFighterProjStar>(), damage, 0, Main.myPlayer, player.whoAmI);
                }
            }


                AI_Timer++;
            if (AI_Timer == 360)
            {
                AI_State = (float)ActionState.DashTowardPlayer;
                AI_Timer = 0;
                NPC.velocity = new Vector2(0, 0);
            }
        }
        private void DashTowardPlayer()
        {

            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 130;
                }
            }
            else
            {
                NPC.damage = 50;
            }
            Player player = Main.player[NPC.target];
            NPC.velocity *= 0.985f;

            if (NPC.velocity.LengthSquared() < 5 * 5)
            {
                NPC.velocity += NPC.velocity.SafeNormalize(Vector2.One) * 0.1f;
            }
            if (NPC.velocity.LengthSquared() < 6 * 6)
            {
                float turn = AGUtils.TurnTowards(5, player.Center, NPC.velocity.ToRotation(), NPC.Center);
                NPC.velocity = NPC.velocity.RotatedBy(turn);
            }

            if (AI_Timer % 150 == 0)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/laser_3")
                {
                    Volume = 0.3f,
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
                int damage = 22;
                if (Main.expertMode)
                {
                    damage = 21;
                    if (Main.masterMode)
                    {
                        damage = 20;
                    }
                }
                if (bossPhase == 2)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 vel = Main.rand.NextVector2Circular(8, 8);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<StarFighterProjStar>(), damage, 0, Main.myPlayer, player.whoAmI);
                    }
                }
                Vector2 direction = player.Center - NPC.Center;
                NPC.velocity += direction.SafeNormalize(Vector2.Zero) * 20f;
            }
            if (bossPhase == 2)
            {
                if (AI_Timer % 150 == 60)
                {
                    SoundEngine.PlaySound(SoundID.Item60, NPC.Center);

                    Vector2 direction = player.Center - NPC.Center;
                    NPC.velocity = direction.SafeNormalize(Vector2.Zero) * 10f;
                    NPC.velocity += direction / 75;
                }
            }



            AI_Timer++;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            if (AI_Timer == 750)
            {
                AI_State = (float)ActionState.CircleAroundPlayer;
                AI_Timer = 0;

                NPC.velocity = new Vector2(0, 0);
                targetPos = Main.player[NPC.target].Center;

                RotateMagnitude = Vector2.Distance(NPC.Center, player.Center);
                RotateDir = MathHelper.ToDegrees((NPC.Center - Main.player[NPC.target].Center).ToRotation());
            }
        }
        private void CircleAroundPlayer()
        {
            if (Main.expertMode)
            {
                NPC.damage = 110;

                if (Main.masterMode)
                {
                    NPC.damage = 160;
                }
            }
            else
            {
                NPC.damage = 60;
            }
            Player player = Main.player[NPC.target];

            NPC.velocity = new Vector2(0, 0);

            NPC.noTileCollide = true;
            float speed = Vector2.Distance(player.Center, targetPos) / 10000;


            Vector2 direction = player.Center - targetPos;
            direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            targetPos += direction;

            NPC.position = targetPos + new Vector2(1, 0).RotatedBy(MathHelper.ToRadians(RotateDir)) * RotateMagnitude - new Vector2(NPC.width / 2, NPC.height / 2);

            Vector2 temp = targetPos - NPC.Center;



            NPC.rotation = temp.ToRotation() - MathHelper.ToRadians(0);

            if (RotateMagnitude > 400)
            {
                RotateMagnitude -= 3;
            }
            else if (RotateMagnitude < 395)
            {
                RotateMagnitude += 3;
            }
            else
            {
                if (bossPhase == 2)
                {
                    if (AI_Timer % 7 == 0)
                    {
                        int damage = 17;
                        if (Main.expertMode)
                        {
                            damage = 16;
                            if (Main.masterMode)
                            {
                                damage = 15;
                            }
                        }

                        SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/SmallShot2")
                        {
                            MaxInstances = 4,
                        };
                        SoundEngine.PlaySound(impactSound, NPC.Center);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(2, 2), ModContent.ProjectileType<StarFighterProjBlast>(), damage, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-2, 2), ModContent.ProjectileType<StarFighterProjBlast>(), damage, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(2, -2), ModContent.ProjectileType<StarFighterProjBlast>(), damage, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-2, -2), ModContent.ProjectileType<StarFighterProjBlast>(), damage, 0, Main.myPlayer);

                    }
                }
                else
                {
                    if (AI_Timer % 7 == 0)
                    {
                        int damage = 17;
                        if (Main.expertMode)
                        {
                            damage = 16;
                            if (Main.masterMode)
                            {
                                damage = 15;
                            }
                        }

                        SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/SmallShot2")
                        {
                            MaxInstances = 4,
                        };
                        SoundEngine.PlaySound(impactSound, NPC.Center);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, temp.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(42)) * 5, ModContent.ProjectileType<StarFighterProjBlast>(), damage, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, temp.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(-42)) * 5, ModContent.ProjectileType<StarFighterProjBlast>(), damage, 0, Main.myPlayer);
                    }
                }
                AI_Timer++;

                if (AI_Timer == 750)
                {
                    AI_State = (float)ActionState.ChargeLaser;
                    AI_Timer = 0;
                }
            }
            RotateDir += 2f;
        }
        private void ChargeLaser()
        {

            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 130;
                }
            }
            else
            {
                NPC.damage = 50;
            }
            Player player = Main.player[NPC.target];

            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            if (AI_Timer < 390)
            {
                NPC.velocity += direction * 0.001f + direction.SafeNormalize(Vector2.Zero) * 0.1f;
            }
            NPC.velocity *= 0.9f;

            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            if (bossPhase == 1)
            {
                int tempmod;
                if (Main.expertMode)
                {
                    tempmod = 5;
                    if (Main.masterMode)
                    {
                        tempmod = 4;
                    }
                }
                else
                {
                    tempmod = 6;
                }

                if (AI_Timer % tempmod == 0 && AI_Timer < 330)
                {
                    int damage = 16;
                    if (Main.expertMode)
                    {
                        damage = 15;
                        if (Main.masterMode)
                        {
                            damage = 14;
                        }
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (player.Center - NPC.Center).SafeNormalize(Vector2.One).RotatedByRandom(MathHelper.Pi) * 1400, Main.rand.NextVector2CircularEdge(5.25f, 5.25f), ModContent.ProjectileType<StarFighterProjCharge>(), damage, 0, Main.myPlayer, NPC.whoAmI);
                }
            }
            else
            {
                int tempmod;
                if (Main.expertMode)
                {
                    tempmod = 60;
                    if (Main.masterMode)
                    {
                        tempmod = 50;
                    }
                }
                else
                {
                    tempmod = 75;
                }

                if (AI_Timer % tempmod == 0 && AI_Timer < 330)
                {
                    int damage = 16;
                    if (Main.expertMode)
                    {
                        damage = 15;
                        if (Main.masterMode)
                        {
                            damage = 14;
                        }
                    }
                    float dir = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    for (int i = 0; i < 20; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (player.Center - NPC.Center).SafeNormalize(Vector2.One).RotatedBy(i * MathHelper.TwoPi / 20 + dir) * 1400, Main.rand.NextVector2CircularEdge(6f, 6f), ModContent.ProjectileType<StarFighterProjCharge>(), damage, 0, Main.myPlayer, NPC.whoAmI);
                    }
                }
            }
            if (AI_Timer == 60)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<StarFighterProjLaserHead>(), 0, 0, Main.myPlayer, NPC.whoAmI, 390);
            }
            if (AI_Timer == 60 || AI_Timer == 120 || AI_Timer == 180 || AI_Timer == 240 || AI_Timer == 300)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/charge_3")
                {
                    Volume = 0.2f,
                    
                };
                SoundEngine.PlaySound(impactSound, NPC.Center);
            }
                AI_Timer++;
            if (AI_Timer == 420)
            {
                AI_State = (float)ActionState.Laser;
                AI_Timer = 0;
                temp = 0;
            }
        }
        private void Laser()
        {

            if (Main.expertMode)
            {
                NPC.damage = 90;

                if (Main.masterMode)
                {
                    NPC.damage = 130;
                }
            }
            else
            {
                NPC.damage = 50;
            }
            Player player = Main.player[NPC.target];

            NPC.velocity *= 0.985f;

            if (NPC.velocity.LengthSquared() < 2 * 2)
            {
                NPC.velocity += NPC.velocity.SafeNormalize(Vector2.One) * 0.05f;
            }

            if (AI_Timer > 30 && AI_Timer < 480)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/LaserBeam1");

                if (SoundEngine.FindActiveSound(impactSound) == null)
                {
                    SoundEngine.PlaySound(impactSound, NPC.Center);
                } 
            }
            if (AI_Timer == 480)
            {
                SoundStyle impactSound = new SoundStyle($"{nameof(AwfulGarbageMod)}/Assets/Sound/LaserBeam1");

                SoundEngine.FindActiveSound(impactSound).Stop();
            }

            float turn = AGUtils.TurnTowards(temp, player.Center, NPC.velocity.ToRotation(), NPC.Center);
            NPC.velocity = NPC.velocity.RotatedBy(turn);
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            temp += 0.01f;
            if (temp> 2)
            {
                temp = 2;
            }
            if (AI_Timer == 30)
            {
                int damage = 18;
                if (Main.expertMode)
                {
                    damage = 17;
                    if (Main.masterMode)
                    {
                        damage = 16;
                    }
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<StarFighterProjLaser>(), damage, 0, Main.myPlayer, NPC.whoAmI, 0);
            }


            AI_Timer++;
            if (AI_Timer == 510)
            {
                AI_State = (float)ActionState.ChasePlayer;
                AI_Timer = 0;
            }
        }

        private void Despawn()
        {
            if (Main.expertMode)
            {
                NPC.damage = 75;

                if (Main.masterMode)
                {
                    NPC.damage = 110;
                }
            }
            else
            {
                NPC.damage = 40;
            }
            Player player = Main.player[NPC.target];


            Vector2 direction = player.Center - NPC.Center;
            direction.SafeNormalize(Vector2.Zero);
            NPC.velocity -= direction * 0.0002f;
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

            if (direction.Length() > 5000)
            {
                NPC.active = false;
            }
        }
    }

	internal class StarFighterBody : StarFighterWormBody
	{
		public override void SetStaticDefaults() {
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;

        }
        string texture = "AwfulGarbageMod/NPCs/StarFighterHead";

        public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.damage = 60; // The amount of damage that this npc deals
            NPC.defense = 15; // The amount of defense that this npc has

            NPC.HitSound = SoundID.NPCHit42; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath37; // The sound the NPC will make when it dies.
            }

        public override void AI()
        {
            if (NPC.HasBuff(BuffID.OnFire))
            {
                NPC.DelBuff(NPC.FindBuffIndex(BuffID.OnFire));
            }
            if (NPC.HasBuff(BuffID.Frostburn))
            {
                NPC.DelBuff(NPC.FindBuffIndex(BuffID.Frostburn));
            }
            if (NPC.ai[2] % 4 == 0)
            {
                NPC.dontTakeDamage = false;
            }
            else
            {
                NPC.dontTakeDamage = true;
            }
                base.AI();
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[2] > 9)
            {
                texture = "AwfulGarbageMod/NPCs/StarFighterBody4";
            }
            else if (NPC.ai[2] > 6)
            {
                texture = "AwfulGarbageMod/NPCs/StarFighterBody3";
            }
            else if (NPC.ai[2] > 3)
            {
                texture = "AwfulGarbageMod/NPCs/StarFighterBody2";
            }
            else
            {
                texture = "AwfulGarbageMod/NPCs/StarFighterBody";
            }
            NPC.frame = new Rectangle(0, 0, ModContent.Request<Texture2D>(texture).Width(), ModContent.Request<Texture2D>(texture).Height());
        }
        public override void Init() {
			StarFighterHead.CommonWormInit(this);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * NPC.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;

            Vector2 drawOrigin = NPC.frame.Size() / 2;
            Vector2 drawPos = NPC.position - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY); //.RotatedBy(NPC.rotation);
            Color color = NPC.GetAlpha(drawColor);
            spriteBatch.Draw(ModContent.Request<Texture2D>(texture).Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }

	internal class StarFighterTail : StarFighterWormTail
	{
		public override void SetStaticDefaults() {
			NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) {
				Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
		}

		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit42; // The sound the NPC will make when being hit.
            NPC.DeathSound = SoundID.NPCDeath37; // The sound the NPC will make when it dies.
            NPC.damage = 60; // The amount of damage that this npc deals
            NPC.defense = 15; // The amount of defense that this npc has
        }

        public override void AI()
        {
            base.AI();
            if (NPC.ai[2] % 4 == 0)
            {
                NPC.dontTakeDamage = false;
            }
            else
            {
                NPC.dontTakeDamage = true;
            }
        }
        public override void Init() {
			StarFighterHead.CommonWormInit(this);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * NPC.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;

            Vector2 drawOrigin = NPC.frame.Size() / 2;
            Vector2 drawPos = NPC.position - screenPos + new Vector2(NPC.width / 2, NPC.height / 2) + new Vector2(0, NPC.gfxOffY); //.RotatedBy(NPC.rotation);
            Color color = NPC.GetAlpha(drawColor);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);

            // It's important to return false, otherwise we also draw the original texture.
            return false;
        }
    }
}
