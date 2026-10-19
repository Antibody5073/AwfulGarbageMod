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
using StramClasses;
using System.Collections.Generic;
using Terraria.Localization;
using AwfulGarbageMod.Items.Armor;
using AwfulGarbageMod.Items.Placeable.OresBars;

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{
    [AutoloadEquip(EquipType.Shoes)]
    public class ShadowstepStalkers : ModItem
	{
        public static LocalizedText TooltipWithVar { get; private set; }

        public override void SetStaticDefaults()
        {
            TooltipWithVar = this.GetLocalization(nameof(TooltipWithVar));

            // DisplayName.SetDefault("Meat Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("2 defense\nIncreases health healed by potions the lower your health, up to a max of 30% extra health below 15% health");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(player.GetModPlayer<GlobalPlayer>().ShadowstepChance));
            tooltips.Add(tooltip);
        }

        public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 150000;
            Item.rare = 6;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 7f;
            player.moveSpeed += 0.15f;
            player.runAcceleration *= 1.25f;
            player.runSlowdown *= 1.25f;
            player.dashType = 1;
            player.waterWalk2 = true;
            player.spikedBoots = 2;
            player.noFallDmg = true;
            player.GetModPlayer<GlobalPlayer>().ShadowstepDodge = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DemonSoles>()
                .AddIngredient(ItemID.MasterNinjaGear)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            
        }
    }
}