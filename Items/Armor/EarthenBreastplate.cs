using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class EarthenBreastplate : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("Aerogel Breastplate");
            // Tooltip.SetDefault("Increases ranged damage by 12% while in the air");

        }

        public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(silver: 10); // How many coins the item is worth
			Item.rare = 1; // The rarity of the item
			Item.defense = 4; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
            player.statManaMax2 += 40;
            player.GetDamage(DamageClass.Ranged) += 0.06f;
            player.GetCritChance(DamageClass.Ranged) += 6f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Flint>(30)
                .AddIngredient(ItemID.DirtBlock, 75)
                .AddIngredient(ItemID.ClayBlock, 45)
                .AddIngredient(ItemID.StoneBlock, 45)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
