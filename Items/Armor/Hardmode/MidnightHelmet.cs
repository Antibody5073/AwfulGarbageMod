using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Projectiles;
using AwfulGarbageMod.Tiles.Furniture;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using System;
using System.Transactions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static AssGen.Assets;

namespace AwfulGarbageMod.Items.Armor.Hardmode
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
	public class MidnightHelmet : ModItem
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
            Item.value = Item.sellPrice(platinum: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the item
            Item.defense = 15; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.1f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.07f;
            player.statManaMax2 += 40;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<MidnightBreastplate>() && legs.type == ModContent.ItemType<MidnightLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases movement speed and acceleration\nGrants immunity to Cursed Inferno\nSummons a clinger minion that latches to enemies, reducing enemy defense by 10 and inflicting Cursed Inferno"; // This is the setbonus tooltip
            player.shadowArmor = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.GetModPlayer<GlobalPlayer>().MidnightBonus = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<MidnightMinion>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<MidnightMinion>(), 12, 0, Main.myPlayer);
                projectile.OriginalArmorPenetration += 20; 
                projectile.originalDamage = 12;

            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ShadowHelmet)
                .AddIngredient<RottingHelmet>()
                .AddIngredient(ItemID.IronHelmet)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.ShadowHelmet)
                .AddIngredient<RottingHelmet>()
                .AddIngredient(ItemID.LeadHelmet)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
        }
    }
}
