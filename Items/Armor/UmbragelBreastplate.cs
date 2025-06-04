using AwfulGarbageMod.Global;
using StramClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
    public class UmbragelBreastplate : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("Umbragel Breastplate");
            // Tooltip.SetDefault("Increases ranged damage by 12% while in the air");

        }

        public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.defense = 3; // The amount of defense the item will give when equipped

            Item.GetGlobalItem<ItemTypes>().Unreal = true;
		}

		public override void UpdateEquip(Player player) {
            player.moveSpeed += 0.1f;
            player.GetCritChance(DamageClass.Generic) += 7;
            player.maxMinions += 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NinjaShirt)
                .AddIngredient<UnrealEssence>(10)
                .AddIngredient(ItemID.Gel, 10)
                .Register();
        }
    }
}
