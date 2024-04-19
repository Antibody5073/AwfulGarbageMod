using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Buffs
{
    public class PurityBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Consumed"); // Buff display name
            // Description.SetDefault("Reduced defense by 3, take 8% more damage"); // Buff description
            Main.debuff[Type] = true;  // Is it a debuff?
            Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
            BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
        }
    }

    public class PurityBuffNPC : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (projectile.npcProj || projectile.trap)
                return;


            // SummonTagDamageMultiplier scales down tag damage for some specific minion and sentry projectiles for balance purposes.
            if (npc.HasBuff<PurityBuff>())
            {
                // Apply a flat bonus to every hit
                modifiers.SourceDamage *= 1.07f;
            }
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<PurityBuff>() == true)
            {
                if (Main.rand.Next(6) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.PurificationPowder, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color));
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= Main.rand.NextFloat(0.5f, 1f);
                }
            }
        }
    }
}