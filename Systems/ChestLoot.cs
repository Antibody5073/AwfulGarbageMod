using AwfulGarbageMod.Items.Consumables; using AwfulGarbageMod.Items.Consumables.BossSummon;
using AwfulGarbageMod.NPCs.Boss;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using AwfulGarbageMod.Items.Weapons.Melee;
using AwfulGarbageMod.Items.Accessories;

namespace AwfulGarbageMod.ModIntegration
{
    public class ChestSpawn : ModSystem
    {
        public override void PostWorldGen()
        {

            //Add cloud relic and feather pendant to sky chest loot
            int itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("CloudRelic").Type, Mod.Find<ModItem>("FeatherPendant").Type, ItemID.None };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 13 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = 1;
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            }
            //Add frost shards to ice chest loot
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("FrostShard").Type };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 11 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = Main.rand.Next(3, 6);
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            }

            //Add Frozen Petals and Bag of Frozen Knives to ice chest loot
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("FrozenPetals").Type, Mod.Find<ModItem>("BagOfFrozenKnives").Type, ItemID.None };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 11 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = 1;
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            }

            //Add Cirno painting to ice chest loot
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("PaintingOfAnIceFairyDumpingFrozenFrogsOutOfATrashCan").Type };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 11 * 36)
                {
                    if (Main.rand.NextBool(252))
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.None)
                            {
                                chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                                chest.item[inventoryIndex].stack = 1;
                                itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                                break;
                            }
                        }
                    }
                }
            }

            //Add Bag of Poisoned Knives to jungle chest loot
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("BagOfPoisonedKnives").Type, ItemID.None, ItemID.None };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 8 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = 1;
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            }

            //Add surface chest loot
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { ModContent.ItemType<FortifyingLink>(), ModContent.ItemType<SunflowerSeed>(), ItemID.None };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 0 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = 1;
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            } 
            //Add Insect on a stick to surface chest loot
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("InsectOnAStick").Type };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 0 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = 1;
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            }

            //Add Petrified Rose and Blood Bolt to underground chest loot
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("PetrifiedRose").Type, Mod.Find<ModItem>("BloodBolt").Type, ItemID.None, ItemID.None };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 1 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = 1;
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            }

            //Add dungeon chest loot #1
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("BloomingMarrow").Type, Mod.Find<ModItem>("BonePendant").Type, Mod.Find<ModItem>("BoneBadge").Type, Mod.Find<ModItem>("FractureKnife").Type };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 2 * 36)
                {
                    if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
                    {
                        if (itemsToPlaceInChests4[itemNum] == Mod.Find<ModItem>("FractureKnife").Type)
                        {
                            for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                            {
                                if (chest.item[inventoryIndex].type == ItemID.None)
                                {
                                    chest.item[inventoryIndex].SetDefaults(Mod.Find<ModItem>("FractureKnife2").Type);
                                    chest.item[inventoryIndex].stack = 1;
                                    break;
                                }
                            }
                        }
                    }
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = 1;
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            }

            //Add dungeon chest loot #2
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("SkullSigil").Type, Mod.Find<ModItem>("IronFist").Type, Mod.Find<ModItem>("SpikyThread").Type, Mod.Find<ModItem>("CursedCandleStaff").Type };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 2 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = 1;
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            }

            //Add shadow chest loot
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("UnholyStaff").Type, Mod.Find<ModItem>("Sunblades").Type, Mod.Find<ModItem>("FlareScepter").Type, ModContent.ItemType<ShadeVortex>(), ItemID.None, ItemID.None};

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 4 * 36)
                {
                    if (ModLoader.TryGetMod("StramClasses", out Mod stramClasses))
                    {
                        if (itemsToPlaceInChests4[itemNum] == Mod.Find<ModItem>("Sunblades").Type)
                        {
                            for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                            {
                                if (chest.item[inventoryIndex].type == ItemID.None)
                                {
                                    chest.item[inventoryIndex].SetDefaults(Mod.Find<ModItem>("Sunblades2").Type);
                                    chest.item[inventoryIndex].stack = 1;
                                    break;
                                }
                            }
                        }
                    }
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = 1;
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            }

            //Add Cactus Shell to desert chest loot
            itemNum = 0;

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                int[] itemsToPlaceInChests4 = { Mod.Find<ModItem>("CactusShell").Type, ItemID.None };

                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers2 && Main.tile[chest.x, chest.y].TileFrameX == 10 * 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None)
                        {
                            chest.item[inventoryIndex].SetDefaults(itemsToPlaceInChests4[itemNum]);
                            chest.item[inventoryIndex].stack = 1;
                            itemNum = (itemNum + 1) % itemsToPlaceInChests4.Length;
                            break;
                        }
                    }
                }
            }
        }
    }
}