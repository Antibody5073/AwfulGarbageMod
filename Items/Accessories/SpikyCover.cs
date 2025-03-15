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

    public class SpikyCover : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Stormy Charm"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Increases max number of minions\n12% decreased summon damage\n8% increased movement speed");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
			Item.rare = 0;
            Item.accessory = true;
        }


		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetArmorPenetration<FlailDamageClass>() += 8;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Spike, 8)
				.AddRecipeGroup(RecipeGroupID.IronBar, 2)
                .Register();
        }
    }
}