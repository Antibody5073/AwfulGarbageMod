using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class WorthlessJunkLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("WorthlessJunk Greaves");
            // Tooltip.SetDefault("20 increased maximum health\n15% increased whip range");
            
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 5; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
            player.moveSpeed *= 1.25f;
            player.GetAttackSpeed(DamageClass.Melee) *= 1.25f;
            player.GetModPlayer<GlobalPlayer>().JunkGreaves = true;
            player.GetModPlayer<GlobalPlayer>().ScepterMaxStatMult *= 0.85f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Garbage>(60)
                .AddTile<Tiles.Furniture.TrashMelter>()
                .Register();
        }
    }
}
