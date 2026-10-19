using AwfulGarbageMod.Global;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AwfulGarbageMod.Items.Accessories.Hardmode
{

    public class NecroScope : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 1;
            Item.width = 20;
			Item.height = 20;
            Item.value = 7500;
            Item.rare = 6;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.10f;
			player.bulletDamage += 0.05f;
			player.GetCritChance(DamageClass.Ranged) += 10;
            player.scope = true;
            player.aggro -= 300;

            player.GetModPlayer<GlobalPlayer>().NecroScope += Item.damage;

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ReconScope)
                .AddIngredient<SkullSigil>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}