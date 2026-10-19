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

    public class AnarchulesBeetleUnreal : UnrealNPCOverride
    {
        protected override int OverriddenNpc => ModContent.NPCType<AnarchulesBeetleBoss>();
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        public enum ActionState
        {
            SpawnAi, 
            MosquitoMonarchHoverAbovePlayerAnarchulesBeetleShootSpread,
            BeetleSpreadRecoilMosquitoMonarchDash, 
            BeetleCursedWallsMosquitoMonarchDash,
            StyletConfinedSpaceClawAttack,
            IchorHoverAroundAbovePlayerBeetleSummonEggAndDash,

            PhaseChange,
            Dash
        }
        // These are reference properties. One, for example, lets us write AI_State as if it's Npc.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "Npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        public float AI_State;
        float Next_State;
        public float AI_Timer;
        public float bossPhase;
        public float atkDir;
        float turnSpeed;
        float phaseWhenAttackStarted;
        float delay;
        public bool shield;
        bool checkedForOtherBoss = false;
        bool phase3AttackChange = false;
        int atkUseCounter;
        float inheritMomentum;

        public List<Vector2> oldPos = [];
        Vector2 targetArea;
        Vector2 direction;
        Vector2 vel;
        Vector2 storedVel;
        Vector2 storedPos;
        Vector2 recoil;
        List<Vector2> playerPos = new List<Vector2> { };



        private Vector2[] LegPos = new Vector2[12]
        {
        Vector2.Zero,
        Vector2.Zero,
        Vector2.Zero,
        Vector2.Zero,
        Vector2.Zero,
        Vector2.Zero,
        Vector2.Zero,
        Vector2.Zero,
        Vector2.Zero,
        Vector2.Zero,
        Vector2.Zero,
        Vector2.Zero
        };

        private int auraTimer;

        private float auraDistance = 1f;
        private static Asset<Texture2D> glowTexture;

        private static Asset<Texture2D> headTexture;

        private static Asset<Texture2D> frontLegUpperTexture;

        private static Asset<Texture2D> frontLeg1Texture;

        private static Asset<Texture2D> frontLeg2Texture;

        private static Asset<Texture2D> frontLeg3Texture;

        private static Asset<Texture2D> backLegUpperTexture;

        private static Asset<Texture2D> backLeg1Texture;

        private static Asset<Texture2D> backLeg2Texture;

        private static Asset<Texture2D> backLeg3Texture;

        public override void Load()
        {
            glowTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/AnarchulesBeetleBoss_Glow");
            headTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/Head_Front");
            frontLegUpperTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/Leg_Front_Upper");
            frontLeg1Texture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/Leg_Front_1");
            frontLeg2Texture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/Leg_Front_2");
            frontLeg3Texture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/Leg_Front_3");
            backLegUpperTexture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/Leg_Back_Upper");
            backLeg1Texture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/Leg_Back_1");
            backLeg2Texture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/Leg_Back_2");
            backLeg3Texture = ModContent.Request<Texture2D>("VitalityMod/Content/NPCs/Bosses/AnarchulesBeetle/Leg_Back_3");
            base.Load();
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (DifficultyModes.Difficulty > 0)
            {
                //hand1 = NPC.NewNPC(NPC.GetBossSpawnSource(Npc.target), (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 1);
                // hand2 = NPC.NewNPC(source, (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 2);
                //hand3 = NPC.NewNPC(source, (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 3);
                //hand4 = NPC.NewNPC(source, (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<Echelon>(), Npc.whoAmI, Npc.whoAmI, 4);
                npc.lifeMax *= 3;
                npc.lifeMax /= 5;
                npc.life = npc.lifeMax;
                if (DifficultyModes.Difficulty == 2)
                {
                    npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                    npc.life = npc.lifeMax;
                }
            }
        }
        public override bool UnrealDraw(NPC Npc, SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D value = backLegUpperTexture.Value;
            Texture2D value2 = backLeg1Texture.Value;
            Texture2D value3 = backLeg2Texture.Value;
            Texture2D value4 = backLeg3Texture.Value;
            SpriteEffects effects = ((base.Npc.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            Vector2 vector = new Vector2(100f, 91f);
            float globalTimeWrappedHourly = Main.GlobalTimeWrappedHourly;
            float num = (float)auraTimer / 240f + globalTimeWrappedHourly * 0.04f;
            globalTimeWrappedHourly %= 4f;
            globalTimeWrappedHourly /= 2f;
            if (globalTimeWrappedHourly >= 1f)
            {
                globalTimeWrappedHourly = 2f - globalTimeWrappedHourly;
            }

            globalTimeWrappedHourly = globalTimeWrappedHourly * 0.5f + 0.5f;
            if (base.Npc.ai[0] >= 450f && base.Npc.ai[0] < 550f)
            {
                for (int i = 0; i < base.Npc.oldPos.Length; i++)
                {
                    Vector2 vector2 = base.Npc.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, base.Npc.gfxOffY);
                    Color color = base.Npc.GetAlpha(lightColor) * ((float)(base.Npc.oldPos.Length - i) / (float)base.Npc.oldPos.Length);
                    Main.EntitySpriteDraw(TextureAssets.Npc[base.Npc.type].Value, vector2 - new Vector2(25f, 26f), base.Npc.frame, color, base.Npc.rotation, vector, base.Npc.scale, effects);
                }
            }

            if (bossPhase > 1)
            {
                for (float num2 = 0f; num2 < 1f; num2 += 0.25f)
                {
                    float num3 = (num2 + num) * (MathF.PI * 2f);
                    Main.EntitySpriteDraw(glowTexture.Value, base.Npc.Center - Main.screenPosition - new Vector2(0f, 12f) + new Vector2(0f, 16f * auraDistance).RotatedBy(0f - num3) * globalTimeWrappedHourly, base.Npc.frame, new Color(50, 50, 50, 0), base.Npc.rotation, new Vector2(glowTexture.Width(), glowTexture.Height() / 4) / 2f, base.Npc.scale, effects);
                    Main.EntitySpriteDraw(glowTexture.Value, base.Npc.Center - Main.screenPosition - new Vector2(0f, 12f) + new Vector2(0f, 6f).RotatedBy(num3) * globalTimeWrappedHourly, base.Npc.frame, new Color(150, 150, 150, 0), base.Npc.rotation, new Vector2(glowTexture.Width(), glowTexture.Height() / 4) / 2f, base.Npc.scale, effects);
                }
            }

            Main.EntitySpriteDraw(value, LegPos[8] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(lightColor), LegRot[8], new Vector2(value.Width, value.Height) / 2f, base.Npc.scale, effects);
            Main.EntitySpriteDraw(value, LegPos[7] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(lightColor), LegRot[7], new Vector2(value.Width, value.Height) / 2f, base.Npc.scale, effects);
            Main.EntitySpriteDraw(value, LegPos[6] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(lightColor), LegRot[6], new Vector2(value.Width, value.Height) / 2f, base.Npc.scale, effects);
            Main.EntitySpriteDraw(value4, LegPos[11] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(lightColor), LegRot[11], new Vector2(value4.Width, value4.Height) / 2f, base.Npc.scale, effects);
            Main.EntitySpriteDraw(value3, LegPos[10] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(lightColor), LegRot[10], new Vector2(value3.Width, value3.Height) / 2f, base.Npc.scale, effects);
            Main.EntitySpriteDraw(value2, LegPos[9] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(lightColor), LegRot[9], new Vector2(value2.Width, value2.Height) / 2f, base.Npc.scale, effects);
            return true;
        }
        public override void PostDraw(NPC Npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (ShouldOverride())
            {
                Texture2D value = headTexture.Value;
                Texture2D value2 = frontLegUpperTexture.Value;
                Texture2D value3 = frontLeg1Texture.Value;
                Texture2D value4 = frontLeg2Texture.Value;
                Texture2D value5 = frontLeg3Texture.Value;
                SpriteEffects effects = ((base.Npc.spriteDirection != -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                Main.EntitySpriteDraw(value2, LegPos[2] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(drawColor), LegRot[2], new Vector2(value2.Width, value2.Height) / 2f, base.Npc.scale, effects);
                Main.EntitySpriteDraw(value2, LegPos[1] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(drawColor), LegRot[1], new Vector2(value2.Width, value2.Height) / 2f, base.Npc.scale, effects);
                Main.EntitySpriteDraw(value2, LegPos[0] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(drawColor), LegRot[0], new Vector2(value2.Width, value2.Height) / 2f, base.Npc.scale, effects);
                Main.EntitySpriteDraw(value5, LegPos[5] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(drawColor), LegRot[5], new Vector2(value5.Width, value5.Height) / 2f, base.Npc.scale, effects);
                Main.EntitySpriteDraw(value4, LegPos[4] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(drawColor), LegRot[4], new Vector2(value4.Width, value4.Height) / 2f, base.Npc.scale, effects);
                Main.EntitySpriteDraw(value3, LegPos[3] - screenPos, null, base.Npc.GetNPCColorTintedByBuffs(drawColor), LegRot[3], new Vector2(value3.Width, value3.Height) / 2f, base.Npc.scale, effects);
                Main.EntitySpriteDraw(value, base.Npc.Center - Main.screenPosition - new Vector2(0f, 12f), null, base.Npc.GetNPCColorTintedByBuffs(drawColor), base.Npc.rotation, new Vector2(100f, 91f), base.Npc.scale, effects);

                Texture2D texture2D = (Texture2D)ModContent.Request<Texture2D>("VitalityMod/Content/Projectiles/Magic/Wands/ChlorophyteShockwaveEffect");

                if (shield)
                {
                    Main.EntitySpriteDraw(texture2D, Npc.Center - Main.screenPosition, null, new Color(255 - Npc.alpha, 255 - Npc.alpha, 255 - Npc.alpha, 0), 0f, new Vector2(texture2D.Width, texture2D.Height) / 2f, 0.15f, SpriteEffects.None);
                }
            }
        }
        private int oldSpriteDir;

        private float legIdleTimer;

        private float[] LegRot = new float[12];

        private float[] LegIdleRot = new float[12];
        private float GetAverageLegRot(float rad1, float rad2)
        {
            float y = MathF.Sin(rad1) + MathF.Sin(rad2);
            float x = MathF.Cos(rad1) + MathF.Cos(rad2);
            return (MathF.Atan2(y, x) + MathF.PI * 2f) % (MathF.PI * 2f);
        }

        private void LegSway(float recoilMult = 1f)
        {
            legIdleTimer += 1f;
            for (int i = 0; i < 12; i++)
            {
                LegIdleRot[i] = (0f - MathF.Sin(legIdleTimer * 0.06f + (float)i * 0.35f)) * 0.25f;
            }

            Vector2 vector = base.Npc.velocity.RotatedBy(base.Npc.rotation * (float)base.Npc.spriteDirection);
            float num = 2f;
            if (base.Npc.velocity.Y < 0f)
            {
                num = 1f;
            }

            if (base.Npc.spriteDirection == 1)
            {
                vector = base.Npc.velocity.RotatedBy((0f - base.Npc.rotation) * (float)base.Npc.spriteDirection);
                LegRot[0] = LegRot[0].AngleLerp(GetAverageLegRot(MathF.PI / 8f + base.Npc.rotation + vector.X * 0.07f, MathF.PI / 8f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[3], 0.2f);
                LegRot[1] = LegRot[1].AngleLerp(GetAverageLegRot(MathF.PI / 4f + base.Npc.rotation + vector.X * 0.06f, MathF.PI / 4f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[7], 0.25f);
                LegRot[2] = LegRot[2].AngleLerp(GetAverageLegRot(MathF.PI * 3f / 8f + base.Npc.rotation + vector.X * 0.07f, MathF.PI * 3f / 8f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[11], 0.3f);
                LegRot[3] = LegRot[3].AngleLerp(GetAverageLegRot(MathF.PI / 8f + base.Npc.rotation + vector.X * 0.08f, MathF.PI / 8f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[2], 0.2f);
                LegRot[4] = LegRot[4].AngleLerp(GetAverageLegRot(MathF.PI / 20f + base.Npc.rotation + vector.X * 0.07f, MathF.PI / 20f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[6], 0.25f);
                LegRot[5] = LegRot[5].AngleLerp(GetAverageLegRot(0f + base.Npc.rotation + vector.X * 0.08f, 0f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[10], 0.3f);
                LegRot[6] = LegRot[6].AngleLerp(GetAverageLegRot(MathF.PI / 8f + base.Npc.rotation + vector.X * 0.07f, MathF.PI / 8f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[1], 0.2f);
                LegRot[7] = LegRot[7].AngleLerp(GetAverageLegRot(MathF.PI / 4f + base.Npc.rotation + vector.X * 0.06f, MathF.PI / 4f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[5], 0.25f);
                LegRot[8] = LegRot[8].AngleLerp(GetAverageLegRot(MathF.PI * 3f / 8f + base.Npc.rotation + vector.X * 0.07f, MathF.PI * 3f / 8f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[9], 0.3f);
                LegRot[9] = LegRot[9].AngleLerp(GetAverageLegRot(MathF.PI / 8f + base.Npc.rotation + vector.X * 0.08f, MathF.PI / 8f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[0], 0.2f);
                LegRot[10] = LegRot[10].AngleLerp(GetAverageLegRot(MathF.PI / 20f + base.Npc.rotation + vector.X * 0.07f, MathF.PI / 20f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[4], 0.25f);
                LegRot[11] = LegRot[11].AngleLerp(GetAverageLegRot(0f + base.Npc.rotation + vector.X * 0.08f, 0f + base.Npc.velocity.Y * 0.05f * num) - MathF.PI / 4f * recoilMult + LegIdleRot[8], 0.3f);
            }
            else
            {
                LegRot[0] = LegRot[0].AngleLerp(GetAverageLegRot(-MathF.PI / 8f + base.Npc.rotation + vector.X * 0.07f, -MathF.PI / 8f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[3], 0.2f);
                LegRot[1] = LegRot[1].AngleLerp(GetAverageLegRot(-MathF.PI / 4f + base.Npc.rotation + vector.X * 0.06f, -MathF.PI / 4f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[7], 0.25f);
                LegRot[2] = LegRot[2].AngleLerp(GetAverageLegRot(MathF.PI * -3f / 8f + base.Npc.rotation + vector.X * 0.07f, MathF.PI * -3f / 8f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[11], 0.3f);
                LegRot[3] = LegRot[3].AngleLerp(GetAverageLegRot(-MathF.PI / 8f + base.Npc.rotation + vector.X * 0.08f, -MathF.PI / 8f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[2], 0.2f);
                LegRot[4] = LegRot[4].AngleLerp(GetAverageLegRot(-MathF.PI / 20f + base.Npc.rotation + vector.X * 0.07f, -MathF.PI / 20f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[6], 0.25f);
                LegRot[5] = LegRot[5].AngleLerp(GetAverageLegRot(0f + base.Npc.rotation + vector.X * 0.08f, 0f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[10], 0.3f);
                LegRot[6] = LegRot[6].AngleLerp(GetAverageLegRot(-MathF.PI / 8f + base.Npc.rotation + vector.X * 0.07f, -MathF.PI / 8f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[1], 0.2f);
                LegRot[7] = LegRot[7].AngleLerp(GetAverageLegRot(-MathF.PI / 4f + base.Npc.rotation + vector.X * 0.06f, -MathF.PI / 4f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[5], 0.25f);
                LegRot[8] = LegRot[8].AngleLerp(GetAverageLegRot(MathF.PI * -3f / 8f + base.Npc.rotation + vector.X * 0.07f, MathF.PI * -3f / 8f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[9], 0.3f);
                LegRot[9] = LegRot[9].AngleLerp(GetAverageLegRot(-MathF.PI / 8f + base.Npc.rotation + vector.X * 0.08f, -MathF.PI / 8f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[0], 0.2f);
                LegRot[10] = LegRot[10].AngleLerp(GetAverageLegRot(-MathF.PI / 20f + base.Npc.rotation + vector.X * 0.07f, -MathF.PI / 20f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[4], 0.25f);
                LegRot[11] = LegRot[11].AngleLerp(GetAverageLegRot(0f + base.Npc.rotation + vector.X * 0.08f, 0f - base.Npc.velocity.Y * 0.05f * num) + MathF.PI / 4f * recoilMult + LegIdleRot[8], 0.3f);
            }
        }

        private void Worm()
        {
            LegSway(0f);
            if (oldSpriteDir != base.Npc.spriteDirection)
            {
                LegSway(0f);
                oldSpriteDir = base.Npc.spriteDirection;
            }

            LegPos[0] = base.Npc.Center - new Vector2(0f, 12f) + base.Npc.velocity + new Vector2(26 * base.Npc.spriteDirection, 32f).RotatedBy(base.Npc.rotation);
            Vector2[] legPos = LegPos;
            Vector2 vector = base.Npc.Center - new Vector2(0f, 12f) + base.Npc.velocity;
            _ = base.Npc.spriteDirection;
            legPos[1] = vector + new Vector2(0f, 32f).RotatedBy(base.Npc.rotation);
            LegPos[2] = base.Npc.Center - new Vector2(0f, 12f) + base.Npc.velocity + new Vector2(-26 * base.Npc.spriteDirection, 30f).RotatedBy(base.Npc.rotation);
            LegPos[6] = base.Npc.Center - new Vector2(0f, 12f) + base.Npc.velocity + new Vector2(44 * base.Npc.spriteDirection, 24f).RotatedBy(base.Npc.rotation);
            LegPos[7] = base.Npc.Center - new Vector2(0f, 12f) + base.Npc.velocity + new Vector2(18 * base.Npc.spriteDirection, 24f).RotatedBy(base.Npc.rotation);
            LegPos[8] = base.Npc.Center - new Vector2(0f, 12f) + base.Npc.velocity + new Vector2(-8 * base.Npc.spriteDirection, 12f).RotatedBy(base.Npc.rotation);
            if (base.Npc.spriteDirection == 1)
            {
                LegPos[3] = LegPos[0] + Vector2.One.RotatedBy(LegRot[0]) * 10f;
                LegPos[4] = LegPos[1] + Vector2.One.RotatedBy(LegRot[1]) * 10f;
                LegPos[5] = LegPos[2] + Vector2.One.RotatedBy(LegRot[2]) * 10f;
                LegPos[9] = LegPos[6] + Vector2.One.RotatedBy(LegRot[0]) * 10f;
                LegPos[10] = LegPos[7] + Vector2.One.RotatedBy(LegRot[1]) * 10f;
                LegPos[11] = LegPos[8] + Vector2.One.RotatedBy(LegRot[2]) * 10f;
            }
            else
            {
                LegPos[3] = LegPos[0] + Vector2.One.RotatedBy(LegRot[0] + MathF.PI / 2f) * 10f;
                LegPos[4] = LegPos[1] + Vector2.One.RotatedBy(LegRot[1] + MathF.PI / 2f) * 10f;
                LegPos[5] = LegPos[2] + Vector2.One.RotatedBy(LegRot[2] + MathF.PI / 2f) * 10f;
                LegPos[9] = LegPos[6] + Vector2.One.RotatedBy(LegRot[0] + MathF.PI / 2f) * 10f;
                LegPos[10] = LegPos[7] + Vector2.One.RotatedBy(LegRot[1] + MathF.PI / 2f) * 10f;
                LegPos[11] = LegPos[8] + Vector2.One.RotatedBy(LegRot[2] + MathF.PI / 2f) * 10f;
            }
        }

        public override void OnKill(NPC npc)
        {

            for (int i = 0; i < Main.maxNPCs; i++)
            {

                NPC npc2 = Main.npc[i];
                if (npc2.active && npc2.type == ModContent.NPCType<MosquitoMonarchBoss>())
                {
                    if (npc2.GetGlobalNPC<MosquitoMonarchUnreal>().shield)
                    {
                        npc2.GetGlobalNPC<MosquitoMonarchUnreal>().bossPhase = 3;
                    }
                }
            }
            base.OnKill(npc);
        }
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
            else
            {
                if (Npc.life < Npc.lifeMax / 2 && bossPhase < 2)
                {
                    bossPhase = 2;
                }
                if (bossPhase > 1)
                {
                    auraTimer++;
                    auraDistance = (float)Math.Cos((float)auraTimer * 0.08f) + 2f;
                }

                if (Npc.life < Npc.lifeMax / 5 && !checkedForOtherBoss)
                {
                    checkedForOtherBoss = true;
                    bool shouldShield = true;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {

                        NPC npc2 = Main.npc[i];
                        if (npc2.active && npc2.type == ModContent.NPCType<MosquitoMonarchBoss>())
                        {
                            if (npc2.GetGlobalNPC<MosquitoMonarchUnreal>().shield)
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
                    case (float)ActionState.Dash:
                        Dash();
                        break;
                }
            }
            Worm();

            return false;
        }

        public void SwitchAttack(ActionState attack)
        {
            phaseWhenAttackStarted = bossPhase;

            AI_State = (float)attack;
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
                    Npc.velocity = Vector2.Zero;
                    break;
                case (float)ActionState.BeetleCursedWallsMosquitoMonarchDash:
                    AI_Timer = 0;
                    Npc.velocity = Vector2.Zero;
                    Npc.rotation = 0;
                    break;
                case (float)ActionState.StyletConfinedSpaceClawAttack:
                    AI_Timer = 0;
                    break;
                case (float)ActionState.IchorHoverAroundAbovePlayerBeetleSummonEggAndDash:
                    AI_Timer = -90;
                    break;
                case (float)ActionState.PhaseChange:
                    AI_Timer = 0;
                    break;
                case (float)ActionState.Dash:
                    AI_Timer = -90;
                    atkUseCounter = -1;
                    break;
            }
        }
        private void SpawnAi()
        {

            Player player = Main.player[Npc.target];
            targetArea = player.Center + new Vector2(Math.Sign(player.Center.X - Npc.Center.X) * -450 + player.velocity.X * 20, -240);

            float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

            float inertia = 12f;

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
            if (AI_Timer % 240 < 150)
            {
                if (AI_Timer < 240 * 3)
                {
                    targetArea = player.Center + new Vector2(Math.Sign(player.Center.X - Npc.Center.X) * -450 + player.velocity.X * 20, -240);
                }
                else
                {
                    targetArea = player.Center + new Vector2(Math.Sign(player.Center.X - Npc.Center.X) * -300 + player.velocity.X * 20, -300);
                }
                float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

                float inertia = 12f;

                direction = targetArea - Npc.Center;
                direction = direction.SafeNormalize(Vector2.Zero);
                direction *= speed;

                Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;
            }
            else
            {
                Npc.velocity = new Vector2(0, -12);
            }
            if (Npc.Center.X < player.Center.X)
            {
                Npc.spriteDirection = 1;
            }
            else
            {
                Npc.spriteDirection = -1;
            }
            if (AI_Timer % 240 == 150)
            {
                vel = ((player.Center + new Vector2(0, 900)) - Npc.Center).SafeNormalize(Vector2.One);
                phaseWhenAttackStarted = bossPhase;
            }
            if (AI_Timer % 240 > 150)
            {
                if (phaseWhenAttackStarted == 1)
                {
                    if (AI_Timer % 5 == 0)
                    {
                        SoundEngine.PlaySound(in SoundID.Item171, base.Npc.Center);

                        if (Main.netMode != 1)
                        {
                            Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, vel * 4.5f, ModContent.ProjectileType<AnarchyBall>(), 32, 0f);
                        }
                        if (Npc.spriteDirection == 1)
                        {
                            vel = vel.RotatedBy(MathHelper.ToRadians(-5));
                        }
                        else
                        {
                            vel = vel.RotatedBy(MathHelper.ToRadians(5));
                        }
                        Npc.rotation = vel.ToRotation();
                        Npc.netUpdate = true;

                    }
                }
                else
                {

                    if (AI_Timer % 3 == 0)
                    {
                        SoundEngine.PlaySound(in SoundID.Item171, base.Npc.Center);

                        if (Main.netMode != 1)
                        {
                            Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, vel.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-6, 6))) * 4.5f, ModContent.ProjectileType<AnarchyBall>(), 32, 0f);
                        }
                        if (Npc.spriteDirection == 1)
                        {
                            vel = vel.RotatedBy(MathHelper.ToRadians(-5));
                        }
                        else
                        {
                            vel = vel.RotatedBy(MathHelper.ToRadians(5));
                        }
                        Npc.rotation = vel.ToRotation();
                        Npc.netUpdate = true;

                    }

                }
            }
            if (AI_Timer % 240 == 239)
            {
                Npc.Center = new Vector2(player.Center.X + (player.Center.X - Npc.Center.X), Npc.Center.Y);
            }

            else
            {
                Npc.rotation = 0;
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

            base.Npc.rotation = base.Npc.AngleTo(player.Center);
            if (player.Center.X > Npc.Center.X)
            {
                base.Npc.spriteDirection = 1;
            }
            else
            {
                base.Npc.spriteDirection = -1;
                base.Npc.rotation += MathF.PI;
            }
            if ((AI_Timer % 45 == 5 || AI_Timer == -40) && AI_Timer < 180)
            {
                ShootSpread(player);
                ShootRecoil(player);
            }
            Npc.velocity *= 0.95f;
            AI_Timer += 1;

            if (AI_Timer == 105 * 2)
            {
                SwitchAttack(ActionState.BeetleCursedWallsMosquitoMonarchDash);
            }

        }
        private void BeetleCursedWallsMosquitoMonarchDash()
        {

            Player player = Main.player[Npc.target];
            if (AI_Timer % 150 < 105)
            {
                if (AI_Timer % 150 == 0)
                {
                    if (Npc.Center.X < player.Center.X)
                    {
                        targetArea = new Vector2(-660, Main.rand.NextFloat(-300, -600));
                    }
                    else
                    {
                        targetArea = new Vector2(660, Main.rand.NextFloat(-300, -600));
                    }
                }
                float speed = Vector2.Distance(Npc.Center, player.Center + targetArea) / (1f - ((AI_Timer % 150) - 105) * 0.25f);
                float inertia = (1f - ((AI_Timer % 150) - 105) * 0.25f);
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
                if (AI_Timer % 150 == 105)
                {
                    SoundEngine.PlaySound(in SoundID.ForceRoar, base.Npc.Center);

                    Npc.Center = player.Center + targetArea;
                    if (targetArea.X < 0)
                    {
                        Npc.velocity = new Vector2(36, 0);
                    }
                    else
                    {
                        Npc.velocity = new Vector2(-36, 0);
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, new Vector2(0f, 1f) + Main.rand.NextVector2Circular(0.5f, 0.5f), ModContent.ProjectileType<AnarchyShard2>(), 40, 0f);
                    }
                    delay = Main.rand.Next(0, 6);
                }
                if (((AI_Timer % 150) - 105) % 9 == 0)
                {
                    SoundEngine.PlaySound(in SoundID.Item73, base.Npc.position);

                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, new Vector2(0f, 0f), ModContent.ProjectileType<VerticalCursedOrb>(), 20, 0f);
                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, new Vector2(0f, 0f), ModContent.ProjectileType<VerticalCursedOrb2>(), 20, 0f);
                    base.Npc.netUpdate = true;
                }
                if (AI_Timer % 150 == 141)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, new Vector2(0f, 2f) + Main.rand.NextVector2Circular(0.5f, 0.5f), ModContent.ProjectileType<AnarchyShard2>(), 40, 0f);
                    }
                }
                if (phaseWhenAttackStarted == 2)
                {
                    if (AI_Timer % 6 == delay)
                    {
                        Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Bottom, new Vector2(0f, 2f), ModContent.ProjectileType<AnarchyShard2>(), 30, 0f);

                    }
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
            Player player = Main.player[Npc.target];
            if (phaseWhenAttackStarted == 1)
            {
                if (AI_Timer < 150)
                {
                    targetArea = player.Center + new Vector2(Math.Sign(player.Center.X - Npc.Center.X) * -800, 150);

                    float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

                    float inertia = 12f;

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

                    if (AI_Timer % 45 == 0)
                    {
                        targetArea = player.Center + new Vector2(Math.Sign(player.Center.X - Npc.Center.X) * -800, 150) + Main.rand.NextVector2CircularEdge(200, 75);

                        SoundEngine.PlaySound(in SoundID.Item73, base.Npc.position);
                        if (Main.netMode != 1)
                        {
                            if (player.Center.X > Npc.Center.X)
                            {
                                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, new Vector2(0f, 0f), ModContent.ProjectileType<ClawAlt2>(), 25, 0f);
                                base.Npc.netUpdate = true;
                            }
                            else
                            {
                                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, new Vector2(0f, 0f), ModContent.ProjectileType<ClawAlt1>(), 25, 0f);
                                base.Npc.netUpdate = true;
                            }
                            if (AI_Timer % 90 == 0)
                            {
                                Projectile.NewProjectile(base.Npc.GetSource_FromAI(), Npc.Center, (player.Center - Npc.Center).SafeNormalize(Vector2.One) * 4.5f, ModContent.ProjectileType<AnarchyBall>(), 32, 0f);

                            }
                        }
                    }

                    float speed = Vector2.Distance(Npc.Center, targetArea) / 36f;

                    float inertia = 5f;

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
            }
            else
            {

                if (AI_Timer < 150)
                {
                    targetArea = player.Center + new Vector2(Math.Sign(player.Center.X - Npc.Center.X) * -800, 0);

                    float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

                    float inertia = 12f;

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

                    if (AI_Timer % 45 == 0 && AI_Timer < 495)
                    {
                        targetArea = player.Center + new Vector2(Math.Sign(player.Center.X - Npc.Center.X) * -800, 0);

                        SoundEngine.PlaySound(in SoundID.Item73, base.Npc.position);
                        if (Main.netMode != 1)
                        {
                            if (player.Center.X > Npc.Center.X)
                            {
                                for (int i = -3; i < 4; i++)
                                {
                                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center + new Vector2(-300, i * 300), new Vector2(0f, 0f), ModContent.ProjectileType<ClawAlt2>(), 25, 0f);
                                }
                                base.Npc.netUpdate = true;
                            }
                            else
                            {

                                for (int i = -3; i < 4; i++)
                                {
                                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center + new Vector2(300, i * 300), new Vector2(0f, 0f), ModContent.ProjectileType<ClawAlt1>(), 25, 0f);
                                }
                                base.Npc.netUpdate = true;
                            }
                        }
                    }

                    float speed = Vector2.Distance(Npc.Center, targetArea) / 36f;

                    float inertia = 5f;

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

            if (AI_Timer < 0)
            {
                if (phaseWhenAttackStarted == 1)
                {
                    if (AI_Timer < -30)
                    {
                        targetArea = player.Center + new Vector2(Math.Sign(player.Center.X - Npc.Center.X) * -650, 0);
                    }
                    else
                    {
                        targetArea = player.Center + new Vector2(Math.Sign(player.Center.X - Npc.Center.X) * -350, 0);
                    }

                    float speed = Vector2.Distance(Npc.Center, targetArea) / 45f;

                    float inertia = 12f;

                    direction = targetArea - Npc.Center;
                    direction = direction.SafeNormalize(Vector2.Zero);
                    direction *= speed;

                    Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

                    base.Npc.rotation = base.Npc.AngleTo(player.Center);
                    if (player.Center.X > Npc.Center.X)
                    {
                        base.Npc.spriteDirection = 1;
                    }
                    else
                    {
                        base.Npc.spriteDirection = -1;
                        base.Npc.rotation += MathF.PI;
                    }
                    if (AI_Timer % 6 == 0)
                    {
                        Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(Main.rand.Next(-35, 35) * 0.3f, Main.rand.Next(-35, 35) * 0.3f), ModContent.ProjectileType<AnarchyEgg1>(), 0, 1f);
                    }
                }
                else
                {
                    if (AI_Timer < -75)
                    {
                        targetArea = new Vector2(400, 0).RotatedBy(Npc.AngleFrom(player.Center));

                        float speed = Vector2.Distance(Npc.Center, player.Center + targetArea) / (1f - (AI_Timer + 75) * 0.25f);
                        float inertia = (1f - (AI_Timer + 75) * 0.25f);
                        Vector2 direction = (player.Center + targetArea) - Npc.Center;
                        direction = direction.SafeNormalize(Vector2.Zero);
                        direction *= speed;

                        Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;
                    }
                    else
                    {
                        if (AI_Timer == -75)
                        {
                            Npc.Center = player.Center + targetArea;
                            atkDir = Npc.AngleFrom(player.Center);
                            turnSpeed = Main.rand.NextBool() ? -15 : 15;
                        }

                        Npc.Center = player.Center + new Vector2(400, 0).RotatedBy(atkDir);
                        atkDir += MathHelper.ToRadians(turnSpeed);
                        if (AI_Timer < -15)
                        {
                            if (turnSpeed < 0)
                            {
                                turnSpeed += 0.25f;
                            }
                            else
                            {
                                turnSpeed -= 0.25f;
                            }
                        }
                        if (AI_Timer % 5 == 0)
                        {
                            Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, (player.Center - Npc.Center).SafeNormalize(Vector2.One) * -8 + Main.rand.NextVector2Circular(2, 2), ModContent.ProjectileType<AnarchyEgg1>(), 0, 1f);
                        }
                    }
                    if (Npc.Center.X < player.Center.X)
                    {
                        Npc.spriteDirection = 1;
                    }
                    else
                    {
                        Npc.spriteDirection = -1;
                    }
                }
            }
            else
            {
                if (AI_Timer % 60 == 0)
                {
                    SoundEngine.PlaySound(in SoundID.ForceRoar, base.Npc.Center);
                    if (phaseWhenAttackStarted == 2)
                    {
                        ShootCloud();
                    }
                    Npc.velocity = (player.Center - Npc.Center).SafeNormalize(Vector2.One) * 20;
                }
                else
                {

                    if (AI_Timer % 60 > 36)
                    {
                        base.Npc.rotation = base.Npc.AngleTo(player.Center);
                        if (player.Center.X > Npc.Center.X)
                        {
                            base.Npc.spriteDirection = 1;
                        }
                        else
                        {
                            base.Npc.spriteDirection = -1;
                            base.Npc.rotation += MathF.PI;
                        }
                        Npc.velocity *= 0.9f;

                    }
                }
            }
            AI_Timer += 1;
            if (AI_Timer == 240)
            {
                SwitchAttack(ActionState.SpawnAi);
                Npc.rotation = 0;
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
                SwitchAttack(ActionState.Dash);
            }
        }
        private void Dash()
        {
            
            Player player = Main.player[Npc.target];

            if (AI_Timer < 0)
            {
                if (AI_Timer < -75)
                {
                    targetArea = new Vector2(450, 0).RotatedBy(Npc.AngleFrom(player.Center));

                    float speed = Vector2.Distance(Npc.Center, player.Center + targetArea) / (1f - (AI_Timer + 75) * 0.25f);
                    float inertia = (1f - (AI_Timer + 75) * 0.25f);
                    Vector2 direction = (player.Center + targetArea) - Npc.Center;
                    direction = direction.SafeNormalize(Vector2.Zero);
                    direction *= speed;

                    Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;

                    base.Npc.rotation = base.Npc.AngleTo(player.Center);
                    if (player.Center.X > Npc.Center.X)
                    {
                        base.Npc.spriteDirection = 1;
                    }
                    else
                    {
                        base.Npc.spriteDirection = -1;
                        base.Npc.rotation += MathF.PI;
                    }
                }
                else
                {
                    if (AI_Timer == -75)
                    {
                        Npc.Center = player.Center + targetArea;
                        atkDir = Npc.AngleFrom(player.Center);
                        turnSpeed = Main.rand.NextBool() ? -15 : 15;
                        atkUseCounter++;
                    }

                    Npc.Center = player.Center + new Vector2(450, 0).RotatedBy(atkDir);
                    atkDir += MathHelper.ToRadians(turnSpeed);
                    if (AI_Timer < -15)
                    {
                        if (turnSpeed < 0)
                        {
                            turnSpeed += 0.25f;
                        }
                        else
                        {
                            turnSpeed -= 0.25f;
                        }
                    }
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, Main.rand.NextVector2Circular(6, 6), ModContent.ProjectileType<AnarchyCloud>(), 25, 1f);

                    base.Npc.rotation = base.Npc.AngleTo(player.Center);
                    if (player.Center.X > Npc.Center.X)
                    {
                        base.Npc.spriteDirection = 1;
                    }
                    else
                    {
                        base.Npc.spriteDirection = -1;
                        base.Npc.rotation += MathF.PI;
                    }
                }
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
                if (AI_Timer % 54 == 0)
                {
                    SoundEngine.PlaySound(in SoundID.ForceRoar, base.Npc.Center);
                    Npc.velocity = (player.Center - Npc.Center).SafeNormalize(Vector2.One) * 27;
                    inheritMomentum = 0;
                }
                else
                {
                    if (AI_Timer % 54 > 18)
                    {
                        Npc.velocity *= 0.97f;
                        if (inheritMomentum >= 2)
                        {
                            inheritMomentum = 2;
                        }
                        else
                        {
                            inheritMomentum += 1 /10f;
                        }
                    }
                    if (AI_Timer % 54 > 36)
                    {
                        base.Npc.rotation = base.Npc.AngleTo(player.Center);
                        if (player.Center.X > Npc.Center.X)
                        {
                            base.Npc.spriteDirection = 1;
                        }
                        else
                        {
                            base.Npc.spriteDirection = -1;
                            base.Npc.rotation += MathF.PI;
                        }
                    }
                    Npc.Center += player.velocity * inheritMomentum;
                }
            }
            AI_Timer += 1;

            if (AI_Timer == 54 + (atkUseCounter % 3) * 54)
            {
                AI_Timer = -90;
            }

        }
        private void ShootSpread(Player player)
        {
            SoundEngine.PlaySound(in SoundID.Item171, base.Npc.position);
            Vector2 center = base.Npc.Center;
            int type = ModContent.ProjectileType<AnarchyShard2>();
            Vector2 vector = player.Center - base.Npc.Center;
            float num = vector.Length();
            vector = ((!(num > 0f)) ? new Vector2(0f, 5f) : (vector * 6f / num * 2.75f));
            int num2 = 5;
            for (int i = 0; i < num2; i++)
            {
                Vector2 vector2 = vector.RotatedByRandom(MathHelper.ToRadians(15f));
                float num3 = 1f - Main.rand.NextFloat() * 0.3f;
                vector2 *= num3;
                if (Main.netMode != 1)
                {
                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), center, vector2 * 0.2f, type, 20, 0f);
                }
            }
        }
        private void ShootRecoil(Player player)
        {
            float num = 9f;
            Vector2 vector = player.Center - base.Npc.Center;
            float num2 = vector.Length();
            vector *= num / num2;
            base.Npc.velocity = -vector;
        }


        private void ShootCloud()
        {
            if (Main.netMode != 1)
            {
                int type = ModContent.ProjectileType<AnarchyCloud>();
                int num = 12;
                for (int i = 0; i < num; i++)
                {
                    Projectile.NewProjectile(base.Npc.GetSource_FromAI(), base.Npc.Center, new Vector2((float)Main.rand.Next(-35, 35) * 0.3f, (float)Main.rand.Next(-35, 35) * 0.3f), type, 25, 1f);
                }

                base.Npc.netUpdate = true;
            }
        }
    }
}