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
    public class ObsidianShadeShield : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Obsidian Shade Shield"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("4 defense\nIncreases defense the lower your health, up to a max of 30 defense below 30% health\nGrants immunity to knockback and fire blocks\nIncreases life regeneration");
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
            player.statDefense += 4;
            player.GetModPlayer<GlobalPlayer>().ScaledShadeShield += 15f;
            player.fireWalk = true;
            player.noKnockback = true;
        }


        public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("ScaledShadeShield").Type);
            recipe.AddIngredient(ItemID.ObsidianShield);
            recipe.AddIngredient(ItemID.BandofRegeneration);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
	}
}