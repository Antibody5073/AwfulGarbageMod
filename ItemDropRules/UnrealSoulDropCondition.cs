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
    public class UnrealSoulDropCondition : IItemDropRuleCondition
    {
        private static LocalizedText Description;

        public UnrealSoulDropCondition()
        {
            Description ??= Language.GetOrRegister("Mods.AwfulGarbageMod.DropConditions.UnrealEssence");
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            NPC npc = info.npc;
            return DifficultyModes.Difficulty > 0
                && !NPCID.Sets.CannotDropSouls[npc.type]
                && !npc.boss
                && !npc.friendly
                && npc.lifeMax > 25
                && npc.value >= 1f;
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