using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AwfulGarbageMod.Tiles
{
	public class AndHeSawThatTheWorldWasTastyOnceMore : ModTile
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
