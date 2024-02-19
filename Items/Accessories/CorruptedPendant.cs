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

    public class CorruptedPendant : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Corrupted Pendant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("15% increased ranged velocity\n2% increased ranged crit chance\nRanged crits have a 20% chance to inflict cursed flame");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 4;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Ranged) += 2;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.15f;
            player.GetModPlayer<GlobalPlayer>().corruptedPendant = true;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("FeatherPendant").Type);
            recipe.AddIngredient(ItemID.CorruptSeeds, 2);
            recipe.AddIngredient(ItemID.DemoniteBar, 18);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(Mod.Find<ModItem>("FeatherPendant").Type);
            recipe2.AddIngredient(ItemID.SoulofNight, 3);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
	}
}