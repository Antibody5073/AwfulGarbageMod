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

    public class ViceAndVirtue : ModItem
    {
        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 2500000;
            Item.rare = 9;
            Item.accessory = true;
            Item.defense = 4;
        }


        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Melee) += 0.15f;
            player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
            player.GetCritChance(DamageClass.Melee) += 8f;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
            player.GetModPlayer<GlobalPlayer>().ImprovedShadowflameStone = true;
            player.GetModPlayer<GlobalPlayer>().ShadowflameMelee = true;
            player.autoReuseGlove = true;
            player.GetArmorPenetration(DamageClass.Generic) += 8;

            player.GetModPlayer<GlobalPlayer>().Enlightenment += 450;
            player.GetModPlayer<GlobalPlayer>().MeleeWeaponSize += 0.05f;
            if (player.HeldItem.DamageType == DamageClass.MeleeNoSpeed || (player.HeldItem.CountsAsClass(DamageClass.Melee) && player.HeldItem.attackSpeedOnlyAffectsWeaponAnimation) || ItemID.Sets.Yoyo[player.HeldItem.type])
            {
                player.GetAttackSpeed(DamageClass.Melee) += 0.4f;
            }
        }

        public override void AddRecipes()
		{
            CreateRecipe()
                .AddIngredient<ViciousVambrace>()
                .AddIngredient<VirtuousVambrace>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
	}
}