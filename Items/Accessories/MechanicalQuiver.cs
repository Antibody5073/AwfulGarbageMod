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

    public class MechanicalQuiver : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Feather Pendant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("20% increased ranged velocity\n3% increased ranged crit chance");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 6;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.05f;
			player.arrowDamage += 0.08f;
			player.GetCritChance(DamageClass.Ranged) += 5;
            player.aggro -= 300;
			player.magicQuiver = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MagicQuiver)
                .AddIngredient(ItemID.RangerEmblem)
                .AddIngredient(ItemID.PutridScent)
                .AddIngredient(ItemID.Cog, 25)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.StalkersQuiver)
                .AddIngredient(ItemID.RangerEmblem)
                .AddIngredient(ItemID.Cog, 25)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}