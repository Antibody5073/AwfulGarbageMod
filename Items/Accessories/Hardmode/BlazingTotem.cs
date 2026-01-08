using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Tiles.Furniture;
using ExampleMod.Content;

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class BlazingTotem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Necropotence"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("34 damage\n5 defense\nTaking damage causes you to release bone toothpicks\n\"Argh, fine! I'll hit you with this and turn you into a couple of cremated reliquaries!\"");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 80000;
            Item.rare = 5;
			Item.accessory = true;
            Item.defense = 7;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxTurrets += 1;
            player.GetDamage(DamageClass.Summon) += 0.1f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroups.AnyOldOneArmySentryAccessory)
                .AddIngredient(ItemID.FleshKnuckles)
                .AddIngredient<CandesciteBar>(20)
                .AddTile<CandescentCrucible>()
                .Register();
        }
    }
}