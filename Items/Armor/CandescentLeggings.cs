using AwfulGarbageMod.Items.Placeable.OresBars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class CandescentLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Sanguine Greaves");
            // Tooltip.SetDefault("Increases jump height");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 40); // How many coins the item is worth
			Item.rare = 5; // The rarity of the item
			Item.defense = 12; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
            player.GetCritChance(DamageClass.Magic) += 10f;
            player.GetCritChance(DamageClass.Melee) += 10f;
            player.manaCost *= 0.9f;
            player.moveSpeed += 0.15f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MeteorLeggings)
                .AddIngredient(ItemID.MoltenGreaves)
                .AddIngredient(ItemID.SilverGreaves)
                .AddIngredient<CandesciteBar>(25)
                .AddIngredient<Pyrogem>(20)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.MeteorLeggings)
                .AddIngredient(ItemID.MoltenGreaves)
                .AddIngredient(ItemID.TungstenGreaves)
                .AddIngredient<CandesciteBar>(25)
                .AddIngredient<Pyrogem>(20)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
