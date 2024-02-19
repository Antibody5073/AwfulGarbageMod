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

namespace AwfulGarbageMod.Buffs
{
    public class BloodChainBuff : ModBuff
    {
        public static readonly int TagDamage = 0;

        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    public class BloodChainBuffNPC : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
                return;


            // SummonTagDamageMultiplier scales down tag damage for some specific minion and sentry projectiles for balance purposes.
            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<BloodChainBuff>())
            {
                NPC.HitInfo hitInfo = new NPC.HitInfo();
                hitInfo.Crit = false;
                hitInfo.Damage = projectile.damage / 2;

                for (int i = 0; i < npc.buffType.Length; i++)
                {
                    int type = npc.buffType[i];
                    if (type == ModContent.BuffType<BloodChainBuff>())
                    {
                        npc.buffTime[i] = 0;
                    }

                }

                for (var j = 0; j < 16; j++)
                {
                    int dust = Dust.NewDust(npc.Center + new Vector2(240, 0).RotatedBy(MathHelper.ToRadians((360 / 16) * j)), 0, 0, DustID.Blood, 0f, 0f, 0, default(Color), 1f);
                    Main.dust[dust].scale = 1.75f;
                    Main.dust[dust].velocity *= 0;
                    Main.dust[dust].noGravity = true;
                }


                float distanceFromTarget = 240f;

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
                            for (var j = 0; j < 15; j++)
                            {
                                int dust = Dust.NewDust(npc2.position, npc2.width, npc2.height, DustID.Blood, 0f, 0f, 0, default(Color), 1f);
                                Main.dust[dust].scale = 1f + (j * 0.1f);
                                Main.dust[dust].velocity = (npc.Center - npc2.Center) / (2 * (j + 1));
                                Main.dust[dust].noGravity = true;
                            }
                        }
                    }
                    if (npcsHit >= 10)
                    {
                        break;
                    }
                }
            }
        }
        
    }
}