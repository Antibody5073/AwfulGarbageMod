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

namespace AwfulGarbageMod.Items.Accessories
{

    public class CelestialBouquet : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Lightning Ring"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("Mana potions are automatically consumed\nDisable visibility to disable auto-drinking mana potions\nMana sickness wears off twice as fast\nDrinking a mana potion will electrify you for two seconds\n5% reduced mana usage");
		}

		public int counter;

		public override void SetDefaults()
		{
            Item.width = 20;
            Item.height = 20;
            Item.value = 30000;
            Item.rare = 6;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaCost *= 0.9f;
            player.manaFlower = true;
            player.statManaMax2 += 40;
            player.GetModPlayer<GlobalPlayer>().ScepterMaxStatMult *= 0.9f;
            player.manaRegenBonus += 15;
            int defenseBoost = player.GetModPlayer<GlobalPlayer>().scepterProjectiles;
            player.GetModPlayer<GlobalPlayer>().MaxScepterBoost += 1;
            player.GetDamage<ScepterDamageClass>() += 0.15f;
            player.GetDamage(DamageClass.Magic) += 0.15f;
            if (defenseBoost > 25)
            {
                defenseBoost = 25;
            }
            player.statDefense += defenseBoost;
            player.manaMagnet = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
              .AddIngredient<ManaBouquet>()
              .AddIngredient(ItemID.CelestialEmblem)
              .AddTile(TileID.TinkerersWorkbench)
              .Register();
        }
    }
}