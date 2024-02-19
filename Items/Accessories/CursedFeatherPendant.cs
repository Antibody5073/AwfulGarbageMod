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

    public class CursedFeatherPendant : ModItem
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
            Item.rare = 6;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetCritChance(DamageClass.Ranged) += 6;
            player.GetDamage(DamageClass.Ranged) *= 1.06f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.06f;
            player.GetModPlayer<GlobalPlayer>().corruptedPendant = true;
            player.GetModPlayer<GlobalPlayer>().crimsonPendant = true;
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("CorruptedPendant").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("CrimsonPendant").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("BonePendant").Type);
            recipe.AddIngredient(ItemID.SoulofLight, 6);
            recipe.AddIngredient(ItemID.SoulofNight, 6);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
          }
	}
}