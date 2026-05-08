using AwfulGarbageMod;
using AwfulGarbageMod.Buffs;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories
{

    public class BloodMagnet : ModItem
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
            Item.value = 50000;
            Item.rare = 3;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().LifeMagnet = true;
            player.GetModPlayer<GlobalPlayer>().Proboscis = true;
            player.GetModPlayer<GlobalPlayer>().BloodMagnet = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LifeMagnet>()
                .AddIngredient<Proboscis>()
                .AddIngredient<VeinJuice>(10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}