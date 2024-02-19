using AwfulGarbageMod.Global;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class AridSpeedBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed += 0.35f;
            player.runAcceleration *= 1.35f;
            player.runSlowdown *= 1.35f;
        }
    }
}