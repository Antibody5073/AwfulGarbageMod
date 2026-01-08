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
using AwfulGarbageMod.Items.Placeable.OresBars;

namespace AwfulGarbageMod.Items.Accessories
{
    public class CelestitePlating : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Necro Aegis"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting player line.
			// Tooltip.SetDefault("79 damage\n10 defense\nIncreases defense the lower your health, up to a max of 30 defense below 30% health\nIncreases health healed by potions the lower your health, up to a max of 30% extra health below 15% health\nGrants immunity to knockback, most debuffs, and fire blocks\nIncreases life regeneration\nTaking damage causes you to release powerful bone toothpicks");
		}

		public int counter;

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 4500;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 4;
            player.GetCritChance(DamageClass.Magic) += 4f;
            player.manaCost *= 0.96f;
            player.statManaMax2 += 30;
            player.GetDamage(DamageClass.Magic) += 0.04f;
            player.magicCuffs = true;
            player.manaMagnet = true;
        }


        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient(ItemID.CelestialCuffs)
                .AddIngredient<CelestiteShard>(12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}