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

    public class MyceliumGauntlet : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mycelium Gauntlet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("2 defense\n16% increased melee speed\n6% increased movement speed\nMelee crits deal 8 more damage");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 3;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.16f;
            player.statDefense += 1;
            player.GetModPlayer<GlobalPlayer>().FlatMeleeCrit += 8;
            player.moveSpeed *= 1.06f;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FeralClaws);
            recipe.AddIngredient(Mod.Find<ModItem>("MandibleGlove").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("BioluminescentCharm").Type);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
	}
}