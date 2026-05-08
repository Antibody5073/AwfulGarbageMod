using AwfulGarbageMod.Global;
using AwfulGarbageMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor.Hardmode
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class MidnightBreastplate : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("Aerogel Breastplate");
            // Tooltip.SetDefault("Increases ranged damage by 12% while in the air");

        }

        public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(platinum: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the item
            Item.defense = 18; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.maxTurrets += 1;
            player.GetCritChance(DamageClass.Generic) += 17f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.07f;
            player.statManaMax2 += 40;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShadowScalemail)
                .AddIngredient<RottingBreastplate>()
                .AddIngredient(ItemID.IronChainmail)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.ShadowScalemail)
                .AddIngredient<RottingBreastplate>()
                .AddIngredient(ItemID.LeadChainmail)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
        }
    }
}
