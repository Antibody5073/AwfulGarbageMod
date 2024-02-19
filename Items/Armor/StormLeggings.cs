using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class StormLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Storm Greaves");
            // Tooltip.SetDefault("Increases movement speed by 15%");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = 3;
            Item.defense = 4; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.15f; // Increase the movement speed of the player
            player.maxRunSpeed += 0.15f;
            player.GetJumpState(ExtraJump.CloudInABottle).Enable();
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("AerogelLeggings").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("StormEssence").Type, 8);
            recipe.AddIngredient(ItemID.RainCloud, 30);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
