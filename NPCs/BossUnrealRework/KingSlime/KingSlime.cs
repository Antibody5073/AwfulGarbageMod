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
using AwfulGarbageMod.Items.Consumables; using AwfulGarbageMod.Items.Consumables.BossSummon;
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

namespace AwfulGarbageMod.NPCs.BossUnrealRework.KingSlime
{
    // This ModNPC serves as an example of a completely custom AI.

    
    [AutoloadBossHead]
    public class KingSlime : UnrealNPCOverride
    {
        protected override int OverriddenNpc => NPCID.KingSlime;
        // Here we define an enum we will use with the State slot. Using an ai slot as a means to store "state" can simplify things greatly. Think flowchart.
        private enum ActionState
        {
            NormalJump,
            SlimeRain,
            Slam,
            HorizontalLunge,
            BigSlam,
            SpikeDash,
            HalfHealth
        }
        // These are reference properties. One, for example, lets us write AI_State as if it's Npc.ai[0], essentially giving the index zero our own name.
        // Here they help to keep our AI code clear of clutter. Without them, every instance of "AI_State" in the AI code below would be "Npc.ai[0]", which is quite hard to read.
        // This is all to just make beautiful, manageable, and clean code.
        float AI_State;
        float Next_State;
        float AI_Timer;
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
        Vector2 targetArea;
        Vector2 direction;
        Vector2 storedVel;
        Vector2 storedPos;
        Vector2 recoil;
        List<Vector2> playerPos = new List<Vector2> { };

        public override void SetStaticDefaults()
        {
            if (DifficultyModes.Difficulty > 0)
            {
                NPCID.Sets.TrailCacheLength[NPCID.KingSlime] = 8;
                NPCID.Sets.TrailingMode[NPCID.KingSlime] = 0;
            }
        }
        public override void SetDefaults(NPC entity)
        {
            if (DifficultyModes.Difficulty > 0)
            {
                entity.lifeMax = 2800;
                entity.aiStyle = -1;
                entity.width = 78;
                entity.height = 90;
                entity.noGravity = true;
                ChangeScale(entity, 1);
            }
        }
        public override bool UnrealDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {


            DrawSlime(npc, spriteBatch, screenPos, lightColor);

            DrawCrown(npc, spriteBatch, screenPos, lightColor);

            return false;
        }

        private void DrawNinja(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            // Draw Ninja
            Vector2 zero = Vector2.Zero;
            float num243 = 0f;
            zero.Y -= npc.velocity.Y;
            zero.X -= npc.velocity.X * 2f;
            num243 += npc.velocity.X * 0.05f;
            if (npc.frame.Y == 120)
            {
                zero.Y += 2f;
            }
            if (npc.frame.Y == 360)
            {
                zero.Y -= 2f;
            }
            if (npc.frame.Y == 480)
            {
                zero.Y -= 6f;
            }
            spriteBatch.Draw(TextureAssets.Ninja.Value, new Vector2(npc.position.X - screenPos.X + (float)(npc.width / 2) + zero.X, npc.position.Y - screenPos.Y + (float)(npc.height / 2) + zero.Y), new Rectangle(0, 0, TextureAssets.Ninja.Width(), TextureAssets.Ninja.Height()), lightColor, num243, new Vector2(TextureAssets.Ninja.Width() / 2, TextureAssets.Ninja.Height() / 2), 1f, SpriteEffects.None, 0f);
        }

        private void DrawSlime(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            var drawPos = npc.Center - screenPos + Vector2.UnitY * npc.gfxOffY;
            drawPos.Y -= 8f * npc.scale; 
            var texAsset = TextureAssets.Npc[npc.type];
            var texture = texAsset.Value;
            var origin = npc.frame.Size() * 0.5f; 
            for (int k = 0; k < npc.oldPos.Length; k++)
            {
                Vector2 drawPos2 = npc.oldPos[k] - screenPos + Vector2.UnitY * npc.gfxOffY;
                drawPos2.Y -= 8f * npc.scale;
                Color color = npc.GetAlpha(lightColor) * (float)(((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length) / 3);
                spriteBatch.Draw(TextureAssets.Npc[npc.type].Value, drawPos2 + new Vector2(npc.width / 2, npc.height / 2), npc.frame, color, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            }
            DrawNinja(npc, spriteBatch, screenPos, lightColor);

            spriteBatch.Draw(
                texture, drawPos,
                npc.frame, npc.GetAlpha(lightColor),
                npc.rotation, origin, npc.scale,
                SpriteEffects.None, 0f);
        }

        private void DrawCrown(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D value87 = TextureAssets.Extra[39].Value;
            Vector2 center3 = npc.Center;
            float num154 = 0f;
            switch (npc.frame.Y / (TextureAssets.Npc[npc.type].Value.Height / Main.npcFrameCount[npc.type]))
            {
                case 0:
                    num154 = 2f;
                    break;
                case 1:
                    num154 = -6f;
                    break;
                case 2:
                    num154 = 2f;
                    break;
                case 3:
                    num154 = 10f;
                    break;
                case 4:
                    num154 = 2f;
                    break;
                case 5:
                    num154 = 0f;
                    break;
            }
            center3.Y += npc.gfxOffY - (70f - num154) * npc.scale;
            spriteBatch.Draw(value87, center3 - screenPos, null, lightColor, 0f, value87.Size() / 2f, 1f, SpriteEffects.None, 0f);

        }

        private void ChangeScale(NPC npc, float scale)
        {
            // Don't know why it has to be done this way, but it's how the game does it
            npc.position.X += npc.width / 2;
            npc.position.Y += npc.height;
            npc.scale = scale;
            npc.width = (int)(98f * scale);
            npc.height = (int)(92f * scale);
            npc.position.X -= npc.width / 2;
            npc.position.Y -= npc.height;
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
            }
            if (Npc.life < Npc.lifeMax * 2 / 3 && bossPhase == 1)
            {
                Npc.dontTakeDamage = true;
                AI_State = (float)ActionState.HalfHealth;
                AI_Timer = 0;
                bossPhase = 2;
            }

            switch (AI_State)
            {
                case (float)ActionState.NormalJump:
                    NormalJump();
                    break;
                case (float)ActionState.SlimeRain:
                    SlimeRain();
                    break;
                case (float)ActionState.Slam:
                    Slam();
                    break;
                case (float)ActionState.HorizontalLunge:
                    HorizontalLunge();
                    break;
                case (float)ActionState.BigSlam:
                    BigSlam();
                    break;
                case (float)ActionState.SpikeDash:
                    SpikeDash();
                    break;
                case (float)ActionState.HalfHealth:
                    HalfHealth();
                    break;
            }
            return false;
        }

        private void SwitchAttack(ActionState attack)
        {
            AI_State = (AI_State + Main.rand.Next(1, 6)) % 6;

            if (bossPhase == 2)
            {
                KingSlimeShadow shadow = Main.npc[(int)Npc.ai[0]].ModNPC as KingSlimeShadow;
                shadow.ExternalSwitchAttack((AI_State + Main.rand.Next(1, 6)) % 6);
            }
            switch (AI_State)
            {
                case (float)ActionState.NormalJump:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    y0 = 0;
                    jumping = false;
                    break;
                case (float)ActionState.SlimeRain:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    Npc.noTileCollide = true;
                    break;
                case (float)ActionState.Slam:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    break;
                case (float)ActionState.HorizontalLunge:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    lunging = false;
                    jumping = false;
                    y0 = 0;
                    break;
                case (float)ActionState.BigSlam:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    jumping = false;
                    break;
                case (float)ActionState.SpikeDash:
                    AI_Timer = 0;
                    atkUseCounter = 0;
                    atkDelay = 0;
                    lunging = false;
                    jumping = false;
                    y0 = 0;
                    Npc.velocity.Y = 5;
                    break;
            }
        }
        private void NormalJump()
        {
            Player player = Main.player[Npc.target];

            AI_Timer += 1;
            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.SlimeRain);
                return;

            }

            if (jumping && Npc.velocity.Y == 0)
            {
                y0++;
                if (y0 == 2)
                {
                    atkDelay = 45;
                    jumping = false;
                }
            }

            if (atkDelay > 0)
            {
                atkDelay--;
                Npc.velocity.X *= 0.8f;
            }
            else
            {
                if (!jumping && AI_Timer <= 720)
                {
                    atkUseCounter++;
                    y0 = 0;

                    Vector2 jumpvel = Vector2.Zero;

                    float xspd = DifficultyModes.Difficulty == 2 ? 7.5f : 3;
                    jumpvel.X = player.Center.X - Npc.Center.X < 0 ? -xspd : xspd;
                    jumpvel.X += (player.Center.X - Npc.Center.X) / 100;

                    switch (atkUseCounter % 4)
                    {
                        case 0:
                            jumpvel.Y = -6;
                            break;
                        case 1:
                            jumpvel.Y = -7;
                            break;
                        case 2:
                            jumpvel.Y = -5;
                            break;
                        case 3:
                            jumpvel.Y = -14;
                            break;
                    }

                    Npc.velocity = jumpvel;

                    jumping = true;
                }
            }

            if (Npc.velocity.Y < 0)
            {
                Npc.noTileCollide = true;
            }
            else
            {
                if (Main.player[Npc.target].Center.Y > Npc.Bottom.Y)
                {
                    Npc.noTileCollide = true;
                }
                else
                {
                    Npc.noTileCollide = false;
                }
            }

            Npc.velocity.Y += 0.25f;
            if (Npc.velocity.Y > 15)
            {
                Npc.velocity.Y = 15;
            }
        }
        private void SlimeRain()
        {

            Npc.TargetClosest(false);
            Player player = Main.player[Npc.target];
            targetArea = player.Center + new Vector2(player.velocity.X * 20, -320);


            direction = targetArea - Npc.Center;
            direction.Normalize();
            direction += (targetArea - Npc.Center) / 320;



            if (AI_Timer < 720)
            {
                if (Vector2.DistanceSquared(Npc.Center, targetArea) > 48 * 48)
                {
                    Npc.velocity += direction;
                    Npc.velocity *= 0.9f;
                }
                if (AI_Timer % 180 == 0)
                {
                    atkX = player.Center.X - 1500 + Main.rand.NextFloat(-60, 60);
                }
                if (AI_Timer % 180 == 90)
                {
                    atkX = player.Center.X + 1500 + Main.rand.NextFloat(-60, 60);
                }
                if (DifficultyModes.Difficulty == 2 || AI_Timer % 3 == 0)
                {
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, Vector2.Zero, ModContent.ProjectileType<KingSlimeProjSlime>(), 12, 0, Main.myPlayer, atkX, player.Center.Y - 420);

                    if (AI_Timer % 180 < 90)
                    {
                        atkX += DifficultyModes.Difficulty == 2 ? 50 : 150;
                    }
                    else
                    {
                        atkX -= DifficultyModes.Difficulty == 2 ? 50 : 150;
                    }
                }
            }
            else
            {


                if (Npc.velocity.Y < 0)
                {
                    Npc.noTileCollide = true;
                }
                else
                {
                    if (Main.player[Npc.target].Center.Y > Npc.Bottom.Y)
                    {
                        Npc.noTileCollide = true;
                    }
                    else
                    {
                        Npc.noTileCollide = false;
                    }
                }
                if (AI_Timer > 735)
                {
                    Npc.velocity.Y += 0.125f;
                }
                if (Npc.velocity.Y > 15)
                {
                    Npc.velocity.Y = 15;
                }
                Npc.velocity.X *= 0.92f;

            }

            AI_Timer += 1;

            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.Slam);
                return;

            }
        }
        private void Slam()
        {
            AI_Timer += 1;

            atkDelay++;
            if (AI_Timer < 720)
            {
                if (atkDelay <= 0)
                {
                }
                else if (atkDelay < 120)
                {
                    Npc.noTileCollide = true;
                    Player player = Main.player[Npc.target];


                    float inertia = 8f;
                    if (DifficultyModes.Difficulty == 2)
                    {
                        targetArea = player.Center + new Vector2(player.velocity.X * 50, -430);

                    }
                    else
                    {
                        targetArea = player.Center + new Vector2((player.velocity.X * 20), -430);
                    }
                    float speed = Vector2.Distance(Npc.Center, targetArea) / 18f;


                    direction = targetArea - Npc.Center;
                    direction.Normalize();
                    direction *= speed;

                    Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;
                    if (Npc.Center.Y > player.Center.Y - 120)
                    {
                        Npc.velocity.X /= 3;
                    }
                }
                else if (atkDelay == 120)
                {
                    Npc.globalEnemyBossInfo().finishedAtk = false;

                    Npc.velocity = new Vector2(0, 0.05f);
                    tileHitboxIndex = Projectile.NewProjectile(Npc.GetSource_FromAI(), new Vector2(Npc.Center.X, (int)Npc.Center.Y), new Vector2(0, 5.25f), ModContent.ProjectileType<KingSlimeProjSlam>(), 0, 0, Main.myPlayer, Npc.target, Npc.whoAmI);
                }
                else if (atkDelay > 120)
                {
                    if (Npc.globalEnemyBossInfo().finishedAtk)
                    {
                        atkDelay = -15;
                        Npc.velocity = new Vector2(0, 0);

                        for (int i = 0; i < 2; i++)
                        {
                            int type = DifficultyModes.Difficulty == 2 ? Main.rand.NextFromList(NPCID.MotherSlime, NPCID.BlackSlime, NPCID.SpikedIceSlime, NPCID.SpikedJungleSlime) : NPCID.BlueSlime;
                            int nPC = NPC.NewNPC(NPC.GetBossSpawnSource(Npc.target), (int)Npc.Center.X, (int)Npc.Center.Y, type);

                            Main.npc[nPC].lifeMax *= 12;
                            Main.npc[nPC].life *= 12;

                            Main.npc[nPC].lifeMax /= 75;
                            Main.npc[nPC].life /= 75;

                            Main.npc[nPC].damage = (int)(Main.npc[nPC].damage * 2f);
                            Main.npc[nPC].velocity = Main.rand.NextVector2Circular(6, 3) + new Vector2(0, -8);
                        }
                        SoundEngine.PlaySound(SoundID.Item14, Npc.Center);
                    }
                }
            }
            else
            {
                if (Npc.velocity.Y < 0)
                {
                    Npc.noTileCollide = true;
                }
                else
                {
                    if (Main.player[Npc.target].Center.Y > Npc.Bottom.Y)
                    {
                        Npc.noTileCollide = true;
                    }
                    else
                    {
                        Npc.noTileCollide = false;
                    }
                }
                Npc.velocity.Y += 0.2f;
                if (Npc.velocity.Y > 15)
                {
                    Npc.velocity.Y = 15;
                }
                Npc.velocity.X *= 0.92f;

            }


            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.HorizontalLunge);
                return;

            }
        }
        private void HorizontalLunge()
        {
            Player player = Main.player[Npc.target];

            AI_Timer += 1;
            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.BigSlam);
                return;

            }

            if (jumping && Npc.velocity.Y == 0)
            {
                y0++;
                if (y0 == 2)
                {
                    atkDelay = 90;
                    jumping = false;
                    if (AI_Timer <= 720)
                    {
                        lunging = true;

                        if (player.Center.X - Npc.Center.X < 0)
                        {
                            Npc.velocity.X = -27;
                        }
                        else
                        {
                            Npc.velocity.X = 27;
                        }
                        Npc.velocity.X += (player.Center.X - Npc.Center.X) / 75;
                    }
                }
            }
            for (int i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(Npc.position, Npc.width, Npc.height, DustID.t_Slime, 0f, 0f, 0, new Color(0, 0, 255, 0), 1f);
                Main.dust[dust].scale = Main.rand.NextFloat(1.2f, 1.7f);
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true; 
            }
            if (atkDelay > 0)
            {
                atkDelay--;
                Npc.velocity.X *= 0.98f;
            }
            else
            {
                lunging = false;
                if (!jumping && AI_Timer <= 720)
                {
                    atkUseCounter++;
                    y0 = 0;

                    Vector2 jumpvel = Vector2.Zero;
                    if (player.Center.X - Npc.Center.X < 0)
                    {
                        jumpvel.X += (player.Center.X + 540 - Npc.Center.X) / 112;
                    }
                    else
                    {
                        jumpvel.X += (player.Center.X - 540 - Npc.Center.X) / 112;
                    }

                    jumpvel.Y = -14;

                    Npc.velocity = jumpvel;

                    jumping = true;
                }
            }

            if (Npc.velocity.Y < 0)
            {
                Npc.noTileCollide = true;
            }
            else
            {
                if (Main.player[Npc.target].Center.Y - 160 > Npc.Bottom.Y)
                {
                    Npc.noTileCollide = true;
                }
                else
                {
                    Npc.noTileCollide = false;
                }
            }

            Npc.velocity.Y += 0.25f;
            if (Npc.velocity.Y > 15)
            {
                Npc.velocity.Y = 15;
            }

        }
        private void BigSlam()
        {
            AI_Timer += 1;

            atkDelay++;
            if (AI_Timer < 720)
            {
                if (atkDelay <= 0)
                {
                }
                else if (AI_Timer < 240)
                {
                    Npc.noTileCollide = true;
                    Player player = Main.player[Npc.target];


                    float inertia = 8f;
                    if (DifficultyModes.Difficulty == 2)
                    {
                        targetArea = player.Center + new Vector2(player.velocity.X * 25, -450);

                    }
                    else
                    {
                        targetArea = player.Center + new Vector2((player.velocity.X * 10), -450);
                    }
                    float speed = Vector2.Distance(Npc.Center, targetArea) / 18f;

                    direction = targetArea - Npc.Center;
                    direction.Normalize();
                    direction *= speed;

                    Npc.velocity = (Npc.velocity * (inertia - 1) + direction) / inertia;
                    if (Npc.Center.Y > player.Center.Y - 120)
                    {
                        Npc.velocity.X /= 3;
                    }

                    int dust = Dust.NewDust(Npc.BottomLeft + new Vector2(0,-16), 1, 1, DustID.t_Slime, 0f, 0f, 0, new Color(0, 0, 255, 0), 1f);
                    Main.dust[dust].scale = 1.25f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = false;
                    dust = Dust.NewDust(Npc.BottomRight + new Vector2(0, -16), 1, 1, DustID.t_Slime, 0f, 0f, 0, new Color(0, 0, 255, 0), 1f);
                    Main.dust[dust].scale = 1.25f;
                    Main.dust[dust].velocity *= 0f;
                    Main.dust[dust].noGravity = false;
                }
                else if (AI_Timer == 240)
                {
                    Npc.globalEnemyBossInfo().finishedAtk = false;

                    Npc.velocity = new Vector2(0, 0.05f);
                    tileHitboxIndex = Projectile.NewProjectile(Npc.GetSource_FromAI(), new Vector2(Npc.Center.X, (int)Npc.Center.Y), new Vector2(0, 9), ModContent.ProjectileType<KingSlimeProjSlam>(), 0, 0, Main.myPlayer, Npc.target, Npc.whoAmI);
                }
                else if (AI_Timer > 240)
                {
                    if (Npc.globalEnemyBossInfo().finishedAtk)
                    {
                        Npc.velocity = new Vector2(0, 0);

                        Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Bottom, new Vector2(224, 0), ModContent.ProjectileType<KingSlimeProjGround>(), 0, 0, Main.myPlayer, 12);
                        Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Bottom, new Vector2(-224, 0), ModContent.ProjectileType<KingSlimeProjGround>(), 0, 0, Main.myPlayer, 12);
                        Npc.globalEnemyBossInfo().finishedAtk = false;

                        SoundEngine.PlaySound(SoundID.Item14, Npc.Center);
                    }
                }
            }


            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.SpikeDash);
                return;
            }
        }
        private void SpikeDash() 
        {
            Player player = Main.player[Npc.target];

            AI_Timer += 1;
            if (AI_Timer == 840)
            {
                SwitchAttack(ActionState.NormalJump);
                return;
            }

            if (jumping && Npc.velocity.Y == 0)
            {
                y0++;

                if (y0 == 1)
                {
                    if (AI_Timer <= 720)
                    {
                        lunging = true;
                        atkDelay = 45;
                        oldXvel = Npc.velocity.X;
                        if (player.Center.X - Npc.Center.X < 0)
                        {
                            Npc.velocity.X = -30;
                        }
                        else
                        {
                            Npc.velocity.X = 30;
                        }
                        Npc.velocity.X += player.velocity.X / 2;
                        Npc.velocity.Y = 1;

                    }
                }
                else if (y0 == 2)
                {
                    jumping = false;
                    if (AI_Timer <= 720)
                    {
                        lunging = false;
                    }
                }
            }
            if (atkDelay > 0)
            {
                atkDelay--;
                if (atkDelay % 3 == 0)
                {
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, -2).RotatedByRandom(MathHelper.ToRadians(30)), DifficultyModes.Difficulty == 2 ? ProjectileID.IceSpike : ProjectileID.SpikedSlimeSpike, 9, 0, Main.myPlayer);
                }
                if (DifficultyModes.Difficulty == 2 ? atkDelay % 1 == 0 : atkDelay % 6 == 0)
                {
                    Projectile.NewProjectile(Npc.GetSource_FromAI(), Npc.Center, new Vector2(0, -4.5f).RotatedByRandom(MathHelper.ToRadians(45)), ProjectileID.SpikedSlimeSpike, 9, 0, Main.myPlayer);
                }
            }
            else
            {
                if (lunging)
                {
                    Npc.velocity.X = oldXvel / 3;
                }
                lunging = false;
                if (!jumping && AI_Timer <= 720)
                {
                    atkUseCounter++;
                    y0 = 0;

                    Vector2 jumpvel = Vector2.Zero;
                    if (player.Center.X - Npc.Center.X < 0)
                    {
                        jumpvel.X += (player.Center.X + 660 - Npc.Center.X) / 56;
                    }
                    else
                    {
                        jumpvel.X += (player.Center.X - 660 - Npc.Center.X) / 56;
                    }

                    jumpvel.Y = -14;

                    Npc.velocity = jumpvel;

                    jumping = true;
                }
                Npc.velocity.Y += 0.25f;
                if (Npc.velocity.Y > 15)
                {
                    Npc.velocity.Y = 15;
                }
            }

            if (Npc.velocity.Y < 0)
            {
                Npc.noTileCollide = true;
            }
            else
            {
                if (Main.player[Npc.target].Center.Y > Npc.Bottom.Y)
                {
                    Npc.noTileCollide = true;
                }
                else
                {
                    Npc.noTileCollide = false;
                }
            }
        }

        private void HalfHealth()
        {

            AI_Timer += 1;
            if (AI_Timer == 120)
            {
                Npc.ai[0] = NPC.NewNPC(NPC.GetBossSpawnSource(Npc.target), (int)Npc.Center.X, (int)Npc.Center.Y, ModContent.NPCType<KingSlimeShadow>(), ai0: Npc.whoAmI);
                AI_State = (float)ActionState.NormalJump;

                AI_Timer = 0;
                atkUseCounter = 0;
                atkDelay = 0;
                y0 = 0;
                jumping = false;

                Npc.dontTakeDamage = false;
            }
            Npc.velocity.X *= 0.9f;
            Npc.velocity.Y += 0.25f;
            if (Npc.velocity.Y > 15)
            {
                Npc.velocity.Y = 15;
            }
            if (Npc.velocity.Y < 0)
            {
                Npc.noTileCollide = true;
            }
            else
            {
                if (Main.player[Npc.target].Center.Y - 160 > Npc.Bottom.Y)
                {
                    Npc.noTileCollide = true;
                }
                else
                {
                    Npc.noTileCollide = false;
                }
            }
        }
    }
}