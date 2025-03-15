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

    public class ThrushFeather : ModItem
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
            Item.value = 7500;
            Item.rare = 4;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) += 0.08f;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.25f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SoulOfIghtImaHeadOut>(8)
                .AddIngredient(ItemID.Feather)
                .AddTile(TileID.Anvils)
                .Register();
        }
	}
}