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

    public class CorruptedPendant : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Corrupted Pendant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("15% increased ranged velocity\n2% increased ranged crit chance\nRanged crits have a 20% chance to inflict cursed flame");
		}

		public int counter;

		public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 4;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.20f;
            player.GetModPlayer<GlobalPlayer>().FeatherPendant += Item.damage;
            player.GetModPlayer<GlobalPlayer>().FeatherPendantArmorPen += 12;
            player.GetModPlayer<GlobalPlayer>().FeatherPendantCursedFlame = true;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<FeatherPendant>()
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddIngredient(ItemID.EbonstoneBlock, 30)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<FeatherPendant>()
                .AddIngredient(ItemID.SoulofNight, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
	}
}