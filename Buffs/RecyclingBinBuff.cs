using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class RecyclingBinBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }
    }
}