using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using AwfulGarbageMod.Global;
using Microsoft.CodeAnalysis;

namespace AwfulGarbageMod.Buffs
{
    public class CorruptorCurse : ModBuff
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

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CorruptorCurseNPC>().Consumed = true;
        }
    }

    public class CorruptorCurseNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool Consumed;

        public override void ResetEffects(NPC npc)
        {
            Consumed = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (Consumed)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 25 * 2;
                if (damage < 10)
                {
                    damage = 10;
                }
            }
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (Consumed == true && item.CountsAsClass(DamageClass.Melee))
            {
                int proj = Projectile.NewProjectile(npc.GetSource_OnHurt(item), npc.Center, Main.rand.NextVector2CircularEdge(10, 10), ProjectileID.TinyEater, 15 + (int)(item.damage * 0.15f), 0, Main.myPlayer);
                Main.projectile[proj].ArmorPenetration += 50;
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (Consumed == true && projectile.type != ProjectileID.TinyEater && projectile.CountsAsClass(DamageClass.Melee))
            {
                int proj = Projectile.NewProjectile(npc.GetSource_OnHurt(projectile), npc.Center, Main.rand.NextVector2CircularEdge(10, 10), ProjectileID.TinyEater, 15 + (int)(projectile.damage * 0.1f), 0, Main.myPlayer);
                Main.projectile[proj].ArmorPenetration += 50;
            }
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Consumed == true)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.ScourgeOfTheCorruptor, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color));
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= Main.rand.NextFloat(0.5f, 1f);

                }
            }
        }
    }
}