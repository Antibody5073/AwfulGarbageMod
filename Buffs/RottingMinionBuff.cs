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
    public class RottingMinionBuff : ModBuff
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
            npc.GetGlobalNPC<RottingMinionBuffNPC>().Consumed = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statDefense -= 3;
            player.endurance = 1f - (1.1f * (1f - player.endurance));

        }
    }

    public class RottingMinionBuffNPC : GlobalNPC
    {
        // This is required to store information on entities that isn't shared between them.
        public override bool InstancePerEntity => true;

        public bool Consumed;

        public override void ResetEffects(NPC npc)
        {
            Consumed = false;
        }


        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            modifiers.Defense.Flat -= 3;
            if (Consumed)
            {
                modifiers.FinalDamage *= 1.08f;
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