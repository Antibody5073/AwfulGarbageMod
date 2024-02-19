﻿using StramClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
    [ExtendsFromMod("StramClasses")]
    public class CloudAssassinBreastplate : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("CloudAssassin Breastplate");
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
            player.aggro -= 250;
            player.GetCritChance(StramUtils.rogueDamage()) += 4f;
            player.GetDamage(StramUtils.rogueDamage()) += 0.04f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 60)
                .AddIngredient<StormEssence>(12)
                .AddIngredient(ItemID.Silk, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
