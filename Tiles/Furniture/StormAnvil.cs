using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static AssGen.Assets;

namespace AwfulGarbageMod.Tiles.Furniture
{
	public class StormAnvil : ModTile
    {
        public const int FrameWidth = 18 * 3;
        public const int FrameHeight = 18 * 2;
        public override void SetStaticDefaults()
		{
			// Properties
			Main.tileTable[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileFrameImportant[Type] = true;

            TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
            DustType = DustID.Electric;

            AdjTiles = new int[] { TileID.Anvils };

            // PlacementTileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 18];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 1);

            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.DrawYOffset = 2;

            // Additional edits here, such as lava immunity, alternate placements, and subtiles
            TileObjectData.addTile(Type);

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


        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            // Since this tile does not have the hovering part on its sheet, we have to animate it ourselves
            // Therefore we register the top-left of the tile as a "special point"
            // This allows us to draw things in SpecialDraw
            if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0)
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
        }
        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {

            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
            {
                offScreen = Vector2.Zero;
            }

            // Take the tile, check if it actually exists
            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
            {
                return;
            }


            // Get the initial draw parameters
            Texture2D texture = ModContent.Request<Texture2D>("AwfulGarbageMod/Tiles/Furniture/StormAnvil_Glowmask", AssetRequestMode.ImmediateLoad).Value;

            Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

            

            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -45f);

            // Draw the main texture
            spriteBatch.Draw
            (
                texture,
                drawPos,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                0,
                texture.Size() * 0.5f,
                1,
                SpriteEffects.None,
                0f
            );
        }
    }
}