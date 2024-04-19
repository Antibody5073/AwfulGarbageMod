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
using AwfulGarbageMod.DamageClasses;
using System.Collections.Generic;
using Terraria.Localization;

namespace AwfulGarbageMod.Items.Accessories
{

    public class SigilOfSpirits : ModItem
	{
        public static LocalizedText tooltipWithoutArgs { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Necropotence"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("34 damage\n5 defense\nTaking damage causes you to release bone toothpicks\n\"Argh, fine! I'll hit you with this and turn you into a couple of cremated reliquaries!\"");
            tooltipWithoutArgs = this.GetLocalization(nameof(tooltipWithoutArgs));
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            tooltip = new TooltipLine(Mod, "tooltipWithoutArgs", tooltipWithoutArgs.Format(Math.Round(3.5f * player.GetModPlayer<GlobalPlayer>().empowermentCooldowMultiplier * 100) / 100));
            tooltips.Add(tooltip);
        }

        public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = 8;
			Item.accessory = true;
            Item.GetGlobalItem<ItemTypes>().Empowerment = true;

        }


        public override void UpdateEquip(Player player)
        {
			player.GetModPlayer<EctoplasmicEmpowermentPlayer>().hasSigil = true;
        }
    }
}