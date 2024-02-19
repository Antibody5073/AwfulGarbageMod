using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class ReachBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            Player.tileRangeX += 3;
            Player.tileRangeY += 3;
        }
    }

    public class ReachBuffItem : GlobalItem
    {
        public override void GrabRange(Item item, Player player, ref int grabRange)
        {
            if (player.HasBuff<ReachBuff>())
            {
                grabRange += 3 * 16;
            }
        }
    }
}