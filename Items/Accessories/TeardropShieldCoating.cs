using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.Collections.Generic;
using System;
using Terraria.Localization;
using AwfulGarbageMod.Global;

namespace AwfulGarbageMod.Items.Accessories
{
    public class TeardropShieldCoating : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 23;
            Item.height = 13;
            Item.value = Item.buyPrice(0, 6);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        int Timer = 0;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.05f;
            player.GetModPlayer<EarthenShield>().shieldCooldownMult *= 1.3f;
            player.GetModPlayer<EarthenShield>().shieldDurabilityMult *= 0.4f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {

        }
    }

}