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

namespace AwfulGarbageMod.Items.Accessories
{

    public class ViciousMushroomFlask : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Vicious Mushroom Flask"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("15% increased max movement speed");
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<VileMushroomFlask>()] = base.Type;
        }

        public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 1;
            Item.accessory = true;
            Item.value = 5000;

        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Generic) += 6;
            player.manaCost *= 0.94f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.06f;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bottle);
            recipe.AddIngredient(ItemID.ViciousMushroom, 10);
            recipe.AddIngredient(ItemID.Vertebrae, 30);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
	}
}