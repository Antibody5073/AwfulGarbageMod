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
    [AutoloadEquip(EquipType.Shield)]
    public class NecroAegis : ModItem
    {
        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
            Item.value = 1500;
            Item.rare = 8;
            Item.accessory = true;
            Item.lifeRegen = 2;
            Item.defense = 8;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GlobalPlayer>().MeatShield += 0.3f;
            player.GetModPlayer<GlobalPlayer>().ScaledShadeShield += 15f;
            player.GetModPlayer<GlobalPlayer>().necroPotence += 1;
            player.GetModPlayer<GlobalPlayer>().necroPotencePercent += 0.60f;
            player.buffImmune[46] = true;
            player.buffImmune[33] = true;
            player.buffImmune[36] = true;
            player.buffImmune[30] = true;
            player.buffImmune[20] = true;
            player.buffImmune[32] = true;
            player.buffImmune[31] = true;
            player.buffImmune[35] = true;
            player.buffImmune[23] = true;
            player.buffImmune[22] = true;
            player.buffImmune[156] = true;
            player.fireWalk = true;
            player.noKnockback = true;
        }


        public override void AddRecipes()
		{

            CreateRecipe()
                .AddIngredient(ItemID.AnkhCharm)
                .AddIngredient<ObsidianMeatShield>()
                .AddIngredient<Necropotence>()
                .AddTile<ElementalSmeltery>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.AnkhCharm)
                .AddIngredient<ObsidianShadeShield>()
                .AddIngredient<Necropotence>()
                .AddTile<ElementalSmeltery>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.AnkhShield)
                .AddIngredient<ScaledShadeShield>()
                .AddIngredient(ItemID.BandofRegeneration)
                .AddIngredient<Necropotence>()
                .AddTile<ElementalSmeltery>()
                .Register();
            CreateRecipe()
                .AddIngredient(ItemID.AnkhShield)
                .AddIngredient<MeatShield>()
                .AddIngredient(ItemID.BandofRegeneration)
                .AddIngredient<Necropotence>()
                .AddTile<ElementalSmeltery>()
                .Register();
        }
	}
}