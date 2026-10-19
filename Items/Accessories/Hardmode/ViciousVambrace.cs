using AwfulGarbageMod.Global;
using AwfulGarbageMod.Tiles.Furniture;
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

    public class ViciousVambrace : ModItem
    {
        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 250000;
            Item.rare = 7;
            Item.accessory = true;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.15f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.12f;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
            player.GetModPlayer<GlobalPlayer>().ShadowflameStone = true;
            player.GetModPlayer<GlobalPlayer>().ShadowflameMelee = true;
            player.autoReuseGlove = true;
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient(ItemID.FireGauntlet)
                .AddIngredient<ShadowflameStone>()
                .AddIngredient(ItemID.SoulofNight, 30)
                .AddIngredient(ItemID.SpookyTwig)
                .AddTile<ElementalSmeltery>()
                .Register();
        }
	}
}