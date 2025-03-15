using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class ExtensionBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<GlobalPlayer>().flailRange += 0.1f;
        }
    }
}