using AwfulGarbageMod.Global;
using AwfulGarbageMod.Projectiles;
using System.Transactions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AwfulGarbageMod.DamageClasses;

namespace AwfulGarbageMod.Items.Armor
{
	// The AutoloadEquip attribute automatically attaches an equip texture to this item.
	// Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
	[AutoloadEquip(EquipType.Head)]
	public class EarthenHelmet : ModItem
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
			Item.value = Item.sellPrice(silver: 10); // How many coins the item is worth
			Item.rare = 1; // The rarity of the item
			Item.defense = 3; // The amount of defense the item will give when equipped
		}

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<GlobalPlayer>().IgnoreScepterDmgPenalties = true;
            player.GetModPlayer<GlobalPlayer>().rangedVelocity += 0.10f;
            player.GetModPlayer<GlobalPlayer>().ScepterMaxStatMult *= 0.9f;
            player.GetDamage(DamageClass.Magic) += 0.05f;

        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<EarthenBreastplate>() && legs.type == ModContent.ItemType<EarthenLeggings>();
		}

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "A Flint Core hovers over your head to fight for you, dealing " + AGUtils.ScaleDamage(11, player, ModContent.GetInstance<MageRangedClass>()) + " damage\nThe Flint Core scales with magic and ranged damage bonuses\nGrants an Earthen Shield that absorbs 20% of damage taken\nAfter the Earthen Shield absorbs 40 damage, it will break and go on cooldown for 30 seconds\nWhile the Earthen Shield is on cooldown, the Flint Core will attack faster\n'Shattered Realm? What's that?'"; // This is the setbonus tooltip
            player.GetModPlayer<GlobalPlayer>().EarthenBonus = true;
            player.GetModPlayer<EarthenShield>().shieldMaxCooldown = 30 * 60;
            player.GetModPlayer<EarthenShield>().shieldMaxDurability = 40;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FlintCore>()] < 1)
            {
                var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<FlintCore>(), 0, 0, Main.myPlayer);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Flint>(15)
                .AddIngredient(ItemID.DirtBlock, 45)
                .AddIngredient(ItemID.ClayBlock, 25)
                .AddIngredient(ItemID.StoneBlock, 25)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
