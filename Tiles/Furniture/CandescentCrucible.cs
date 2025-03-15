using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AwfulGarbageMod.Tiles.Furniture
{
	public class CandescentCrucible : ModTile
	{
		public override void SetStaticDefaults()
		{
			// Properties
			Main.tileTable[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.

			AdjTiles = new int[] { TileID.Hellforge, TileID.Furnaces };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            // Etc
            LocalizedText name = CreateMapEntryName();

            AddMapEntry(new Color(200, 200, 200), name);
		}
        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            
            frameYOffset = Main.tileFrame[Type] * 38;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            
			// Spend 6 ticks on each of 12 frames, looping
			frameCounter++;
			if (frameCounter >= 6) {
				frameCounter = 0;
				if (++frame >= 8) {
					frame = 0;
				}
			}

        }
    }
}