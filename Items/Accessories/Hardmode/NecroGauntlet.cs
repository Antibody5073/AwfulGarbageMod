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

    public class NecroGauntlet : ModItem
    {
        public int counter;
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 1;
            Item.width = 20;
			Item.height = 20;
            Item.value = 25000;
            Item.rare = ItemRarityID.LightPurple;
            Item.accessory = true;
            Item.defense = 2;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().necroPotence += 1;
            player.GetModPlayer<GlobalPlayer>().necroPotencePercent += 40;
            player.GetModPlayer<GlobalPlayer>().BoneBracelet += Item.damage;
            player.GetAttackSpeed(DamageClass.Melee) += 0.16f;
            player.moveSpeed += 0.16f;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<MyceliumGauntlet>()
                .AddIngredient<Necropotence>()
                .AddIngredient<SkullSigil>()
                .AddIngredient<SoulOfIghtImaHeadOut>(6)
                .AddIngredient(ItemID.SoulofNight, 6)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
	}
}