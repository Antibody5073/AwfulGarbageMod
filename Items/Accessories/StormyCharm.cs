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

    public class StormyCharm : ModItem
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
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) -= 16 / 100f;
            player.maxMinions += 1;
            player.maxTurrets += 1;
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CloudRelic>()
                .AddIngredient<SpiderCharm>()
                .AddIngredient<EvilCharm>()
                .AddIngredient<StormEssence>(6)
                .AddIngredient(ItemID.RainCloud)
                .AddTile(TileID.Anvils)
                .Register();
        }
	}
}