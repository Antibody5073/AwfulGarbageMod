using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Consumables; 
using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.NPCs.Boss;
using AwfulGarbageMod.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AwfulGarbageMod.Tiles
{
	public class EbonwoodArtifact : ModTile
	{
        public static LocalizedText MouseItem { get; private set; }

        public override void SetStaticDefaults() {
			// Properties
			Main.tileTable[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = false;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;

            DustType = DustID.Frost;
            AdjTiles = new int[] { TileID.DemonAltar};
			MinPick = 105;

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			// Etc
			AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Table"));


            MouseItem = this.GetLocalization(nameof(MouseItem));
		}

		public override void MouseOver(int i, int j)
		{
			Main.instance.MouseText(MouseItem.Format());
			Main.player[Main.myPlayer].GetModPlayer<GlobalPlayer>().ImportantHoveredTile = "EbonwoodArtifact";
		}

        public override bool RightClick(int i, int j)
        {
			Player player = Main.LocalPlayer;
			if (player.HasItem(ModContent.ItemType<RootOfEvil>()))
			{
				SubworldSystem.Enter<EbonwoodBossSubworld>();
                return true;
			}
			else
			{
				return false;
			}
			
        }

        public override void NumDust(int x, int y, bool fail, ref int num) {
			num = fail ? 1 : 3;
		}

        public override bool CanExplode(int i, int j)
        {
			return false;
        }
    }
}
