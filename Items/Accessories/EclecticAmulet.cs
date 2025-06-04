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
using ExampleMod.Content;
using AwfulGarbageMod.Items.Placeable.OresBars;

namespace AwfulGarbageMod.Items.Accessories
{

    public class EclecticAmulet : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Poison Pendant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("2 defense\n6% increased ranged crit chance\nIncreases armor penetration by 5\nTaking damage releases bees and douses the user in honey\nDisable visibility to remove bees\nRanged weapons inflict poison");
		}

		public int counter;

		public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 300000;
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.GetGlobalItem<ItemTypes>().Unreal = true;
            Item.accessory = true;
            Item.lifeRegen = 1;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance = 1f - (0.85f * (1f - player.endurance));
            player.GetModPlayer<GlobalPlayer>().ChloroplastCore = true;
            player.GetModPlayer<GlobalPlayer>().SlimyLocket = true;
            player.GetModPlayer<GlobalPlayer>().ShockAbsorber = true;

        }



        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<ChloroplastCore>()
                .AddIngredient<SlimyLocket>()
                .AddIngredient<ShockAbsorber>()
                .AddIngredient<FrostShard>(20)
                .AddIngredient<EnchantedLeaf>(20)
                .AddIngredient<SpiderLeg>(20)
                .AddIngredient<VeinJuice>(20)
                .AddIngredient<VitalliumBar>(20)
                .AddIngredient<UnrealEssence>(20)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
	}
}