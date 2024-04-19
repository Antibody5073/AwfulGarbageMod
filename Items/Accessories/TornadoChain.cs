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
using AwfulGarbageMod.DamageClasses;

namespace AwfulGarbageMod.Items.Accessories
{

    public class TornadoChain : ModItem
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
            Item.value = 25000;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }


		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<GlobalPlayer>().flailSpinSpd += 0.25f;
            player.GetDamage<FlailDamageClass>() -= 0.10f;
            player.GetModPlayer<GlobalPlayer>().flailRange -= 0.10f;

        }

        public override void AddRecipes()
		{
            CreateRecipe()
               .AddRecipeGroup(RecipeGroupID.IronBar, 5)
               .AddIngredient<StormEssence>(7)
               .AddIngredient(ItemID.Chain, 15)
               .AddTile(TileID.Anvils)
               .Register();
        }
	}
}