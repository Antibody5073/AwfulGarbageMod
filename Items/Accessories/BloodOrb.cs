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

    public class BloodOrb : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Stormy Charm"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Increases max number of minions\n12% decreased summon damage\n8% increased movement speed");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = 1;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 8 / 100f;
			player.statLifeMax2 -= (int)Math.Ceiling(player.slotsMinions) * 30;
			Item.lifeRegen = (int)Math.Ceiling(player.slotsMinions);
        }

        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Glass, 15);
            recipe.AddIngredient(Mod.Find<ModItem>("CloudRelic").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("VeinJuice").Type, 24);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}
}