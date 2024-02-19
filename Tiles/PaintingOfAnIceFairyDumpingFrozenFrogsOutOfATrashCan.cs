using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AwfulGarbageMod.Tiles
{
	public class PaintingOfAnIceFairyDumpingFrozenFrogsOutOfATrashCan : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.Paintings[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Painting6X4, 0));
            TileObjectData.addTile(Type);

            AddMapEntry(Color.LightCyan);
        }
    }
}
