using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
using System.Collections.Generic;
using Terraria.Localization;

namespace AwfulGarbageMod.Items.Accessories
{
    public class CurseOfCandescence : ModItem
	{

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meat Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("2 defense\nIncreases health healed by potions the lower your health, up to a max of 30% extra health below 15% health");
        }
        public override void SetDefaults()
		{
            Item.width = 20;
            Item.height = 20;
            Item.value = 6000;
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().MinionSentrySwap = true;
        }
	}
}