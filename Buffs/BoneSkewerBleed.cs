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
    public class BoneSkewerBleed : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bone Skewer Bleed"); // Buff display name
            Description.SetDefault("Losing life"); // Buff description
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<ExampleGlobalNPC>().BoneSkewerBleed += 1;
        }
    }
}