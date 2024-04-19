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

namespace AwfulGarbageMod.Items.Accessories
{

    public class PumpkinRocket : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Magmastone Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("20% increased magic damage when not moving horizontally\n2% increased magic crit chance");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 3000;
            Item.rare = 3;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().PumpkinRocket = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Pumpkin, 25)
                .AddRecipeGroup(RecipeGroupID.IronBar, 8)
                .AddIngredient<SoulOfIghtImaHeadOut>(11)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}