using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Weapons.Summon;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor.Hardmode
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class GoreKnightHelmet : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Sanguine Hood");
			// Tooltip.SetDefault("80 increased max mana");

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
            Item.value = Item.sellPrice(platinum: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the item
            Item.defense = 8; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.statLifeMax2 += 40;
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<GoreKnightBreastplate>() && legs.type == ModContent.ItemType<GoreKnightLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Greatly increased life regeneration\nGrants immunity to Ichor\nSummons an Ichor Sticker minion that spits ichor streams"; // This is the setbonus tooltip
            player.crimsonRegen = true;
            player.buffImmune[BuffID.Ichor] = true; 
            player.GetModPlayer<GlobalPlayer>().GoreKnightBonus = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GoreKnightMinion>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<GoreKnightMinion>(), 120, 0, Main.myPlayer);
                projectile.OriginalArmorPenetration += 20; 
                projectile.originalDamage = 120;

            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimsonHelmet)
                .AddIngredient<VeinHelmet>()
                .AddIngredient(ItemID.IronHelmet)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.CrimsonHelmet)
                .AddIngredient<VeinHelmet>()
                .AddIngredient(ItemID.LeadHelmet)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
        }
    }
}
