using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Tiles.Furniture;

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class SatanicSymbol : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dualism Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("16% increased magic damage when not moving horizontally\n21% reduced mana cost when not moving vertically\n3% increased magic crit chance\n\"Aside from the pepperoni-pizza smell, it's perfect!\"");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 5;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance = 1f - (0.9f * (1f - player.endurance));
            player.statLifeMax2 += 50;
            player.GetModPlayer<GlobalPlayer>().bonusluck += 0.05f;
            player.longInvince = true;
        }
        
        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<DemonsLocket>()
                .AddIngredient(ItemID.CrossNecklace)
                .AddTile<ElementalSmeltery>()
                .Register();
        }
	}
}