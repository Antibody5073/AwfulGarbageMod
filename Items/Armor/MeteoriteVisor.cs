using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod.Systems;

namespace AwfulGarbageMod.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class MeteoriteVisor : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Mycelium Hood");
			// Tooltip.SetDefault("80 increased max mana");

			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			// ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
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
            player.GetDamage<ScepterDamageClass>() += 0.12f;
            player.GetModPlayer<GlobalPlayer>().MaxScepterBoost += 1;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ItemID.MeteorSuit && legs.type == ItemID.MeteorLeggings;
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = $"Asteroid Belt no longer has a penalty\nIncreases defense by 1 for every orbiting scepter projectile\nMax defense boost is 30\nPress ArmorSetAbility key to deal 8% of your max HP as damage to yourself, triggering on-hit effects and releasing any orbiting scepter projectiles\nThis damage can not be dodged and ignores defense\nHas a 12 second cooldown"; // This is the setbonus tooltip

            int defenseBoost = player.GetModPlayer<GlobalPlayer>().scepterProjectiles;
            if (defenseBoost > 30)
            {
                defenseBoost = 30;
            }
            player.statDefense += defenseBoost; player.GetModPlayer<GlobalPlayer>().MeteoriteVisorBonus = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
