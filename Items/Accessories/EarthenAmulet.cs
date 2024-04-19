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
using rail;

namespace AwfulGarbageMod.Items.Accessories
{

    public class EarthenAmulet : ModItem
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
            Item.rare = 1;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.12f;
            player.manaCost *= 0.95f;
            player.statManaMax2 += 20;
            player.manaRegenDelayBonus -= 0.5f;
            player.manaRegenBonus -= 40;
            player.GetModPlayer<GlobalPlayer>().EarthenAmulet = true;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Flint>(6)
                .AddIngredient(ItemID.DirtBlock, 18)
                .AddIngredient(ItemID.ClayBlock, 12)
                .AddIngredient(ItemID.StoneBlock, 12)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}