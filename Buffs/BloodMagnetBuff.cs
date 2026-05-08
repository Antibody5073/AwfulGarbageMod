using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class BloodMagnetBuff : ModBuff
    {

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Generic) += 0.12f;
        }
    }
}