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

    public class CrystalEmblem : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Dualism Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("16% increased magic damage when not moving horizontally\n21% reduced mana cost when not moving vertically\n3% increased magic crit chance\n\"Aside from the pepperoni-pizza smell, it's perfect!\"");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 6;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
            {
                player.GetModPlayer<GlobalPlayer>().IlluminantString = true;
            }
            player.GetDamage(DamageClass.Summon) += 6 / 100f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.12f;
            player.whipRangeMultiplier += 0.24f;
            player.maxMinions += 1;
            player.maxTurrets += 1;
            player.moveSpeed += 0.12f;
        }
        
        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<StormEmblem>()
                .AddIngredient<AuthorityCrystal>()
                .AddIngredient<IlluminantString>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
	}
}