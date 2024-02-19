using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class AerogelLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Aerogel Greaves");
            // Tooltip.SetDefault("Increases movement speed by 10%\nIncreases ranged crit chance by 5%");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 2; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.10f; // Increase the movement speed of the player
            player.maxRunSpeed += 0.10f;
            player.GetCritChance(DamageClass.Ranged) += 5f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Cloud, 20);
            recipe.AddIngredient(ItemID.GoldBar, 5);
            recipe.AddIngredient(ItemID.Gel, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe4 = CreateRecipe();
            recipe4.AddIngredient(ItemID.Cloud, 20);
            recipe4.AddIngredient(ItemID.PlatinumBar, 5);
            recipe4.AddIngredient(ItemID.Gel, 15);
            recipe4.AddTile(TileID.Anvils);
            recipe4.Register();
        }
    }
}
