﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class AerogelBreastplate : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("Aerogel Breastplate");
            // Tooltip.SetDefault("Increases ranged damage by 12% while in the air");

        }

        public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 5; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
            if (player.velocity.Y != 0)
            {
                player.GetDamage(DamageClass.Ranged) += 0.12f;
            }
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Cloud, 25);
            recipe.AddIngredient(ItemID.GoldBar, 6);
            recipe.AddIngredient(ItemID.Gel, 18);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe4 = CreateRecipe();
            recipe4.AddIngredient(ItemID.Cloud, 25);
            recipe4.AddIngredient(ItemID.Gel, 18);
            recipe4.AddIngredient(ItemID.PlatinumBar, 6);
            recipe4.AddTile(TileID.Anvils);
            recipe4.Register();
        }
    }
}
