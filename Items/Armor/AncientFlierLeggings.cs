using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class AncientFlierLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Aerogel Greaves");
            // Tooltip.SetDefault("Increases movement speed by 10%\nIncreases ranged crit chance by 5%");
        }

		public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 50); // How many coins the item is worth
            Item.rare = 5; // The rarity of the item
            Item.defense = 10; // The amount of defense the item will give when equipped
        }

		public override void UpdateEquip(Player player) {

            player.GetDamage(DamageClass.Ranged) += 0.1f;
            player.GetDamage(DamageClass.Summon) += 0.1f;
            player.GetCritChance(DamageClass.Ranged) += 10f;
            player.GetModPlayer<GlobalPlayer>().wingTimeMultiplier += 0.5f;
            player.GetModPlayer<GlobalPlayer>().HorizontalWingSpdMult -= 0.16f;
            player.GetModPlayer<GlobalPlayer>().VerticalWingSpdMult -= 0.16f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NecroGreaves)
                .AddIngredient(ItemID.BeeGreaves)
                .AddIngredient(ItemID.PlatinumGreaves)
                .AddIngredient<DesertScale>(10)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.NecroGreaves)
                .AddIngredient(ItemID.BeeGreaves)
                .AddIngredient(ItemID.PlatinumGreaves)
                .AddIngredient<DesertScale>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
