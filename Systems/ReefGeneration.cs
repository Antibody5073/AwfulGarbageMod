using AwfulGarbageMod.Items.Consumables; 
using AwfulGarbageMod.Items.Consumables.BossSummon;
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
using Microsoft.Build.Tasks;
using Microsoft.Build.Execution;

namespace AwfulGarbageMod.Systems
{
    internal class ReefGeneration : GenPass
    {
        public ReefGeneration(string name, float weight, Mod mod) : base(name, weight) { this.mod = mod; }
        Mod mod;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = ModWorldGeneration.ReefGenMesage.Value;


            if (ModContent.GetInstance<ConfigClient>().ShouldGenerateReefs)
            {
                GenerateReefs();
            }
        }

        public static void GenerateReefs()
        {
            int snowReefs = 0;
            List<Rectangle> noGenerateReefs = new();
            for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 2E-04); i++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next((int)GenVars.rockLayer, GenVars.snowBottom);

                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && (tile.TileType == TileID.IceBlock || tile.TileType == TileID.SnowBlock))
                {
                    bool validPosition = true;
                    foreach (var item in noGenerateReefs)
                    {
                        if (x > item.X && x < item.X + item.Width &&
                            y > item.Y && y < item.Y + item.Height)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                    if (validPosition)
                    {
                        snowReefs++;
                        GenerateReef(x, y, out Rectangle noGenerateReefZone);
                        noGenerateReefs.Add(noGenerateReefZone);
                    }
                }
                if (snowReefs >= Main.maxTilesY / 400)
                {
                    break;
                }
            }

            int desertReefs = 0;
            for (int i = 0; i < (int)((Main.maxTilesX * Main.maxTilesY) * 2E-04); i++)
            {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                int y = WorldGen.genRand.Next((int)GenVars.rockLayer, GenVars.snowBottom);

                Tile tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && (tile.TileType == TileID.FossilOre || tile.TileType == TileID.Sandstone))
                {
                    bool validPosition = true;
                    foreach (var item in noGenerateReefs)
                    {
                        if (x > item.X && x < item.X + item.Width &&
                            y > item.Y && y < item.Y + item.Height)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                    if (validPosition)
                    {
                        desertReefs++;
                        GenerateReef(x, y, out Rectangle noGenerateReefZone);
                        noGenerateReefs.Add(noGenerateReefZone);
                    }
                }
                if (desertReefs >= Main.maxTilesY / 400)
                {
                    break;
                }
            }
        }
        public static void GenerateReef(int x, int y, out Rectangle noGenerateReefZone)
        {
            Point nodeOffset = WorldGen.genRand.NextVector2Circular(40, 40).ToPoint();

            int xSize = WorldGen.genRand.Next(35, 55);
            int ySize = WorldGen.genRand.Next(35, 55);

            WorldUtils.Gen(new Point(x, y), new Shapes.Circle(xSize, ySize), new Actions.SetTileKeepWall(TileID.ReefBlock));
            WorldUtils.Gen(new Point(x, y), new Shapes.Circle(xSize, ySize), new Actions.SetLiquid(0, 0));
            WorldUtils.Gen(new Point(x, y), new Shapes.Circle(xSize - 3, ySize - 3), new Actions.PlaceWall(WallID.ReefWall));
            int caveSize1 = xSize < ySize ? xSize * 3 / 5 : ySize * 3 / 5;

            xSize = WorldGen.genRand.Next(35, 55);
            ySize = WorldGen.genRand.Next(35, 55);
            WorldUtils.Gen(new Point(x, y) + nodeOffset, new Shapes.Circle(xSize, ySize), new Actions.SetTileKeepWall(TileID.ReefBlock));
            WorldUtils.Gen(new Point(x, y) + nodeOffset, new Shapes.Circle(xSize, ySize), new Actions.SetLiquid(0, 0));
            WorldUtils.Gen(new Point(x, y) + nodeOffset, new Shapes.Circle(xSize - 3, ySize - 3), new Actions.PlaceWall(WallID.ReefWall));
            int caveSize2 = xSize < ySize ? xSize * 2 / 3 : ySize * 2 / 3;

            noGenerateReefZone = new Rectangle(x + (nodeOffset.X / 2) - 160, y + (nodeOffset.Y / 2) - 160, 320, 320);

            WorldGen.TileRunner(x + WorldGen.genRand.Next(-32, 32), y + WorldGen.genRand.Next(-32, 32), 36, 8, TileID.Coralstone);
            WorldGen.TileRunner(x + WorldGen.genRand.Next(-32, 32), y + WorldGen.genRand.Next(-32, 32), 24, 16, TileID.Coralstone);
            WorldGen.TileRunner(x + WorldGen.genRand.Next(-32, 32), y + WorldGen.genRand.Next(-32, 32), 12, 24, TileID.Coralstone);
            WorldGen.TileRunner(x + WorldGen.genRand.Next(-32, 32), y + WorldGen.genRand.Next(-32, 32), 4, 96, TileID.Coralstone);
            WorldGen.TileRunner(x + WorldGen.genRand.Next(-32, 32), y + WorldGen.genRand.Next(-32, 32), 4, 96, TileID.Coralstone);
            WorldGen.TileRunner(x + nodeOffset.X + WorldGen.genRand.Next(-32, 32), y + nodeOffset.Y + WorldGen.genRand.Next(-32, 32), 36, 8, TileID.Coralstone);
            WorldGen.TileRunner(x + nodeOffset.X + WorldGen.genRand.Next(-32, 32), y + nodeOffset.Y + WorldGen.genRand.Next(-32, 32), 24, 16, TileID.Coralstone);
            WorldGen.TileRunner(x + nodeOffset.X + WorldGen.genRand.Next(-32, 32), y + nodeOffset.Y + WorldGen.genRand.Next(-32, 32), 12, 24, TileID.Coralstone);
            WorldGen.TileRunner(x + nodeOffset.X + WorldGen.genRand.Next(-32, 32), y + nodeOffset.Y + WorldGen.genRand.Next(-32, 32), 4, 96, TileID.Coralstone);
            WorldGen.TileRunner(x + nodeOffset.X + WorldGen.genRand.Next(-32, 32), y + nodeOffset.Y + WorldGen.genRand.Next(-32, 32), 4, 96, TileID.Coralstone);




            Vector2 dir = nodeOffset.ToVector2().SafeNormalize(Vector2.Zero);
            WorldGen.digTunnel(x, y, dir.X, dir.Y, 25, caveSize1, false);

            dir = nodeOffset.ToVector2().SafeNormalize(Vector2.Zero) * -1;
            WorldGen.digTunnel(x + nodeOffset.X, y + nodeOffset.Y, dir.X, dir.Y, 25, caveSize2, false);

            dir = new Vector2(0, -1).RotatedBy(WorldGen.genRand.NextFloat(-0.5f, 0.5f));
            WorldGen.digTunnel(x, y, dir.X, dir.Y, 200, 3, false);

            dir = new Vector2(0, -1).RotatedBy(WorldGen.genRand.NextFloat(-0.5f, 0.5f));
            WorldGen.digTunnel(x + nodeOffset.X, y + nodeOffset.Y, dir.X, dir.Y, 150, 3, false);

            int balconies = 0;
            for (int i = 0; i < 12; i++)
            {
                Point balconyPosition = new(WorldGen.genRand.Next(-24, 24) + (nodeOffset.X / 2), WorldGen.genRand.Next(-24, 24) + (nodeOffset.Y / 2));
                Tile tile = Framing.GetTileSafely(x + balconyPosition.X, x + balconyPosition.Y);
                if (!tile.HasTile)
                {
                    int strength = WorldGen.genRand.Next(4, 7);
                    WorldGen.TileRunner(x + balconyPosition.X, y + balconyPosition.Y, strength, 24 - (strength * 2), TileID.ReefBlock, addTile: true, speedX: 0.8f, speedY: -0.4f);
                    WorldGen.TileRunner(x + balconyPosition.X, y + balconyPosition.Y, strength, 24 - (strength * 2), TileID.ReefBlock, addTile: true, speedX: -0.8f, speedY: -0.4f);
                    if (WorldGen.genRand.NextBool())
                    {
                        WorldGen.TileRunner(x + balconyPosition.X, y + balconyPosition.Y, 8, 36, TileID.ReefBlock);
                    }
                    if (WorldGen.genRand.NextBool())
                    {
                        WorldGen.digTunnel(x + balconyPosition.X, y + balconyPosition.Y - 2, 0, -1, 1, 2, false);
                    }


                    balconies++;
                    if (balconies > 6)
                    {
                        break;
                    }

                }
            }



            for (int i = 0; i < 10000; i++)
            {
                Point coordinates = new Point(WorldGen.genRand.Next(noGenerateReefZone.X, noGenerateReefZone.X + noGenerateReefZone.Width), WorldGen.genRand.Next(noGenerateReefZone.Y + noGenerateReefZone.Height * 2 / 5, noGenerateReefZone.Y + noGenerateReefZone.Height));
                Tile tile = Framing.GetTileSafely(coordinates.X, coordinates.Y);
                Tile tileUnder = Framing.GetTileSafely(coordinates.X, coordinates.Y + 1);

                if (!tile.HasTile)
                {
                    if (tileUnder.TileType == TileID.ReefBlock || tileUnder.TileType == TileID.Coralstone)
                    {
                        WorldGen.SlopeTile(coordinates.X, coordinates.Y + 1, 0);

                        switch (WorldGen.genRand.Next(0, 3))
                        {
                            case 0:
                                WorldGen.PlaceTile(coordinates.X, coordinates.Y, TileID.Coral);
                                break;
                            case 1:
                                WorldGen.PlaceTile(coordinates.X, coordinates.Y, TileID.BeachPiles);
                                break;
                            case 2:
                                WorldGen.PlaceTile(coordinates.X, coordinates.Y, TileID.BeachPiles, style: 1);
                                break;
                        }
                    }
                    if (tile.WallType == WallID.ReefWall)
                    {
                        if (WorldGen.genRand.NextBool(2, 5))
                        {
                            WorldUtils.Gen(new Point(coordinates.X, coordinates.Y), new Shapes.Circle(1, 1), new Actions.SetLiquid(LiquidID.Water));
                        }
                    }
                }
                if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Lava && tile.WallType == WallID.ReefWall)
                {
                    tile.LiquidType = LiquidID.Water;
                }
            }

            for (int i = 0; i < 750; i++)
            {
                Point coordinates = new Point(WorldGen.genRand.Next(noGenerateReefZone.X + 30, noGenerateReefZone.X + noGenerateReefZone.Width - 30), WorldGen.genRand.Next(noGenerateReefZone.Y + 30, noGenerateReefZone.Y + noGenerateReefZone.Height - 30));
                Tile tile = Framing.GetTileSafely(coordinates.X, coordinates.Y);

                if (tile.HasTile && (tile.TileType == TileID.ReefBlock || tile.TileType == TileID.Coralstone))
                {   if (tile.WallType == WallID.ReefWall)
                    {
                        WorldGen.TileRunner(x + WorldGen.genRand.Next(-32, 32), y + WorldGen.genRand.Next(-32, 32), 3, 16, ModContent.TileType<NeptuniumOre>());

                    }
                }
            }

        }
    }
}