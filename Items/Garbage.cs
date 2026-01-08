using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Items;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using System.Numerics;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework.Input;
using AwfulGarbageMod.Items.Weapons.Magic;
using AwfulGarbageMod.Items.Placeable.OresBars;
using System;
using Terraria.Utilities;

namespace AwfulGarbageMod.Items
{
    public class Garbage : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("\"Doesn't seem to melt\""); // The (English) text shown below your item's name
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
            ItemID.Sets.ExtractinatorMode[Item.type] = Item.type;

        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height
            Item.rare = ItemRarityID.Gray;
            Item.maxStack = 9999; // The item's max stack value
            Item.value = 0; // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.consumable = true;
        }



        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            switch (Main.rand.Next(15))
            {
                case 0:
                    resultType = ModContent.ItemType<FrigidiumOre>();  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = Main.rand.Next(1, 2); // Add a chance to get more than one of ExampleOre from the extractinator.
                    break;
                case 1:
                    resultType = ModContent.ItemType<CandesciteOre>();  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = Main.rand.Next(1, 2); // Add a chance to get more than one of ExampleOre from the extractinator.
                    break;
                case 2:
                    resultType = ItemID.SilverOre;  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = Main.rand.Next(4, 8); // Add a chance to get more than one of ExampleOre from the extractinator.
                    break;
                case 3:
                    resultType = ItemID.TungstenOre;  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = Main.rand.Next(4, 8); // Add a chance to get more than one of ExampleOre from the extractinator.
                    break;
                case 4:
                    resultType = ItemID.GoldOre;  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = Main.rand.Next(4, 7); // Add a chance to get more than one of ExampleOre from the extractinator.
                    break;
                case 5:
                    resultType = ItemID.PlatinumOre;  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = Main.rand.Next(4, 7); // Add a chance to get more than one of ExampleOre from the extractinator.
                    break;
                case 6:
                    if (Main.rand.NextBool())
                    {
                        resultType = ItemID.Diamond;  
                    }
                    else
                    {
                        resultType = ItemID.Ruby;
                    }
                    resultStack = Main.rand.Next(1, 3);
                    break;
                case 7:

                    if (Main.rand.NextBool())
                    {
                        resultType = ItemID.Sapphire;
                    }
                    else
                    {
                        resultType = ItemID.Emerald;
                    }
                    resultStack = Main.rand.Next(1, 3); 
                    break;
                case 8:
                    if (Main.rand.NextBool())
                    {
                        resultType = ItemID.Amethyst;
                    }
                    else
                    {
                        resultType = ItemID.Topaz;
                    }
                    resultStack = Main.rand.Next(1, 3);
                    break;
                case 9:
                    resultType = ItemID.Amber;  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = 1;
                    break;
                case 10:
                    resultType = ModContent.ItemType<Flint>();  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = Main.rand.Next(1, 3); // Add a chance to get more than one of ExampleOre from the extractinator.
                    break;
                case 11:
                    resultType = ItemID.DirtBlock;  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = Main.rand.Next(4, 8); // Add a chance to get more than one of ExampleOre from the extractinator.
                    break;
                case 12:
                    resultType = ItemID.StoneBlock;  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = Main.rand.Next(4, 8); // Add a chance to get more than one of ExampleOre from the extractinator.
                    break;
                case 13:
                    resultType = ItemID.GoldCoin;  // Get this from the extractinator with a 1 in 3 chance.
                    resultStack = Main.rand.Next(1, 3); // Add a chance to get more than one of ExampleOre from the extractinator.
                    break;
                case 14:
                    if (Main.rand.NextBool())
                    {
                        resultType = ItemID.CanOfWorms;  // Get this from the extractinator with a 1 in 3 chance.
                        resultStack = Main.rand.Next(1, 3); // Add a chance to get more than one of ExampleOre from the extractinator.
                    }
                    break;
            }
        }
    }
}