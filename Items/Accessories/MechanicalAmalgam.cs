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

    public class MechanicalAmalgam : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mandible Gauntlet"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("3 defense\n8% increased melee speed");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 25000;
            Item.rare = 6;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().mechanicalArm += 0.4f;
            player.GetModPlayer<GlobalPlayer>().mechanicalScope += 0.4f;
            player.GetModPlayer<GlobalPlayer>().mechanicalLens += 0.4f;
            player.GetDamage(DamageClass.Generic) -= 0.05f;
            player.GetModPlayer<GlobalPlayer>().FleshyAmalgam = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FleshyAmalgam>()
                .AddIngredient<MechanicalArm>()
                .AddIngredient<MechanicalLens>()
                .AddIngredient<MechanicalScope>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}