using AwfulGarbageMod.Items.Placeable.OresBars;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Consumables
{
	public class ExtensionPotion : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 20;
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = 30;
			Item.consumable = true;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 1);
			Item.buffType = ModContent.BuffType<Buffs.ExtensionBuff>(); // Specify an existing buff to be applied when used.
			Item.buffTime = 10 * 60 * 60; // The amount of time the buff declared in Item.buffType will last in ticks. 5400 / 60 is 90, so this buff will last 90 seconds.
		}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<CandesciteOre>(), 1);
            recipe.AddIngredient(ItemID.Blinkroot);
            recipe.AddIngredient(ItemID.Fireblossom);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();
        }
    }
}
