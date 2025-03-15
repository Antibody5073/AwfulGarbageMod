using AwfulGarbageMod.Buffs;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AwfulGarbageMod.Tiles.Furniture
{
	public class RecyclingBin : ModTile
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

            TileID.Sets.HasOutlines[Type] = true;

            AdjTiles = new int[] { TileID.Furnaces };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
            TileObjectData.newTile.DrawYOffset = 2;

            TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            // Etc
            LocalizedText name = CreateMapEntryName();

            AddMapEntry(new Color(200, 200, 200), name);
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            ToggleTile(i, j);
            return true;
        }

        public override void HitWire(int i, int j)
        {
            ToggleTile(i, j);
        }

        public void ToggleTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int topX = i - tile.TileFrameX % 72 / 18;
            int topY = j - tile.TileFrameY % 54 / 18;

            short frameAdjustment = (short)(tile.TileFrameX >= 72 ? -72 : 72);

            for (int x = topX; x < topX + 4; x++)
            {
                for (int y = topY; y < topY + 3; y++)
                {
                    Main.tile[x, y].TileFrameX += frameAdjustment;

                    if (Wiring.running)
                    {
                        Wiring.SkipWire(x, y);
                    }
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, topX, topY, 4, 3);
            }
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            var tile = Main.tile[i, j];
            if (tile.TileFrameX < 72)
            {

            }
            else
            {
                player.AddBuff(ModContent.BuffType<RecyclingBinBuff>(), 10);
            }
        }
        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            var tile = Main.tile[i, j];
            frameXOffset = 0;

        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            int item = ModContent.ItemType<Items.Placeable.Furniture.RecyclingBin>();

            yield return new Item(item);
        }
    }
}