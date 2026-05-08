using AwfulGarbageMod.Items.Placeable.OresBars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Placeable.Furniture
{
	public class ElementalSmeltery : ModItem
	{
		public override void SetDefaults() {
			// ModContent.TileType<Tiles.Furniture.ExampleWorkbench>() retrieves the id of the tile that this item should place when used.
			// DefaultToPlaceableTile handles setting various Item values that placeable items use
			// Hover over DefaultToPlaceableTile in Visual Studio to read the documentation!
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.ElementalSmeltery>());
			Item.width = 28; // The item texture's width
			Item.height = 14; // The item texture's height
			Item.value = 150;
            Item.rare = ItemRarityID.Expert;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteForge)
                .AddIngredient<CandescentCrucible>()
                .AddIngredient<FrigidForge>()
                .AddIngredient<TrashMelter>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteOre, 30)
                .AddIngredient<CandescentCrucible>()
                .AddIngredient<FrigidForge>()
                .AddIngredient<TrashMelter>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteForge)
                .AddIngredient<CandesciteBar>(2)
                .AddIngredient<CandesciteOre>(30)
                .AddIngredient<FrigidForge>()
                .AddIngredient<TrashMelter>()
                .Register();
        }
	}
}
