using AwfulGarbageMod.Global;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Placeable.OresBars
{
	public class NeptuniumOre : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;
			ItemID.Sets.SortingPriorityMaterials[Item.type] = 58;
		}

		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.OresBars.NeptuniumOre>());
			Item.width = 12;
			Item.height = 12;
			Item.value = 7500;

            Item.rare = ItemRarityID.Blue;
        }
	}
}