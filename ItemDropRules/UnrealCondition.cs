using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Items.Armor;
using Microsoft.Xna.Framework;
using Steamworks;
using static Humanizer.In;
using AwfulGarbageMod.Buffs;
using Terraria.GameInput;
using AwfulGarbageMod.Systems;
using AwfulGarbageMod.Tiles;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace AwfulGarbageMod.ItemDropRules
{
    public class UnrealCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public UnrealCondition()
        {
            Description ??= Language.GetOrRegister("Mods.AwfulGarbageMod.DropConditions.Unreal");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            NPC npc = info.npc;
            return DifficultyModes.Difficulty > 0;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Description.Value;
        }
    }
}