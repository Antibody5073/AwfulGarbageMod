using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Accessories.Hardmode;
using AwfulGarbageMod.Items.Armor;
using AwfulGarbageMod.Items.Placeable.OresBars;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using StramClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shoes)]
    public class DemonSoles : ModItem
	{

        public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Toad Focus"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("6% increased crit chance\n4% further increased crit chance when staying still\nCritical strikes deal 20% more damage");

        }

        public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 7f;
            player.moveSpeed += 0.15f;
            player.runAcceleration *= 1.25f;
            player.runSlowdown *= 1.25f;
            player.noFallDmg = true;
        }

        public override void AddRecipes()
        {

            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddTile(TileID.Hellforge)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddTile(TileID.Hellforge)
                .Register();
        }
	}
}