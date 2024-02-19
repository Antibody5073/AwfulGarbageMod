using AwfulGarbageMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Tiles
{
	public class Harujion : ModPalmTree
	{
		// This is a blind copy-paste from Vanilla's PurityPalmTree settings.
		//TODO: This needs some explanations
		public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings {
			UseSpecialGroups = true,
			SpecialGroupMinimalHueValue = 11f / 72f,
			SpecialGroupMaximumHueValue = 0.25f,
			SpecialGroupMinimumSaturationValue = 0.88f,
			SpecialGroupMaximumSaturationValue = 1f
		};

		public override void SetStaticDefaults() {
			// Makes Example Palm Tree grow on Gold Ore
			GrowsOnTileId = new int[1] { ModContent.TileType<Tiles.LifelessBlock>() };

        }

		// This is the primary texture for the trunk. Branches and foliage use different settings.
		// The first row will be the Ocean textures, the second row will be Oasis Textures.
		public override Asset<Texture2D> GetTexture() {
			return ModContent.Request<Texture2D>("AwfulGarbageMod/Tiles/Harujion");
		}
	

		public override int SaplingGrowthType(ref int style) {
			style = 1;
			return ModContent.TileType<HarujionSapling>();
		}

        public override Asset<Texture2D> GetOasisTopTextures() {
			// Palm Trees come in an Oasis variant. The Top Textures for it:
			return ModContent.Request<Texture2D>("AwfulGarbageMod/Tiles/HarujionOasis_Tops");
		}

		public override Asset<Texture2D> GetTopTextures() {
			// Palm Trees come in a Beach variant. The Top Textures for it:
			return ModContent.Request<Texture2D>("AwfulGarbageMod/Tiles/Harujion_Tops");
		}
        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            Item.NewItem(WorldGen.GetItemSource_FromTreeShake(x, y), new Vector2(x, y) * 16, ModContent.ItemType<Items.Accessories.HarujionPetal>());
            return false;
        }

        public override int DropWood() {
			return ItemID.MasterBait;
		}
	}
}