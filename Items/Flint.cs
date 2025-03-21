using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Items;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using AwfulGarbageMod.Tiles.OresBars;

namespace AwfulGarbageMod.Items
{
    public class Flint : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("\"Doesn't seem to melt\""); // The (English) text shown below your item's name
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
        }

        public override void SetDefaults()
        {
            Item.rare = 0;
            Item.width = 14;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(silver: 5); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.


            Item.useStyle = 1;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.useTurn = true;
            //Item.consumable = true;
            //Item.createTile = ModContent.TileType<FlintDirt>();
        }
    }
}