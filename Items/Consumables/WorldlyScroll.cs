using AwfulGarbageMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using AwfulGarbageMod.NPCs.Boss;
using AwfulGarbageMod.Items.Weapons.Ranged;
using AwfulGarbageMod.Systems;
using Microsoft.Xna.Framework;
using Terraria.Map;
using Terraria.IO;
using AwfulGarbageMod.Tiles.OresBars;

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
            if (new Vector2(Structures.IcePalacePosX, Structures.IcePalacePosY) == new Vector2(0, 0))
            {
                Structures.GenerateIcePalace(Mod);
            }
            if (AGUtils.GetTileCounts(ModContent.TileType<FrigidiumOre>()) <= 0)
            {
                OreGeneration.GenerateFrigidium();
            }
            if (AGUtils.GetTileCounts(ModContent.TileType<FlintDirt>()) <= 0)
            {
                OreGeneration.GenerateFlint();
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .Register();
        }
    }
}