using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Global;
using AwfulGarbageMod.Items.Placeable.Furniture;
using AwfulGarbageMod.Items.Placeable.OresBars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class EquilibriumPlatemail : ModItem
	{
        public override void Load()
        {
            // The code below runs only if we're not loading on a server
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
        }
        
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Sanguine Robes");
            // Tooltip.SetDefault("Increases magic damage by 8%");

        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = 3; // The rarity of the item
            Item.defense = 7; // The amount of defense the item will give when equipped
        }
        
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.03f;
            player.maxTurrets += 1;
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
            player.setBonus = "Press ArmorAbilityKey to Equilibrium Overload the nearest sentry\nEquilibrium Overloaded sentries deal double damage and explode in bouncy fire and ice balls after 10 seconds\nFireballs and ice balls deal " + AGUtils.ScaleDamage(40, player, DamageClass.Summon) + " summon damage\nStrike an enemy with a whip 20 times to be able to use the ability again\nWhile ability is on cooldown, 30% reduced whip range"; // This is the setbonus tooltip
            player.GetModPlayer<GlobalPlayer>().EquilibriumSentryBonus = true;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 30)
                .AddIngredient<FrigidiumBar>(30)
                .AddTile<Tiles.Furniture.TrashMelter>()
                .Register();
        }
    }
}
