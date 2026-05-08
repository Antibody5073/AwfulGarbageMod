using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class AmplifyingRail : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Feather Pendant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("20% increased ranged velocity\n3% increased ranged crit chance");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = Item.buyPrice(gold:50);
            Item.rare = 6;
            Item.accessory = true;
        }


		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			float maxspd = 40 * 42240 / 216000;

            float spd = Vector2.Distance(Vector2.Zero, player.velocity);
			if (spd > maxspd)
			{
				spd = maxspd;
            }
			player.GetDamage(DamageClass.Generic) += 0.20f * spd / maxspd;
			player.statDefense -= (int)(20 * spd / maxspd);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 20)
                .AddIngredient<SoulOfIghtImaHeadOut>(20)
                .AddIngredient(ItemID.CobaltBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 20)
                .AddIngredient<SoulOfIghtImaHeadOut>(20)
                .AddIngredient(ItemID.PalladiumBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}