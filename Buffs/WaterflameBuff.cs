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
using AwfulGarbageMod.Items.Weapons.Summon;

namespace AwfulGarbageMod.Buffs
{
    public class WaterflameBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;

        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.type != NPCID.WallofFleshEye)
            {
                npc.GetGlobalNPC<WaterflameBuffNPC>().hasWaterflame = true;
            }
        }
    }

    public class WaterflameBuffNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool hasWaterflame;

        public override void ResetEffects(NPC npc)
        {
            hasWaterflame = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (hasWaterflame)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 30 * 2;
                if (damage < 10)
                {
                    damage = 10;
                }
            }
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (hasWaterflame)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.WaterCandle, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color));
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 1.2f;
                }
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Torch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color));
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 1.2f;
                }
            }
        }
        // TODO: Inconsistent with vanilla, increasing damage AFTER it is randomised, not before. Change to a different hook in the future.
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (hasWaterflame && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]) && projectile.type != ModContent.ProjectileType<CursedCandleStaffProj>())
            {
                modifiers.FinalDamage *= 1.12f;
            }
        }
    }
}