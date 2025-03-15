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

    public class SpiderCharm : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spider Charm"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Increases max number of minions\n18% decreased summon damage");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) -= 22 / 100f;
            player.maxMinions += 1;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient(Mod.Find<ModItem>("SpiderLeg").Type, 8);
            recipe.AddIngredient(ItemID.Chain, 4);
            recipe.AddIngredient(ItemID.StoneBlock, 20);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}
}