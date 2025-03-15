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
using System.Collections.Generic;
using Terraria.Localization;
using AwfulGarbageMod.DamageClasses;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.CodeAnalysis;
using Terraria.GameContent;
using AwfulGarbageMod.Items.Placeable.OresBars;

namespace AwfulGarbageMod.Items.Accessories
{

    public class DemonGauntlet : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 150000;
            Item.rare = 3;
            Item.accessory = true;
            Item.defense = 4;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().DemonClaw = true;
            player.thorns += 0.75f;

            player.GetAttackSpeed(DamageClass.Melee) += 0.07f;
            if (player.GetModPlayer<GlobalPlayer>().FortifyingLink < 12) { player.GetModPlayer<GlobalPlayer>().FortifyingLink = 8; }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DemonClaw>()
                .AddIngredient<DesertGauntlet>()
                .AddIngredient<ThornyShackle>()
                .AddIngredient<FrigidiumBar>(5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }
}