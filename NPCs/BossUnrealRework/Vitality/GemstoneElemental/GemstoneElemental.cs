using AwfulGarbageMod;
using AwfulGarbageMod.BossBars;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Consumables; 
using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Systems;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using rail;
using ReLogic.Content;
using ReLogic.Peripherals.RGB;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using VitalityMod.Common.Players;
using VitalityMod.Content.NPCs.Bosses.AnarchulesBeetle;
using VitalityMod.Content.NPCs.Bosses.GemstoneElemental;
using VitalityMod.Content.NPCs.Bosses.MosquitoMonarch;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.NPCs.BossUnrealRework.Vitality.Infestation
{
    // This ModNPC serves as an example of a completely custom AI.
    [ExtendsFromMod("VitalityMod")]

    public class GemstoneElementalUnreal : UnrealNPCOverride
    {
        protected override int OverriddenNpc => ModContent.NPCType<GemstoneElementalBoss>();
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            SpawnAi,
            SkeletronSpin,
            WavyMovement
        }
        // These are reference properties. One, for example, lets us write AI_State as if it's Npc.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "Npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        public float AI_State;
        float Next_State;
        public float AI_Timer;
        public float bossPhase;
        public float atkDir;
        public List<Vector2> oldPos = [];
        Vector2 targetArea;
        Vector2 direction;
        Vector2 vel;
        Vector2 storedVel;
        Vector2 storedPos;
        Vector2 recoil;
        List<Vector2> playerPos = new List<Vector2> { };
        float phaseWhenAttackStarted;
        public bool shield;
        bool checkedForOtherBoss = false;
        bool phase3AttackChange = false;
        int atkUseCounter;
        public override void SetStaticDefaults()
        {
            if (DifficultyModes.Difficulty > 0)
            {
                NPCID.Sets.TrailCacheLength[NPCID.EyeofCthulhu] = 144;
                NPCID.Sets.TrailingMode[NPCID.EyeofCthulhu] = 1;
            }
        }


        private int currentFrame;

        private int yOffset;

        private int auraTimer;


        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (DifficultyModes.Difficulty > 0)
            {
                npc.lifeMax *= 5;
                npc.lifeMax /= 3;
                npc.life = npc.lifeMax;
                //hand1 = NPC.NewNPC(NPC.GetBossSpawnSource(Npc.target), (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 1);
                // hand2 = NPC.NewNPC(source, (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 2);
                //hand3 = NPC.NewNPC(source, (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 3);
                //hand4 = NPC.NewNPC(source, (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 4);
                if (DifficultyModes.Difficulty == 2)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                    npc.life = npc.lifeMax;
                }
                npc.damage *= 4;
                npc.damage /= 3;
            }
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (ShouldOverride())
            {
                Texture2D texture2D = (Texture2D)ModContent.Request<Texture2D>("VitalityMod/Assets/Textures/Effects/PinkBlueShockwave");

                if (shield)
                {
                    Main.EntitySpriteDraw(texture2D, Npc.Center - Main.screenPosition, null, new Color(255 - Npc.alpha, 255 - Npc.alpha, 255 - Npc.alpha, 0), 0f, new Vector2(texture2D.Width, texture2D.Height) / 2f, 0.1f, SpriteEffects.None);
                }
            }
        }
        private void PlayShootSound()
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    {
                        SoundStyle style = new SoundStyle("VitalityMod/Assets/SFX/NPC/MosquitoMonarchShot1");
                        SoundEngine.PlaySound(in style, base.Npc.Center);
                        break;
                    }
                case 1:
                    {
                        SoundStyle style = new SoundStyle("VitalityMod/Assets/SFX/NPC/MosquitoMonarchShot2");
                        SoundEngine.PlaySound(in style, base.Npc.Center);
                        break;
                    }
                case 2:
                    {
                        SoundStyle style = new SoundStyle("VitalityMod/Assets/SFX/NPC/MosquitoMonarchShot3");
                        SoundEngine.PlaySound(in style, base.Npc.Center);
                        break;
                    }
                case 3:
                    {
                        SoundStyle style = new SoundStyle("VitalityMod/Assets/SFX/NPC/MosquitoMonarchShot4");
                        SoundEngine.PlaySound(in style, base.Npc.Center);
                        break;
                    }
            }
        }
        private void PlayChargeSound()
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    {
                        SoundStyle style = new SoundStyle("VitalityMod/Assets/SFX/NPC/MosquitoMonarchCharge1");
                        SoundEngine.PlaySound(in style, base.Npc.Center);
                        break;
                    }
                case 1:
                    {
                        SoundStyle style = new SoundStyle("VitalityMod/Assets/SFX/NPC/MosquitoMonarchCharge2");
                        SoundEngine.PlaySound(in style, base.Npc.Center);
                        break;
                    }
                case 2:
                    {
                        SoundStyle style = new SoundStyle("VitalityMod/Assets/SFX/NPC/MosquitoMonarchCharge3");
                        SoundEngine.PlaySound(in style, base.Npc.Center);
                        break;
                    }
                case 3:
                    {
                        SoundStyle style = new SoundStyle("VitalityMod/Assets/SFX/NPC/MosquitoMonarchCharge4");
                        SoundEngine.PlaySound(in style, base.Npc.Center);
                        break;
                    }
            }
        }


        private void ShootUltimate()
        {
            SoundEngine.PlaySound(in SoundID.Item73, base.Npc.Center);
            int type = ModContent.ProjectileType<AmethystGemstoneHoming>();
            int type2 = ModContent.ProjectileType<TopazGemstoneHoming>();
            int type3 = ModContent.ProjectileType<SapphireGemstoneHoming>();
            int type4 = ModContent.ProjectileType<EmeraldGemstoneHoming>();
            int type5 = ModContent.ProjectileType<RubyGemstoneHoming>();
            int type6 = ModContent.ProjectileType<DiamondGemstoneHoming>();
            Vector2 vector = new Vector2(66f, 50f);
            Vector2 vector2 = new Vector2(0f, 50f);
            Vector2 vector3 = new Vector2(-66f, 50f);
            Vector2 vector4 = new Vector2(-66f, -50f);
            Vector2 vector5 = new Vector2(0f, -50f);
            Vector2 vector6 = new Vector2(66f, -50f);
            float num = vector.Length();
            if (num > 0f)
            {
                vector *= 6f / num;
            }
            else
            {
                vector = new Vector2(0f, 5f);
            }

            float num2 = vector2.Length();
            if (num2 > 0f)
            {
                vector2 *= 6f / num2;
            }
            else
            {
                vector2 = new Vector2(0f, 5f);
            }

            float num3 = vector3.Length();
            if (num3 > 0f)
            {
                vector3 *= 6f / num3;
            }
            else
            {
                vector3 = new Vector2(0f, 5f);
            }

            float num4 = vector4.Length();
            if (num4 > 0f)
            {
                vector4 *= 6f / num4;
            }
            else
            {
                vector4 = new Vector2(0f, 5f);
            }

            float num5 = vector5.Length();
            if (num5 > 0f)
            {
                vector5 *= 6f / num5;
            }
            else
            {
                vector5 = new Vector2(0f, 5f);
            }

            float num6 = vector6.Length();
            if (num6 > 0f)
            {
                vector6 *= 6f / num6;
            }
            else
            {
                vector6 = new Vector2(0f, 5f);
            }

            if (Main.netMode != 1)
            {
                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, vector * 2f, type, 20, 0f);
                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, vector2 * 2f, type2, 20, 0f);
                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, vector3 * 2f, type3, 20, 0f);
                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, vector4 * 2f, type4, 20, 0f);
                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, vector5 * 2f, type5, 20, 0f);
                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, vector6 * 2f, type6, 20, 0f);
                base.Npc.netUpdate = true;
            }
        }

        public override void OnKill(NPC npc)
        {

            for (int i = 0; i < Main.maxNPCs; i++)
            {

                NPC npc2 = Main.npc[i];
                if (npc2.active && npc2.type == ModContent.NPCType<AnarchulesBeetleBoss>())
                {
                    if (npc2.GetGlobalNPC<AnarchulesBeetleUnreal>().shield)
                    {
                        npc2.GetGlobalNPC<AnarchulesBeetleUnreal>().bossPhase = 3;
                    }
                }
            }
            base.OnKill(npc);
        }

        public override bool UnrealAI(NPC npc)
        {
            npc.ai[1] = 1;

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
            else
            {
                if (Npc.life < Npc.lifeMax / 2 && bossPhase < 2)
                {
                    bossPhase = 2;
                }
                if (bossPhase > 1)
                {
                    auraTimer++;
                }
                if (Npc.life < Npc.lifeMax * 6 / 7 && !checkedForOtherBoss)
                {
                    checkedForOtherBoss = true;
                    bool shouldShield = true;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {

                        NPC npc2 = Main.npc[i];
                        if (npc2.active && npc2.type == ModContent.NPCType<AnarchulesBeetleBoss>())
                        {
                            if (npc2.GetGlobalNPC<AnarchulesBeetleUnreal>().shield)
                            {
                                shouldShield = false;
                            }
                        }
                    }
                    if (shouldShield)
                    {
                        shield = true;
                        Npc.dontTakeDamage = true;
                    }
                }
                switch (AI_State)
                {
                    case (float)ActionState.SpawnAi:
                        SpawnAi();
                        break;
                    case (float)ActionState.SkeletronSpin:
                        SkeletronSpin();
                        break;
                    case (float)ActionState.WavyMovement:
                        WavyMovement();
                        break;
                }
            }
            return false;
        }

        private void SwitchAttack(ActionState attack)
        {
            Player player = Main.player[Npc.target];
            AI_State = (float)attack;
            phaseWhenAttackStarted = bossPhase;
            switch (AI_State)
            {
                case (float)ActionState.SpawnAi:
                    AI_Timer = 0;
                    break;
                case (float)ActionState.SkeletronSpin:
                    AI_Timer = 0;
                    break;
                case (float)ActionState.WavyMovement:
                    AI_Timer = 0;
                    storedPos = Npc.Center;
                    atkUseCounter = 0;
                    if (Npc.Center.X < player.Center.X)
                    {
                        storedVel = new Vector2(4, 450);
                    }
                    else
                    {
                        storedVel = new Vector2(-4, -450);
                    }
                    break;
            }
        }
        private void SpawnAi()
        {

            Player player = Main.player[Npc.target];
            targetArea = player.Center + new Vector2(0, -400 + player.velocity.Y * 40);

            float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

            float inertia = 6f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;
            Npc.rotation += 0.05f;
            AI_Timer += 1;
            if (Npc.Center.X < player.Center.X)
            {
                Npc.spriteDirection = 1;
            }
            else
            {
                Npc.spriteDirection = -1;
            }
            if (AI_Timer == 120)
            {
                SwitchAttack(ActionState.SkeletronSpin);
            }
        }
        private void SkeletronSpin()
        {
            Player player = Main.player[Npc.target];

            targetArea = player.Center;
            
            float speed = Vector2.Distance(Npc.Center, targetArea) / 30f;
            speed += 1;

            float inertia = 4f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;
            Npc.position.Y -= Npc.velocity.Y / 3;
            Npc.rotation += 0.13f;

            AI_Timer += 1;
            if (AI_Timer % 180 == 90)
            {
                storedPos = player.velocity.SafeNormalize(Vector2.One) * 300;
            }
            if ((AI_Timer % 180 >= 90 && AI_Timer % 30 == 0) || AI_Timer % 180 == 0)
            {
                for (int i = 0; i < 24; i++)
                {
                    int dustID = 1;
                    switch (Main.rand.Next(6))
                    {
                        case 0:
                            dustID = DustID.GemAmethyst;
                            break;
                        case 1:
                            dustID = DustID.GemDiamond;
                            break;
                        case 2:
                            dustID = DustID.GemEmerald;
                            break;
                        case 3:
                            dustID = DustID.GemRuby;
                            break;
                        case 4:
                            dustID = DustID.GemSapphire;
                            break;
                        case 5:
                            dustID = DustID.GemTopaz;
                            break;
                    }
                    int num = Dust.NewDust(Npc.Center, 0, 0, dustID, 0, 0, 0, default(Color), 1.4f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position = player.Center + storedPos;
                    Main.dust[num].velocity = new Vector2(10, 0).RotatedBy(MathHelper.TwoPi * i/ 24);
                    num = Dust.NewDust(Npc.Center, 0, 0, DustID.Stone, 0, 0, 0, default(Color), 1.4f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position = player.Center + storedPos;
                    Main.dust[num].velocity = new Vector2(10, 0).RotatedBy(MathHelper.TwoPi * i/ 24);
                }
            }
            if (AI_Timer % 180 == 0)
            {
                ShootUltimate();
                Npc.Center = player.Center + storedPos;
                ShootUltimate();

            }

            if (AI_Timer == 180 * 3 + 75)
            {
                SwitchAttack(ActionState.WavyMovement);
            }
        }
        private void WavyMovement()
        {
            Player player = Main.player[Npc.target];

            targetArea = new Vector2(storedPos.X, player.Center.Y + MathF.Sin(AI_Timer / 12) * 420);
            
            float speed = Vector2.Distance(Npc.Center, targetArea) / 15f;
            speed += 1;

            float inertia = 20f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;
            Npc.rotation += 0.1f;
            storedPos.X += storedVel.X;
            if (storedVel.Y > 0)
            {
                storedVel.X += 0.05f;
                if (storedPos.X > player.Center.X + storedVel.Y)
                {
                    storedVel.Y *= -1;
                    storedVel.X = -3;
                    atkUseCounter++;
                    ShootUltimate();

                }
            }
            else
            {
                storedVel.X -= 0.05f;
                if (storedPos.X < player.Center.X + storedVel.Y)
                {
                    storedVel.Y *= -1;
                    storedVel.X = 3;
                    atkUseCounter++;
                    ShootUltimate();

                }
            }
            AI_Timer += 1;
            if (atkUseCounter == 3)
            {
                SwitchAttack(ActionState.SkeletronSpin);

            }
        }
    }
}