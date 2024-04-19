using AwfulGarbageMod.Global;
using StramClasses;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class EarthenShieldBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.LocalPlayer;

            tip = "Earthen Shield absorbs 20% of damage taken\nCurrent shields: " + player.GetModPlayer<EarthenShield>().shieldDurability + " / " + player.GetModPlayer<EarthenShield>().shieldMaxDurability;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (!player.GetModPlayer<GlobalPlayer>().EarthenBonusPrev)
            {
                player.DelBuff(buffIndex);
            }
        }
    }
}