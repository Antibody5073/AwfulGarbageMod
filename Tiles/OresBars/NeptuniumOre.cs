using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AwfulGarbageMod.Tiles.OresBars
{
	public class NeptuniumOre : ModTile
	{
		public override void SetStaticDefaults() {
			TileID.Sets.Ore[Type] = true;
			Main.tileSpelunker[Type] = true; // The tile will be affected by spelunker highlighting
			Main.tileOreFinderPriority[Type] = 290; // Metal Detector value, see https://terraria.wiki.gg/wiki/Metal_Detector
			Main.tileShine2[Type] = true; // Modifies the draw color slightly.
			Main.tileShine[Type] = 315; // How often tiny dust appear off this tile. Larger is less frequently
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(39, 64, 191), name);

			DustType = DustID.DungeonWater;
			SoundStyle sound = SoundID.Drip;
			sound.Volume = 1.75f;
			HitSound = sound;

			MineResist = 2.5f;
			MinPick = 0;
		}
	}
}
