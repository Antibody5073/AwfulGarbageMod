using AwfulGarbageMod.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AwfulGarbageMod.Tiles.OresBars
{
	public class FlintDirt : ModTile
	{
		public override void SetStaticDefaults() {
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 150; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
			Main.tileShine2[Type] = true; // Modifies the draw color slightly.
			Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(15, 15, 15), name);

			DustType = DustID.Wraith;
			HitSound = SoundID.Dig;

			MineResist = 2.2f;
			MinPick = 30;
		}

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
			yield return new Item(ItemID.DirtBlock);
            yield return new Item(ModContent.ItemType<Flint>());
        }


        public override bool CanPlace(int i, int j)
        {
			Tile tile = Framing.GetTileSafely(i, j);
			if (tile.TileType == TileID.Dirt)
			{
				return true;
			}
            return false;
        }
    }
}
