using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Accessories;
using AwfulGarbageMod.Items.Placeable.OresBars;
using AwfulGarbageMod.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class CrabChainmail : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("Frozen Spirit Breastplate");
            // Tooltip.SetDefault("Increases melee armor penetration by 4");

        }

        public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
			Item.rare = 2; // The rarity of the item
			Item.defense = 5; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<EarthenShield>().shieldDurabilityMult += 0.07f;
            player.GetDamage<SentinelDamageClass>() += 0.07f;

        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<CrabHelmet>() && legs.type == ModContent.ItemType<CrabLeggings>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "On shield break, swing a powerful crab claw towards the nearest enemy that leaves targets vulnerable,\nreducing defense by 15 and taking 30% more damage for 5 seconds"; // This is the setbonus tooltip
            player.GetModPlayer<GlobalPlayer>().CrabBonus = true;
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
        }
    }
}
