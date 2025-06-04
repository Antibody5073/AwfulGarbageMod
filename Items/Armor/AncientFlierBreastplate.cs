using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class AncientFlierBreastplate : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("Aerogel Breastplate");
            // Tooltip.SetDefault("Increases ranged damage by 12% while in the air");

        }

        public override void SetDefaults() {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 50); // How many coins the item is worth
            Item.rare = 5; // The rarity of the item
            Item.defense = 12; // The amount of defense the item will give when equipped
        }

		public override void UpdateEquip(Player player) {
            player.maxMinions += 2;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.2f;
            player.GetModPlayer<GlobalPlayer>().wingTimeMultiplier += 0.25f;

            player.GetModPlayer<GlobalPlayer>().HorizontalWingSpdMult -= 0.08f;
            player.GetModPlayer<GlobalPlayer>().VerticalWingSpdMult -= 0.08f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NecroBreastplate)
                .AddIngredient(ItemID.BeeBreastplate)
                .AddIngredient(ItemID.PlatinumChainmail)
                .AddIngredient<DesertScale>(36)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.NecroBreastplate)
                .AddIngredient(ItemID.BeeBreastplate)
                .AddIngredient(ItemID.GoldChainmail)
                .AddIngredient<DesertScale>(36)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
