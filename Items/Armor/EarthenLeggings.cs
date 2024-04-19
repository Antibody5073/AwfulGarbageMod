using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class EarthenLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Aerogel Greaves");
            // Tooltip.SetDefault("Increases movement speed by 10%\nIncreases ranged crit chance by 5%");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(silver: 10); // How many coins the item is worth
			Item.rare = 1; // The rarity of the item
			Item.defense = 3; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.10f; // Increase the movement speed of the player
            player.maxRunSpeed += 0.10f;
            player.GetDamage(DamageClass.Magic) += 0.05f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.10f;

        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Flint>(20)
                .AddIngredient(ItemID.DirtBlock, 60)
                .AddIngredient(ItemID.ClayBlock, 35)
                .AddIngredient(ItemID.StoneBlock, 35)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
