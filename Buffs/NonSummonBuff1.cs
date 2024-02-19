using AwfulGarbageMod.Global;
using StramClasses;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class NonSummonBuff1 : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Magic) += 0.1f;
            player.GetDamage(DamageClass.Ranged) += 0.1f;
            player.GetDamage(DamageClass.Melee) += 0.1f;
            if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
            {
                DoStramClasses(player);
            }
        }
        [JITWhenModsEnabled("StramClasses")]
        public static void DoStramClasses(Player player)
        {
            player.rogue().critDamage += 0.05f;
            player.GetDamage(StramUtils.rogueDamage()) += 0.05f;
        }
    }
}