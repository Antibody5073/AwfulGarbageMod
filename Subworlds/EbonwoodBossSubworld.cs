using AwfulGarbageMod.Configs;
using AwfulGarbageMod.NPCs.Boss;
using Microsoft.Xna.Framework;
using StructureHelper.API;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace AwfulGarbageMod.Subworlds
{
    public class EbonwoodBossSubworld : Subworld
    {
        public override int Width => 200;
        public override int Height => 200;
        public override bool ShouldSave => false;
        
        public override List<GenPass> Tasks => new List<GenPass>()
        {
        new ExampleGenPass("Generating terrain", 1, Mod)
        };
        public override bool ChangeAudio()
        {
            if (NPC.CountNPCS(ModContent.NPCType<Blightdrasil>()) > 0)
            {
                Main.newMusic = MusicLoader.GetMusicSlot(Mod, "Assets/Music/EvilTreeTheme");
            }
            else
            {
                Main.newMusic = MusicID.OtherworldlyEerie;
            }
            return true;
        }
        public override void OnLoad()
        {
            Main.dayTime = true;
            Main.time = 27000;
            SubworldSystem.hideUnderworld = true;
        }
        public override void Update()
        {
            Main.time = 27000;
        }
       
        public override bool GetLight(Tile tile, int x, int y, ref FastRandom rand, ref Vector3 color)
        {
            if (!tile.HasTile)
            {
                color.X = 1f;
                color.Y = 1f;
                color.Z = 1f; // The result of integer division is rounded down, so one of the numbers must be a float
            }
            return false;
        }
    }
    public class ExampleGenPass : GenPass
    {
        //TODO: remove this once tML changes generation passes
        public ExampleGenPass(string name, float weight, Mod mod) : base(name, weight) { this.mod = mod; }
        Mod mod;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating terrain"; // Sets the text displayed for this pass
            Main.worldSurface = Main.maxTilesY - 42; // Hides the underground layer just out of bounds
            Main.rockLayer = Main.maxTilesY; // Hides the cavern layer way out of bounds
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
                    Tile tile = Main.tile[i, j];
                    tile.HasTile = true;
                    if (i <= Main.maxTilesX / 2)
                    {
                        tile.TileType = TileID.Ebonstone;
                    }
                    else
                    {
                        tile.TileType = TileID.Crimstone;
                    }
                }
            }
            Main.spawnTileY -= 20;
            Point point = new Point(Main.maxTilesX / 2, Main.maxTilesY / 2);

            WorldUtils.Gen(point, new Shapes.Circle(52, 52), new Actions.SetTile(TileID.CorruptGrass));
            WorldUtils.Gen(point, new Shapes.Circle(50, 50), new Actions.ClearTile());

            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
                    Tile tile = Main.tile[i, j];
                    if (tile.HasTile && tile.TileType == TileID.CorruptGrass)
                    {
                        if (Main.tile[i + 1, j].HasTile && Main.tile[i, j + 1].HasTile && !Main.tile[i - 1, j].HasTile && !Main.tile[i, j - 1].HasTile)
                        {
                            tile.Slope = SlopeType.SlopeDownRight;
                        }
                        else if(!Main.tile[i + 1, j].HasTile && !Main.tile[i, j + 1].HasTile && Main.tile[i - 1, j].HasTile && Main.tile[i, j - 1].HasTile)
                        {
                            tile.Slope = SlopeType.SlopeUpLeft;
                        }
                        else if(!Main.tile[i + 1, j].HasTile && Main.tile[i, j + 1].HasTile && Main.tile[i - 1, j].HasTile && !Main.tile[i, j - 1].HasTile)
                        {
                            tile.Slope = SlopeType.SlopeDownLeft;
                        }
                        else if(Main.tile[i + 1, j].HasTile && !Main.tile[i, j + 1].HasTile && !Main.tile[i - 1, j].HasTile && Main.tile[i, j - 1].HasTile)
                        {
                            tile.Slope = SlopeType.SlopeUpRight;
                        }
                        else
                        {
                            tile.Slope = SlopeType.Solid;
                        }
                        if (i > Main.maxTilesX / 2)
                        {
                            tile.TileType = TileID.CrimsonGrass;
                        }
                    }
                }
            }

            Generator.GenerateStructure("Assets/Structures/EbonPlatform", new Point16(point.X - 10, point.Y + 11), mod);
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    if (j >= Main.maxTilesY / 2 + 11)
                    {
                        progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
                        Tile tile = Main.tile[i, j];
                        tile.LiquidType = LiquidID.Water;
                        tile.LiquidAmount = 255;

                    }
                }
            }
        }
    }
}