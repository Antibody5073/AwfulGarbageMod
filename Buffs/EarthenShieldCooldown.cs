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
    public class EarthenShieldCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Vein Armor Set Bonus Cooldown"); // Buff display name
            // Description.SetDefault("Can't burst blood from enemies"); // Buff description
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = (int)player.GetModPlayer<EarthenShield>().shieldCooldown;

            if (!player.GetModPlayer<GlobalPlayer>().EarthenBonusPrev)
            {
                player.DelBuff(buffIndex);
            }
        }
    }
}