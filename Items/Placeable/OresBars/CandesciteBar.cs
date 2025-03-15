using AwfulGarbageMod.Tiles.Furniture;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Placeable.OresBars
{
    public class CandesciteBar : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 59; // Influences the inventory sort order. 59 is PlatinumBar, higher is more valuable.
		}

		public override void SetDefaults() {
			// ModContent.TileType returns the ID of the tile that this item should place when used. ModContent.TileType<T>() method returns an integer ID of the tile provided to it through its generic type argument (the type in angle brackets)
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.OresBars.CandesciteBar>());
			Item.width = 20;
			Item.height = 20;
			Item.value = 100000;
            Item.rare = 4;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<CandesciteOre>(4)
                .AddTile<CandescentCrucible>()
                .Register();
		}
	}
}
