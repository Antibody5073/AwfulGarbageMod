using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class MyceliumLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Mycelium Greaves");
            // Tooltip.SetDefault("Increases jump height");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 1; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
            player.jumpSpeedBoost += 1.125f;
            player.moveSpeed += 0.25f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.GlowingMushroom, 24);
            recipe.AddIngredient(ItemID.SilverBar, 11);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.GlowingMushroom, 24);
            recipe2.AddIngredient(ItemID.TungstenBar, 11);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
