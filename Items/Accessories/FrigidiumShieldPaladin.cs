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
using StramClasses;

namespace AwfulGarbageMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    [ExtendsFromMod("StramClasses")]
    public class FrigidiumShieldPaladin : ModItem
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
            Item.value = 1500;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.lifeRegen = 2;
            Item.QuickWeightArmor(8);
            Item.paladinItem().dr = 0.03f;
            Item.defense = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.Chilled] = true;
            player.paladin().smiteCharge++;
            player.paladin().smiteDamageBoost += 0.1f;
            player.GetCritChance<Paladin>() += 5;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<StramClasses.Classes.Paladin.Accessories.SigilShield>()
                .AddIngredient<FrigidiumBar>(20)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}