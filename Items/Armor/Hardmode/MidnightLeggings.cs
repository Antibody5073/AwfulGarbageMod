using AwfulGarbageMod.Global;
using AwfulGarbageMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor.Hardmode
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class MidnightLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Aerogel Greaves");
            // Tooltip.SetDefault("Increases movement speed by 10%\nIncreases ranged crit chance by 5%");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(platinum: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the item
            Item.defense = 15; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.GetCritChance(DamageClass.Generic) += 17f;
            player.manaCost *= 0.88f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.07f;

        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShadowGreaves)
                .AddIngredient<RottingLeggings>()
                .AddIngredient(ItemID.IronGreaves)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.ShadowGreaves)
                .AddIngredient<RottingLeggings>()
                .AddIngredient(ItemID.LeadGreaves)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
        }
    }
}
