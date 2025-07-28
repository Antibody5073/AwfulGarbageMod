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
    public class MothDefenseBuff : ModBuff
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
            npc.GetGlobalNPC<MothDefenseBuffNPC>().HasDefenseDebuff = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense -= 3;
        }
    }

    public class MothDefenseBuffNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool HasDefenseDebuff;

        public override void ResetEffects(NPC npc)
        {
            HasDefenseDebuff = false;
        }


        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (HasDefenseDebuff)
            {
                modifiers.Defense.Flat -= 16;
            }
        }
    }
}