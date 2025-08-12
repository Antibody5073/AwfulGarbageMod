using AwfulGarbageMod;
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

namespace AwfulGarbageMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class NecroAegis : ModItem
    {
        public static LocalizedText TooltipWithVar { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meat Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("2 defense\nIncreases health healed by potions the lower your health, up to a max of 30% extra health below 15% health");
            TooltipWithVar = this.GetLocalization(nameof(TooltipWithVar));

        }
        public int counter;

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player player = Main.LocalPlayer;
            TooltipLine tooltip;
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(AGUtils.ScaleDamage(25, Main.LocalPlayer, DamageClass.Summon), AGUtils.ScaleDamage(50, Main.LocalPlayer, DamageClass.Summon)));
            tooltips.Add(tooltip);
        }

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = ItemRarityID.Purple;
            Item.expert = true;
            Item.accessory = true;
            Item.lifeRegen = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 8;
            player.GetModPlayer<GlobalPlayer>().MeatShield += 0.3f;
            player.GetModPlayer<GlobalPlayer>().ScaledShadeShield += 15f;
            player.GetModPlayer<GlobalPlayer>().necroPotence += 25;
            player.GetModPlayer<GlobalPlayer>().necroPotencePercent += 50;
            player.buffImmune[46] = true;
            player.buffImmune[33] = true;
            player.buffImmune[36] = true;
            player.buffImmune[30] = true;
            player.buffImmune[20] = true;
            player.buffImmune[32] = true;
            player.buffImmune[31] = true;
            player.buffImmune[35] = true;
            player.buffImmune[23] = true;
            player.buffImmune[22] = true;
            player.buffImmune[156] = true;
            player.fireWalk = true;
            player.noKnockback = true;
        }


        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.AnkhCharm);
            recipe.AddIngredient(Mod.Find<ModItem>("ObsidianMeatShield").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("ScaledShadeShield").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("Necropotence").Type);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.AnkhCharm);
            recipe2.AddIngredient(Mod.Find<ModItem>("MeatShield").Type);
            recipe2.AddIngredient(Mod.Find<ModItem>("ObsidianShadeShield").Type);
            recipe2.AddIngredient(Mod.Find<ModItem>("Necropotence").Type);
            recipe2.AddTile(TileID.TinkerersWorkbench);
            recipe2.Register();
            Recipe recipe3 = CreateRecipe();
            recipe3.AddIngredient(ItemID.AnkhShield);
            recipe3.AddIngredient(Mod.Find<ModItem>("MeatShield").Type);
            recipe3.AddIngredient(Mod.Find<ModItem>("ScaledShadeShield").Type);
            recipe3.AddIngredient(ItemID.BandofRegeneration);
            recipe3.AddIngredient(Mod.Find<ModItem>("Necropotence").Type);
            recipe3.AddTile(TileID.TinkerersWorkbench);
            recipe3.Register();
        }
	}
}