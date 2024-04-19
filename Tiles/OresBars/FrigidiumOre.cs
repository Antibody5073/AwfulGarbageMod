using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AwfulGarbageMod.Tiles.OresBars
{
	public class FrigidiumOre : ModTile
	{
		public override void SetStaticDefaults() {
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 280; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
			Main.tileShine2[Type] = true; // Modifies the draw color slightly.
			Main.tileShine[Type] = 975; // How often tiny dust appear off this tile. Larger is less frequently
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;

            Main.tileMerge[Type][TileID.IceBlock] = true;
            Main.tileMerge[Type][TileID.SnowBlock] = true;

            LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(152, 171, 198), name);

			DustType = DustID.Ice;
			HitSound = SoundID.Tink;

			MineResist = 3f;
			MinPick = 42;
		}

	}
}
