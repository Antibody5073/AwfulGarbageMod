using AwfulGarbageMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor.Hardmode
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class GoreKnightLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Sanguine Greaves");
            // Tooltip.SetDefault("Increases jump height");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(platinum: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Lime; // The rarity of the item
			Item.defense = 8; // The amount of defense the item will give when equipped
            Item.lifeRegen = 1;
		}

		public override void UpdateEquip(Player player) {
            player.maxMinions += 1;
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.statLifeMax2 += 40;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimsonGreaves)
                .AddIngredient<VeinLeggings>()
                .AddIngredient(ItemID.IronGreaves)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.CrimsonGreaves)
                .AddIngredient<VeinLeggings>()
                .AddIngredient(ItemID.LeadGreaves)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
        }
    }
}
