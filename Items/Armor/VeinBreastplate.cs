using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class VeinBreastplate : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("Vein Breastplate");
            // Tooltip.SetDefault("20 increased maximum health\nIncreases maximum number of minions by 1");

        }

        public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 1; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.statLifeMax2 += 20;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Vertebrae, 6);
            recipe.AddIngredient(ItemID.Shadewood, 30);
            recipe.AddIngredient(ItemID.ViciousPowder, 15);
            recipe.AddIngredient(ItemID.Silk, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
