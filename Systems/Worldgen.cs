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
using Terraria.Localization;

namespace AwfulGarbageMod.Systems

{
    internal class ModWorldGeneration : ModSystem
    {
        public static LocalizedText OreGenMessage { get; private set; }
        public static LocalizedText CandesciteGenMessage { get; private set; }

        public override void SetStaticDefaults()
        {
            OreGenMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(OreGenMessage)}"));
            CandesciteGenMessage = Language.GetOrRegister(Mod.GetLocalizationKey($"WorldGen.{nameof(CandesciteGenMessage)}"));
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int shiniesIndex = tasks.FindIndex(t => t.Name.Equals("Shinies"));
            if (shiniesIndex != -1)
            {
                tasks.Insert(shiniesIndex + 1, new OreGeneration("Awful Garbage Mod ores", 237.4298f));
            }
            shiniesIndex = tasks.FindIndex(t => t.Name.Equals("Underworld"));
            if (shiniesIndex != -1)
            {
                tasks.Insert(shiniesIndex + 1, new CandesciteGen("Filling the underworld with more hot rocks", 237.4298f));
            }
            int surfaceStone = tasks.FindIndex(t => t.Name.Equals("Surface Ore and Stone"));
            if (surfaceStone != -1)
            {
                tasks.Insert(surfaceStone + 1, new SurfaceStructureGen("Creating structures on the surface", 237.4298f, Mod));
            }
        }
    }
}