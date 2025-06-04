using AwfulGarbageMod.Global;
using rail;
using StramClasses;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
    public class UmbragelLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Umbragel Greaves");
            // Tooltip.SetDefault("Increases movement speed by 10%\nIncreases ranged crit chance by 5%");
        }


		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ModContent.RarityType<UnrealRarity>();
			Item.defense = 2; // The amount of defense the item will give when equipped

            Item.GetGlobalItem<ItemTypes>().Unreal = true;
        }

        public override void UpdateEquip(Player player) {
            player.GetAttackSpeed(DamageClass.Melee) += 0.09f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.09f;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NinjaPants)
                .AddIngredient<UnrealEssence>(10)
                .AddIngredient(ItemID.Gel, 10)
                .Register();
        }
    }
}
