using AwfulGarbageMod.Global;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class ScepterCapacityBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<GlobalPlayer>().MaxScepterBoost += 1;
        }
    }
}