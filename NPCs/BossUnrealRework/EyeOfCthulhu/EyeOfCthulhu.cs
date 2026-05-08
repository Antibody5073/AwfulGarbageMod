using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
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
using AwfulGarbageMod.Items.Consumables.BossSummon;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.CodeAnalysis;
using ReLogic.Peripherals.RGB;
using AwfulGarbageMod.Configs;
using Terraria.UI;
using AwfulGarbageMod.Items.Placeable.OresBars;
using ReLogic.Content;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Terraria.Graphics.Effects;
using AwfulGarbageMod.Global;
using rail;

namespace AwfulGarbageMod.NPCs.BossUnrealRework.EyeOfCthulhu
{
    // This ModNPC serves as an example of a completely custom AI.

    public class EyeOfCthulhu : UnrealNPCOverride
    {
        protected override int OverriddenNpc => NPCID.EyeofCthulhu;
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            SpawnAi,
            VerticalLaser,
            FormationLaser,
            Dash,
            TeardropRain,
            PhaseTransition,
            Flamethrower,
            LaserWall,
        }
        // These are reference properties. One, for example, lets us write AI_State as if it's Npc.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "Npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        public float AI_State;
        float Next_State;
        public float AI_Timer;
        float bossPhase;
        float atkX;
        int atkUseCounter = 0;
        int atkDelay = 0;
        int frame;
        int frameCounter;
        bool jumping = false;
        bool lunging = false;
        int y0;
        bool rand;
        bool rand2;
        int tileHitboxIndex;
        float oldXvel;
        float xacc = 0.2f;
        float yacc = -0.1f;
        int hand1;
        int hand2;
        int hand3;
        int hand4;
        float spinspd;
        public float atkDir;
        public List<Vector2> oldPos = [];
        Vector2 targetArea;
        Vector2 direction;
        Vector2 vel;
        Vector2 storedVel;
        Vector2 storedPos;
        Vector2 recoil;
        List<Vector2> playerPos = new List<Vector2> { };

        public override void SetStaticDefaults()
        {
            if (DifficultyModes.Difficulty > 0)
            {
                NPCID.Sets.TrailCacheLength[NPCID.EyeofCthulhu] = 144;
                NPCID.Sets.TrailingMode[NPCID.EyeofCthulhu] = 1;
            }
        }
        public override void SetDefaults(NPC entity)
        {
            if (DifficultyModes.Difficulty > 0)
            {
                entity.lifeMax = 4500;
                entity.aiStyle = -1;
                entity.noGravity = true;
                entity.damage = 35;
                entity.defense = 5;
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (DifficultyModes.Difficulty > 0)
            {
                //hand1 = NPC.NewNPC(NPC.GetBossSpawnSource(Npc.target), (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 1);
                // hand2 = NPC.NewNPC(source, (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 2);
                //hand3 = NPC.NewNPC(source, (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 3);
                //hand4 = NPC.NewNPC(source, (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 4);
                if (DifficultyModes.Difficulty == 2)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                    npc.life = npc.lifeMax;
                }
            }
        }


        // Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame

        public override bool UnrealAI(NPC npc)
        {
            Player player = Main.player[Npc.target];
            if (player.dead)
            {
                Npc.position.Y += 999;
                Npc.EncourageDespawn(0);
                return false;
            }

            if (bossPhase == 0)
            {
                Npc.TargetClosest(true);
                bossPhase = 1;
                AI_Timer = 0;
                AI_State = 0;
                npc.ai[0] = 0;
            }
            if (npc.life < npc.lifeMax * 3 / 5 && bossPhase == 1)
            {

                Npc.TargetClosest(true);
                bossPhase = 2;

                SwitchAttack(ActionState.PhaseTransition);
            }
            if (Main.dayTime)
            {
                Npc.velocity.Y -= 0.1f;
                Npc.EncourageDespawn(0);
            }
            else
            {
                switch (AI_State)
                {
                    case (float)ActionState.VerticalLaser:
                        VerticalLaser();
                        break;
                    case (float)ActionState.SpawnAi:
                        SpawnAi();
                        break;
                    case (float)ActionState.FormationLaser:
                        FormationLaser();
                        break;
                    case (float)ActionState.Dash:
                        Dash();
                        break;
                    case (float)ActionState.TeardropRain:
                        TeardropRain();
                        break;
                    case (float)ActionState.PhaseTransition:
                        PhaseTransition();
                        break;
                    case (float)ActionState.Flamethrower:
                        Flamethrower();
                        break;
                    case (float)ActionState.LaserWall:
                        LaserWall();
                        break;
                }
            }
            return false;
        }

        private void SwitchAttack(ActionState attack)
        {
            AI_State = (float)attack;
            switch (AI_State)
            {
                case (float)ActionState.VerticalLaser:
                    AI_Timer = -30;
                    xacc = 0.2f;
                    xacc = -0.1f;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    y0 = 0;
                    jumping = false;


                    oldPos.Clear();
                    oldPos.Insert(0, Npc.Center);
                    break;
                case (float)ActionState.FormationLaser:
                    AI_Timer = -15;
                    break;
                case (float)ActionState.Dash:
                    AI_Timer = -45;
                    break;
                case (float)ActionState.TeardropRain:
                    AI_Timer = -30;
                    break;
                case (float)ActionState.PhaseTransition:
                    AI_Timer = -30;
                    spinspd = 0;
                    break;
                case (float)ActionState.Flamethrower:
                    AI_Timer = -90;
                    break;
                case (float)ActionState.LaserWall:
                    AI_Timer = -60;
                    Player player = Main.player[Npc.target];
                    targetArea = player.Center + new Vector2(Main.rand.NextFloat(-300, 300), -400);

                    break;
            }
        }
        private void SpawnAi()
        {

            Player player = Main.player[Npc.target];
            targetArea = player.Center + new Vector2(player.velocity.X * 20, -320);

            float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

            float inertia = 12f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer += 1;
            if (AI_Timer == 90)
            {

                hand1 = NPC.NewNPC(NPC.GetBossSpawnSource(Npc.target), (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 1);
                hand2 = NPC.NewNPC(NPC.GetBossSpawnSource(Npc.target), (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 2);
                hand3 = NPC.NewNPC(NPC.GetBossSpawnSource(Npc.target), (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 3);
                hand4 = NPC.NewNPC(NPC.GetBossSpawnSource(Npc.target), (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 4);

                Main.npc[hand1].realLife = Npc.whoAmI;
                Main.npc[hand2].realLife = Npc.whoAmI;
                Main.npc[hand3].realLife = Npc.whoAmI;
                Main.npc[hand4].realLife = Npc.whoAmI;
                SwitchAttack(ActionState.VerticalLaser);
            }
        }
        private void VerticalLaser()
        {

            Npc.TargetClosest(false);
            Player player = Main.player[Npc.target];
            targetArea = player.Center + new Vector2(player.velocity.X * 20, -320);

            oldPos.Insert(0, Npc.Center);
            if (oldPos.Count > 96)
            {
                oldPos.RemoveAt(oldPos.Count - 1);
            }
            direction = targetArea - Npc.Center;
            direction.Normalize();
            direction *= 0.5f;

            if (Npc.velocity.X > 12)
            {
                Npc.velocity.X = 12;
            }
            if (Npc.velocity.X < -12)
            {
                Npc.velocity.X = -12;
            }

            if (Npc.velocity.Y > 6)
            {
                Npc.velocity.Y = 6;
            }
            if (Npc.velocity.Y < -6)
            {
                Npc.velocity.Y = -6;
            }

            if (Npc.Center.X < targetArea.X - 300)
            {
                xacc = 0.3f;
            }
            if (Npc.Center.X > targetArea.X + 300)
            {
                xacc = -0.3f;
            }
            Npc.velocity.X += xacc;

            if (Npc.Center.Y < targetArea.Y - 20)
            {
                yacc = 0.4f;
            }
            if (Npc.Center.Y > targetArea.Y + 20)
            {
                yacc = -0.4f;
            }
            Npc.velocity.Y += yacc;
            if (AI_Timer % 120 == 0)
            {
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 15), ModContent.ProjectileType<EyeOfCthulhuProjLaser>(), 17, 0, Npc.target, Npc.whoAmI, 60, 72);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand1].Center, new Vector2(0, 15), ModContent.ProjectileType<EyeOfCthulhuProjLaser>(), 17, 0, Npc.target, Main.npc[hand1].whoAmI, 60, 32);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand2].Center, new Vector2(0, 15), ModContent.ProjectileType<EyeOfCthulhuProjLaser>(), 17, 0, Npc.target, Main.npc[hand2].whoAmI, 60, 32);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand3].Center, new Vector2(0, 15), ModContent.ProjectileType<EyeOfCthulhuProjLaser>(), 17, 0, Npc.target, Main.npc[hand3].whoAmI, 60, 32);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand4].Center, new Vector2(0, 15), ModContent.ProjectileType<EyeOfCthulhuProjLaser>(), 17, 0, Npc.target, Main.npc[hand4].whoAmI, 60, 32);
            }
            
            AI_Timer += 1;

            if (AI_Timer == 450)
            {
                SwitchAttack(ActionState.FormationLaser);
                return;

            }

            Npc.rotation += MathHelper.PiOver2;
            float turn = AGUtils.TurnTowards(5, Npc.Center + new Vector2(0, 5), Npc.rotation, Npc.Center);
            Npc.rotation += turn;
            Npc.rotation -= MathHelper.PiOver2;

        }
        private void FormationLaser()
        {
            Player player = Main.player[Npc.target];
            if (AI_Timer > 0)
            {
                if (AI_Timer % 42 == 0)
                {
                    targetArea = player.Center + new Vector2(player.velocity.X + Main.rand.NextFloat(-150, 150), -400 + Main.rand.NextFloat(0, 20));
                    for (int i = 0; i < 6; i++)
                    {
                        Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 6).RotatedBy(MathHelper.ToRadians(38 + i * 4)), ProjectileID.EyeLaser, 16, 0, Npc.target);
                        Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 6).RotatedBy(MathHelper.ToRadians(-38 - i * 4)), ProjectileID.EyeLaser, 16, 0, Npc.target);
                    }
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand1].Center, new Vector2(0, 7), ProjectileID.DeathLaser, 14, 0, Npc.target);
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand2].Center, new Vector2(0, 7), ProjectileID.DeathLaser, 14, 0, Npc.target);
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand3].Center, new Vector2(0, 7), ProjectileID.DeathLaser, 14, 0, Npc.target);
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand4].Center, new Vector2(0, 7), ProjectileID.DeathLaser, 14, 0, Npc.target);

                }
            }
            else
            {
                targetArea = player.Center + new Vector2(player.velocity.X, -420);
            }
            float speed = Vector2.Distance(Npc.Center, targetArea) / 5f;

            float inertia = 4f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer += 1;

            Npc.rotation += MathHelper.PiOver2;
            float turn = AGUtils.TurnTowards(5, Npc.Center + new Vector2(0, 5), Npc.rotation, Npc.Center);
            Npc.rotation += turn;
            Npc.rotation -= MathHelper.PiOver2;

            if (AI_Timer == 570)
            {
                SwitchAttack(ActionState.Dash);
                return;

            }
        }
        private void Dash()
        {
            Player player = Main.player[Npc.target];
            if (AI_Timer == 0)
            {

                for (int i = -5; i < 6; i++)
                {
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, -5.5f).RotatedBy(MathHelper.ToRadians(i * 6)), ModContent.ProjectileType<EyeOfCthulhuProjTeardrop>(), 13, 0, Npc.target, 0.075f);
                }
                for (int i = -5; i < 5; i++)
                {
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, -6.5f).RotatedBy(MathHelper.ToRadians(i * 6 + 3)), ModContent.ProjectileType<EyeOfCthulhuProjTeardrop>(), 13, 0, Npc.target, 0.075f);
                }

                vel = new Vector2(0, 8);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand1].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand2].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand3].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand4].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                Npc.velocity = new Vector2(0, 12);
            }
            else if (AI_Timer > 0)
            {
                if (AI_Timer % 120 >= 75)
                {
                    Npc.rotation += MathHelper.PiOver2;
                    float turn = AGUtils.TurnTowards(5, player.Center, Npc.rotation, Npc.Center);
                    Npc.rotation += turn;
                    Npc.rotation -= MathHelper.PiOver2;

                    Npc.velocity *= 0.95f;
                }
                if (AI_Timer % 120 >= 30)
                {
                    Npc.velocity *= 0.98f;
                }
                if (AI_Timer % 120 == 0)
                {

                    direction = player.Center - Npc.Center;
                    direction = direction.SafeNormalize(Vector2.Zero);
                    direction *= 12;

                    Npc.velocity = direction;
                    for (int i = -5; i < 6; i++)
                    {
                        Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, -5.5f).RotatedBy(MathHelper.ToRadians(i * 6)), ModContent.ProjectileType<EyeOfCthulhuProjTeardrop>(), 13, 0, Npc.target, 0.075f);
                    }
                    for (int i = -5; i < 5; i++)
                    {
                        Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, -6.5f).RotatedBy(MathHelper.ToRadians(i * 6 + 3)), ModContent.ProjectileType<EyeOfCthulhuProjTeardrop>(), 13, 0, Npc.target, 0.075f);
                    }

                    vel = (player.Center - Npc.Center).SafeNormalize(Vector2.One) * 8;
                }
                if (AI_Timer % 120 == 0 || AI_Timer % 120 == 20 || AI_Timer % 120 == 40)
                {
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand1].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand2].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand3].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand4].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                }
            }
            else
            {
                targetArea = player.Center + new Vector2(player.velocity.X, -330);


                float speed = Vector2.Distance(Npc.Center, targetArea) / 15f;

                float inertia = 4f;

                direction = targetArea - Npc.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

                Npc.rotation += MathHelper.PiOver2;
                float turn = AGUtils.TurnTowards(5, Npc.Center + new Vector2(0, 5), Npc.rotation, Npc.Center);
                Npc.rotation += turn;
                Npc.rotation -= MathHelper.PiOver2;
            }
            AI_Timer += 1;


            if (AI_Timer == 450)
            {
                SwitchAttack(ActionState.TeardropRain);
                return;
            }
        }
        private void TeardropRain()
        {
            Player player = Main.player[Npc.target];
            targetArea = player.Center + new Vector2(player.velocity.X, -300);


            float speed = Vector2.Distance(Npc.Center, targetArea) / 12f;

            float inertia = 2f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

            Npc.rotation += MathHelper.PiOver2;
            float turn = AGUtils.TurnTowards(5, Npc.Center + new Vector2(0, -5), Npc.rotation, Npc.Center);
            Npc.rotation += turn;
            Npc.rotation -= MathHelper.PiOver2;

            AI_Timer += 1;
            if (AI_Timer == 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand1].Center, new Vector2(0, -i), ModContent.ProjectileType<EyeOfCthulhuProjTeardrop>(), 13, 0, Npc.target, 0.075f);
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand2].Center, new Vector2(0, -i), ModContent.ProjectileType<EyeOfCthulhuProjTeardrop>(), 13, 0, Npc.target, 0.075f);
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand3].Center, new Vector2(0, -i), ModContent.ProjectileType<EyeOfCthulhuProjTeardrop>(), 13, 0, Npc.target, 0.075f);
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand4].Center, new Vector2(0, -i), ModContent.ProjectileType<EyeOfCthulhuProjTeardrop>(), 13, 0, Npc.target, 0.075f);
                }
            }
            if (AI_Timer % 45 == 30)
            {

                for (int i = -4; i < 5; i++)
                {
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, -5.5f).RotatedBy(MathHelper.ToRadians(i * 8)), ModContent.ProjectileType<EyeOfCthulhuProjTeardrop>(), 13, 0, Npc.target, 0.075f);
                }
            }

            if (AI_Timer == 420)
            {
                SwitchAttack(ActionState.VerticalLaser);
                return;
            }
        }
        private void PhaseTransition()
        {
            Player player = Main.player[Npc.target];

            Npc.velocity *= 0.97f;

            AI_Timer += 1;
            if (AI_Timer >= 0 && AI_Timer < 90)
            {
                if (spinspd < 1)
                {
                    spinspd += 0.02f;
                }
                Npc.rotation += spinspd;
            }
            if (AI_Timer >= 90)
            {
                Npc.ai[0] = 2;

                if (spinspd > 0.1)
                {
                    spinspd -= 0.02f;
                    Npc.rotation += spinspd;
                }
                else
                {

                    Npc.rotation += MathHelper.PiOver2;
                    float turn = AGUtils.TurnTowards(5, player.Center, Npc.rotation, Npc.Center);
                    Npc.rotation += turn;
                    Npc.rotation -= MathHelper.PiOver2;
                }
            }
            if (AI_Timer == 180)
            {
                SwitchAttack(ActionState.Flamethrower);
                return;
            }
        }
        private void Flamethrower()
        {
            Player player = Main.player[Npc.target];
            targetArea = player.Center + new Vector2(player.velocity.X, -200);


            float speed = Vector2.Distance(Npc.Center, targetArea) / 12f;

            float inertia = 2f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

            Npc.rotation += MathHelper.PiOver2;
            float turn = AGUtils.TurnTowards(5, Npc.Center + new Vector2(0, 5), Npc.rotation, Npc.Center);
            Npc.rotation += turn;
            Npc.rotation -= MathHelper.PiOver2;

            AI_Timer += 1;
            if (AI_Timer == 15)
            {
                if (Npc.Center.X > player.Center.X)
                {
                    atkDir = 1;
                }
                else
                {
                    atkDir = -1;
                }
            }
            if (AI_Timer > 0)
            {
                if (AI_Timer % 8 == 0)
                {
                    int proj = Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Bottom, new Vector2(0, 8), ProjectileID.Flames, 16, 0, Main.myPlayer);
                    Main.projectile[proj].hostile = true;
                    Main.projectile[proj].friendly = false;
                }
            }
            if (AI_Timer == 420)
            {
                SwitchAttack(ActionState.LaserWall);
                return;

            }
        }
        private void LaserWall()
        {
            Player player = Main.player[Npc.target];

            if (AI_Timer % 120 == 0)
            {
                Npc.TargetClosest(true);
                targetArea = player.Center + new Vector2(Main.rand.NextFloat(-120, 120), -400);

                Vector2 vel = (player.Center - Npc.Center).SafeNormalize(Vector2.One);

                for (int i = -6; i < 7; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Npc.GetSource_FromAI(), Npc.Center, vel.RotatedBy(MathHelper.ToRadians(i * 10)) * 3, ModContent.ProjectileType<EyeOfCthulhuProjBloodshot>(), 13, 0, Npc.target);
                }
                for (int i = -4; i < 4; i++)
                {
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, vel.RotatedBy(MathHelper.ToRadians(i * 10 + 5f)) * 2.4f, ModContent.ProjectileType<EyeOfCthulhuProjBloodshot>(), 13, 0, Npc.target);
                }
                for (int i = -3; i < 4; i++)
                {
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, vel.RotatedBy(MathHelper.ToRadians(i * 10)) * 1.8f, ModContent.ProjectileType<EyeOfCthulhuProjBloodshot>(), 13, 0, Npc.target);
                }
                if (AI_Timer% 240 == 0)
                {
                    for (int i = -3; i < 4; i++)
                    {
                        Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, vel.RotatedBy(MathHelper.ToRadians(i * 5 - 30)) * 1.2f, ModContent.ProjectileType<EyeOfCthulhuProjBloodshot>(), 13, 0, Npc.target);
                        Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, vel.RotatedBy(MathHelper.ToRadians(i * 5 + 30)) * 1.2f, ModContent.ProjectileType<EyeOfCthulhuProjBloodshot>(), 13, 0, Npc.target);
                    }
                }
                else
                {

                    for (int i = -3; i < 4; i++)
                    {
                        Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, vel.RotatedBy(MathHelper.ToRadians(i * 5)) * 1.2f, ModContent.ProjectileType<EyeOfCthulhuProjBloodshot>(), 13, 0, Npc.target);
                    }
                }
                Npc.velocity = vel * -10;
            }

            if (AI_Timer % 20 == 0 && AI_Timer > 0)
            {
                Vector2 vel = (player.Center - Npc.Center).SafeNormalize(Vector2.One) * 6;

                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand1].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand2].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand3].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
                Projectile.NewProjectile(Npc.GetSource_FromAI(), Main.npc[hand4].Center, vel, ProjectileID.DeathLaser, 22, 0, Npc.target);
            }

                float speed = Vector2.Distance(Npc.Center, targetArea) / 30f;

            float inertia = 10f;


            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer += 1;

            Npc.rotation += MathHelper.PiOver2;
            float turn = AGUtils.TurnTowards(5, player.Center, Npc.rotation, Npc.Center);
            Npc.rotation += turn;
            Npc.rotation -= MathHelper.PiOver2;

            if (AI_Timer == 450)
            {
                SwitchAttack(ActionState.Flamethrower);
                return;

            }
        }
    }
}