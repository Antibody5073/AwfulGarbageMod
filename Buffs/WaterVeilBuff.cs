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
    public class WaterVeilBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
            BuffID.Sets.IsATagBuff[Type] = true;

        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<WaterVeilBuffNPC>().hasDebuff = true;
        }
    }

    public class WaterVeilBuffNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool hasDebuff;

        public override void ResetEffects(NPC npc)
        {
            hasDebuff = false;
        }

        // TODO: Inconsistent with vanilla, increasing damage AFTER it is randomised, not before. Change to a different hook in the future.
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // Only player attacks should benefit from this buff, hence the NPC and trap checks.
            if (hasDebuff)
            {

                int damageBoost = (int)(projectile.damage * 0.25f);
                if (damageBoost > 5)
                {
                    damageBoost = 5;
                }

                modifiers.FlatBonusDamage += damageBoost;
            }
        }
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            int damageBoost = (int)(item.damage * 0.25f);
            if (damageBoost > 5)
            {
                damageBoost = 5;
            }

            modifiers.FlatBonusDamage += damageBoost;
        }
    }
}