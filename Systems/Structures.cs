using AwfulGarbageMod.Items.Consumables;
using AwfulGarbageMod.NPCs.Boss;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using StructureHelper;
using Terraria.ID;
using Terraria;
using Terraria.WorldBuilding;
using Terraria.DataStructures;
using System.IO;
using Terraria.ModLoader.IO;
using AwfulGarbageMod.Configs;

namespace AwfulGarbageMod.Systems;
public class Structures : ModSystem
{
    public static int IcePalacePosX = 0;
    public static int IcePalacePosY = 0;
    public static int HarujionPosX = 0;
    public static int HarujionPosY = 0;
    public static bool HardmodeStructures = false;
    public static bool SkyStructure = false;
    public static bool SnakeCharmerStructure = false;
    public static bool TrashStructure = false;

    public static int timer = 0;
    public override void ClearWorld()
    {
        IcePalacePosX = 0;
        IcePalacePosY = 0;
    }

    public static void GenerateIcePalace(Mod mod)
    {
        Main.NewText("Generated Ice Palace", Color.LightCyan);

        int x;
        int y;
        int SnowMiddleY = GenVars.snowTop + (int)((float)(GenVars.snowBottom - GenVars.snowTop) / 3);
        for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-04); i++)
        {
            x = WorldGen.genRand.Next(100, Main.maxTilesX);
            y = SnowMiddleY;

            Tile tile = Framing.GetTileSafely(x, y);
            if (tile.HasTile && (tile.TileType == TileID.IceBlock || tile.TileType == TileID.SnowBlock))
            {
                Generator.GenerateStructure("Assets/Structures/IcePalace", new Point16(x, y), mod);
                IcePalacePosX = x + 54;
                IcePalacePosY = y + 51;
                break;
            }
        }
    }

    public static void GenerateTrashStructure(Mod mod)
    {
        Main.NewText("Generated a small pile of trash", Color.DarkSlateGray);
        TrashStructure = true;

        int x;
        int y;
        for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-04); i++)
        {
            x = Main.spawnTileX + (WorldGen.genRand.NextBool() ? WorldGen.genRand.Next(200, 500) : WorldGen.genRand.Next(-500, -200));
            y = Main.spawnTileY - 50;

            Tile tile = Framing.GetTileSafely(x, y);
            while (!tile.HasTile)
            {
                y += 1;
                tile = Framing.GetTileSafely(x, y);
            }
            if (tile.HasTile && (TileID.Sets.Grass[tile.TileType]))
            {
                Generator.GenerateStructure("Assets/Structures/trash", new Point16(x - 6, y - 4), mod);
                break;
            }
        }
    }
    public static void GenerateSnakeCharmerStructure(Mod mod)
    {
        Main.NewText("Generated some ruins in the desert", Color.DarkOrange);
        SnakeCharmerStructure = true;

        int x;
        int y;
        for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-04); i++)
        {
            x = WorldGen.genRand.Next(200, Main.maxTilesX - 200);
            y = Main.spawnTileY - 150;

            Tile tile = Framing.GetTileSafely(x, y);
            while (!tile.HasTile)
            {
                y += 1;
                tile = Framing.GetTileSafely(x, y);
            }
            if (tile.HasTile && tile.TileType == TileID.Sand && Framing.GetTileSafely(x, y - 1).LiquidAmount == 0 && Framing.GetTileSafely(x + 10, y - 2).LiquidAmount == 0 && Framing.GetTileSafely(x - 10, y - 2).LiquidAmount == 0)
            {
                Generator.GenerateStructure("Assets/Structures/snakecharmer", new Point16(x - 8, y - 14), mod);
                break;
            }
        }
    }
    public static void GenerateEotsStructure(Mod mod)
    {
        Main.NewText("Generated the storm shrine", Color.DarkSlateBlue);
        SkyStructure = true;

        int x;
        int y;
        for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-04); i++)
        {
            x = Main.spawnTileX + WorldGen.genRand.Next(-50, 50);
            y = 50;

            Tile tile = Framing.GetTileSafely(x, y);
            Generator.GenerateStructure("Assets/Structures/eotsStructure", new Point16(x - 15, y), mod);
            break;
        }
    }

    public override void PreUpdateWorld()
    {
        if (timer > -1)
        {
            timer--;
        }
        if (timer == 0)
        {
            GenerateHardmodeStructures(Mod);
        }
        base.PreUpdateWorld();
    }
    public static void GenerateHardmodeStructures(Mod mod)
    {
        Main.NewText("Structures have been generated in the underground night and light regions", Color.Firebrick);
        HardmodeStructures = true;

        int structures = 0;

        for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 2E-03); i++)
        {
            int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
            int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY * 9 / 10);

            Tile tile = Framing.GetTileSafely(x, y);
            if (tile.HasTile && (tile.TileType == TileID.Ebonstone || tile.TileType == TileID.Ebonsand || tile.TileType == TileID.CorruptHardenedSand || tile.TileType == TileID.CorruptIce || tile.TileType == TileID.CorruptSandstone))
            {
                for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
                {
                    Chest chest = Main.chest[chestIndex];

                    if (Math.Abs(chest.x - x) > 40 && Math.Abs(chest.x - x) > 40)
                    {
                        Generator.GenerateStructure("Assets/Structures/CorruptionStructure" + Main.rand.Next(1, 5), new Point16(x, y), mod);
                        structures++;
                        break;
                    }
                }
            }
            if (structures > 7)
            {
                break;
            }
        }
        structures = 0;

        for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 2E-03); i++)
        {
            int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
            int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY * 9 / 10);

            Tile tile = Framing.GetTileSafely(x, y);
            if (tile.HasTile && (tile.TileType == TileID.Crimstone || tile.TileType == TileID.Crimsand || tile.TileType == TileID.CrimsonHardenedSand || tile.TileType == TileID.FleshIce || tile.TileType == TileID.CrimsonSandstone))
            {
                for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
                {
                    Chest chest = Main.chest[chestIndex];

                    if (Math.Abs(chest.x - x) > 40 && Math.Abs(chest.x - x) > 40)
                    {
                        Generator.GenerateStructure("Assets/Structures/CrimsonStructure" + Main.rand.Next(1, 5), new Point16(x, y), mod);
                        structures++;
                        break;
                    }
                }
            }
            if (structures > 7)
            {
                break;
            }
        }
        structures = 0;

        for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 2E-03); i++)
        {
            int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
            int y = WorldGen.genRand.Next((int)GenVars.rockLayer, Main.maxTilesY * 9 / 10);

            Tile tile = Framing.GetTileSafely(x, y);
            if (tile.HasTile && (tile.TileType == TileID.Pearlstone || tile.TileType == TileID.Pearlsand || tile.TileType == TileID.HallowHardenedSand || tile.TileType == TileID.HallowedIce || tile.TileType == TileID.HallowSandstone))
            {
                for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
                {
                    Chest chest = Main.chest[chestIndex];

                    if (Math.Abs(chest.x - x) > 40 && Math.Abs(chest.x - x) > 40)
                    {
                        Generator.GenerateStructure("Assets/Structures/HallowStructure" + Main.rand.Next(1, 5), new Point16(x, y), mod);
                        structures++;
                        break;
                    }
                }
            }
            if (structures > 7)
            {
                break;
            }
        }
    }

    public override void PostWorldGen()
    {
        if (ModContent.GetInstance<ConfigClient>().ShouldGenerateIcePalace)
        {
            GenerateIcePalace(Mod);
        }
        if (ModContent.GetInstance<ConfigClient>().ShouldGenerateSkyStructure)
        {
            GenerateEotsStructure(Mod);
        }
        if (ModContent.GetInstance<ConfigClient>().ShouldGenerateMiscStructures)
        {
            GenerateSnakeCharmerStructure(Mod);
            GenerateTrashStructure(Mod);
        }
        /*
        SnowMiddleY = GenVars.snowTop + (int)((float)(GenVars.snowBottom - GenVars.snowTop) * 2 / 3);
        for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 6E-04); i++)
        {
            x = WorldGen.genRand.Next(100, Main.maxTilesX);
            y = SnowMiddleY;

            Tile tile = Framing.GetTileSafely(x, y);
            if (tile.HasTile && (tile.TileType == TileID.IceBlock || tile.TileType == TileID.SnowBlock))
            {
                Generator.GenerateStructure("Assets/Structures/Harujion", new Point16(x, y), Mod);
                HarujionPosX = x + 64;
                HarujionPosY = y + 73;
                break;
            }
        }
        */
    }

    public override void LoadWorldData(TagCompound tag)
    {
        IcePalacePosX = tag.GetInt("IcePalacePosX");
        IcePalacePosY = tag.GetInt("IcePalacePosY");
        HarujionPosX = tag.GetInt("HarujionPosX");
        HarujionPosY = tag.GetInt("HarujionPosY");
        HardmodeStructures = tag.GetBool("HardmodeStructures");
        SkyStructure = tag.GetBool("SkyStructure");
        TrashStructure = tag.GetBool("TrashStructure");
        SnakeCharmerStructure = tag.GetBool("SnakeCharmerStructure");
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["IcePalacePosX"] = IcePalacePosX; 
        tag["IcePalacePosY"] = IcePalacePosY;
        tag["HarujionPosX"] = HarujionPosX;
        tag["HarujionPosY"] = HarujionPosY;
        tag["HardmodeStructures"] = HardmodeStructures;
        tag["SkyStructure"] = SkyStructure;
        tag["TrashStructure"] = TrashStructure;
        tag["SnakeCharmerStructure"] = SnakeCharmerStructure;
    }
}