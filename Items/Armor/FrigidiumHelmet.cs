using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.OresBars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class FrigidiumHelmet : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Frozen Spirit Helmet");
			// Tooltip.SetDefault("4% increased melee crit chance");

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
			Item.defense = 6; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.GetAttackSpeed(DamageClass.Melee) *= 1.08f;
            player.GetModPlayer<GlobalPlayer>().FrigidHelmet = true;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<FrigidiumBreastplate>() && legs.type == ModContent.ItemType<FrigidiumLeggings>();
		}

		// UpdateArmorSet allows you to give set bonuses to the armor.
		public override void UpdateArmorSet(Player player) {
			player.setBonus = "Generates an orbiting ice chunk for every 350 melee damage dealt, with a maximum of 5 ice chunks.\nEach ice chunk reduces damage taken by 8%, for a total of 40% reduced damage taken.\nTaking damage shatters all ice chunks, and no more ice chunks can generate for 18 seconds.\nPress armor ability key to absorb all orbiting ice chunks, increasing your melee damage by 6% for each ice chunk for 18 seconds.\nIncreases melee damage by 10%"; // This is the setbonus tooltip
            player.GetDamage(DamageClass.Melee) += 0.1f;
            player.GetModPlayer<GlobalPlayer>().FrigidiumBonus = true;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FrozenSpiritHelmet>()
                .AddIngredient<FrigidiumBar>(16)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
