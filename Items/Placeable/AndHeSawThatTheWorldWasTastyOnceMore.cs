using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Placeable
{
	public class AndHeSawThatTheWorldWasTastyOnceMore : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.AndHeSawThatTheWorldWasTastyOnceMore>());
			Item.width = 38;
			Item.height = 24;
			Item.value = Item.buyPrice(gold:50);
		}
	}
}
