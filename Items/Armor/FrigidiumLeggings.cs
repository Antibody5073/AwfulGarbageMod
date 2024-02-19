using AwfulGarbageMod.Items.Placeable.OresBars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class FrigidiumLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Frozen Spirit Greaves");
            // Tooltip.SetDefault("Increases movement speed by 10%");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 6; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.12f; // Increase the movement speed of the player
            player.GetAttackSpeed(DamageClass.Melee) *= 1.08f;
            player.iceSkate = true;

        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient<FrozenSpiritLeggings>()
               .AddIngredient<FrigidiumBar>(20)
               .AddTile(TileID.Anvils)
               .Register();
        }
    }
}
