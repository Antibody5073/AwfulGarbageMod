using AwfulGarbageMod.Global;
using System.Transactions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class AerogelHelmet : ModItem
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
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 3; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.20f;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<AerogelBreastplate>() && legs.type == ModContent.ItemType<AerogelLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Grants a built-in usage of Cloud in a Bottle\n6% increased ranged damage and 6% reduced damage taken"; // This is the setbonus tooltip
            player.GetModPlayer<GlobalPlayer>().AerogelBonus = true;
            player.GetJumpState(ExtraJump.CloudInABottle).Enable();
            player.GetDamage(DamageClass.Ranged) += 0.06f;
            player.endurance = 1f - (0.94f * (1f - player.endurance));
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Cloud, 15);
            recipe.AddIngredient(ItemID.GoldBar, 8);
            recipe.AddIngredient(ItemID.Gel, 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe4 = CreateRecipe();
            recipe4.AddIngredient(ItemID.Cloud, 15);
            recipe4.AddIngredient(ItemID.PlatinumBar, 8);
            recipe4.AddIngredient(ItemID.Gel, 12);
            recipe4.AddTile(TileID.Anvils);
            recipe4.Register();
        }
    }
}
