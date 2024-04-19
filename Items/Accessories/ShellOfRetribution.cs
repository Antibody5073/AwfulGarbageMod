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
using System.Collections.Generic;
using Terraria.Localization;

namespace AwfulGarbageMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class ShellOfRetribution : ModItem
	{
        public static LocalizedText TooltipWithVar { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meat Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("2 defense\nIncreases health healed by potions the lower your health, up to a max of 30% extra health below 15% health");
            TooltipWithVar = this.GetLocalization(nameof(TooltipWithVar));

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(AGUtils.ScaleDamage(35, Main.LocalPlayer, DamageClass.Ranged)));
            tooltips.Add(tooltip);
        }


        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 3;
            player.thorns += 0.5f;
            player.GetModPlayer<GlobalPlayer>().cactusShell += 35;
            player.GetModPlayer<GlobalPlayer>().VenomOnDamanged += 120;
        }


        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("CactusShell").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("VenomGland").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("ThornyShackle").Type);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
	}
}