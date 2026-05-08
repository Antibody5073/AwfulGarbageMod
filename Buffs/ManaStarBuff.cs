using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class ManaStarBuff : ModBuff
    {

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetCritChance(DamageClass.Generic) += 10;
            player.manaCost *= 0.95f;
        }
    }
}