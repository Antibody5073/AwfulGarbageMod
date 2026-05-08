using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor.Hardmode
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class GoreKnightBreastplate : ModItem
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
            Item.value = Item.sellPrice(platinum: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the items
            Item.defense = 9; // The amount of defense the item will give when equipped
        }


        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
            player.statLifeMax2 += 60;
            player.statManaMax2 += 60;
            player.manaCost *= 0.85f;
            player.moveSpeed += 0.25f;
            player.whipRangeMultiplier += 0.25f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimsonScalemail)
                .AddIngredient<VeinBreastplate>()
                .AddIngredient(ItemID.IronChainmail)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.CrimsonScalemail)
                .AddIngredient<VeinBreastplate>()
                .AddIngredient(ItemID.LeadChainmail)
                .AddIngredient<HeartOfBlight>()
                .AddTile<ElementalSmeltery>()
                .Register();
        }
    }
}
