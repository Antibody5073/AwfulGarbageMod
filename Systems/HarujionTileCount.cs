using System;
using AwfulGarbageMod.Items.Placeable;
using AwfulGarbageMod.Tiles;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Systems
{
	public class HarujionTileCount : ModSystem
	{
		public int lifelessBlockCount;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
			lifelessBlockCount = tileCounts[ModContent.TileType<Tiles.LifelessBlock>()];
		}
	}
}
