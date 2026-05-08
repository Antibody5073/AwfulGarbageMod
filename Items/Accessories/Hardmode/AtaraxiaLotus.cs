using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class AtaraxiaLotus : ModItem
	{

        public static LocalizedText TooltipWithVar { get; private set; }
        public static LocalizedText TooltipEffect2 { get; private set; }
        public static LocalizedText TooltipEffect3 { get; private set; }
        public static LocalizedText TooltipEffect4 { get; private set; }

        public override void SetStaticDefaults()
        {
            TooltipWithVar = this.GetLocalization(nameof(TooltipWithVar));
            TooltipEffect2 = this.GetLocalization(nameof(TooltipEffect2));
            TooltipEffect3 = this.GetLocalization(nameof(TooltipEffect3));
            TooltipEffect4 = this.GetLocalization(nameof(TooltipEffect4));

            // DisplayName.SetDefault("Meat Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("2 defense\nIncreases health healed by potions the lower your health, up to a max of 30% extra health below 15% health");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            int boost = player.GetModPlayer<GlobalPlayer>().AtaraxiaBoost;
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(boost * 4, boost * 5, boost * 3, boost * 0.5f, boost * 40));
            tooltips.Add(tooltip);

            if (boost >= 3)
            {
                tooltip = new TooltipLine(Mod, "tooltipEffect2", TooltipEffect2.Format());
                tooltips.Add(tooltip);
            }
            if (boost >= 5)
            {
                tooltip = new TooltipLine(Mod, "tooltipEffect3", TooltipEffect3.Format());
                tooltips.Add(tooltip);
            }
            if (boost >= 6)
            {
                tooltip = new TooltipLine(Mod, "tooltipEffect4", TooltipEffect4.Format());
                tooltips.Add(tooltip);
            }
        }

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 300000;
            Item.rare = ItemRarityID.Quest;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().Ataraxia = true;
        }
        public override void AddRecipes()
        {

        }
    }
}