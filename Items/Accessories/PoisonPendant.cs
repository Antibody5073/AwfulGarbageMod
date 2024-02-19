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

    public class PoisonPendant : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Poison Pendant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("2 defense\n6% increased ranged crit chance\nIncreases armor penetration by 5\nTaking damage releases bees and douses the user in honey\nDisable visibility to remove bees\nRanged weapons inflict poison");
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
            player.GetCritChance(DamageClass.Ranged) += 5;
            player.GetArmorPenetration(DamageClass.Generic) += 5;
            player.GetModPlayer<GlobalPlayer>().spiderPendant = true;
            player.GetModPlayer<GlobalPlayer>().HoneyOnDamaged += 300;
            if (hideVisual)
            {
                player.GetModPlayer<GlobalPlayer>().Bees = false;
            }
            else
            {
                player.GetModPlayer<GlobalPlayer>().Bees = true;
            }
        }



        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
			recipe.AddIngredient(Mod.Find<ModItem>("SpiderPendant").Type);
            recipe.AddIngredient(ItemID.StingerNecklace);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
	}
}