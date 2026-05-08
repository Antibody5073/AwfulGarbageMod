using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class CoinBuff : ModBuff
    {

        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.25f;
        }
    }
}