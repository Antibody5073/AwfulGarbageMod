using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Items.Accessories
{

    public class UndeadTyranny : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Stormy Charm"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Increases max number of minions\n12% decreased summon damage\n8% increased movement speed");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = 3;
            Item.accessory = true;
            Item.defense = 4;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) += 0.1f;
            player.whipRangeMultiplier += 0.1f;
            player.GetKnockback(DamageClass.Summon).Base += 1f;
            player.GetArmorPenetration(DamageClass.Generic) += 4;
            player.thorns += 1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IronFist>()
                .AddIngredient<SpikyThread>()
                .AddIngredient(ItemID.SharkToothNecklace)
                .AddIngredient<ThornyShackle>()
                .AddIngredient<SoulOfIghtImaHeadOut>(20)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
	}
}