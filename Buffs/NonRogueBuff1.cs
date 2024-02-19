using AwfulGarbageMod.Global;
using StramClasses;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    [ExtendsFromMod("StramClasses")]
    public class NonRogueBuff1 : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Magic) += 0.1f;
            player.GetDamage(DamageClass.Ranged) += 0.1f;
            player.GetDamage(DamageClass.Melee) += 0.1f;
            player.GetDamage(DamageClass.Summon) += 0.1f;
        }
    }
}