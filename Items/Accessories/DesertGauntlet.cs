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

    public class DesertGauntlet : ModItem
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
            Item.value = 12500;
            Item.rare = 2;
            Item.accessory = true;
            Item.defense = 3;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Melee) += 0.06f;
            if (player.GetModPlayer<GlobalPlayer>().FortifyingLink < 7) { player.GetModPlayer<GlobalPlayer>().FortifyingLink = 7; }

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MandibleGlove>()
                .AddIngredient<FortifyingLink>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}