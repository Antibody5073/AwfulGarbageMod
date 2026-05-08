using AwfulGarbageMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Placeable
{
	public class EbonwoodArtifact : ModItem
	{
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.EbonwoodArtifact>());
			Item.width = 38;
			Item.height = 24;
			Item.value = 0;
			Item.rare = ItemRarityID.LightPurple;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.PixieDust, 25)
				.AddIngredient(ItemID.SoulofLight, 15)
				.AddIngredient(ItemID.HallowedBar, 10)
				.AddTile<ElementalSmeltery>()
				.Register();
		}
	}
}
