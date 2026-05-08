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
using AwfulGarbageMod.Items.Accessories.Hardmode;
using AwfulGarbageMod.Tiles.Furniture;

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{
    [AutoloadEquip(EquipType.Shoes)]
    public class PrismicAccelerators : ModItem
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
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 7.25f;
            player.maxTurrets += 1;

            player.iceSkate = true;

            player.spikedBoots = 2;
            player.jumpSpeedBoost += AGUtils.OnePerentJumpSpeed * 24;
            player.accFlipper = true;
            player.autoJump = true;

            player.moveSpeed += 0.4f;
            player.hasMagiluminescence = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.GetModPlayer<GlobalPlayer>().PumpkinRocket = true;
            player.extraFall += 20;
            player.fireWalk = true;
            player.waterWalk2 = true;
            player.lavaRose = true;
            player.lavaMax += 420;
            player.rocketBoots = (player.vanityRocketBoots = 1);

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
                .AddIngredient(ItemID.TerrasparkBoots)
                .AddIngredient<ArcticusBlur>()
                .AddIngredient<PumpkinRocket>()
                .AddIngredient(ItemID.Magiluminescence)
                .AddIngredient<AmplifyingRail>()
                .AddIngredient(ItemID.BrokenHeroSword)
                .AddTile<ElementalSmeltery>()
                .Register();
        }
	}
}