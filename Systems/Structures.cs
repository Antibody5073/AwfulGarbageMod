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

    public override void PostWorldGen()
    {
        if (ModContent.GetInstance<ConfigClient>().ShouldGenerateIcePalace)
        {
            GenerateIcePalace(Mod);
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
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["IcePalacePosX"] = IcePalacePosX; 
        tag["IcePalacePosY"] = IcePalacePosY;
        tag["HarujionPosX"] = HarujionPosX;
        tag["HarujionPosY"] = HarujionPosY;
    }
}