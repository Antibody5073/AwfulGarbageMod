﻿using AwfulGarbageMod.Global;
using System.Transactions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class AncientFlierHelmet : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Aerogel Mask");
			// Tooltip.SetDefault("20% increased ranged velocity");

			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			// ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
			// ArmorIDs.Head.Sets.DrawBackHair[Item.headSlot] = true;
			// ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true; 
		}

		public override void SetDefaults() {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 50); // How many coins the item is worth
            Item.rare = 5; // The rarity of the item
            Item.defense = 8; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.16f;
            player.GetDamage(DamageClass.Summon) += 0.16f;
            player.GetArmorPenetration(DamageClass.Generic) += 4;
            player.GetModPlayer<GlobalPlayer>().wingTimeMultiplier += 0.25f;
            player.GetModPlayer<GlobalPlayer>().HorizontalWingSpdMult -= 0.08f;
            player.GetModPlayer<GlobalPlayer>().VerticalWingSpdMult -= 0.08f;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<AncientFlierBreastplate>() && legs.type == ModContent.ItemType<AncientFlierLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            //player.setBonus = "Ranged projectiles hit a second time for 10% damage that counts as and scales with summon damage, which can recieve whip tag effects\nMinion projectiles are affected by ranged velocity"; // This is the setbonus tooltip

            player.setBonus = "Ranged projectiles recieve 50% of whip tag damage\nMinion projectiles are affected by ranged velocity modifiers"; // This is the setbonus tooltip
            player.GetModPlayer<GlobalPlayer>().AncientFlierBonus = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NecroHelmet)
                .AddIngredient(ItemID.BeeHeadgear)
                .AddIngredient(ItemID.PlatinumHelmet)
                .AddIngredient<DesertScale>(8)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.NecroHelmet)
                .AddIngredient(ItemID.BeeHeadgear)
                .AddIngredient(ItemID.GoldHelmet)
                .AddIngredient<DesertScale>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
