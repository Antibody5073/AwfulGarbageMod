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

    public class ManaBouquet : ModItem
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
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaCost *= 0.9f;
            player.manaFlower = true;
            player.statManaMax2 += 20;
            player.GetModPlayer<GlobalPlayer>().ScepterMaxStatMult *= 0.9f;
            player.manaRegenBonus += 10;
            int defenseBoost = player.GetModPlayer<GlobalPlayer>().scepterProjectiles;
            player.GetModPlayer<GlobalPlayer>().MaxScepterBoost += 1;
            player.GetDamage<ScepterDamageClass>() += 0.1f;
            if (defenseBoost > 18)
            {
                defenseBoost = 18;
            }
            player.statDefense += defenseBoost;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.ManaFlower);
            recipe.AddIngredient(Mod.Find<ModItem>("FrozenPetals").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("PetrifiedRose").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("BloomingMarrow").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("BloomingEvil").Type);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}