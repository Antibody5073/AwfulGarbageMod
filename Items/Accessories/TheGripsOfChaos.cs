using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AwfulGarbageMod.Tiles.Furniture;

namespace AwfulGarbageMod.Items.Accessories
{

    public class TheGripsOfChaos : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mandible Gauntlet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("3 defense\n8% increased melee speed");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 500000;
            Item.rare = 4;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().SwampyGrip += 40;
            player.GetModPlayer<GlobalPlayer>().flailRetractSpeed += 0.1f;

            player.GetModPlayer<GlobalPlayer>().FieryGrip += 25;
            player.GetModPlayer<GlobalPlayer>().flailExtendSpeed += 0.1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SwampyGrip>()
                .AddIngredient<FieryGrip>()
                .AddTile<TrashMelter>()
                .Register();
        }
    }
}