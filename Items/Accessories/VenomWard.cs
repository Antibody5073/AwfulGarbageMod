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
    [AutoloadEquip(EquipType.Shield)]
    public class VenomWard : ModItem
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
            Item.rare = 8;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 6;
            player.GetArmorPenetration(DamageClass.Generic) += 12;

            if (player.GetModPlayer<GlobalPlayer>().OverflowingVenom < 1.15f)
            {
                player.GetModPlayer<GlobalPlayer>().OverflowingVenom = 1.15f;
            }
            player.hasPaladinShield = true;
            player.noKnockback = true;
            if (player.statLife <= player.statLifeMax2 / 2)
            {
                player.AddBuff(BuffID.IceBarrier, 3);
            }
            player.GetModPlayer<GlobalPlayer>().HoneyOnDamaged += 300;
            if (hideVisual)
            {
                player.GetModPlayer<GlobalPlayer>().Bees = false;
            }
            else
            {
                player.GetModPlayer<GlobalPlayer>().Bees = true;
            }
        }


        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<OverflowingVenomVial>()
                .AddIngredient(ItemID.FrozenShield)
                .AddIngredient(ItemID.StingerNecklace)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
	}
}