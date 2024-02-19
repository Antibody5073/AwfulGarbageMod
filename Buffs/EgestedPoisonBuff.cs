using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.IO;
using Microsoft.CodeAnalysis;

namespace AwfulGarbageMod.Buffs
{
    [ExtendsFromMod("StramClasses")]
    public class EgestedPoisonBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
        }
    }

    [ExtendsFromMod("StramClasses")]
    public class EgestedPoisonBuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        int timer = 0;
        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);

            if (npc.HasBuff<EgestedPoisonBuff>())
            {
                if (timer == 0)
                {
                    timer = 60;


                    NPC.HitInfo hitInfo = new NPC.HitInfo();
                    hitInfo.Crit = false;
                    hitInfo.Damage = 10;



                    for (var j = 0; j < 16; j++)
                    {
                        int dust = Dust.NewDust(npc.Center + new Vector2(240, 0).RotatedBy(MathHelper.ToRadians((360 / 16) * j)), 0, 0, DustID.Poisoned, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].scale = 1.75f;
                        Main.dust[dust].velocity *= 0;
                        Main.dust[dust].noGravity = true;
                    }


                    float distanceFromTarget = 180f;

                    // This code is required either way, used for finding a target
                    int npcsHit = 0;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc2 = Main.npc[i];

                        if (npc2.CanBeChasedBy())
                        {
                            float between = Vector2.Distance(npc2.Center, npc.Center);

                            bool inRange = between < distanceFromTarget;

                            if (inRange)
                            {
                                npcsHit++;
                                npc2.StrikeNPC(hitInfo);
                                for (var j = 0; j < 8; j++)
                                {
                                    int dust = Dust.NewDust(npc2.position, npc2.width, npc2.height, DustID.Poisoned, 0f, 0f, 0, default(Color), 1f);
                                    Main.dust[dust].scale = 0.75f + (j * 0.05f);
                                    Main.dust[dust].velocity = new Vector2(0, Main.rand.NextFloat(-2, -4)).RotatedByRandom(MathHelper.ToRadians(20));
                                }
                            }
                        }
                    }
                }
                timer--;
            }
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;


            // SummonTagDamageMultiplier scales down tag damage for some specific minion and sentry projectiles for balance purposes.
            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            
        }
        
    }
}