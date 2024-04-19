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

    public class BipolarEmblem : ModItem
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
            player.GetCritChance(DamageClass.Magic) += 2;

            if (player.velocity.X == 0)
            {
                player.GetDamage(DamageClass.Magic) += 18 / 100f;
            }
            if (player.velocity.Y == 0)
            {
                player.manaCost *= 0.7f;
            }
            else
            {
                player.manaCost *= 0.88f;
            }
            player.GetDamage(DamageClass.Magic) += 12 / 100f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DualismRing>()
                .AddIngredient(ItemID.SorcererEmblem)
                .AddIngredient<BoneBadge>()
                .AddIngredient<SoulOfIghtImaHeadOut>(7)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}