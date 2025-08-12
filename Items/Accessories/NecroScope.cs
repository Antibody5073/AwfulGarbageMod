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

    public class NecroScope : ModItem
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
            tooltip = new TooltipLine(Mod, "tooltipWithVar", TooltipWithVar.Format(AGUtils.ScaleDamage(20, Main.LocalPlayer, DamageClass.Summon), AGUtils.ScaleDamage(45, Main.LocalPlayer, DamageClass.Summon)));
            tooltips.Add(tooltip);
        }

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 6;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.10f;
			player.bulletDamage += 0.05f;
			player.GetCritChance(DamageClass.Ranged) += 10;
            player.scope = true;
            player.aggro -= 300;

            player.statDefense += 2;
            player.GetModPlayer<GlobalPlayer>().BoneGloveDamage += 40;
            player.GetModPlayer<GlobalPlayer>().necroPotence += 20;
            player.GetModPlayer<GlobalPlayer>().necroPotencePercent += 45;
            player.boneGloveItem = Item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ReconScope)
                .AddIngredient<NecroHand>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}