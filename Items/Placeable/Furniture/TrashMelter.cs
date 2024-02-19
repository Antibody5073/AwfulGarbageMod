using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Placeable.Furniture
{
	public class TrashMelter : ModItem
	{
		public override void SetDefaults() {
			// ModContent.TileType<Tiles.Furniture.ExampleWorkbench>() retrieves the id of the tile that this item should place when used.
			// DefaultToPlaceableTile handles setting various Item values that placeable items use
			// Hover over DefaultToPlaceableTile in Visual Studio to read the documentation!
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.TrashMelter>());
			Item.width = 28; // The item texture's width
			Item.height = 14; // The item texture's height
			Item.value = 150;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.Furnace)
				.AddIngredient<Garbage>(30)
                .AddRecipeGroup(RecipeGroupID.Wood, 30)
                .AddRecipeGroup(RecipeGroupID.IronBar, 12)
				.AddIngredient(ItemID.BeeWax, 8)
                .AddTile(TileID.Anvils)
				.Register();
		}
	}
}
