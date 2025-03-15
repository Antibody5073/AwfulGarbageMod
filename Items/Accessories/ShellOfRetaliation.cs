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
    public class ShellOfRetaliation : ModItem
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
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(AGUtils.ScaleDamage(45, Main.LocalPlayer, DamageClass.Ranged), AGUtils.ScaleDamage(50, Main.LocalPlayer, DamageClass.Magic)));
            tooltips.Add(tooltip);
        }


        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = 6;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 5;
            player.thorns += 0.75f;
            player.GetModPlayer<GlobalPlayer>().cactusShell += 45;
            player.GetModPlayer<GlobalPlayer>().VenomOnDamanged += 150;
            player.GetModPlayer<GlobalPlayer>().HellShellSparks += 50;
            player.longInvince = true;
            player.starCloakItem = Item;
            player.GetModPlayer<GlobalPlayer>().HoneyOnDamaged += 300;
            if (hideVisual)
            {
                player.GetModPlayer<GlobalPlayer>().Bees = false;
            }
            else
            {
                player.GetModPlayer<GlobalPlayer>().Bees = true;
            }
        }


        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<ShellOfRetribution>()
                .AddIngredient<HellShell>()
                .AddIngredient(ItemID.BeeCloak)
                .AddIngredient(ItemID.CrossNecklace)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            CreateRecipe()
                .AddIngredient<ShellOfRetribution>()
                .AddIngredient<HellShell>()
                .AddIngredient(ItemID.StarVeil)
                .AddIngredient(ItemID.HoneyComb)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
	}
}