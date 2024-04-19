using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using AwfulGarbageMod;
using AwfulGarbageMod.Global;
using ExampleMod.Content;

namespace AwfulGarbageMod.Items.Accessories
{

    public class DriedBlossom : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Feather Pendant"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("20% increased ranged velocity\n3% increased ranged crit chance");
        }

        public int counter;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 20);
            Item.rare = 5;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().wingTimeMultiplier += 0.15f;
            player.GetDamage(DamageClass.Magic) += 0.06f;
            player.GetCritChance(DamageClass.Magic) += 6f;
            player.manaCost *= 0.94f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroups.AnyHerb, 5)
                .AddIngredient<DesertScale>(10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}