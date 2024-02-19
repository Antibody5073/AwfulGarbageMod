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

namespace AwfulGarbageMod.Items.Accessories
{

    public class IceCrystalGeode : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ice Crystal Geode"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("3 defense\nMelee damage has a low chance to inflict Frostburn");
        }


        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 1500;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 2;
            player.GetModPlayer<GlobalPlayer>().iceCrystalGeode = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod.Find<ModItem>("FrostShard").Type, 8);
            recipe.AddIngredient(Mod.Find<ModItem>("SpiritItem").Type, 10);
            recipe.AddIngredient(ItemID.StoneBlock, 30);
            recipe.AddIngredient(ItemID.IceBlock, 30);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}