using AwfulGarbageMod.DamageClasses;
using AwfulGarbageMod.Items.Placeable.OresBars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Armor
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Body value here will result in TML expecting X_Arms.png, X_Body.png and X_FemaleBody.png sprite-sheet files to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Body)]
	public class CandescentBreastplate : ModItem
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
            Item.value = Item.sellPrice(gold: 50); // How many coins the item is worth
            Item.rare = 5; // The rarity of the item
            Item.defense = 14; // The amount of defense the item will give when equipped
        }


        public override void UpdateEquip(Player player) {
            player.GetDamage(DamageClass.Magic) += 0.1f;
            player.GetDamage(DamageClass.Melee) += 0.1f;
            player.GetCritChance(DamageClass.Magic) += 10f;
            player.GetCritChance(DamageClass.Melee) += 10f;
            player.statManaMax2 += 40;
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MeteorSuit)
                .AddIngredient(ItemID.MoltenBreastplate)
                .AddIngredient(ItemID.SilverChainmail)
                .AddIngredient<CandesciteBar>(25)
                .AddIngredient<Pyrogem>(16)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.MeteorSuit)
                .AddIngredient(ItemID.MoltenBreastplate)
                .AddIngredient(ItemID.TungstenChainmail)
                .AddIngredient<CandesciteBar>(25)
                .AddIngredient<Pyrogem>(16)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
