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
using AwfulGarbageMod.Items.Weapons;

namespace AwfulGarbageMod.Items.Accessories
{

    public class BottledAmalgamationOfMushrooms : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Amalgamation of Mushrooms"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("50 increased maximum life and mana\n15% increased max movement speed and acceleration");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 4;
            Item.accessory = true;
            Item.value = 200000;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.15f;
            player.statManaMax2 += 40;
            player.statLifeMax2 += 40;
            player.GetDamage(DamageClass.Generic) += 0.04f;
            player.GetCritChance(DamageClass.Generic) += 4f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.04f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.04f;
            player.manaCost *= 0.96f;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MushroomFlask>());
            recipe.AddIngredient(ModContent.ItemType<GlowingMushroomFlask>()); 
            recipe.AddIngredient(ModContent.ItemType<VileMushroomFlask>());
            recipe.AddIngredient(ModContent.ItemType<FrosshroomFlask>());
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<MushroomFlask>());
            recipe2.AddIngredient(ModContent.ItemType<GlowingMushroomFlask>());
            recipe2.AddIngredient(ModContent.ItemType<ViciousMushroomFlask>());
            recipe2.AddIngredient(ModContent.ItemType<FrosshroomFlask>());
            recipe2.AddTile(TileID.TinkerersWorkbench);
            recipe2.Register();
        }
	}
}