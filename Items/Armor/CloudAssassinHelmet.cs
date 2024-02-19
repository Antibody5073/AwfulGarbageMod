using AwfulGarbageMod.Global;
using StramClasses;
using System.Transactions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
    [ExtendsFromMod("StramClasses")]    
    public class CloudAssassinHelmet : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("CloudAssassin Mask");
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
			Item.defense = 4; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<GlobalStramPlayer>().rogueVelocity *= 1.2f;
            player.rogue().critDamageFlat += 4;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<CloudAssassinBreastplate>() && legs.type == ModContent.ItemType<CloudAssassinLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Rogue critical strikes summon lightning from the sky\nLightning damage is equal to 8 + 50% of the projectile's damage\nIncreases rogue armor penetration by 5"; // This is the setbonus tooltip
            player.GetArmorPenetration(StramUtils.rogueDamage()) += 5;
            player.GetModPlayer<GlobalStramPlayer>().CloudAssassinBonus = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cloud, 40)
                .AddIngredient<StormEssence>(9)
                .AddIngredient(ItemID.Silk, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
