using AwfulGarbageMod.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class StormHeadgear : ModItem
    {
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Storm Headgear");
			// Tooltip.SetDefault("15% increased ranged velocity\n15% increased arrow damage");

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
            Item.rare = 3;
            Item.defense = 4; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.15f;
            player.bulletDamage += 0.12f;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<StormBreastplate>() && legs.type == ModContent.ItemType<StormLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Every third gun shot converts bullets into chain lightning\nIncreases armor penetration by 5"; // This is the setbonus tooltip

            player.GetModPlayer<GlobalPlayer>().StormHeadgearBonus = true;
            player.GetArmorPenetration(DamageClass.Generic) += 5;

        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("AerogelHelmet").Type);
            recipe.AddIngredient(Mod.Find<ModItem>("StormEssence").Type, 8);
            recipe.AddIngredient(ItemID.RainCloud, 30);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
