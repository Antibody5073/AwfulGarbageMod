using AwfulGarbageMod.Items.Placeable.OresBars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Placeable.Furniture
{
	public class RecyclingBin : ModItem
	{
		public override void SetDefaults() {
			// ModContent.TileType<Tiles.Furniture.ExampleWorkbench>() retrieves the id of the tile that this item should place when used.
			// DefaultToPlaceableTile handles setting various Item values that placeable items use
			// Hover over DefaultToPlaceableTile in Visual Studio to read the documentation!
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.RecyclingBin>());
			Item.width = 28; // The item texture's width
			Item.height = 14; // The item texture's height
			Item.value = 1500000;
			Item.rare = 1;
		}
	}
}
