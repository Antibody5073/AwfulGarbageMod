using AwfulGarbageMod.Global;
using AwfulGarbageMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Buffs
{
    public class FrigidArmorBuff1 : ModBuff
    {
        public static LocalizedText DmgResAmt { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slimeballs"); // Buff display name
            // Description.SetDefault("Orbiting slimeballs");
            Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
            Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;

            DmgResAmt = this.GetLocalization(nameof(DmgResAmt));
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.player[Main.myPlayer];
            tip = DmgResAmt.Format(8 * player.ownedProjectileCounts[ModContent.ProjectileType<FrigidiumArmorProj>()]);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // If the minions exist reset the buff time, otherwise remove the buff from the player
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FrigidiumArmorProj>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
                player.endurance = 1f - ((1 - (0.08f * player.ownedProjectileCounts[ModContent.ProjectileType<FrigidiumArmorProj>()])) * (1f - player.endurance));
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}