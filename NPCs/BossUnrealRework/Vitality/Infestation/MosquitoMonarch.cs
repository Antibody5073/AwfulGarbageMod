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
using VitalityMod.Content.NPCs.Bosses.MosquitoMonarch;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace AwfulGarbageMod.NPCs.BossUnrealRework.Vitality.Infestation
{
    // This ModNPC serves as an example of a completely custom AI.
    [ExtendsFromMod("VitalityMod")]

    public class MosquitoMonarchUnreal : UnrealNPCOverride
    {
        protected override int OverriddenNpc => ModContent.NPCType<MosquitoMonarchBoss>();
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            SpawnAi,
            MosquitoMonarchHoverAbovePlayerAnarchulesBeetleShootSpread,
            BeetleSpreadRecoilMosquitoMonarchDash,
            BeetleCursedWallsMosquitoMonarchDash,
            StyletConfinedSpaceClawAttack,
            IchorHoverAroundAbovePlayerBeetleSummonEggAndDash,

            PhaseChange,
            StyletDash,
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


        private static Asset<Texture2D> headTexture;

        private static Asset<Texture2D> limbsTexture;

        private static Asset<Texture2D> lowerBodyTexture;

        private static Asset<Texture2D> glowMainTexture;

        private static Asset<Texture2D> glowHeadTexture;

        private static Asset<Texture2D> glowLimbsTexture;

        private static Asset<Texture2D> glowLowerBodyTexture;

        public override void Load()
        {
            headTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/MosquitoMonarch/MosquitoHead");
            limbsTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/MosquitoMonarch/MosquitoLimbsLower");
            lowerBodyTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/MosquitoMonarch/MosquitoLowerBody");
            glowMainTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/MosquitoMonarch/MosquitoMonarchBoss_Glow");
            glowHeadTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/MosquitoMonarch/MosquitoHead_Glow");
            glowLimbsTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/MosquitoMonarch/MosquitoLimbsLower_Glow");
            glowLowerBodyTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/MosquitoMonarch/MosquitoLowerBody_Glow");
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (DifficultyModes.Difficulty > 0)
            {
                npc.lifeMax *= 3;
                npc.lifeMax /= 5;
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
            }
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            if ((npc.frameCounter += 1.0) >= 4.0)
            {
                npc.frame.Y = currentFrame * frameHeight;
                if (currentFrame < 2)
                {
                    yOffset = 0;
                }
                else
                {
                    yOffset = 2;
                }

                npc.frameCounter = 0.0;
                if (++currentFrame >= 4)
                {
                    currentFrame = 0;
                }
            }
        }
        public override bool UnrealDraw(NPC Npc, SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            SpriteEffects effects = ((base.Npc.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            new Vector2(99f, 94f);
            float globalTimeWrappedHourly = Main.GlobalTimeWrappedHourly;
            float num = (float)auraTimer / 240f + globalTimeWrappedHourly * 0.04f;
            globalTimeWrappedHourly %= 4f;
            globalTimeWrappedHourly /= 2f;
            if (globalTimeWrappedHourly >= 1f)
            {
                globalTimeWrappedHourly = 2f - globalTimeWrappedHourly;
            }

            globalTimeWrappedHourly = globalTimeWrappedHourly * 0.5f + 0.5f;
            if (bossPhase > 1)
            {
                for (float num2 = 0f; num2 < 1f; num2 += 0.25f)
                {
                    float num3 = (num2 + num) * (MathF.PI * 2f);
                    Main.EntitySpriteDraw(glowLimbsTexture.Value, base.Npc.Center - Main.screenPosition + new Vector2(17 * -base.Npc.spriteDirection, yOffset - 4) + new Vector2(0f, 6f).RotatedBy(num3) * globalTimeWrappedHourly, null, new Color(150, 150, 150, 0), 0f, new Vector2(82f, 94f), 1f, effects);
                    if (Npc.ai[0] == 6f)
                    {
                        Main.EntitySpriteDraw(glowLowerBodyTexture.Value, base.Npc.Center - Main.screenPosition + new Vector2(5 * -base.Npc.spriteDirection, yOffset - 4) + new Vector2(0f, 6f).RotatedBy(num3) * globalTimeWrappedHourly, null, new Color(150, 150, 150, 0), MathF.PI * 9f / 20f * (float)base.Npc.spriteDirection, new Vector2(30f, 94f), 1f, effects);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(glowLowerBodyTexture.Value, base.Npc.Center - Main.screenPosition + new Vector2(5 * -base.Npc.spriteDirection, yOffset - 4) + new Vector2(0f, 6f).RotatedBy(num3) * globalTimeWrappedHourly, null, new Color(150, 150, 150, 0), base.Npc.velocity.X * 0.03f, new Vector2(30f, 94f), 1f, effects);
                    }

                    Main.EntitySpriteDraw(glowMainTexture.Value, base.Npc.Center - Main.screenPosition + new Vector2(0f, -2f) + new Vector2(0f, 6f).RotatedBy(num3) * globalTimeWrappedHourly, base.Npc.frame, new Color(150, 150, 150, 0), base.Npc.rotation, new Vector2(glowMainTexture.Width(), glowMainTexture.Height() / 4) / 2f, base.Npc.scale, effects);
                    if (Npc.ai[0] == 6f)
                    {
                        Main.EntitySpriteDraw(glowHeadTexture.Value, base.Npc.Center - Main.screenPosition + new Vector2(-11 * -base.Npc.spriteDirection, yOffset - 18) + new Vector2(0f, 6f).RotatedBy(num3) * globalTimeWrappedHourly, null, new Color(150, 150, 150, 0), MathF.PI / 5f * (float)(-base.Npc.spriteDirection), new Vector2(54f, 56f), 1f, effects);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(glowHeadTexture.Value, base.Npc.Center - Main.screenPosition + new Vector2(-11 * -base.Npc.spriteDirection, yOffset - 18) + new Vector2(0f, 6f).RotatedBy(num3) * globalTimeWrappedHourly, null, new Color(150, 150, 150, 0), 0f, new Vector2(54f, 56f), 1f, effects);
                    }
                }
            }

            Main.EntitySpriteDraw(limbsTexture.Value, base.Npc.Center - Main.screenPosition + new Vector2(17 * -base.Npc.spriteDirection, yOffset - 3), null, base.Npc.GetNPCColorTintedByBuffs(lightColor), 0f, new Vector2(82f, 94f), 1f, effects, 0f);
            if (Npc.ai[0] == 6f)
            {
                Main.EntitySpriteDraw(lowerBodyTexture.Value, base.Npc.Center - Main.screenPosition + new Vector2(5 * -base.Npc.spriteDirection, yOffset - 3), null, base.Npc.GetNPCColorTintedByBuffs(lightColor), MathF.PI * 9f / 20f * (float)base.Npc.spriteDirection, new Vector2(30f, 94f), 1f, effects);
            }
            else
            {
                Main.EntitySpriteDraw(lowerBodyTexture.Value, base.Npc.Center - Main.screenPosition + new Vector2(5 * -base.Npc.spriteDirection, yOffset - 3), null, base.Npc.GetNPCColorTintedByBuffs(lightColor), base.Npc.velocity.X * 0.03f, new Vector2(30f, 94f), 1f, effects);
            }

            return true;
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (ShouldOverride())
            {
                Texture2D texture2D = (Texture2D)ModContent.Request<Texture2D>("VitalityMod/Content/Projectiles/Accessories/LordRingShockwaveEffect");

                if (shield)
                {
                    Main.EntitySpriteDraw(texture2D, Npc.Center - Main.screenPosition, null, new Color(255 - Npc.alpha, 255 - Npc.alpha, 255 - Npc.alpha, 0), 0f, new Vector2(texture2D.Width, texture2D.Height) / 2f, 0.15f, SpriteEffects.None);
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
                if (Npc.life < Npc.lifeMax / 5 && !checkedForOtherBoss)
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
                if (bossPhase == 3 && !phase3AttackChange)
                {
                    phase3AttackChange = true;
                    SwitchAttack(ActionState.PhaseChange);
                }
                switch (AI_State)
                {
                    case (float)ActionState.SpawnAi:
                        SpawnAi();
                        break;
                    case (float)ActionState.MosquitoMonarchHoverAbovePlayerAnarchulesBeetleShootSpread:
                        MosquitoMonarchHoverAbovePlayerAnarchulesBeetleShootSpread();
                        break;
                    case (float)ActionState.BeetleSpreadRecoilMosquitoMonarchDash:
                        BeetleSpreadRecoilMosquitoMonarchDash();
                        break;
                    case (float)ActionState.BeetleCursedWallsMosquitoMonarchDash:
                        BeetleCursedWallsMosquitoMonarchDash();
                        break;
                    case (float)ActionState.StyletConfinedSpaceClawAttack:
                        StyletConfinedSpaceClawAttack();
                        break;
                    case (float)ActionState.IchorHoverAroundAbovePlayerBeetleSummonEggAndDash:
                        IchorHoverAroundAbovePlayerBeetleSummonEggAndDash();
                        break;
                    case (float)ActionState.PhaseChange:
                        PhaseChange();
                        break;
                    case (float)ActionState.StyletDash:
                        StyletDash();
                        break;
                }
            }
            return false;
        }

        private void SwitchAttack(ActionState attack)
        {
            AI_State = (float)attack;
            phaseWhenAttackStarted = bossPhase;
            switch (AI_State)
            {
                case (float)ActionState.SpawnAi:
                    AI_Timer = 0;
                    break;
                case (float)ActionState.MosquitoMonarchHoverAbovePlayerAnarchulesBeetleShootSpread:
                    AI_Timer = 0;
                    break;
                case (float)ActionState.BeetleSpreadRecoilMosquitoMonarchDash:
                    AI_Timer = -45;
                    break;
                case (float)ActionState.BeetleCursedWallsMosquitoMonarchDash:
                    AI_Timer = 0;
                    break;
                case (float)ActionState.StyletConfinedSpaceClawAttack:
                    AI_Timer = 0;
                    break;
                case (float)ActionState.IchorHoverAroundAbovePlayerBeetleSummonEggAndDash:
                    AI_Timer = -90;
                    storedPos = new Vector2(0, -400) + Main.rand.NextVector2Circular(600, 100);
                    break;
                case (float)ActionState.PhaseChange:
                    AI_Timer = 0;
                    break;
                case (float)ActionState.StyletDash:
                    AI_Timer = 0;
                    break;
            }
        }
        private void SpawnAi()
        {

            Player player = Main.player[Npc.target];
            targetArea = player.Center + new Vector2(player.velocity.X * 120, -400 + player.velocity.Y * 40);

            float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

            float inertia = 6f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer += 1;
            if (Npc.Center.X < player.Center.X)
            {
                Npc.spriteDirection = 1;
            }
            else
            {
                Npc.spriteDirection = -1;
            }
            if (AI_Timer == 90)
            {
                SwitchAttack(ActionState.MosquitoMonarchHoverAbovePlayerAnarchulesBeetleShootSpread);
            }
        }
        private void MosquitoMonarchHoverAbovePlayerAnarchulesBeetleShootSpread()
        {
            Player player = Main.player[Npc.target];

            if (AI_Timer < 240 * 3 + 60)
            {
                targetArea = player.Center + new Vector2(player.velocity.X * 100, -400 + player.velocity.Y * 40);
            }
            else
            {
                if (AI_Timer == 240 * 3 + 60)
                targetArea = player.Center + new Vector2(0, -400);

            }
            float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

            float inertia = 6f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;


            if (Npc.Center.X < player.Center.X)
            {
                Npc.spriteDirection = 1;
            }
            else
            {
                Npc.spriteDirection = -1;
            }

            if (AI_Timer % 240 < 150)
            {
                if (AI_Timer <= 240 * 3 + 60)
                {
                    if (AI_Timer % 30 == 0)
                    {
                        PlayShootSound();

                        if (Main.netMode != 1)
                        {
                            if (phaseWhenAttackStarted == 1)
                            {
                                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8), ModContent.ProjectileType<Stylet>(), 25, 0f);
                                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(60)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(-60)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                                //Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(12)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                                //Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(-12)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                            }
                            else
                            {

                                for (int i = 0; i < 5; i++)
                                {
                                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 4.5f) + Main.rand.NextVector2Circular(1.75f, 1), ModContent.ProjectileType<IchorStream>(), 25, 0f);
                                }
                                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8), ModContent.ProjectileType<Stylet>(), 25, 0f);
                                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(60)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(-60)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                            }
                            Npc.netUpdate = true;

                        }
                    }
                }
                else
                {
                    if (AI_Timer % 15 == 0)
                    {

                        SoundEngine.PlaySound(in SoundID.NPCHit1, base.Npc.Center);
                        if (Main.netMode != 1)
                        {
                            NPC npc;
                            npc = NPC.NewNPCDirect(base.Npc.GetSource_FromAI(), (int)base.Npc.Center.X, (int)base.Npc.Center.Y, ModContent.NPCType<MosquitoMinion>());
                            npc.velocity = new Vector2(2, 0);
                            npc = NPC.NewNPCDirect(base.Npc.GetSource_FromAI(), (int)base.Npc.Center.X, (int)base.Npc.Center.Y, ModContent.NPCType<MosquitoMinion>());
                            npc.velocity = new Vector2(-2, 0);
                            npc = NPC.NewNPCDirect(base.Npc.GetSource_FromAI(), (int)base.Npc.Center.X, (int)base.Npc.Center.Y, ModContent.NPCType<MosquitoMinion>());
                            npc.velocity = new Vector2(0, -0.5f);
                        }
                    }
                }

            }
            if (AI_Timer % 240 == 150)
            {
                phaseWhenAttackStarted = bossPhase;
            }
            if (AI_Timer == 240 * 3 + 125)
            {
                SwitchAttack(ActionState.BeetleSpreadRecoilMosquitoMonarchDash);
            }
            AI_Timer += 1;
        }
        private void BeetleSpreadRecoilMosquitoMonarchDash()
        {
            Player player = Main.player[Npc.target];
            if (AI_Timer < 0)
            {
                targetArea = player.Center + new Vector2(0, -400);

                float speed = Vector2.Distance(Npc.Center, targetArea) / 36f;

                float inertia = 4f;

                direction = targetArea - Npc.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

                if (Npc.Center.X < player.Center.X)
                {
                    Npc.spriteDirection = 1;
                }
                else
                {
                    Npc.spriteDirection = -1;
                }
            }
            else
            {
                if (phaseWhenAttackStarted == 1)
                {
                    if (AI_Timer % 105 < 45)
                    {
                        if (AI_Timer % 105 == 0)
                        {
                            if (Npc.Center.X < player.Center.X)
                            {
                                targetArea = new Vector2(-600, 0);
                            }
                            else
                            {
                                targetArea = new Vector2(600, 0);
                            }
                        }
                        if (AI_Timer == 0)
                        {
                            Npc.velocity = new Vector2(0, -8);
                        }
                        float speed = Vector2.Distance(Npc.Center, player.Center + targetArea) / (1f - ((AI_Timer % 105) - 45) * 0.25f);
                        float inertia = (1f - ((AI_Timer % 105) - 45) * 0.25f);
                        Vector2 direction = (player.Center + targetArea) - Npc.Center;
                        direction = direction.SafeNormalize(Vector2.Zero);
                        direction *= speed;

                        Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

                        if (Npc.Center.X < player.Center.X)
                        {
                            Npc.spriteDirection = 1;
                        }
                        else
                        {
                            Npc.spriteDirection = -1;
                        }
                    }
                    else
                    {
                        if (AI_Timer % 105 == 45)
                        {
                            Npc.Center = player.Center + targetArea;
                            if (targetArea.X < 0)
                            {
                                Npc.velocity = new Vector2(25, 0);
                            }
                            else
                            {
                                Npc.velocity = new Vector2(-25, 0);
                            }
                            PlayChargeSound();
                        }
                        Npc.ai[1] = 6;
                    }
                }
                else
                {

                    if (AI_Timer % 70 < 30)
                    {
                        if (AI_Timer % 70 == 0)
                        {
                            if (Npc.Center.X < player.Center.X)
                            {
                                targetArea = new Vector2(-600, 0);
                            }
                            else
                            {
                                targetArea = new Vector2(600, 0);
                            }
                        }
                        if (AI_Timer == 0)
                        {
                            Npc.velocity = new Vector2(0, -8);
                        }
                        float speed = Vector2.Distance(Npc.Center, player.Center + targetArea) / (1f - ((AI_Timer % 70) - 30) * 0.35f);
                        float inertia = (1f - ((AI_Timer % 70) - 30) * 0.35f);
                        Vector2 direction = (player.Center + targetArea) - Npc.Center;
                        direction = direction.SafeNormalize(Vector2.Zero);
                        direction *= speed;

                        Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

                        if (Npc.Center.X < player.Center.X)
                        {
                            Npc.spriteDirection = 1;
                        }
                        else
                        {
                            Npc.spriteDirection = -1;
                        }
                    }
                    else
                    {
                        if (AI_Timer % 70 == 30)
                        {
                            Npc.Center = player.Center + targetArea;
                            if (targetArea.X < 0)
                            {
                                Npc.velocity = new Vector2(32, 0);
                            }
                            else
                            {
                                Npc.velocity = new Vector2(-32, 0);
                            }
                            PlayChargeSound();
                        }
                        Npc.ai[1] = 6;
                    }
                }
            }
            AI_Timer += 1;

            if (AI_Timer == 105 * 2)
            {
                SwitchAttack(ActionState.BeetleCursedWallsMosquitoMonarchDash);
            }
        }
        private void BeetleCursedWallsMosquitoMonarchDash()
        {
            Player player = Main.player[Npc.target];
            if (phaseWhenAttackStarted == 1)
            {
                if (AI_Timer % 105 < 45)
                {
                    if (AI_Timer % 105 == 0)
                    {
                        if (Npc.Center.X < player.Center.X)
                        {
                            targetArea = new Vector2(-600, 0);
                        }
                        else
                        {
                            targetArea = new Vector2(600, 0);
                        }
                    }
                    if (AI_Timer == 0)
                    {
                        Npc.velocity = new Vector2(0, -8);
                    }
                    float speed = Vector2.Distance(Npc.Center, player.Center + targetArea) / (1f - ((AI_Timer % 105) - 45) * 0.25f);
                    float inertia = (1f - ((AI_Timer % 105) - 45) * 0.25f);
                    Vector2 direction = (player.Center + targetArea) - Npc.Center;
                    direction = direction.SafeNormalize(Vector2.Zero);
                    direction *= speed;

                    Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

                    if (Npc.Center.X < player.Center.X)
                    {
                        Npc.spriteDirection = 1;
                    }
                    else
                    {
                        Npc.spriteDirection = -1;
                    }
                }
                else
                {
                    if (AI_Timer % 105 == 45)
                    {
                        Npc.Center = player.Center + targetArea;
                        if (targetArea.X < 0)
                        {
                            Npc.velocity = new Vector2(25, 0);
                        }
                        else
                        {
                            Npc.velocity = new Vector2(-25, 0);
                        }
                        PlayChargeSound();
                    }
                    Npc.ai[1] = 6;
                }
            }
            else
            {
                if (AI_Timer % 70 < 30)
                {
                    if (AI_Timer % 70 == 0)
                    {
                        if (Npc.Center.X < player.Center.X)
                        {
                            targetArea = new Vector2(-600, 0);
                        }
                        else
                        {
                            targetArea = new Vector2(600, 0);
                        }
                    }
                    if (AI_Timer == 0)
                    {
                        Npc.velocity = new Vector2(0, -8);
                    }
                    float speed = Vector2.Distance(Npc.Center, player.Center + targetArea) / (1f - ((AI_Timer % 70) - 30) * 0.35f);
                    float inertia = (1f - ((AI_Timer % 70) - 30) * 0.35f);
                    Vector2 direction = (player.Center + targetArea) - Npc.Center;
                    direction = direction.SafeNormalize(Vector2.Zero);
                    direction *= speed;

                    Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

                    if (Npc.Center.X < player.Center.X)
                    {
                        Npc.spriteDirection = 1;
                    }
                    else
                    {
                        Npc.spriteDirection = -1;
                    }
                }
                else
                {
                    if (AI_Timer % 70 == 30)
                    {
                        Npc.Center = player.Center + targetArea;
                        if (targetArea.X < 0)
                        {
                            Npc.velocity = new Vector2(32, 0);
                        }
                        else
                        {
                            Npc.velocity = new Vector2(-32, 0);
                        }
                        PlayChargeSound();
                    }
                    Npc.ai[1] = 6;
                }
            }

            if (AI_Timer == 660)
            {
                SwitchAttack(ActionState.StyletConfinedSpaceClawAttack);
            }
            AI_Timer += 1;
        }
        public void StyletConfinedSpaceClawAttack()
        {

            if (AI_Timer < 150)
            {
                Player player = Main.player[Npc.target];
                targetArea = player.Center + new Vector2(player.velocity.X * 20, -360);

                float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

                float inertia = 6f;

                direction = targetArea - Npc.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

                if (Npc.Center.X < player.Center.X)
                {
                    Npc.spriteDirection = 1;
                }
                else
                {
                    Npc.spriteDirection = -1;
                }
                if (AI_Timer > 60 && AI_Timer % 15 == 0)
                {
                    SoundEngine.PlaySound(in SoundID.Item17, base.Npc.Center);

                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(45)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(-45)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                }
                if (AI_Timer % 45 == 0 && phaseWhenAttackStarted == 2)
                {

                    PlayShootSound();
                    int type = ModContent.ProjectileType<Stylet2>();
                    Vector2 vector = new Vector2(0f, 200f);
                    float num = 8f;
                    float num2 = MathHelper.ToRadians(180f);
                    base.Npc.Center += Vector2.Normalize(new Vector2(vector.X, vector.Y)) * 0f;
                    for (int i = 0; (float)i < num; i++)
                    {
                        Vector2 vector2 = new Vector2(vector.X, vector.Y).RotatedBy(MathHelper.Lerp(0f - num2, num2, (float)i / num)) * 0.2f;
                        if (Main.netMode != 1)
                        {
                            Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, new Vector2(vector2.X * 0.025f, vector2.Y * 0.025f), type, 25, 0f, -1, 0f, 0f, base.Npc.target);
                            base.Npc.netUpdate = true;
                        }
                    }
                }
            }
            else
            {
                Npc.velocity = Vector2.Zero;
                if (AI_Timer % 10 == 0)
                {
                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(45)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(-45)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                    if (AI_Timer < 510)
                    {
                        Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center + new Vector2(-1200, 1000), new Vector2(8, 0), ModContent.ProjectileType<Stylet>(), 25, 0f);
                        Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center + new Vector2(1200, 1000), new Vector2(-8, 0), ModContent.ProjectileType<Stylet>(), 25, 0f);
                    }
                }
                if (AI_Timer % 45 == 0)
                {
                    for (int i = -3; i < 3; i++)
                    {

                        Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(8, 0).RotatedBy(MathHelper.ToRadians(i * 15)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                        Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(-8, 0).RotatedBy(MathHelper.ToRadians(i * 15)), ModContent.ProjectileType<Stylet>(), 25, 0f);
                    }
                }
            }
            AI_Timer += 1;

            if (AI_Timer == 540)
            {
                SwitchAttack(ActionState.IchorHoverAroundAbovePlayerBeetleSummonEggAndDash);
            }
        }
        public void IchorHoverAroundAbovePlayerBeetleSummonEggAndDash()
        {
            Player player = Main.player[Npc.target];
            targetArea = player.Center + storedPos;

            float speed = Vector2.Distance(Npc.Center, targetArea) / 30f;
            speed += 5f;
            float inertia = 48f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;
            Npc.position += player.velocity * 0.75f;
            if (Npc.Center.X < player.Center.X)
            {
                Npc.spriteDirection = 1;
            }
            else
            {
                Npc.spriteDirection = -1;
            }
            if (AI_Timer % 90 == 0)
            {
                storedPos = new Vector2(0, -400) + Main.rand.NextVector2Circular(600, 100);
            }

            if (phaseWhenAttackStarted == 1)
            {
                if (AI_Timer % 45 == 0 && AI_Timer >= -45)
                {
                    PlayShootSound();

                    for (int i = -3; i < 4; i++)
                    {
                        Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, (player.Center - Npc.Center).SafeNormalize(Vector2.One).RotatedBy(MathHelper.TwoPi * i / 8) * 12f, ModContent.ProjectileType<IchorStream>(), 25, 0f);
                    }

                }
            }
            else
            {
                if (AI_Timer % 30 == 0 && AI_Timer >= -30)
                {
                    PlayShootSound();

                    for (int i = -4; i < 5; i++)
                    {
                        Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, (player.Center - Npc.Center).SafeNormalize(Vector2.One).RotatedBy(MathHelper.TwoPi * i / 10) * 14f, ModContent.ProjectileType<IchorStream>(), 25, 0f);
                    }

                }
            }
                AI_Timer += 1;
            if (AI_Timer == 240)
            {
                SwitchAttack(ActionState.SpawnAi);
            }
        }

        private void PhaseChange()
        {
            Player player = Main.player[Npc.target];
            targetArea = player.Center + new Vector2(player.velocity.X * 40, -400 + player.velocity.Y * 40);

            float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

            float inertia = 6f;

            direction = targetArea - Npc.Center;
            direction = direction.SafeNormalize(Vector2.Zero);
            direction *= speed;

            Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

            AI_Timer += 1;
            if (Npc.Center.X < player.Center.X)
            {
                Npc.spriteDirection = 1;
            }
            else
            {
                Npc.spriteDirection = -1;
            }
            if (AI_Timer == 75)
            {
                SoundEngine.PlaySound(in SoundID.ForceRoar, Npc.Center);
                shield = false;
                Npc.dontTakeDamage = false;
            }
            if (AI_Timer >= 75 && AI_Timer < 120)
            {

                float shakeDistance = Npc.Distance(Main.LocalPlayer.Center);
                if (shakeDistance > 1200f)
                {
                    shakeDistance = 1200f;
                }

                float shakeIntensity = (1200f - shakeDistance + 1f) / 20f;
                Main.LocalPlayer.GetModPlayer<VitalityPlayer>().screenShakeIntensity = shakeIntensity;
            }
            if (AI_Timer == 150)
            {
                SwitchAttack(ActionState.StyletDash);
            }
        }
        private void StyletDash()
        {
            Player player = Main.player[Npc.target];

            if (AI_Timer % 70 < 30)
            {
                if (AI_Timer % 70 == 0)
                {
                    if (Npc.Center.X < player.Center.X)
                    {
                        targetArea = new Vector2(-640, 0);
                    }
                    else
                    {
                        targetArea = new Vector2(640, 0);
                    }
                }
                if (AI_Timer == 0)
                {
                    Npc.velocity = new Vector2(0, -8);
                }
                float speed = Vector2.Distance(Npc.Center, player.Center + targetArea) / (1f - ((AI_Timer % 70) - 30) * 0.35f);
                float inertia = (1f - ((AI_Timer % 70) - 30) * 0.35f);
                Vector2 direction = (player.Center + targetArea) - Npc.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

                if (Npc.Center.X < player.Center.X)
                {
                    Npc.spriteDirection = 1;
                }
                else
                {
                    Npc.spriteDirection = -1;
                }
            }
            else
            {
                if (AI_Timer % 70 == 30)
                {
                    Npc.Center = player.Center + targetArea;
                    if (targetArea.X < 0)
                    {
                        Npc.velocity = new Vector2(32, 0);
                    }
                    else
                    {
                        Npc.velocity = new Vector2(-32, 0);
                    }
                    PlayChargeSound();
                }
                Npc.ai[1] = 6;
                if (AI_Timer % 4 == 0)
                {
                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, 8), ModContent.ProjectileType<Stylet>(), 25, 0f);
                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, -8), ModContent.ProjectileType<Stylet>(), 25, 0f);

                }
            }
            AI_Timer++;
        }
    }
}