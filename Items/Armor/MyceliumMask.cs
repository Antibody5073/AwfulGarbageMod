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
	public class MyceliumMask : ModItem
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
			Item.defense = 2; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 60;
            player.GetModPlayer<GlobalPlayer>().ScepterMaxStatMult *= 0.92f;
            player.GetDamage(DamageClass.Magic) -= 0.1f;
            player.GetDamage<ScepterDamageClass>() += 0.2f;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<MyceliumBreastplate>() && legs.type == ModContent.ItemType<MyceliumLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = $"Increases casting speed of scepters by 25%\nPress ArmorSetAbility key to deal 10% of your max HP as damage to yourself, triggering on-hit effects and releasing any orbiting scepter projectiles\nThis damage can not be dodged and ignores defense\nHas a two second cooldown"; // This is the setbonus tooltip

            player.GetAttackSpeed<ScepterDamageClass>() *= 1.25f;
            player.GetModPlayer<GlobalPlayer>().MyceliumMaskBonus = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.GlowingMushroom, 20);
            recipe.AddIngredient(ItemID.SilverBar, 10);
            recipe.AddIngredient(ItemID.Silk, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.GlowingMushroom, 20);
            recipe2.AddIngredient(ItemID.TungstenBar, 10);
            recipe2.AddIngredient(ItemID.Silk, 8);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
