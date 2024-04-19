using ExampleMod.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Placeable.OresBars
{
	public class FlintDirt : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 100;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;
		}

		public override void SetDefaults()
		{
			//Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.OresBars.FlintDirt>());
			Item.width = 12;
			Item.height = 12;
			Item.value = 50;
			Item.rare = 0;
		}
	}
}