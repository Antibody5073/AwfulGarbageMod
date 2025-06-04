using AwfulGarbageMod.Global;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
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
    public class UmbragelHelmet : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Umbragel Mask");
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
            Item.rare = ModContent.RarityType<UnrealRarity>();
            Item.defense = 2; // The amount of defense the item will give when equipped

            Item.GetGlobalItem<ItemTypes>().Unreal = true;

        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
			player.GetCritChance(DamageClass.Generic) += 5;
			player.GetDamage(DamageClass.Generic) += 0.05f;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<UmbragelBreastplate>() && legs.type == ModContent.ItemType<UmbragelLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "22% increased movement speed\nYour shadow follows you, throwing shurikens at nearby enemies while moving"; // This is the setbonus tooltip
			player.GetModPlayer<GlobalPlayer>().UmbragelBonus = true;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<UmbragelShadow>()] < 1)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.GetModPlayer<GlobalPlayer>().umbragelPos[0], new Vector2(0, 0), ModContent.ProjectileType<UmbragelShadow>(), 0, 0, Main.myPlayer);
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.NinjaHood)
                .AddIngredient<UnrealEssence>(10)
                .AddIngredient(ItemID.Gel, 10)
                .Register();
        }
    }
}
