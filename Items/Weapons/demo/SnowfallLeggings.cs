using AwfulGarbageMod.Items.Placeable.OresBars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Weapons.demo
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
	public class SnowfallLeggings : ModItem
	{
		public override void SetStaticDefaults() {
            // DisplayName.SetDefault("Sanguine Greaves");
            // Tooltip.SetDefault("Increases jump height");
        }

		public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
			Item.rare = 1; // The rarity of the item
			Item.defense = 4; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player) {
            player.GetDamage(DamageClass.Ranged) += 0.07f;
        }
    }
}
