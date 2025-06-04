using AwfulGarbageMod.Items.Consumables; using AwfulGarbageMod.Items.Consumables.BossSummon;
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
using Terraria.IO;
using AwfulGarbageMod.Tiles.OresBars;
using AwfulGarbageMod.Configs;

namespace AwfulGarbageMod.Systems
{
    internal class OreGeneration : GenPass
    {
        public OreGeneration(string name, float weight) : base(name, weight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = ModWorldGeneration.OreGenMessage.Value;


            //Frigidium Ore
            if (ModContent.GetInstance<ConfigClient>().ShouldGenerateFrigidiumOre)
            {
                GenerateFrigidium();
            }
            //Flint
            if (ModContent.GetInstance<ConfigClient>().ShouldGenerateFlint)
            {
                GenerateFlint();
            }
        }

        public static void GenerateFrigidium()
        {
            Main.NewText("Generated Frigidium", Color.Cyan);

            for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 2E-04); i++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next((int)GenVars.rockLayer, GenVars.snowBottom);

                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && (tile.TileType == TileID.IceBlock || tile.TileType == TileID.SnowBlock))
                {
                    WorldGen.TileRunner(x, y, WorldGen.genRand.Next(4, 12), WorldGen.genRand.Next(4, 8), ModContent.TileType<FrigidiumOre>());
                }
            }
        }
        public static void GenerateCandescite()
        {
            Main.NewText("Generated Candescite", Color.OrangeRed);

            for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 1E-03); i++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next(Main.maxTilesY * 2 / 3, Main.maxTilesY);

                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && (tile.TileType == TileID.Ash || tile.TileType == TileID.Stone))
                {
                    WorldGen.OreRunner(x, y, WorldGen.genRand.NextFloat(2, 3f), WorldGen.genRand.Next(15, 35), (ushort)ModContent.TileType<CandesciteOre>());
                }
            }

            for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 3E-05); i++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next(Main.maxTilesY * 4 / 5, Main.maxTilesY);

                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && (tile.TileType == TileID.Ash || tile.TileType == TileID.Stone))
                {
                    WorldGen.OreRunner(x, y, WorldGen.genRand.Next(5, 8), WorldGen.genRand.Next(12, 20), (ushort)ModContent.TileType<CandesciteOre>());
                }
            }
            for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 1E-05); i++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next(Main.maxTilesY * 9 / 10, Main.maxTilesY);

                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && (tile.TileType == TileID.Ash || tile.TileType == TileID.Stone))
                {
                    WorldGen.OreRunner(x, y, WorldGen.genRand.NextFloat(4, 5), WorldGen.genRand.Next(24, 40), (ushort)ModContent.TileType<CandesciteOre>());
                }
            }
        }
        public static void GenerateVitallium()
        {
            Main.NewText("Generated Vitallium", Color.Red);


            for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 5E-04); i++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next(Main.maxTilesY * 3 / 5, Main.maxTilesY);

                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && (tile.TileType == TileID.Stone))
                {
                    WorldGen.OreRunner(x, y, WorldGen.genRand.Next(3, 5), WorldGen.genRand.Next(5, 12), (ushort)ModContent.TileType<VitalliumOre>());
                }
            }
        }
        public static void GenerateFlint()
        {
            Main.NewText("Generated Flint", Color.DarkSlateGray);

            for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 7E-04); i++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next(0, (int)GenVars.rockLayerLow);

                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && (tile.TileType == TileID.Dirt))
                {
                    WorldGen.OreRunner(x, y, WorldGen.genRand.NextFloat(1, 2.5f), WorldGen.genRand.Next(15, 55), (ushort)ModContent.TileType<FlintDirt>());
                }
            }
        }
    }
    internal class CandesciteGen : GenPass
    {
        public CandesciteGen(string name, float weight) : base(name, weight) { }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {

            //Candescite
            if (ModContent.GetInstance<ConfigClient>().ShouldGenerateCandesciteOre)
            {
                OreGeneration.GenerateCandescite();
            }
        }
    }
}