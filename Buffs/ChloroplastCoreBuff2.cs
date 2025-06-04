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
    public class ChloroplastCoreBuff2 : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // This allows the debuff to be inflicted on NPCs that would otherwise be immune to all debuffs.
            // Other mods may check it for different purposes.
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<ChloroplastBuffNPC2>().hasBuff = true;
        }
    }

    public class ChloroplastBuffNPC2 : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool hasBuff;

        public override void ResetEffects(NPC npc)
        {
            hasBuff = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (hasBuff)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 30 * 2;
                if (damage < 5)
                {
                    damage = 5;
                }
            }
        }
    }
}