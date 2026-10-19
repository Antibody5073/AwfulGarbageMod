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

    public class AncientGadgets : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Feather Pendant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("20% increased ranged velocity\n3% increased ranged crit chance");
		}

		public int counter;

		public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 1;
            Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 9;
            Item.accessory = true;
            Item.defense = 2;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.15f;
            player.GetCritChance(DamageClass.Ranged) += 10;
            player.scope = true;
            player.aggro -= 450;

            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.15f;

            player.GetModPlayer<GlobalPlayer>().NecroScope += Item.damage;
            

            player.ammoCost75 = true;

            if (!hideVisual)
            {
                player.GetModPlayer<GlobalPlayer>().mechanicalArm += 0.4f;
                player.GetModPlayer<GlobalPlayer>().mechanicalScope += 0.4f;
                player.GetModPlayer<GlobalPlayer>().mechanicalLens += 0.4f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NecroScope>()
                .AddIngredient<MechanicalQuiver>()
                .AddIngredient<MechanicalAmalgam>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}