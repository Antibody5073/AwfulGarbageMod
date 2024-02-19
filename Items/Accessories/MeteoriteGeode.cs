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

    public class MeteoriteGeode : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Meteorite Geode"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            // Tooltip.SetDefault("Melee damage summons a small meteor chunk from the sky\nMeteors summoned by sword strikes deal 25% more damage\nHas a two second cooldown");
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
            player.GetModPlayer<GlobalPlayer>().meteoriteGeode = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.StoneBlock, 30);
            recipe.AddIngredient(ItemID.Meteorite, 30);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}