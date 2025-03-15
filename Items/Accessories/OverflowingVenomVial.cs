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

    public class OverflowingVenomVial : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Contact Lens"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("24% increased critical strike damage");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = Item.sellPrice(gold: 75);
            Item.rare = 4;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.GetModPlayer<GlobalPlayer>().OverflowingVenom < 1.15f)
            {
                player.GetModPlayer<GlobalPlayer>().OverflowingVenom = 1.15f;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PotentVenomGland>()
                .AddIngredient(ItemID.VialofVenom, 50)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}