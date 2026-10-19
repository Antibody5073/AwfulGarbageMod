using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Redemption;

namespace AwfulGarbageMod.Items.Accessories
{

    public class CloudRelic : ModItem
    {
        public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = 2;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().PercentArmorIgnoreSummon *= 0.8f;
            player.GetModPlayer<GlobalPlayer>().MaxPercentArmorIgnoreSummon += 8;
            player.moveSpeed += 0.05f;

        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Cloud, 30);
            recipe.AddIngredient(ItemID.RainCloud, 15);
            recipe.AddIngredient(ItemID.CopperBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Cloud, 30);
            recipe2.AddIngredient(ItemID.RainCloud, 15);
            recipe2.AddIngredient(ItemID.TinBar, 8);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
	}
}