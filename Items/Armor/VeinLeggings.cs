using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class VeinLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Vein Greaves");
            // Tooltip.SetDefault("20 increased maximum health\n15% increased whip range");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 0; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
            player.statLifeMax2 += 20;
            player.GetDamage(DamageClass.Summon).Flat += 1;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Vertebrae, 5);
            recipe.AddIngredient(ItemID.Shadewood, 20);
            recipe.AddIngredient(ItemID.ViciousPowder, 10);
            recipe.AddIngredient(ItemID.CopperBar, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Vertebrae, 5);
            recipe2.AddIngredient(ItemID.Shadewood, 20);
            recipe2.AddIngredient(ItemID.ViciousPowder, 10);
            recipe2.AddIngredient(ItemID.TinBar, 8);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
