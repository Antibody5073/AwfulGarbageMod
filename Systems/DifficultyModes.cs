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
using AwfulGarbageMod.Configs;

namespace AwfulGarbageMod.Systems;
public class DifficultyModes : ModSystem
{
    public static int Difficulty = 0;


    public override void LoadWorldData(TagCompound tag)
    {
        Difficulty = tag.GetInt("Difficulty");
    }

    public override void SaveWorldData(TagCompound tag)
    {
        tag["Difficulty"] = Difficulty;
    }
}