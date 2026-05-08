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
using StramClasses;
using System.Collections.Generic;
using Terraria.Localization;
using AwfulGarbageMod.Items.Armor;
using AwfulGarbageMod.Items.Placeable.OresBars;

namespace AwfulGarbageMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shoes)]
    public class ArcticusBlur : ModItem
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
            player.accRunSpeed = 6f;
            player.maxTurrets += 1;

            player.iceSkate = true;

            player.spikedBoots = 2;
            player.jumpSpeedBoost += AGUtils.OnePerentJumpSpeed * 24;
            player.accFlipper = true;
            player.autoJump = true;
            player.extraFall += 15;
            player.moveSpeed += 0.24f;

            player.buffImmune[BuffID.Chilled] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FlinxFurBoots>()
                .AddIngredient<FrosshroomFlask>()
                .AddIngredient(ItemID.IceSkates)
                .AddIngredient(ItemID.FrogGear)
                .AddIngredient<FrigidiumBar>(10)
                .AddIngredient(ItemID.FlinxFur, 6)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
	}
}