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
    [AutoloadEquip(EquipType.Shield)]
    public class ObsidianMeatShield : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Obsidian Meat Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("3 defense\nIncreases health healed by potions the lower your health, up to a max of 30% extra health below 15% health\nGrants immunity to knockback and fire blocks\nIncreases life regen");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = 3;
            Item.accessory = true;
            Item.lifeRegen = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 3;
            player.GetModPlayer<GlobalPlayer>().MeatShield += 0.3f;
            player.fireWalk = true;
            player.noKnockback = true;
        }


        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("MeatShield").Type);
            recipe.AddIngredient(ItemID.ObsidianShield);
            recipe.AddIngredient(ItemID.BandofRegeneration);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
	}
}