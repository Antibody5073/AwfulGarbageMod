using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Global;
using System.Collections.Generic;
using Terraria.Localization;

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{
    [AutoloadEquip(EquipType.Shield)]
    public class ShieldOfTheFallen : ModItem
	{
        public static LocalizedText TooltipWithVar { get; private set; }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meat Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("2 defense\nIncreases health healed by potions the lower your health, up to a max of 30% extra health below 15% health");
            TooltipWithVar = this.GetLocalization(nameof(TooltipWithVar));

        }

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
            Item.defense += 7;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.maxTurrets += 1;
            player.GetDamage(DamageClass.Generic) += 0.11f;
        }


        public override void AddRecipes()
		{
            /*
            CreateRecipe()
                .AddIngredient<BlazingTotem>()
                .AddIngredient(ItemID.AvengerEmblem)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();*/
        }
	}
}