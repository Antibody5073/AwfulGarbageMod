using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class RottingLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Rotting Greaves");
            // Tooltip.SetDefault("20 reduced maximum health\n20% increased movement speed");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 3; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
            player.statLifeMax2 -= 20;
            player.moveSpeed += 0.20f; // Increase the movement speed of the player
            player.maxRunSpeed += 0.20f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.RottenChunk, 5);
            recipe.AddIngredient(ItemID.Ebonwood, 20);
            recipe.AddIngredient(ItemID.VilePowder, 10);
            recipe.AddIngredient(ItemID.CopperBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.RottenChunk, 5);
            recipe2.AddIngredient(ItemID.Ebonwood, 20);
            recipe2.AddIngredient(ItemID.VilePowder, 10);
            recipe2.AddIngredient(ItemID.TinBar, 8);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
