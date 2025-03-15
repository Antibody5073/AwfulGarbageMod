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
using AwfulGarbageMod.DamageClasses;
using System.Collections.Generic;
using Terraria.Localization;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace AwfulGarbageMod.Items.Accessories
{

    public class BagOfSeeds : ModItem
	{

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Necropotence"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("34 damage\n5 defense\nTaking damage causes you to release bone toothpicks\n\"Argh, fine! I'll hit you with this and turn you into a couple of cremated reliquaries!\"");
        }


		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 400000;
            Item.rare = 7;
			Item.accessory = true;

        }

        public override void UpdateEquip(Player player)
        {
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Ichor] = true;
            player.GetModPlayer<GlobalPlayer>().BagOfSeeds = true;
            player.GetModPlayer<GlobalPlayer>().extraWhipTagDamage += 3;
            player.GetModPlayer<GlobalPlayer>().InfectionSeed = true;
            player.GetModPlayer<GlobalPlayer>().IchorSeed = true;
            player.GetModPlayer<GlobalPlayer>().CursedSeed = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SunflowerSeed>()
                .AddIngredient<FrigidSeed>()
                .AddIngredient<MagmaSeed>()
                .AddIngredient<SoulflowerSeed>()
                .AddIngredient<SeedOfInfection>()
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
            CreateRecipe()
                .AddIngredient<SunflowerSeed>()
                .AddIngredient<FrigidSeed>()
                .AddIngredient<MagmaSeed>()
                .AddIngredient<BloodflowerSeed>()
                .AddIngredient<SeedOfInfection>()
                .AddIngredient(ItemID.ChlorophyteBar, 10)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}