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
    }
}