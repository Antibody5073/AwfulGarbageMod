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

    public class SpiderPendant : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spider Pendant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("2 defense\n6% increased ranged crit chance\nRanged weapons inflict poison");
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
            player.statDefense += 1;
            player.GetCritChance(DamageClass.Ranged) += 4;
            player.GetModPlayer<GlobalPlayer>().spiderPendant = true;
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