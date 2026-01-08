using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Placeable
{
	public class CirnoFumo : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.CirnoFumo>());
			Item.width = 38;
			Item.height = 24;
			Item.value = 0;
		}
	}
}
