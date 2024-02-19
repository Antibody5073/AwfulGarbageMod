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

    public class BioluminescentCharm : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mushroom Charm"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("8% increased movement speed\nMelee crits deal 10 more damage");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 1;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed *= 1.08f;
            player.GetModPlayer<GlobalPlayer>().FlatMeleeCrit += 10;

        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SilverBar, 8);
            recipe.AddIngredient(ItemID.GlowingMushroom, 40);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.TungstenBar, 8);
            recipe2.AddIngredient(ItemID.GlowingMushroom, 40);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
	}
}