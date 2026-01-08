using AwfulGarbageMod;
using AwfulGarbageMod.Configs;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.NPCs.Boss;
using AwfulGarbageMod.Systems;
using AwfulGarbageMod.Tiles.OresBars;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.IO;
using Terraria.Map;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AwfulGarbageMod.Items.Consumables
{
    //imported from my tAPI mod because I'm lazy
    public class WorldlyScroll : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Insect on a Stick"); 
            // Tooltip.SetDefault("Will attract a great forest-dwelling amphibian");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;
            Item.rare = ItemRarityID.LightPurple;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }



        public override bool? UseItem(Player player)
        {
            if (Main.netMode == NetmodeID.SinglePlayer && !SubworldSystem.AnyActive()) 
            {
                if (player.altFunctionUse == 2)
                {
                    if (new Vector2(Structures.IcePalacePosX, Structures.IcePalacePosY) == new Vector2(0, 0))
                    {
                        Main.NewText("Missing Ice Palace");
                    }
                    if (AGUtils.GetTileCounts(ModContent.TileType<CandesciteOre>()) <= 0)
                    {
                        Main.NewText("Missing Candescite");
                    }
                    if (AGUtils.GetTileCounts(ModContent.TileType<FrigidiumOre>()) <= 0)
                    {
                        Main.NewText("Missing Frigidium");
                    }
                    if (AGUtils.GetTileCounts(ModContent.TileType<FlintDirt>()) <= 0)
                    {
                        Main.NewText("Missing Flint");
                    }
                }
                else
                {
                    if (new Vector2(Structures.IcePalacePosX, Structures.IcePalacePosY) == new Vector2(0, 0))
                    {
                        Structures.GenerateIcePalace(Mod);
                    }
                    if (AGUtils.GetTileCounts(ModContent.TileType<FrigidiumOre>()) <= 0)
                    {
                        OreGeneration.GenerateFrigidium();
                    }
                    if (AGUtils.GetTileCounts(ModContent.TileType<CandesciteOre>()) <= 0)
                    {
                        OreGeneration.GenerateCandescite();
                    }
                    if (AGUtils.GetTileCounts(ModContent.TileType<FlintDirt>()) <= 0)
                    {
                        OreGeneration.GenerateFlint();
                    }
                    if (DifficultyModes.Difficulty > 0 && NPC.downedBoss2 && AGUtils.GetTileCounts(ModContent.TileType<VitalliumOre>()) <= 0)
                    {
                        //OreGeneration.GenerateVitallium();
                    }
                    if (!Structures.SkyStructure)
                    {
                        Structures.GenerateEotsStructure(Mod);
                    }
                    if (!Structures.TrashStructure)
                    {
                        Structures.GenerateTrashStructure(Mod);
                    }
                    if (!Structures.SnakeCharmerStructure)
                    {
                        Structures.GenerateSnakeCharmerStructure(Mod);
                    }
                    if (Main.hardMode && !Structures.HardmodeStructures)
                    {
                        //Structures.GenerateHardmodeStructures(Mod);
                    }
                }
        }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .Register();
        }
    }
}