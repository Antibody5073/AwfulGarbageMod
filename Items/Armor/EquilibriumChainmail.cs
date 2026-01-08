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
	public class EquilibriumChainmail : ModItem
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
            // DisplayName.SetDefault("Frozen Spirit Breastplate");
            // Tooltip.SetDefault("Increases melee armor penetration by 4");

        }

        public override void SetDefaults() {
			Item.width = 18; // Width of the item
			Item.height = 18; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Orange; // The rarity of the item
			Item.defense = 6; // The amount of defense the item will give when equipped
		}

		public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.03f;
            player.maxMinions += 1;
            player.moveSpeed += 0.1f;
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<EquilibriumHelmet>() && legs.type == ModContent.ItemType<EquilibriumLeggings>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Chained equilibrium spirits will fight for you, dealing " + AGUtils.ScaleDamage(47, player, DamageClass.Summon) + " summon damage"; // This is the setbonus tooltip
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<EquilibriumArmorFire>()] < 1)
                {
                    var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<EquilibriumArmorFire>(), 30, 7, Main.myPlayer);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<EquilibriumArmorIce>()] < 1)
                {
                    var projectile = Projectile.NewProjectileDirect(Projectile.GetSource_NaturalSpawn(), player.Center, new Vector2(0, 0), ModContent.ProjectileType<EquilibriumArmorIce>(), 30, 7, Main.myPlayer);
                }

                player.GetModPlayer<GlobalPlayer>().EquilibriumMinionBonus = true;
            }
        }
        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 30)
                .AddIngredient<FrigidiumBar>(35)
                .AddTile<Tiles.Furniture.TrashMelter>()
                .Register();
        }
    }
}
