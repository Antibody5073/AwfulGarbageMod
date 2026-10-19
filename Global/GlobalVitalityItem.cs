using AwfulGarbageMod;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Items.Armor;
using AwfulGarbageMod.Systems;
using Infernus.Items.Weapon.HardMode.Accessories;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Steamworks;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using VitalityMod.Content.Items.Consumables.Summons;
using static Humanizer.In;

namespace AwfulGarbageMod.Global
{
    [ExtendsFromMod("VitalityMod")]
    public class RemovedBossSummonFunctionality : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ModContent.ItemType<MeatyMushroom>() || entity.type == ModContent.ItemType<AnarchyCrystal>();
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (DifficultyModes.Difficulty > 0)
            {
                Player player = Main.LocalPlayer;
                TooltipLine tooltip = new TooltipLine(Mod, "AGEffect", "[c/B841DC:Unreal Mode: Unusable. Combine the Anarchy Crystal and the Meaty Mushroom items to fight both bosses at the same time]");
                tooltips.Add(tooltip);
            }
        }
        public override bool CanUseItem(Item item, Player player)
        {
            return DifficultyModes.Difficulty == 0;
        }
    }
}